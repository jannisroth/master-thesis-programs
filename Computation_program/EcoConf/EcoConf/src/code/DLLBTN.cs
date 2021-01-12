using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace EcoConf
{
    class DLLBTN : iDLL
    {
        BTNClass btn;

        public DLLBTN()
        {
            btn = new BTNClass();
        }

        /**
         * use computation of the DLL
         * the idx is the index in the array
         */
        public bool BlackBoxComputing(ref iInput arguments,ref iOutput result, int idx = 0)
        {
            Debug.Assert(arguments.GetInputBlackbox().Count > idx && arguments.GetInputBlackbox()[idx].Length > 2);
            Debug.Assert(result.GetOutput().Count > idx && result.GetOutput()[idx].Length > 2);


            var output = result.GetOutput()[idx];
            var input = arguments.GetInputBlackbox()[idx];
            BlackBoxComputing(ref input, ref output);
            return true;
        }

        public bool BlackBoxComputing(ref object[] arguments, ref object[] result)
        {
            Array input = arguments;
            Array output = new object[Utility.outputDllSize];
            //Console.WriteLine(string.Join(", ", arguments));
            btn.BTNCalc(ref input, ref output);
            //FixFinSpacing(ref input, ref output);

            int lengthArguments = arguments.Length;
            int lengthResult = result.Length;

            ArrayCopy(output, result);
            result[lengthResult - 2] = arguments[lengthArguments - 2];
            result[lengthResult - 1] = arguments[lengthArguments - 1];
            return true;
        }

        /**
         * start a new process for dll computation, because the DLL will stop working if we input a wrong circuit number
         */
        public bool BlackBoxComputingPIPE(ref object[] arguments, ref object[] result)
        {
            bool res = false;

            iInput arg = new InputBTN();
            iOutput output = new OutputBTN();
            arg.GetInputBlackbox().Add(arguments);
            output.GetOutput().Add(new object[Utility.outputDllSize]);
            res = BlackBoxComputingPIPE(ref arg, ref output);
            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = arg.GetInputBlackbox(0)[i];
            }
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = output.GetOutput(0)[i];
            }
            return res;
        }
         public bool BlackBoxComputingPIPE(ref iInput arguments, ref iOutput result, int idx = 0)
        {
            Debug.Assert(arguments.GetInputBlackbox().Count > idx && arguments.GetInputBlackbox()[idx].Length > 2);
            Debug.Assert(result.GetOutput().Count > idx && result.GetOutput()[idx].Length > 2);


            Array input = arguments.GetInputBlackbox()[idx];


            #region New DLL PIPE
            //Console.WriteLine("Started application (Process A)...");

            var pipeOutput = new List<object>();
            string btndllpipe = @".\..\..\BTNDLLpipe\bin\Release\BTNDLLpipe.exe";
            string test = Path.GetFullPath(@".\..\..");
            if (!File.Exists(btndllpipe))
            {
                btndllpipe = @".\..\..\..\BTNDLLpipe\bin\Release\BTNDLLpipe.exe";
            }
            if (!File.Exists(btndllpipe))
            {
                Console.WriteLine("Can' find the File!");
            }
            // Create separate process
            var anotherProcess = new Process
            {
                StartInfo =
                {
                    //FileName = @"C:\Users\Roth.J\Documents\Master\EcoConfBasic\BTNDLLpipe\bin\Debug\BTNDLLpipe.exe",
                    //FileName = @"C:\Users\Roth.J\Documents\Master\EcoConfBasic\BTNDLLpipe\bin\Release\BTNDLLpipe.exe", // TODO change
                    FileName = btndllpipe,
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            // Create 2 anonymous pipes (read and write) for duplex communications
            // (each pipe is one-way)
            using (var pipeRead = new AnonymousPipeServerStream(PipeDirection.In,
                HandleInheritability.Inheritable))
            using (var pipeWrite = new AnonymousPipeServerStream(PipeDirection.Out,
                HandleInheritability.Inheritable))
            {
                // Pass to the other process handles to the 2 pipes
                anotherProcess.StartInfo.Arguments = pipeRead.GetClientHandleAsString() + " " +
                    pipeWrite.GetClientHandleAsString();
                anotherProcess.Start();

                //Console.WriteLine("Started other process (Process B)...");
                //Console.WriteLine();

                pipeRead.DisposeLocalCopyOfClientHandle();
                pipeWrite.DisposeLocalCopyOfClientHandle();

                try
                {
                    using (var sw = new StreamWriter(pipeWrite))
                    {
                        // Send a 'sync message' and wait for the other process to receive it
                        sw.Write("SYNC");
                        pipeWrite.WaitForPipeDrain();

                        //Console.WriteLine("Sending message to Process B...");
                        sw.WriteLine("Dummy, because the first line gets lost?!?");

                        // Send message to the other process
                        //sw.Write("Hello from Process A!");
                        foreach (var item in input)
                        {
                            sw.WriteLine(""+item);
                        }
                        sw.Write("END");
                    }

                    // Get message from the other process
                    using (var sr = new StreamReader(pipeRead))
                    {
                        string temp;

                        // Wait for 'sync message' from the other process
                        do
                        {
                            temp = sr.ReadLine();
                        } while (temp == null || !temp.StartsWith("SYNC"));

                        // Read until 'end message' from the other process
                        while ((temp = sr.ReadLine()) != null && !temp.StartsWith("END"))
                        {
                            pipeOutput.Add(temp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TODO Exception handling/logging
                    throw;
                }
                finally
                {
                    anotherProcess.WaitForExit();
                    anotherProcess.Close();
                }

                //if (pipeOutput.Count > 0)
                //    Console.WriteLine("Received message from Process B: ");

            }
            #endregion

            int lengthArguments = arguments.GetInputBlackbox()[idx].Length;
            int lengthResult = pipeOutput.Count;

            pipeOutput.CopyTo(result.GetOutput()[idx], 0);
            ArrayCopy(pipeOutput.ToArray(), result.GetOutput()[idx]);
            result.GetOutput()[idx][lengthResult - 2] = arguments.GetInputBlackbox()[idx][lengthArguments - 2];
            result.GetOutput()[idx][lengthResult - 1] = arguments.GetInputBlackbox()[idx][lengthArguments - 1];

            //update arguments, since the FixFinSpacing Method could have changed the fin spacing in the other program
            arguments.GetInputBlackbox(idx)[11] = pipeOutput[7];
            return true;
        }

        /**
        * error 11: fin spacing does not work
        */
        private void FixFinSpacing(ref Array input, ref Array output)
        {
            throw new NotImplementedException();
            /*
            if (Convert.ToString(output.GetValue(0)) != "11")
            {
                return;
            }

            foreach (var item in Utility.finSpacings)
            {
                input.SetValue("" + item, 11);
                btn.BTNCalc(ref input, ref output);
                if (output.GetValue(0) == null)
                {
                    return;
                }
            }
            */
        }



        void ArrayCopy(Array source, object[] target)
        {
            if (source.Length == 0)
            {
                for (int i = 0; i < 60; i++)
                {
                    target[i] = "";
                }
            }
            else
            {
                Debug.Assert(source.Length == target.Length);
            }

            for (int i = 0; i < source.Length; i++)
            {
                target[i] = source.GetValue(i) == null ? "" : source.GetValue(i);
            }
        }
    }

}
