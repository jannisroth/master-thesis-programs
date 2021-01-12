using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using CsvHelper;
using System.Threading;
using System.Threading.Tasks;



namespace DataPopulation
{

    public enum PlottingType{
        Height,
        Length,
        NumberOfRows,
        NumberCircuits,
        FinSpacing,
        TubeThickness,
        FinThickness,
        FluidTempIn,
        ErrorCode,
        All,

        DuoHeightLength,
        DuoNumberRowsTubeThickness,
        DuoNumberCircuitsNumberRows,
        DuoFinSpacingFinThickness,
        DuoAll,
        Special,
        MultiSpecial,
    };

    class DLLBTN
    {
        public static List<Array> inputs = new List<Array>();
        public static List<Array> outputs = new List<Array>();
        static Dictionary<int,Tuple<int, int>> startEndList = new Dictionary<int,Tuple<int, int>>();
        public static int numDirectories= Directory.GetDirectories(Utility.outputFolderPath).Length + 1;

        public static Tuple<double, int>[] configInit = new Tuple<double, int>[10];

        private static void CalcConfiguration(int height, int length, int hc, int numberCircuits, int finSpacing, int numberRows, double finThickness, double fluitTempIn,int airMass, int waterMass, PlottingType plottingType, bool isIntake, double power = 0)
        {
            configInit[0] = Tuple.Create((double)height,3);
            configInit[1] = Tuple.Create((double)length,4);
            configInit[2] = Tuple.Create((double)numberRows,12);
            configInit[3] = Tuple.Create((double)numberCircuits,10);
            configInit[4] = Tuple.Create((double)finSpacing,11);
            configInit[5] = Tuple.Create((double)finThickness,33);
            configInit[6] = Tuple.Create((double)fluitTempIn,14);
            configInit[7] = Tuple.Create((double)airMass,23);
            configInit[8] = Tuple.Create((double)waterMass, 37);
            configInit[9] = Tuple.Create((double)power,26);

            string newOutputfolderName = "\\" + $"config_h{height}_l{length}_c{numberCircuits}_fs{finSpacing}_r{numberRows}_ft{finThickness}_fti{fluitTempIn}_am{airMass}"+(isIntake?"_I" : "_O")+(power == 0? "":$"_p{power}");
            Utility.outputFolderPath += newOutputfolderName;
            Directory.CreateDirectory(Utility.outputFolderPath);

            numDirectories = Directory.GetDirectories(Utility.outputFolderPath).Length + 1;

            Console.WriteLine("Hello!");
            InputGenerator.Initialize();
            string[] files = Directory.GetFiles(Utility.inputFolderPath);

            //------------------------
            //
            PlottingType type = plottingType;
            //
            object[] chooseInput = InputGenerator.input_7;
            chooseInput[3] = "" + height;
            chooseInput[4] = "" + length;
            chooseInput[10] = "" + numberCircuits;
            chooseInput[11] = "" + finSpacing;
            chooseInput[12] = "" + numberRows;
            chooseInput[33] = "" + finThickness;
            chooseInput[14] = "" + fluitTempIn;
            chooseInput[23] = "" + airMass;
            chooseInput[37] = "" + waterMass;
            if (power != 0) { 
                chooseInput[26] = "" + power;
                chooseInput[31] = "";
            }
            //
            bool printfiles = false;
            //
            bool useFilesAsInput = false;
            //
            //------------------------

            //Only use if one choose to store files of inputs
            #region Filebased
            if (useFilesAsInput)
            {
                foreach (var file in files)
                {
                    Console.WriteLine(file);
                    Array input = new dynamic[Utility.lengthInput];
                    InputParser.ParseBlackboxSingle(file, ref input);
                    inputs.Add(input);
                }
            }
            #endregion

            #region InMemory
            if (!useFilesAsInput) switch (type)
                {
                    case PlottingType.NumberOfRows:
                        InputGenerator.PopulateInputnumberOfRows(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.NumberCircuits:
                        InputGenerator.PopulateInputnumberOfCircuits(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.Height:
                        InputGenerator.PopulateInputHeight(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.Length:
                        InputGenerator.PopulateInputLength(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.TubeThickness:
                        InputGenerator.PopulateInputTubeThickness(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.FinThickness:
                        InputGenerator.PopulateInputFinThickness(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.FinSpacing:
                        InputGenerator.PopulateInputFinSpacing(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.FluidTempIn:
                        InputGenerator.PopulateInputFluidTempIn(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.Special:
                        InputGenerator.PopulateInputSpecial(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.MultiSpecial:
                        InputGenerator.PopulateInputMultiSpecial(ref inputs, ref chooseInput, isIntake, hc);
                        break;
                    case PlottingType.ErrorCode:
                        InputGenerator.PopulateErrorCode(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.All:
                        InputGenerator.PopulateInputHeight(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputLength(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputnumberOfRows(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputnumberOfCircuits(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputTubeThickness(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputFinThickness(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputFinSpacing(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputFluidTempIn(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.DuoHeightLength:
                        InputGenerator.PopulateInputListDuoHeightLength(ref inputs, ref chooseInput);
                        break;
                    case PlottingType.DuoAll:
                        InputGenerator.PopulateInputListDuoHeightLength(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputListDuoFinSpacingFinThickness(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputListDuoNumberCircuitsNumberRows(ref inputs, ref chooseInput);
                        InputGenerator.PopulateInputListDuoNumberRowsTubeThickness(ref inputs, ref chooseInput);
                        break;
                    default:
                        break;
                }

            Stopwatch sw = new Stopwatch();
            for (int i = 0; i < inputs.Count; i++)
            {
                Array output = new dynamic[Utility.lengthOutput];
                outputs.Add(output);
            }
            int numberThreads = 1;
            List<Task> taskList = new List<Task>();
            int numberOfElem = inputs.Count / numberThreads + 1;
            for (int i = 0; i < numberThreads; i++)
            {
                Task t = new Task(computeThread);
                taskList.Add(t);
                startEndList[t.Id] = (Tuple.Create(i * numberOfElem, (i + 1) * numberOfElem > inputs.Count ? inputs.Count : (i + 1) * numberOfElem));
            }
            sw.Start();
            for(int i = 0; i < taskList.Count; i++)
            {
                taskList[i].Start();
            }

            Task.WaitAll(taskList.ToArray());
            Console.WriteLine("Computed "+hc+" and am: "+ airMass);
            startEndList.Clear();

            sw.Stop();  
            #endregion

            switch (type)
            {
                case PlottingType.NumberOfRows:
                    WritePlotDataNumberOfRows();
                    break;
                case PlottingType.NumberCircuits:
                    WritePlotDataNumberCircuits();
                    break;
                case PlottingType.Height:
                    WritePlotDataHeight();
                    break;
                case PlottingType.Length:
                    WritePlotDataLength();
                    break;
                case PlottingType.TubeThickness:
                    WritePlotDataTubeThickness();
                    break;
                case PlottingType.FinThickness:
                    WritePlotDataFinThickness();
                    break;
                case PlottingType.FinSpacing:
                    WritePlotDataFinSpacing();
                    break;
                case PlottingType.FluidTempIn:
                    WritePlotDataFluidTempIn();
                    break;
                case PlottingType.Special:
                    WritePlotDataSpecial();
                    break;
                case PlottingType.MultiSpecial:
                    WritePlotDataMultiSpecial();
                    break;
                case PlottingType.ErrorCode:
                    WritePlotDataErrorCode();
                    break;
                case PlottingType.All:
                    WritePlotDataAll();
                    break;
                case PlottingType.DuoHeightLength:
                    WritePlotDataDuoHeightLength();
                    break;
                case PlottingType.DuoAll:
                    WritePlotDataDuoAll();
                    break;
                default:
                    break;
            }
            numDirectories++;

            Utility.outputFolderPath = Utility.outputFolderPath.Remove(Utility.outputFolderPath.Length - newOutputfolderName.Length);
            inputs.Clear();
            outputs.Clear();
            InputGenerator.Clear();

            Console.WriteLine($"Finish! Time to Compute: {sw.Elapsed}");
            //if(hadError) Console.ReadKey();
        }

        static void computeThread()
        {
            //Console.WriteLine($"TASK {Task.CurrentId}");
            for (int i = startEndList[(int)Task.CurrentId].Item1; i < startEndList[(int)Task.CurrentId].Item2; i++)
            {
                //DLL does not work for these combinations, there is a bug it run into an endless loop
                if (Convert.ToInt32(configInit[0].Item1) == 642 && Convert.ToInt32(configInit[1].Item1) == 606) 
                    if (i >=210 && i <= 251 || i >= 1260 && i <=1301 || i >= 2310 && i <=2351 || i >= 3360 && i <= 3401 || i >= 4410 && i <= 4461 || i>= 5460 && i <= 5511 || i>= 6510 && i <= 6561 || i>=7560 && i <= 7601 || i >= 8610 && i<= 8661) continue;
                if(Convert.ToInt32(configInit[0].Item1) == 642 && Convert.ToInt32(configInit[1].Item1) == 912)
                    if (i >= 210 && i <= 251 || i >= 1680 && i <= 1721 || i >= 3150 && i <= 3191 || i >= 4620 && i <= 4661 || i>= 6090 && i<=6131 || i >= 7560 && i<=7601) continue;
                for (int j = 0; j < configInit.Length; j++)
                {
                    //configInit[0] = height;
                    //configInit[1] = length;
                    //configInit[2] = numberRows;
                    //configInit[3] = numberCircuits;
                    //configInit[4] = finSpacing;
                    //configInit[5] = finThickness;
                    //configInit[6] = fluitTempIn;
                    //configInit[7] = airMass;
                    //configInit[8] = power;
                    if (configInit[j].Item1 != 0)
                    {
                        inputs[1].SetValue("" + configInit[j].Item1.ToString(), configInit[j].Item2);
                    }
                }
                Array input = inputs[i];
                Array output = new dynamic[Utility.lengthOutput];
         
                BTNClass btn = new BTNClass();
                BTNCalc(btn, ref input, ref output);
                outputs[i] = output;




                //if (output.GetValue(0) != null)
                //{
                //    hadError = true;
                //    Console.WriteLine("ErrorCode: " + output.GetValue(0));
                //    Console.WriteLine(" " + output.GetValue(1));
                //    Console.WriteLine("");
                //}
            }
        }

        static void Main(string[] args)
        {
            Tuple<int, int> hc4 = Tuple.Create(642, 606);
            Tuple<int, int> hc6 = Tuple.Create(642, 912);
            Tuple<int, int> hc9 = Tuple.Create(948, 912);
            Tuple<int, int> hc12 = Tuple.Create(948, 1218);
            Tuple<int, int> hc16 = Tuple.Create(1254, 1218);
            Tuple<int, int> hc20 = Tuple.Create(1254, 1524);
            Tuple<int, int> hc25 = Tuple.Create(1560, 1524);
            Tuple<int, int> hc30 = Tuple.Create(1560, 1830);
            Tuple<int, int> hc36 = Tuple.Create(1866, 1830);
            Tuple<int, int> hc42 = Tuple.Create(1866, 2136);
            Tuple<int, int> hc49 = Tuple.Create(2172, 2136);
            Tuple<int, int> hc56 = Tuple.Create(2172, 2442);
            Tuple<int, int> hc64 = Tuple.Create(2478, 2442);
            Tuple<int, int> hc72 = Tuple.Create(2478, 2748);
            Tuple<int, int> hc80 = Tuple.Create(2750, 2748);
            Tuple<int, int> hc90 = Tuple.Create(2750, 3054);
            Tuple<int, int> hc100 = Tuple.Create(2750, 3360);
            Tuple<int, int> hc110 = Tuple.Create(2750, 3666);



            PlottingType type = PlottingType.MultiSpecial;

            //CalcConfiguration(hc4.Item1, hc4.Item2, 4, 0, 0, 0, 1, 0, 1000, 300, type, true);
            //CalcConfiguration(hc4.Item1, hc4.Item2, 4, 0, 0, 0, 2, 0, 1000, 300, type, true);
            //CalcConfiguration(hc4.Item1, hc4.Item2, 4, 0, 0, 0, 3, 0, 1000, 300, type, true);
            //CalcConfiguration(hc4.Item1, hc4.Item2, 4, 0, 0, 0, 1, 0, 1000, 300, type, false);
            //CalcConfiguration(hc4.Item1, hc4.Item2, 4, 0, 0, 0, 2, 0, 1000, 300, type, false);
            //CalcConfiguration(hc4.Item1, hc4.Item2, 4, 0, 0, 0, 3, 0, 1000, 300, type, false);

            //CalcConfiguration(hc6.Item1, hc6.Item2, 6, 0, 0, 0, 1, 0, 2000, 700, type, true);
            //CalcConfiguration(hc6.Item1, hc6.Item2, 6, 0, 0, 0, 2, 0, 2000, 700, type, true);
            //CalcConfiguration(hc6.Item1, hc6.Item2, 6, 0, 0, 0, 3, 0, 2000, 700, type, true);
            //CalcConfiguration(hc6.Item1, hc6.Item2, 6, 0, 0, 0, 1, 0, 2000, 700, type, false);
            //CalcConfiguration(hc6.Item1, hc6.Item2, 6, 0, 0, 0, 2, 0, 2000, 700, type, false);
            //CalcConfiguration(hc6.Item1, hc6.Item2, 6, 0, 0, 0, 3, 0, 2000, 700, type, false);

            //CalcConfiguration(hc9.Item1, hc9.Item2, 9, 0, 0, 0, 1, 0, 4000, 1300, type, true);
            //CalcConfiguration(hc9.Item1, hc9.Item2, 9, 0, 0, 0, 2, 0, 4000, 1300, type, true);
            //CalcConfiguration(hc9.Item1, hc9.Item2, 9, 0, 0, 0, 3, 0, 4000, 1300, type, true);
            //CalcConfiguration(hc9.Item1, hc9.Item2, 9, 0, 0, 0, 1, 0, 4000, 1300, type, false);
            //CalcConfiguration(hc9.Item1, hc9.Item2, 9, 0, 0, 0, 2, 0, 4000, 1300, type, false);
            //CalcConfiguration(hc9.Item1, hc9.Item2, 9, 0, 0, 0, 3, 0, 4000, 1300, type, false);

            //CalcConfiguration(hc12.Item1, hc12.Item2, 12, 0, 0, 0, 1, 0, 6000, 2000, type, true);
            //CalcConfiguration(hc12.Item1, hc12.Item2, 12, 0, 0, 0, 2, 0, 6000, 2000, type, true);
            //CalcConfiguration(hc12.Item1, hc12.Item2, 12, 0, 0, 0, 3, 0, 6000, 2000, type, true);
            //CalcConfiguration(hc12.Item1, hc12.Item2, 12, 0, 0, 0, 1, 0, 6000, 2000, type, false);
            //CalcConfiguration(hc12.Item1, hc12.Item2, 12, 0, 0, 0, 2, 0, 6000, 2000, type, false);
            //CalcConfiguration(hc12.Item1, hc12.Item2, 12, 0, 0, 0, 3, 0, 6000, 2000, type, false);

            //CalcConfiguration(hc16.Item1, hc16.Item2, 16, 0, 0, 0, 1, 0, 9000, 2900, type, true);
            //CalcConfiguration(hc16.Item1, hc16.Item2, 16, 0, 0, 0, 2, 0, 9000, 2900, type, true);
            //CalcConfiguration(hc16.Item1, hc16.Item2, 16, 0, 0, 0, 3, 0, 9000, 2900, type, true);
            //CalcConfiguration(hc16.Item1, hc16.Item2, 16, 0, 0, 0, 1, 0, 9000, 2900, type, false);
            //CalcConfiguration(hc16.Item1, hc16.Item2, 16, 0, 0, 0, 2, 0, 9000, 2900, type, false);
            //CalcConfiguration(hc16.Item1, hc16.Item2, 16, 0, 0, 0, 3, 0, 9000, 2900, type, false);

            //CalcConfiguration(hc20.Item1, hc20.Item2, 20, 0, 0, 0, 1, 0, 12000, 3900, type, true);
            //CalcConfiguration(hc20.Item1, hc20.Item2, 20, 0, 0, 0, 2, 0, 12000, 3900, type, true);
            //CalcConfiguration(hc20.Item1, hc20.Item2, 20, 0, 0, 0, 3, 0, 12000, 3900, type, true);
            //CalcConfiguration(hc20.Item1, hc20.Item2, 20, 0, 0, 0, 1, 0, 12000, 3900, type, false);
            //CalcConfiguration(hc20.Item1, hc20.Item2, 20, 0, 0, 0, 2, 0, 12000, 3900, type, false);
            //CalcConfiguration(hc20.Item1, hc20.Item2, 20, 0, 0, 0, 3, 0, 12000, 3900, type, false);

            //CalcConfiguration(hc25.Item1, hc25.Item2, 25, 0, 0, 0, 1, 0, 15000, 4900, type, true);
            //CalcConfiguration(hc25.Item1, hc25.Item2, 25, 0, 0, 0, 2, 0, 15000, 4900, type, true);
            //CalcConfiguration(hc25.Item1, hc25.Item2, 25, 0, 0, 0, 3, 0, 15000, 4900, type, true);
            //CalcConfiguration(hc25.Item1, hc25.Item2, 25, 0, 0, 0, 1, 0, 15000, 4900, type, false);
            //CalcConfiguration(hc25.Item1, hc25.Item2, 25, 0, 0, 0, 2, 0, 15000, 4900, type, false);
            //CalcConfiguration(hc25.Item1, hc25.Item2, 25, 0, 0, 0, 3, 0, 15000, 4900, type, false);

            //CalcConfiguration(hc30.Item1, hc30.Item2, 30, 0, 0, 0, 1, 0, 18000, 5900, type, true);  
            //CalcConfiguration(hc30.Item1, hc30.Item2, 30, 0, 0, 0, 2, 0, 18000, 5900, type, true);
            //CalcConfiguration(hc30.Item1, hc30.Item2, 30, 0, 0, 0, 3, 0, 18000, 5900, type, true);
            //CalcConfiguration(hc30.Item1, hc30.Item2, 30, 0, 0, 0, 1, 0, 18000, 5900, type, false);
            //CalcConfiguration(hc30.Item1, hc30.Item2, 30, 0, 0, 0, 2, 0, 18000, 5900, type, false);
            //CalcConfiguration(hc30.Item1, hc30.Item2, 30, 0, 0, 0, 3, 0, 18000, 5900, type, false);

            //CalcConfiguration(hc36.Item1, hc36.Item2, 36, 0, 0, 0, 1, 0, 21000, 6800, type, true);
            //CalcConfiguration(hc36.Item1, hc36.Item2, 36, 0, 0, 0, 2, 0, 21000, 6800, type, true);
            //CalcConfiguration(hc36.Item1, hc36.Item2, 36, 0, 0, 0, 3, 0, 21000, 6800, type, true);
            //CalcConfiguration(hc36.Item1, hc36.Item2, 36, 0, 0, 0, 1, 0, 21000, 6800, type, false);
            //CalcConfiguration(hc36.Item1, hc36.Item2, 36, 0, 0, 0, 2, 0, 21000, 6800, type, false);
            //CalcConfiguration(hc36.Item1, hc36.Item2, 36, 0, 0, 0, 3, 0, 21000, 6800, type, false);

            //CalcConfiguration(hc42.Item1, hc42.Item2, 42, 0, 0, 0, 1, 0, 25000, 8100, type, true);
            //CalcConfiguration(hc42.Item1, hc42.Item2, 42, 0, 0, 0, 2, 0, 25000, 8100, type, true);
            //CalcConfiguration(hc42.Item1, hc42.Item2, 42, 0, 0, 0, 3, 0, 25000, 8100, type, true);
            //CalcConfiguration(hc42.Item1, hc42.Item2, 42, 0, 0, 0, 1, 0, 25000, 8100, type, false);
            //CalcConfiguration(hc42.Item1, hc42.Item2, 42, 0, 0, 0, 2, 0, 25000, 8100, type, false);
            //CalcConfiguration(hc42.Item1, hc42.Item2, 42, 0, 0, 0, 3, 0, 25000, 8100, type, false);

            //CalcConfiguration(hc49.Item1, hc49.Item2, 49, 0, 0, 0, 1, 0, 30000, 9800, type, true);
            //CalcConfiguration(hc49.Item1, hc49.Item2, 49, 0, 0, 0, 2, 0, 30000, 9800, type, true);
            //CalcConfiguration(hc49.Item1, hc49.Item2, 49, 0, 0, 0, 3, 0, 30000, 9800, type, true);
            //CalcConfiguration(hc49.Item1, hc49.Item2, 49, 0, 0, 0, 1, 0, 30000, 9800, type, false);
            //CalcConfiguration(hc49.Item1, hc49.Item2, 49, 0, 0, 0, 2, 0, 30000, 9800, type, false);
            //CalcConfiguration(hc49.Item1, hc49.Item2, 49, 0, 0, 0, 3, 0, 30000, 9800, type, false);

            //CalcConfiguration(hc56.Item1, hc56.Item2, 56, 0, 0, 0, 1, 0, 35000, 11400, type, true);
            //CalcConfiguration(hc56.Item1, hc56.Item2, 56, 0, 0, 0, 2, 0, 35000, 11400, type, true);
            //CalcConfiguration(hc56.Item1, hc56.Item2, 56, 0, 0, 0, 3, 0, 35000, 11400, type, true);
            //CalcConfiguration(hc56.Item1, hc56.Item2, 56, 0, 0, 0, 1, 0, 35000, 11400, type, false);
            //CalcConfiguration(hc56.Item1, hc56.Item2, 56, 0, 0, 0, 2, 0, 35000, 11400, type, false);
            //CalcConfiguration(hc56.Item1, hc56.Item2, 56, 0, 0, 0, 3, 0, 35000, 11400, type, false);

            //CalcConfiguration(hc64.Item1, hc64.Item2, 64, 0, 0, 0, 1, 0, 40000, 13000, type, true);
            //CalcConfiguration(hc64.Item1, hc64.Item2, 64, 0, 0, 0, 2, 0, 40000, 13000, type, true);
            //CalcConfiguration(hc64.Item1, hc64.Item2, 64, 0, 0, 0, 3, 0, 40000, 13000, type, true);
            //CalcConfiguration(hc64.Item1, hc64.Item2, 64, 0, 0, 0, 1, 0, 40000, 13000, type, false);
            //CalcConfiguration(hc64.Item1, hc64.Item2, 64, 0, 0, 0, 2, 0, 40000, 13000, type, false);
            //CalcConfiguration(hc64.Item1, hc64.Item2, 64, 0, 0, 0, 3, 0, 40000, 13000, type, false);

            //CalcConfiguration(hc72.Item1, hc72.Item2, 72, 0, 0, 0, 1, 0, 45000, 14650, type, true);
            //CalcConfiguration(hc72.Item1, hc72.Item2, 72, 0, 0, 0, 2, 0, 45000, 14650, type, true);
            //CalcConfiguration(hc72.Item1, hc72.Item2, 72, 0, 0, 0, 3, 0, 45000, 14650, type, true);
            //CalcConfiguration(hc72.Item1, hc72.Item2, 72, 0, 0, 0, 1, 0, 45000, 14650, type, false);
            //CalcConfiguration(hc72.Item1, hc72.Item2, 72, 0, 0, 0, 2, 0, 45000, 14650, type, false);
            //CalcConfiguration(hc72.Item1, hc72.Item2, 72, 0, 0, 0, 3, 0, 45000, 14650, type, false);

            //CalcConfiguration(hc80.Item1, hc80.Item2, 80, 0, 0, 0, 1, 0, 50000, 16300, type, true);
            //CalcConfiguration(hc80.Item1, hc80.Item2, 80, 0, 0, 0, 2, 0, 50000, 16300, type, true);
            //CalcConfiguration(hc80.Item1, hc80.Item2, 80, 0, 0, 0, 3, 0, 50000, 16300, type, true);
            //CalcConfiguration(hc80.Item1, hc80.Item2, 80, 0, 0, 0, 1, 0, 50000, 16300, type, false);
            //CalcConfiguration(hc80.Item1, hc80.Item2, 80, 0, 0, 0, 2, 0, 50000, 16300, type, false);
            //CalcConfiguration(hc80.Item1, hc80.Item2, 80, 0, 0, 0, 3, 0, 50000, 16300, type, false);

            //CalcConfiguration(hc90.Item1, hc90.Item2, 90, 0, 0, 0, 1, 0, 55000, 17900, type, true);
            //CalcConfiguration(hc90.Item1, hc90.Item2, 90, 0, 0, 0, 2, 0, 55000, 17900, type, true);
            //CalcConfiguration(hc90.Item1, hc90.Item2, 90, 0, 0, 0, 3, 0, 55000, 17900, type, true);
            //CalcConfiguration(hc90.Item1, hc90.Item2, 90, 0, 0, 0, 1, 0, 55000, 17900, type, false);
            //CalcConfiguration(hc90.Item1, hc90.Item2, 90, 0, 0, 0, 2, 0, 55000, 17900, type, false);
            //CalcConfiguration(hc90.Item1, hc90.Item2, 90, 0, 0, 0, 3, 0, 55000, 17900, type, false);

            //CalcConfiguration(hc100.Item1, hc100.Item2, 100, 0, 0, 0, 1, 0, 60000, 19500, type, true);
            //CalcConfiguration(hc100.Item1, hc100.Item2, 100, 0, 0, 0, 2, 0, 60000, 19500, type, true);
            //CalcConfiguration(hc100.Item1, hc100.Item2, 100, 0, 0, 0, 3, 0, 60000, 19500, type, true);
            //CalcConfiguration(hc100.Item1, hc100.Item2, 100, 0, 0, 0, 1, 0, 60000, 19500, type, false);
            //CalcConfiguration(hc100.Item1, hc100.Item2, 100, 0, 0, 0, 2, 0, 60000, 19500, type, false);
            //CalcConfiguration(hc100.Item1, hc100.Item2, 100, 0, 0, 0, 3, 0, 60000, 19500, type, false);

            //CalcConfiguration(hc110.Item1, hc110.Item2, 110, 0, 0, 0, 1, 0, 65000, 21200, type, true);
            //CalcConfiguration(hc110.Item1, hc110.Item2, 110, 0, 0, 0, 2, 0, 65000, 21200, type, true);
            //CalcConfiguration(hc110.Item1, hc110.Item2, 110, 0, 0, 0, 3, 0, 65000, 21200, type, true);
            //CalcConfiguration(hc110.Item1, hc110.Item2, 110, 0, 0, 0, 1, 0, 65000, 21200, type, false);
            //CalcConfiguration(hc110.Item1, hc110.Item2, 110, 0, 0, 0, 2, 0, 65000, 21200, type, false);
            //CalcConfiguration(hc110.Item1, hc110.Item2, 110, 0, 0, 0, 3, 0, 65000, 21200, type, false);





            Console.ReadKey();
        }

        private static void BTNCalc(BTNClass btn, ref Array input, ref Array output)
        {
            btn.BTNCalc(ref input, ref output);
            //Console.WriteLine(output.GetValue(0) == null ? "" : output.GetValue(0).ToString() + " " + output.GetValue(1) == null ? "" : output.GetValue(1).ToString());
            return;
            if (Convert.ToString(input.GetValue(57)).Contains("Circuits"))
            {
                #region New DLL PIPE
                //Console.WriteLine("Started application (Process A)...");

                var pipeOutput = new List<object>();

                // Create separate process
                var anotherProcess = new Process
                {
                    StartInfo =
                {
                    //FileName = @"C:\Users\Roth.J\Documents\Master\EcoConfBasic\BTNDLLpipe\bin\Debug\BTNDLLpipe.exe",
                    FileName = @"C:\Users\Roth.J\Documents\EcoCond\Programm\EcoConf\EcoConf\BTNDLLpipe\bin\Release\BTNDLLpipe.exe", // TODO change
                    //FileName = @"..\..\..\BTNDLLpipe\bin\Release\BTNDLLpipe.exe",
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
                                sw.WriteLine("" + item);
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
                        //Console.WriteLine("Received message from Process B: ");

                }
                #endregion
                object[] outputHelp = new object[200];
                pipeOutput.CopyTo(outputHelp,0);
                output = outputHelp;

            }
            else
            {
                btn.BTNCalc(ref input, ref output);
            }
        }

        private static void WritePlotDataDuoHeightLength()
        {
            AllDUOPlots(ref InputGenerator.totalHeight, ref InputGenerator.totalLength, "heigth", "length");
        }
        private static void WritePlotDataDuoAll()
        {
            AllDUOPlots(ref InputGenerator.totalHeight, ref InputGenerator.totalLength, "heigth", "length");
            outputs.RemoveRange(0, InputGenerator.totalHeight.Count * InputGenerator.totalLength.Count);
            numDirectories++;
            AllDUOPlots(ref InputGenerator.finSpacings, ref InputGenerator.finThicknesses, "finSpacing", "finThickness");
            outputs.RemoveRange(0, InputGenerator.finSpacings.Count * InputGenerator.finThicknesses.Count);
            numDirectories++;
            AllDUOPlots(ref InputGenerator.numberOfRows, ref InputGenerator.tubeThicknesses, "rows", "tubeThickness");
            outputs.RemoveRange(0, InputGenerator.numberOfRows.Count * InputGenerator.tubeThicknesses.Count);
            numDirectories++;
            AllDUOPlots(ref InputGenerator.numberOfCircuits, ref InputGenerator.numberOfRows, "circuits", "rows");
            outputs.RemoveRange(0, InputGenerator.numberOfCircuits.Count * InputGenerator.numberOfRows.Count);
            numDirectories++;
        }

        private static void WritePlotDataNumberOfRows()
        {
            AllPlots(ref InputGenerator.numberOfRows, "numberRows");
        }
        private static void WritePlotDataNumberCircuits()
        {
            AllPlots(ref InputGenerator.numberOfCircuits, "numberCircuits");
        }
        private static void WritePlotDataHeight()
        {
            AllPlots(ref InputGenerator.totalHeight, "height");
        }
        private static void WritePlotDataLength()
        {
            AllPlots(ref InputGenerator.totalLength, "length");
        }
        private static void WritePlotDataFinSpacing()
        {
            AllPlots(ref InputGenerator.finSpacings,"finSpacing");
        }
        private static void WritePlotDataFinThickness()
        {
            AllPlots(ref InputGenerator.finThicknesses, "fin Thickness");
        }
        private static void WritePlotDataTubeThickness()
        {
            AllPlots(ref InputGenerator.tubeThicknesses, "tube Thickness");

        }
        private static void WritePlotDataFluidTempIn()
        {
            AllPlots(ref InputGenerator.fluidTempIns, "fluid TempIn");

        }
        private static void WritePlotDataAll()
        {
            AllPlots(ref InputGenerator.totalHeight,"height");
            outputs.RemoveRange(0, InputGenerator.totalHeight.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.totalLength, "length");
            outputs.RemoveRange(0, InputGenerator.totalLength.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.numberOfRows, "numberRows");
            outputs.RemoveRange(0, InputGenerator.numberOfRows.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.numberOfCircuits, "numberCircuits");
            outputs.RemoveRange(0, InputGenerator.numberOfCircuits.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.tubeThicknesses, "tubeThickness");
            outputs.RemoveRange(0, InputGenerator.tubeThicknesses.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.finThicknesses, "finThickness");
            outputs.RemoveRange(0, InputGenerator.finThicknesses.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.finSpacings, "finSpacing");
            outputs.RemoveRange(0, InputGenerator.finSpacings.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.fluidTempIns, "fuidTempIn");
            outputs.RemoveRange(0, InputGenerator.fluidTempIns.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.powers, "powers");
            outputs.RemoveRange(0, InputGenerator.powers.Count);
            numDirectories++;



        }
        private static void WritePlotDataSpecial()
        {
            AllPlots(ref InputGenerator.totalHeight, "height");
            outputs.RemoveRange(0, InputGenerator.totalHeight.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.totalLength, "length");
            outputs.RemoveRange(0, InputGenerator.totalLength.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.numberOfRows, "numberRows");
            outputs.RemoveRange(0, InputGenerator.numberOfRows.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.numberOfCircuits, "numberCircuits");
            outputs.RemoveRange(0, InputGenerator.numberOfCircuits.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.fluidTempIns, "fuidTempIn");
            outputs.RemoveRange(0, InputGenerator.fluidTempIns.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.finSpacings, "finSpacing");
            outputs.RemoveRange(0, InputGenerator.finSpacings.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.finThicknesses, "finThickness");
            outputs.RemoveRange(0, InputGenerator.finThicknesses.Count);
            numDirectories++;
            AllPlots(ref InputGenerator.powers, "powers");
            outputs.RemoveRange(0, InputGenerator.powers.Count);
            numDirectories++;


        }
        private static void WritePlotDataMultiSpecial()
        {
            if (false)
            {
                AllMultiPlots(ref InputGenerator.numberOfRows,ref InputGenerator.numberOfCircuits, "numberRows_x_numberCircuits");
                outputs.RemoveRange(0, InputGenerator.numberOfRows.Count* InputGenerator.numberOfCircuits.Count);
                numDirectories++;
            }
            if (true)
            {
                List<Tuple<List<string>, string>> lists = new List<Tuple<List<string>, string>>();
                lists.Add(Tuple.Create(InputGenerator.totalHeight,"h"));
                lists.Add(Tuple.Create(InputGenerator.totalLength, "l"));
                lists.Add(Tuple.Create(InputGenerator.numberOfRows, "nRows"));
                lists.Add(Tuple.Create(InputGenerator.numberOfCircuits, "nCircuits"));
                lists.Add(Tuple.Create(InputGenerator.finSpacings, "fSpac"));
                lists.Add(Tuple.Create(InputGenerator.finThicknesses, "fThick"));
                lists.Add(Tuple.Create(InputGenerator.fluidTempIns, "fTempIn"));
                lists.Add(Tuple.Create(InputGenerator.airMasses, "aMass"));
                string fieldname = lists.Aggregate("", (i, j) => i + (i == "" ? "" : "_") + j.Item2);
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                //if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);


                PlotMultiSpecial(ref lists, "APresDrop", 28, newPath, fieldname);
                PlotMultiSpecial(ref lists, "ASpeed", 25, newPath, fieldname);
                PlotMultiSpecial(ref lists, "ATempOut", 21, newPath, fieldname);
                PlotMultiSpecial(ref lists, "FPresDrop", 37, newPath, fieldname);
                PlotMultiSpecial(ref lists, "FSpeed", 36, newPath, fieldname);
                PlotMultiSpecial(ref lists, "FTempOut", 34, newPath, fieldname);
                PlotMultiSpecial(ref lists, "P", 53, newPath, fieldname);
                //PlotMultiSpecial(ref lists, "SF", 24, newPath, fieldname);

            }
        }
        private static void WritePlotDataErrorCode()
        {
            PlotErrorCodes(ref InputGenerator.errorCodes, "ErrorCode");
        }

        private static void PlotMultiSpecial(ref List<Tuple<List<string>,string>> lists, string plotname, int indexField, string newPath, string fieldname)
        {
            StreamWriter writer;
            try
            {
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_" + plotname + ".csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    foreach (var item in lists)
                    {
                        csv.WriteField(item.Item2);
                    }
                    csv.WriteField(plotname);
                    csv.WriteField("Error");
                    csv.WriteField("ErrorMessage");
                    csv.NextRecord();
                    List<string> outputAllFields = new List<string>();
                    PlotMultiSpecialRecursive(ref lists, indexField, csv, ref outputAllFields);

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        private static void PlotMultiSpecialRecursive(ref List<Tuple<List<string>, string>> lists, int indexField, CsvWriter csv, ref List<string> output, int indexList = 0)
        {
            if (lists.Count <= 0)
                return;
            if (lists.Count == 1)
            {
                for (int i = 0; i < lists[0].Item1.Count; i++)
                {
                    foreach (var o in output)
                    {
                        csv.WriteField(o);
                    }
                    csv.WriteField(lists[0].Item1[i]);
                    if (indexField == 24)
                    {
                        if (outputs[indexList+i].GetValue(24) != null && Convert.ToString(outputs[indexList+i].GetValue(24)) != "")
                        {
                            csv.WriteField(Convert.ToInt32(outputs[indexList + i].GetValue(24)) - 100);
                        }
                        else
                        {
                            csv.WriteField("");
                        }
                    }
                    else
                    {
                        csv.WriteField(outputs[indexList + i].GetValue(indexField));
                    }
                    csv.WriteField(outputs[indexList + i].GetValue(0));
                    csv.WriteField(outputs[indexList + i].GetValue(1));
                    csv.NextRecord();
                }
                return;
            }
            Tuple<List<string>, string> first = lists[0];
            lists.RemoveAt(0);
            int indexListhelp = lists.Aggregate(1,(i,j) => i*j.Item1.Count);
            for (int i = 0; i < first.Item1.Count; i++)
            {
                //csv.WriteField(first.Item1[i]);
                output.Add(first.Item1[i]);
                int idx = indexList + indexListhelp * i;
                PlotMultiSpecialRecursive(ref lists, indexField, csv, ref output ,idx);
                output.RemoveAt(output.Count -1);
            }
            lists.Insert(0, first);
        }
        private static void AllPlots(ref List<string> data, string fieldname = "")
        {
            PlotPrice(ref data, fieldname);
            PlotSafetyFactor(ref data, fieldname);
            PlotName(ref data, fieldname);
            PlotFluidSpeed(ref data, fieldname);
            PlotAirSpeed(ref data, fieldname);
            PlotAirPressureDrop(ref data, fieldname);
            PlotFluidPressureDrop(ref data, fieldname);
            PlotTogether(ref data, fieldname);
            PlotFluidTempOut(ref data, fieldname);
            PlotAirTempOut(ref data, fieldname);
        }
        public static void PlotFluidSpeed(ref List<string> loop, string fieldname ="")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\"+ (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories)
                    +"-"+fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\"+(fieldname == ""? "output": fieldname)+"_FluidSpeed.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(36));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotAirSpeed(ref List<string> loop, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_AirSpeed.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(25));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotName(ref List<string> loop, string fieldname="")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath+ "\\" + (fieldname == "" ? "output" : fieldname) + "_Name.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(2));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotSafetyFactor(ref List<string> loop, string fieldname="")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_SafetyFactor.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i<loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        if (outputs[i].GetValue(24) != null && Convert.ToString(outputs[i].GetValue(24)) != "")
                        {
                            csv.WriteField(Convert.ToInt32(outputs[i].GetValue(24)) - 100);
                        }
                        else
                        {
                            csv.WriteField("");
                        }
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotPrice(ref List<string> loop, string fieldname ="")
        { 
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_Price.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(53));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotAirPressureDrop(ref List<string> loop, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_AirPressureDrop.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value"); 
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(28));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotFluidPressureDrop(ref List<string> loop, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_FluidPressureDrop.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(37));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotFluidTempOut(ref List<string> loop, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_FluidTempOut.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(34));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotAirTempOut(ref List<string> loop, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_AirTempOut.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(21));
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotTogether(ref List<string> loop, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_Together.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname);
                    csv.WriteField("name");
                    csv.WriteField("SafetyFactor");
                    csv.WriteField("Price");
                    csv.WriteField("AirPressureDrop");
                    csv.WriteField("FluidPressureDrop");
                    csv.WriteField("FluidSpeed");
                    csv.WriteField("AirSpeed");
                    csv.WriteField("FluidTempOut");
                    csv.WriteField("Power");
                    csv.WriteField("sensible Power");
                    csv.WriteField("Fin spacing");
                    csv.WriteField("Fin thickness");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(2));
                        csv.WriteField(outputs[i].GetValue(24) != null && Convert.ToString(outputs[i].GetValue(24)) != "" ? ""+(Convert.ToInt32(outputs[i].GetValue(24)) - 100) : "");
                        csv.WriteField(outputs[i].GetValue(53));
                        csv.WriteField(outputs[i].GetValue(28));
                        csv.WriteField(outputs[i].GetValue(37));
                        csv.WriteField(outputs[i].GetValue(36));
                        csv.WriteField(outputs[i].GetValue(25));
                        csv.WriteField(outputs[i].GetValue(34));
                        csv.WriteField(outputs[i].GetValue(16));
                        csv.WriteField(outputs[i].GetValue(124));
                        csv.WriteField(outputs[i].GetValue(7));
                        csv.WriteField(outputs[i].GetValue(12));
                        csv.WriteField(outputs[i].GetValue(0) + " " +outputs[i].GetValue(1));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }

        private static void AllMultiPlots(ref List<string> data1, ref List<string> data2, string fieldname = "")
        {
            PlotMultiPrice(ref data1, ref data2, fieldname);
            PlotMultiSafetyfactor(ref data1, ref data2, fieldname);
            PlotMultiName(ref data1, ref data2, fieldname);
            PlotMultiFluidSpeed(ref data1, ref data2, fieldname);
            PlotMultiAirSpeed(ref data1, ref data2, fieldname);
            PlotMultiAirPressureDrop(ref data1, ref data2, fieldname);
            PlotMultiFluidPressureDrop(ref data1, ref data2, fieldname);
            PlotMultiTogether(ref data1, ref data2, fieldname);
            PlotMultiFluidTempOut(ref data1, ref data2, fieldname);
            PlotMultiAirTempOut(ref data1, ref data2, fieldname);
        }
        public static void PlotMultiFluidSpeed(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_FluidSpeed.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i*loop2.Count+j].GetValue(36));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiName(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_Name.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(2));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiAirSpeed(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_AirSpeed.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(25));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiSafetyfactor(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_Safetyfactor.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            if (outputs[i * loop2.Count + j].GetValue(24) != null && Convert.ToString(outputs[i * loop2.Count + j].GetValue(24)) != "")
                            {
                                csv.WriteField(Convert.ToInt32(outputs[i * loop2.Count + j].GetValue(24)) - 100);
                            }
                            else
                            {
                                csv.WriteField("");
                            }
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiPrice(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_Price.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(53));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiAirPressureDrop(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_AirPressureDrop.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(28));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiFluidPressureDrop(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_FluidPressureDrop.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(37));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiFluidTempOut(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_FluidTempOut.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(34));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiAirTempOut(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_AirTempOut.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("Value");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(21));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotMultiTogether(ref List<string> loop1, ref List<string> loop2, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories)
                    + "-" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_Together.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField(fieldname.Split('_').First());
                    csv.WriteField(fieldname.Split('_').Last());
                    csv.WriteField("name");
                    csv.WriteField("SafetyFactor");
                    csv.WriteField("Price");
                    csv.WriteField("AirPressureDrop");
                    csv.WriteField("FluidPressureDrop");
                    csv.WriteField("FluidSpeed");
                    csv.WriteField("AirSpeed");
                    csv.WriteField("FluidTempOut");
                    csv.WriteField("Power");
                    csv.WriteField("sensible Power");
                    csv.WriteField("Fin spacing");
                    csv.WriteField("Fin thickness");
                    csv.WriteField("Error");
                    csv.NextRecord();
                    for (int i = 0; i < loop1.Count; i++)
                    {
                        for (int j = 0; j < loop2.Count; j++)
                        {
                            csv.WriteField(loop1[i]);
                            csv.WriteField(loop2[j]);
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(2));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(24) != null && Convert.ToString(outputs[i * loop2.Count + j].GetValue(24)) != "" ? "" + (Convert.ToInt32(outputs[i * loop2.Count + j].GetValue(24)) - 100) : "");
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(53));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(28));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(37));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(36));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(25));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(34));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(16));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(124));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(7));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(12));
                            csv.WriteField(outputs[i * loop2.Count + j].GetValue(0) + " " + outputs[i * loop2.Count + j].GetValue(1));
                            csv.NextRecord();
                        }
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotErrorCodes(ref List<string> loop, string fieldname = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + fieldname;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname == "" ? "output" : fieldname) + "_ErrorCodes.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    for (int i = 0; i < loop.Count; i++)
                    {
                        csv.WriteField(loop[i]);
                        csv.WriteField(outputs[i].GetValue(0));
                        csv.WriteField(outputs[i].GetValue(1));
                        csv.NextRecord();
                    }

                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }

        private static void AllDUOPlots(ref List<string> data1, ref List<string> data2, string fieldname1 = "", string fieldname2 = "")
        {
            PlotDUOPrice(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOSafetyFactor(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOName(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOfinnedHeight(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOfinnedLength(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOPressurLossAir(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOPressurLossFluid(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOFluidTempOut(ref data1, ref data2, fieldname1, fieldname2);
            PlotDUOFluidErrorCode(ref data1, ref data2, fieldname1, fieldname2);
        }
        public static void PlotDUOPrice(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1="", string fieldname2="")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname1+fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2== ""? "output" : fieldname1+fieldname2) + "_DuoPrice.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(53));
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOSafetyFactor(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_DuoSafetyFactor.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            if (outputs[outerLoop.Count * j + i].GetValue(24) != null && Convert.ToString(outputs[outerLoop.Count * j + i].GetValue(24)) != "")
                            { 
                                csv.WriteField(Convert.ToInt32(outputs[outerLoop.Count * j + i].GetValue(24))-100);
                            }
                            else
                            {
                                csv.WriteField("");
                            }
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOName(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_DuoName.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(2));
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOPressurLossAir(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath+ "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_DuoPressurLossAir.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(28));
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOPressurLossFluid(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_DuoPressureLossFluid.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(37));
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOfinnedHeight(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_DuofinnedHeight.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(61));
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOfinnedLength(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0"+numDirectories : ""+numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_DuofinnedLength.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(66));
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOFluidTempOut(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_DuoFluidTempOut.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(34));
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }
        public static void PlotDUOFluidErrorCode(ref List<string> outerLoop, ref List<string> innerLoop, string fieldname1 = "", string fieldname2 = "")
        {
            StreamWriter writer;
            try
            {
                string newPath = Utility.outputFolderPath + "\\" + (numDirectories < 10 ? "0" + numDirectories : "" + numDirectories) + "-" + fieldname1 + fieldname2;
                Directory.CreateDirectory(newPath);
                writer = new StreamWriter(newPath + "\\" + (fieldname1 == "" && fieldname2 == "" ? "output" : fieldname1 + fieldname2) + "_ErrorCode.csv");
                Utility.numberInOut++;
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteField("");
                    for (int i = 0; i < innerLoop.Count; i++)
                    {
                        csv.WriteField(innerLoop[i]);
                    }
                    csv.WriteField(fieldname2);
                    csv.NextRecord();
                    for (int i = 0; i < outerLoop.Count; i++)
                    {
                        csv.WriteField(outerLoop[i]);
                        for (int j = 0; j < innerLoop.Count; j++)
                        {
                            if (outputs[outerLoop.Count * j + i].GetValue(0) != null)
                            {
                                csv.WriteField(outputs[outerLoop.Count * j + i].GetValue(0)+" - "+ outputs[outerLoop.Count * j + i].GetValue(1));
                            }
                            else
                            {
                                csv.WriteField("");
                            }
                        }
                        csv.NextRecord();
                    }
                    csv.WriteField(fieldname1);
                }
                writer.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not write output file, Error: " + e.Message);
            }
        }

    }


    public static class InputParser
    {
        public static void ParseBlackboxSingle(string filename, ref Array input)
        {
            StreamReader file;
            try
            {
                file = new StreamReader(filename);

                string line;
                object[] intakeOuttake = new object[Utility.lengthInput];

                for (int counter = 0; counter < Utility.lengthInput && (line = file.ReadLine()) != null; counter++)
                {

                    Utility.EatWhitespaces(ref line);
                    intakeOuttake[counter] = line;
                }
                input = intakeOuttake;
                file.Close();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not load file, Error: " + e.Message);
            }
        }

    }


    public static class Utility
    {
        //----------------
        public static int lengthInput = 60;
        public static int lengthOutput = 200;
        public static int numberInOut = 0;  //for naming files
        public static int numberOfInputs = 1000;
        public static string inputFolderPath = "..\\..\\..\\..\\input";
        public static string outputFolderPath = "..\\..\\..\\..\\output";
        //----------------
        public static void EatWhitespaces(ref string s)
        {
            s = new string(s.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }
    }


    public static class InputGenerator {


        public static object[] input_1 = new object[Utility.lengthInput];
        public static object[] input_2 = new object[Utility.lengthInput];
        public static object[] input_3 = new object[Utility.lengthInput];
        public static object[] input_4 = new object[Utility.lengthInput];
        public static object[] input_5 = new object[Utility.lengthInput];
        public static object[] input_6 = new object[Utility.lengthInput];
        public static object[] input_7 = new object[Utility.lengthInput];

        public static int number = 0;

        public class Bound
        {
            public int Heightstart { get; set; }
            public int Heightend { get; set; }
            public int Heightsteps { get; set; }
            public int Lengthstart { get; set; }
            public int Lengthend { get; set; }
            public int Lengthsteps { get; set; }
        }

        public static List<Bound> inputBounds = new List<Bound>();

        public static void Initialize()
        {
            #region input1

            inputBounds.Add(new Bound { Heightstart = 100, Heightend = 2750, Heightsteps = 1, Lengthstart = 100, Lengthend = 4200, Lengthsteps = 5});

            input_1[0] = "8";
            input_1[1] = "0";
            input_1[2] = "1";
            input_1[3] = "2750";
            input_1[4] = "3360";
            input_1[5] = "";
            input_1[6] = "";
            input_1[7] = "";
            input_1[8] = "";
            input_1[9] = "0.35";
            input_1[10] = "38";
            input_1[11] = "30";
            input_1[12] = "12";
            input_1[13] = "1";
            input_1[14] = "21.7";
            input_1[15] = "";
            input_1[16] = "25";
            input_1[17] = "";
            input_1[18] = "";
            input_1[19] = "";
            input_1[20] = "";
            input_1[21] = "";
            input_1[22] = "0";
            input_1[23] = "60000";
            input_1[24] = "1013";
            input_1[25] = "0";
            input_1[26] = "";
            input_1[27] = "C";
            input_1[28] = "5";
            input_1[29] = "0";
            input_1[30] = "80";
            input_1[31] = "18.6";
            input_1[32] = "2";
            input_1[33] = "1";
            input_1[34] = "N5H8L-C857X-7NR2R";
            input_1[35] = "1";
            input_1[36] = "2";
            input_1[37] = "19500";
            input_1[38] = "";
            input_1[39] = "0";
            input_1[40] = "1";
            input_1[41] = "1";
            input_1[42] = @"C:\Program Files (x86)\BATLLN";
            input_1[43] = "2";
            input_1[44] = "1";
            input_1[45] = "0000000AF";
            input_1[46] = "15";
            input_1[47] = "15";
            input_1[48] = "0.015";
            input_1[49] = "0";
            input_1[50] = "0";
            input_1[51] = "";
            input_1[52] = "";
            input_1[53] = "0";
            input_1[54] = "0";
            input_1[55] = "";
            input_1[56] = "";
            input_1[57] = "";
            input_1[58] = "";
            input_1[59] = "0";

            #endregion
            #region input2

            inputBounds.Add(new Bound { Heightstart = 2550, Heightend = 2750, Heightsteps = 20, Lengthstart = 3160, Lengthend = 3360, Lengthsteps = 20 });

            input_2[0] = "8";
            input_2[1] = "0";
            input_2[2] = "1";
            input_2[3] = "2750";
            input_2[4] = "3360";
            input_2[5] = "";
            input_2[6] = "";
            input_2[7] = "";
            input_2[8] = "";
            input_2[9] = "0.35";
            input_2[10] = "38";
            input_2[11] = "30";
            input_2[12] = "12";
            input_2[13] = "1";
            input_2[14] = "21.7";
            input_2[15] = "";
            input_2[16] = "25";
            input_2[17] = "";
            input_2[18] = "";
            input_2[19] = "";
            input_2[20] = "";
            input_2[21] = "";
            input_2[22] = "0";
            input_2[23] = "60000";
            input_2[24] = "1013";
            input_2[25] = "0";
            input_2[26] = "";
            input_2[27] = "C";
            input_2[28] = "5";
            input_2[29] = "0";
            input_2[30] = "80";
            input_2[31] = "18.6";
            input_2[32] = "2";
            input_2[33] = "1";
            input_2[34] = "N5H8L-C857X-7NR2R";
            input_2[35] = "1";
            input_2[36] = "1";
            input_2[37] = "19500";
            input_2[38] = "";
            input_2[39] = "0";
            input_2[40] = "1";
            input_2[41] = "1";
            input_2[42] = @"C:\Program Files (x86)\BATLLN";
            input_2[43] = "0";
            input_2[44] = "1";
            input_2[45] = "0000000AN";
            input_2[46] = "15";
            input_2[47] = "15";
            input_2[48] = "";
            input_2[49] = "0";
            input_2[50] = "0";
            input_2[51] = "";
            input_2[52] = "";
            input_2[53] = "0";
            input_2[54] = "0";
            input_2[55] = "";
            input_2[56] = "";
            input_2[57] = "";
            input_2[58] = "";
            input_2[59] = "1";

            #endregion
            #region input3

            inputBounds.Add(new Bound { Heightstart = 100, Heightend = 2750, Heightsteps = 1, Lengthstart = 100, Lengthend = 4200, Lengthsteps = 1 });

            input_3[0] = "8";
            input_3[1] = "0";
            input_3[2] = "1";
            input_3[3] = "1254";
            input_3[4] = "1524";
            input_3[5] = "";
            input_3[6] = "";
            input_3[7] = "";
            input_3[8] = "";
            input_3[9] = "0.35";
            input_3[10] = "8";
            input_3[11] = "30";
            input_3[12] = "12";
            input_3[13] = "1";
            input_3[14] = "21.5";
            input_3[15] = "";
            input_3[16] = "25";
            input_3[17] = "";
            input_3[18] = "";
            input_3[19] = "";
            input_3[20] = "";
            input_3[21] = "";
            input_3[22] = "0";
            input_3[23] = "12000";
            input_3[24] = "1013";
            input_3[25] = "0";
            input_3[26] = "";
            input_3[27] = "C";
            input_3[28] = "5";
            input_3[29] = "0";
            input_3[30] = "80";
            input_3[31] = "18.6";
            input_3[32] = "2";
            input_3[33] = "3";
            input_3[34] = "N5H8L-C857X-7NR2R";
            input_3[35] = "1";
            input_3[36] = "2";
            input_3[37] = "3900";
            input_3[38] = "";
            input_3[39] = "0";
            input_3[40] = "1";
            input_3[41] = "1";
            input_3[42] = @"C:\Program Files (x86)\BATLLN";
            input_3[43] = "0";
            input_3[44] = "1";
            input_3[45] = "0000000AF";
            input_3[46] = "15";
            input_3[47] = "15";
            input_3[48] = "0.015";
            input_3[49] = "0";
            input_3[50] = "0";
            input_3[51] = "";
            input_3[52] = "";
            input_3[53] = "0";
            input_3[54] = "0";
            input_3[55] = "";
            input_3[56] = "";
            input_3[57] = "";
            input_3[58] = "";
            input_3[59] = "2";


            #endregion
            #region input4

            inputBounds.Add(new Bound { Heightstart = 2550, Heightend = 2750, Heightsteps = 5, Lengthstart = 3160, Lengthend = 3360, Lengthsteps = 5 });

            input_4[0] = "8";
            input_4[1] = "0";
            input_4[2] = "1";
            input_4[3] = "2750";
            input_4[4] = "3360";
            input_4[5] = "";
            input_4[6] = "";
            input_4[7] = "";
            input_4[8] = "";
            input_4[9] = "0.35";
            input_4[10] = "38";
            input_4[11] = "30";
            input_4[12] = "12";
            input_4[13] = "1";
            input_4[14] = "8.2";
            input_4[15] = "";
            input_4[16] = "25";
            input_4[17] = "";
            input_4[18] = "";
            input_4[19] = "";
            input_4[20] = "";
            input_4[21] = "";
            input_4[22] = "0";
            input_4[23] = "60000";
            input_4[24] = "1013";
            input_4[25] = "0";
            input_4[26] = "";
            input_4[27] = "R";
            input_4[28] = "25";
            input_4[29] = "0";
            input_4[30] = "20";
            input_4[31] = "11.4";
            input_4[32] = "2";
            input_4[33] = "1";
            input_4[34] = "N5H8L-C857X-7NR2R";
            input_4[35] = "1";
            input_4[36] = "2";
            input_4[37] = "19500";
            input_4[38] = "";
            input_4[39] = "0";
            input_4[40] = "1";
            input_4[41] = "1";
            input_4[42] = @"C:\Program Files (x86)\BATLLN";
            input_4[43] = "0";
            input_4[44] = "1";
            input_4[45] = "0000000AF";
            input_4[46] = "15";
            input_4[47] = "15";
            input_4[48] = "0.015";
            input_4[49] = "0";
            input_4[50] = "0";
            input_4[51] = "";
            input_4[52] = "";
            input_4[53] = "0";
            input_4[54] = "0";
            input_4[55] = "";
            input_4[56] = "";
            input_4[57] = "";
            input_4[58] = "";
            input_4[59] = "3";

            #endregion
            #region input5

            inputBounds.Add(new Bound { Heightstart = 2550, Heightend = 2750, Heightsteps = 5, Lengthstart = 3160, Lengthend = 3360, Lengthsteps = 5 });

            input_5[0] = "8";
            input_5[1] = "0";
            input_5[2] = "1";
            input_5[3] = "2750";
            input_5[4] = "3360";
            input_5[5] = "";
            input_5[6] = "";
            input_5[7] = "";
            input_5[8] = "";
            input_5[9] = "0.35";
            input_5[10] = "46";
            input_5[11] = "30";
            input_5[12] = "12";
            input_5[13] = "1";
            input_5[14] = "21.7";
            input_5[15] = "";
            input_5[16] = "25";
            input_5[17] = "";
            input_5[18] = "";
            input_5[19] = "";
            input_5[20] = "";
            input_5[21] = "";
            input_5[22] = "0";
            input_5[23] = "60000";
            input_5[24] = "1013";
            input_5[25] = "0";
            input_5[26] = "";
            input_5[27] = "C";
            input_5[28] = "5";
            input_5[29] = "0";
            input_5[30] = "80";
            input_5[31] = "18.6";
            input_5[32] = "2";
            input_5[33] = "1";
            input_5[34] = "N5H8L-C857X-7NR2R";
            input_5[35] = "1";
            input_5[36] = "2";
            input_5[37] = "19500";
            input_5[38] = "";
            input_5[39] = "0";
            input_5[40] = "1";
            input_5[41] = "1";
            input_5[42] = @"C:\Program Files (x86)\BATLLN";
            input_5[43] = "0";
            input_5[44] = "1";
            input_5[45] = "0000000AF";
            input_5[46] = "15";
            input_5[47] = "15";
            input_5[48] = "0.015";
            input_5[49] = "0";
            input_5[50] = "0";
            input_5[51] = "";
            input_5[52] = "";
            input_5[53] = "0";
            input_5[54] = "0";
            input_5[55] = "";
            input_5[56] = "";
            input_5[57] = "";
            input_5[58] = "";
            input_5[59] = "4";

            #endregion
            #region input6

            inputBounds.Add(new Bound { Heightstart = 2550, Heightend = 2750, Heightsteps = 5, Lengthstart = 3160, Lengthend = 3360, Lengthsteps = 5 });

            input_6[0] = "8";
            input_6[1] = "0";
            input_6[2] = "1";
            input_6[3] = "2750";
            input_6[4] = "3360";
            input_6[5] = "";
            input_6[6] = "";
            input_6[7] = "";
            input_6[8] = "";
            input_6[9] = "0.25";
            input_6[10] = "20";
            input_6[11] = "40";
            input_6[12] = "8";
            input_6[13] = "1";
            input_6[14] = "15S";
            input_6[15] = "";
            input_6[16] = "25";
            input_6[17] = "";
            input_6[18] = "";
            input_6[19] = "";
            input_6[20] = "";
            input_6[21] = "";
            input_6[22] = "0";
            input_6[23] = "60000";
            input_6[24] = "1013";
            input_6[25] = "0";
            input_6[26] = "";
            input_6[27] = "C";
            input_6[28] = "5";
            input_6[29] = "0";
            input_6[30] = "80";
            input_6[31] = "18.6";
            input_6[32] = "2";
            input_6[33] = "1";
            input_6[34] = "N5H8L-C857X-7NR2R";
            input_6[35] = "1";
            input_6[36] = "2";
            input_6[37] = "19500";
            input_6[38] = "";
            input_6[39] = "0";
            input_6[40] = "1";
            input_6[41] = "1";
            input_6[42] = @"C:\Program Files (x86)\BATLLN";
            input_6[43] = "0";
            input_6[44] = "1";
            input_6[45] = "0000000AF";
            input_6[46] = "15";
            input_6[47] = "15";
            input_6[48] = "0.015";
            input_6[49] = "0";
            input_6[50] = "0";
            input_6[51] = "";
            input_6[52] = "";
            input_6[53] = "0";
            input_6[54] = "0";
            input_6[55] = "";
            input_6[56] = "";
            input_6[57] = "";
            input_6[58] = "";
            input_6[59] = "4";

            #endregion
            #region input7

            inputBounds.Add(new Bound { Heightstart = 2750, Heightend = 2750, Heightsteps = 1, Lengthstart = 3666, Lengthend = 3666, Lengthsteps = 1 });

            input_7[0] = "8";
            input_7[1] = "0";
            input_7[2] = "1";
            input_7[3] = "2750";
            input_7[4] = "3360";
            input_7[5] = "";
            input_7[6] = "";
            input_7[7] = "";
            input_7[8] = "";
            input_7[9] = "0.35";
            input_7[10] = "30";
            input_7[11] = "18";
            input_7[12] = "12";
            input_7[13] = "1";
            input_7[14] = "21";
            input_7[15] = "";
            input_7[16] = "25";
            input_7[17] = "";
            input_7[18] = "";
            input_7[19] = "";
            input_7[20] = "";
            input_7[21] = "";
            input_7[22] = "0";
            input_7[23] = "18000";
            input_7[24] = "1013";
            input_7[25] = "0";
            input_7[26] = "";
            input_7[27] = "C";
            input_7[28] = "5";
            input_7[29] = "0";
            input_7[30] = "80";
            input_7[31] = "18.6";
            input_7[32] = "2";
            input_7[33] = "2";
            input_7[34] = "N5H8L-C857X-7NR2R";
            input_7[35] = "1";
            input_7[36] = "2";
            input_7[37] = "9800";
            input_7[38] = "";
            input_7[39] = "0";
            input_7[40] = "1";
            input_7[41] = "1";
            input_7[42] = @"C:\Program Files (x86)\BATLLN";
            input_7[43] = "2";
            input_7[44] = "1";
            input_7[45] = "0000000AF";
            input_7[46] = "15";
            input_7[47] = "15";
            input_7[48] = "0.015";
            input_7[49] = "0";
            input_7[50] = "0";
            input_7[51] = "";
            input_7[52] = "";
            input_7[53] = "0";
            input_7[54] = "0";
            input_7[55] = "";
            input_7[56] = "";
            input_7[57] = "";
            input_7[58] = "";
            input_7[59] = "0";

            #endregion

        }



        public static List<string> totalHeight = new List<string>();
        public static List<string> totalLength = new List<string>();
        public static List<string> numberOfRows = new List<string>();
        public static List<string> numberOfCircuits = new List<string>();
        public static List<string> finSpacings = new List<string>();
        public static List<string> finThicknesses = new List<string>();
        public static List<string> tubeThicknesses = new List<string>();
        public static List<string> fluidTempIns = new List<string>();
        public static List<string> errorCodes = new List<string>();
        public static List<string> powers = new List<string>();
        public static List<string> specials = new List<string>();
        public static List<string> airMasses = new List<string>();
        public static void Clear()
        {
            totalHeight.Clear();
            totalLength.Clear();
            numberOfRows.Clear();
            numberOfCircuits.Clear();
            finSpacings.Clear();
            finThicknesses.Clear();
            tubeThicknesses.Clear();
            fluidTempIns.Clear();
            errorCodes.Clear();
            powers.Clear();
            specials.Clear();
            inputBounds.Clear();
        }

        public static void PopulateInputListDuoHeightLength(ref List<Array> inputs, ref object[] original)
        {
            int idx = Convert.ToInt32(original.Last());

            totalHeight.Clear();
            totalLength.Clear();

            for (int i = inputBounds[idx].Heightstart; i <= inputBounds[idx].Heightend; i+=inputBounds[idx].Heightsteps)
            {
                totalHeight.Add(""+i);
            }

            for (int i = inputBounds[idx].Lengthstart; i <= inputBounds[idx].Lengthend ; i+=inputBounds[idx].Lengthsteps)
            {
                totalLength.Add(""+i);
            }


            foreach (var length in totalLength)
            {
                foreach (var height in totalHeight)
                {
                    object[] input = new object[Utility.lengthInput];
                    Array.Copy(original, 0, input, 0, original.Length);

                    Console.WriteLine("Populating: " + number);
                    input[3] = height;
                    input[4] = length;
                    input[59] = ""+number;

                    inputs.Add(input);
                    number++;
                }
            }
        }
        public static void PopulateInputListDuoNumberRowsTubeThickness(ref List<Array> inputs, ref object[] original)
        {
            tubeThicknesses.Clear();
            numberOfRows.Clear();

            tubeThicknesses.Add("0.15");
            tubeThicknesses.Add("0.25");
            tubeThicknesses.Add("0.35");    //only one tube Thickness?
            tubeThicknesses.Add("0.40");
            tubeThicknesses.Add("0.5");
            tubeThicknesses.Add("1");


            for (int i = 0; i <= 21; i += 1)
            {
                numberOfRows.Add("" + i);
            }




            foreach (var row in numberOfRows)
            {
                foreach (var tube in tubeThicknesses)
                {
                    object[] input = new object[Utility.lengthInput];
                    Array.Copy(original, 0, input, 0, original.Length);

                    Console.WriteLine("Populating: " + number);
                    input[9] = tube;
                    input[12] = row;
                    input[59] = "" + number;

                    inputs.Add(input);
                    number++;
                }
            }
        }
        public static void PopulateInputListDuoNumberCircuitsNumberRows(ref List<Array> inputs, ref object[] original)
        {
            numberOfRows.Clear();
            numberOfCircuits.Clear();

            for (int i = 0; i <= 21; i += 1)
            {
                numberOfRows.Add("" + i);
            }

            for (int i = 19; i <= 80; i++)
            {
                numberOfCircuits.Add("" + i);
            }


            foreach (var row in numberOfRows)
            {
                foreach (var circuit in numberOfCircuits)
                {
                    object[] input = new object[Utility.lengthInput];
                    Array.Copy(original, 0, input, 0, original.Length);

                    Console.WriteLine("Populating: " + number);
                    input[10] = circuit;
                    input[12] = row;
                    input[59] = "" + number;

                    inputs.Add(input);
                    number++;
                }
            }
        }
        public static void PopulateInputListDuoFinSpacingFinThickness(ref List<Array> inputs, ref object[] original)
        {
            finSpacings.Clear();
            finThicknesses.Clear();

            finSpacings.Add("15");  //not existing
            finSpacings.Add("18");
            finSpacings.Add("21");
            finSpacings.Add("25");
            finSpacings.Add("30");      //FS: 12
            finSpacings.Add("40");
            finSpacings.Add("45");  //not existing
            finSpacings.Add("50");
            finSpacings.Add("60");
            finSpacings.Add("70");   //not existing
            finSpacings.Add("10000"); //only for 5/8" tube with 0.89 mm thickness
            finSpacings.Add("20000");   //not existing


            for (int i = 0; i <= 5; i += 1)
            {
                finThicknesses.Add("" + i);
            }   

            foreach (var finSpacing in finSpacings)
            {
                foreach (var finThickness in finThicknesses)
                {
                    object[] input = new object[Utility.lengthInput];
                    Array.Copy(original, 0, input, 0, original.Length);

                    Console.WriteLine("Populating: " + number);
                    input[11] = finSpacing;
                    input[33] = finThickness;
                    input[59] = "" + number;

                    inputs.Add(input);
                    number++;
                }
            }
        }

        public static void PopulateInputnumberOfRows(ref List<Array> inputs, ref object[] original)
        {
            for (int i = 0; i <= 21; i += 1)
            {
                numberOfRows.Add("" + i);
            }

            foreach (var item in numberOfRows)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[12] = item;
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputnumberOfCircuits(ref List<Array> inputs, ref object[] original)
        {
            for (int i = 1; i <= 150; i++)
            {
               numberOfCircuits.Add(""+i);
            }

            foreach (var item in numberOfCircuits)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[10] = item;
                input[57] = "numberCircuits";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputHeight(ref List<Array> inputs, ref object[] original)
        {
            int idx = Convert.ToInt32(original.Last());
            for (int i = inputBounds[idx].Heightstart; i <= inputBounds[idx].Heightend; i += inputBounds[idx].Heightsteps)
            {
                totalHeight.Add("" + i);
            }

            foreach (var item in totalHeight)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[3] = item;
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputLength(ref List<Array> inputs, ref object[] original)
        {
            int idx = Convert.ToInt32(original.Last());
            for (int i = inputBounds[idx].Lengthstart; i <= inputBounds[idx].Lengthend; i += inputBounds[idx].Lengthsteps)
            {
                totalLength.Add("" + i);
            }

            foreach (var item in totalLength)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[4] = item;
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputFinThickness(ref List<Array> inputs, ref object[] original)
        {
            for (int i = 0; i <= 10; i += 1)
            {
                finThicknesses.Add("" + i);
            }

            foreach (var item in finThicknesses)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[33] = item;
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputFinSpacing(ref List<Array> inputs, ref object[] original)
        {
            finSpacings.Add("15");  //not existing
            finSpacings.Add("18");
            finSpacings.Add("21");
            finSpacings.Add("25");
            finSpacings.Add("30");      //FS: 12
            finSpacings.Add("40");
            finSpacings.Add("45");  //not existing
            finSpacings.Add("50");
            finSpacings.Add("60");
            finSpacings.Add("70");   //not existing
            finSpacings.Add("10000"); //only for 5/8" tube with 0.89 mm thickness
            finSpacings.Add("20000");   //not existing

            foreach (var item in finSpacings)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[11] = item;
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputTubeThickness(ref List<Array> inputs, ref object[] original)
        {
            tubeThicknesses.Add("0.15");
            tubeThicknesses.Add("0.25");
            tubeThicknesses.Add("0.35");    //only one tube thickness??
            tubeThicknesses.Add("0.40");
            tubeThicknesses.Add("0.5");
            tubeThicknesses.Add("1");

            foreach (var item in tubeThicknesses)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[9] = item;
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputFluidTempIn(ref List<Array> inputs, ref object[] original)
        {
            for (int i = 0; i <= 60; i++)
            {
                fluidTempIns.Add(""+(i-20));
            }

            foreach (var item in fluidTempIns)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[14] = item;
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputSpecial(ref List<Array> inputs, ref object[] original)
        {
            inputBounds.Add(new Bound { Heightstart = 100, Heightend = 2750, Heightsteps = 1, Lengthstart = 100, Lengthend = 4200, Lengthsteps = 1 });
            for (int i = inputBounds.Last().Heightstart; i <= inputBounds.Last().Heightend; i += inputBounds.Last().Heightsteps)
                totalHeight.Add(""+i);
            for (int i = inputBounds.Last().Lengthstart; i <= inputBounds.Last().Lengthend; i += inputBounds.Last().Lengthsteps)
                totalLength.Add("" + i);
            for (int i = 1; i <= 20; i++)
                numberOfRows.Add("" + i);
            for (int i = -20; i <= 40; i++)
                fluidTempIns.Add(""+ i);
            finSpacings = new List<string> { "18", "21", "25", "30", "40", "50"};
            finThicknesses = new List<string> { "2", "1", "3" };
            for (int i = 1; i <= 150; i++)
                numberOfCircuits.Add("" + i);
            for (int i = 0; i <= 400; i++)
                powers.Add(""+i);

            foreach (var item in totalHeight)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[3] = item;
                input[57] = "height";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
            foreach (var item in totalLength)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[4] = item;
                input[57] = "length";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
            foreach (var item in numberOfRows)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[12] = item;
                input[57] = "numberRows";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
            foreach (var item in numberOfCircuits)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[10] = item;
                input[57] = "numberCircuits";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
            foreach (var item in fluidTempIns)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[14] = item;
                input[57] = "fluidTempsIn";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
            foreach (var item in finSpacings)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[11] = item;
                input[57] = "finSpacing";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
            foreach (var item in finThicknesses)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[33] = item;
                input[57] = "finThickness";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
            foreach (var item in powers)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                Console.WriteLine("Populating: " + number);

                input[26] = item;
                input[31] = "";
                input[57] = "power";
                input[59] = "" + number;

                inputs.Add(input);
                number++;
            }
        }
        public static void PopulateInputMultiSpecial(ref List<Array> inputs, ref object[] original, bool isIntake, int hc)
        {
            inputBounds.Add(new Bound { Heightstart = 250, Heightend = 2750, Heightsteps = 500, Lengthstart = 200, Lengthend = 4200, Lengthsteps = 800 });
  

            List<Tuple<List<string>, int>> lists = new List<Tuple<List<string>, int>>();
            List<string> names = new List<string>();

            if (isIntake)
            {
                original[27] = "C";
                original[28] = "" + 5;
                original[30] = "" + 80;
                fluidTempIns = new List<string> { "18", "19", "20", "21", "22", "23" };
            }
            else
            {
                original[27] = "R";
                original[28] = "" + 25;
                original[30] = "" + 20;
                fluidTempIns = new List<string> { "6", "7", "8", "9", "10", "11" };
            }

            if (false)
            {
                if(original[3].ToString() == "0")
                {
                    for (int i = inputBounds.Last().Heightstart; i <= inputBounds.Last().Heightend; i += inputBounds.Last().Heightsteps)
                        totalHeight.Add("" + i);
                }
                else
                {
                    totalHeight.Add("" + original[3]);
                }
                if (original[4] .ToString()== "0")
                {
                    for (int i = inputBounds.Last().Lengthstart; i <= inputBounds.Last().Lengthend; i += inputBounds.Last().Lengthsteps)
                        totalLength.Add("" + i);
                }
                else
                {
                    totalLength.Add("" + original[4]);
                }
                if (original[12].ToString() == "0")
                {
                    for (int i = 6; i <= 14; i += 2)
                        numberOfRows.Add("" + i);
                }
                else
                {
                    numberOfRows.Add("" + original[12]);
                }
                if (original[10].ToString() == "0")
                {
                    for (int i = 20; i <= 60; i += 10)
                        numberOfCircuits.Add("" + i);
                }
                else
                {
                    numberOfCircuits.Add("" + original[10]);
                }
                if (original[11].ToString() == "0")
                {
                    finSpacings = new List<string> {"25", "30", "40", "50", "60"};
                }
                else
                {
                    finSpacings.Add("" + original[11]);
                }
                if (original[33].ToString() == "0")
                {
                    finThicknesses = new List<string> { "1", "2", "3"}; // 1,2,3
                }
                else
                {
                    finThicknesses.Add("" + original[33]);
                }
                if (original[14].ToString() == "0")
                {
                    for (int i = 16; i <= 25; i += 3)
                        fluidTempIns.Add("" + i);
                }
                else
                {
                    fluidTempIns.Add("" + original[14]);
                }
                if (original[23].ToString() == "0")
                {
                    for (int i = 10000; i <= 150000; i += 20000)
                        airMasses.Add("" + i);
                }
                else
                {
                    airMasses.Add("" + original[23]);
                }

                powers = new List<string> { "10", "80", "150", "220", "300" };
            } else
            {
                totalHeight = new List<string> { ""+original[3]};
                totalLength = new List<string> { ""+original[4]};
                finThicknesses = new List<string> { ""+original[33]};
                int airSize = 3;
                if (hc == 4) 
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "1", "2", "3", "4", "5"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 1000;
                    int step = 100;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 6) 
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "1", "2", "3", "4", "5", "6", "7"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 2000;
                    int step = 250;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 9) 
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8" };
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 4000;
                    int step = 500;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 12) //DN20
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 6000;
                    int step = 500;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 16)
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "3", "4", "5", "6", "7", "8", "9", "10"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 9000;
                    int step = 750;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 20) //DN25
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> {"5", "6", "7", "8", "9", "10", "11", "12"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 12000;
                    int step = 1000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 25)
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "7", "8", "9", "10", "11", "12", "13", "14"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 15000;
                    int step = 1000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 30) //DN32
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "9", "10", "11", "12", "13", "14", "15", "16"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 18000;
                    int step = 1000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 36)
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "10", "11", "12", "13", "14", "15", "16", "17"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 21000;
                    int step = 1000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 42) //DN 40
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> {"10", "12", "14", "16", "18", "20", "22", "16"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 25000;
                    int step = 1000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 49) //DN 40
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> {"13", "15", "17", "19", "21", "23", "25", "27"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 30000;
                    int step = 2000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 56)
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> {"16", "18", "20", "22", "24", "26", "28", "30"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 35000;
                    int step = 2000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 64)
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "19", "21", "23", "25", "27", "29", "31", "33"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 40000;
                    int step = 2000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 72) //DN65/2
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "22", "24", "26", "28", "30", "32", "34", "36" };
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 45000;
                    int step = 2000;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 80) //DN65
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "26", "28", "30", "32", "34", "36", "38", "40" };
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 50000;
                    int step = 2500;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 90) //DN80/2
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "26", "29", "32", "35", "38", "41", "44", "47" };
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 55000;
                    int step = 2500;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 100) //DN65
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14"};
                    numberOfCircuits = new List<string> { "29", "32", "35", "38", "41", "44", "47", "51"};
                    finSpacings = new List<string> { "25", "30", "40", "50", "60"};
                    int air = 60000;
                    int step = 2500;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
                if (hc == 110) //DN65
                {
                    numberOfRows = new List<string> { "6", "8", "10", "12", "14" };
                    numberOfCircuits = new List<string> { "32", "35", "38", "41", "44", "47", "51", "54" };
                    finSpacings = new List<string> { "25", "30", "40", "50", "60" };
                    int air = 65000;
                    int step = 2500;
                    airMasses = new List<string>();
                    for (int i = -airSize; i <= airSize; i++)
                        airMasses.Add("" + (air + step * i));
                }
            }


            lists.Add(Tuple.Create(totalHeight,3));
            names.Add("height");
            lists.Add(Tuple.Create(totalLength, 4));
            names.Add("length");
            lists.Add(Tuple.Create(numberOfRows, 12));
            names.Add("numberRows");
            lists.Add(Tuple.Create(numberOfCircuits, 10));
            names.Add("numberCircuits");
            lists.Add(Tuple.Create(finSpacings, 11));
            names.Add("finSpacings");
            lists.Add(Tuple.Create(finThicknesses, 33));
            names.Add("finThickness");
            lists.Add(Tuple.Create(fluidTempIns, 14));
            names.Add("fluidTempIns");
            lists.Add(Tuple.Create(airMasses, 23));
            names.Add("airMasses");
            string name = string.Join("_x_", names);

            PopulateMultiSpecialRecursive(ref lists, ref inputs, ref original, name);
            Console.WriteLine("Populating: " + number);
            number = 0;

        }
        private static void PopulateMultiSpecialRecursive(ref List<Tuple<List<string>,int>> lists, ref List<Array> inputs, ref object[] original, string name)
        {
            if (lists.Count <= 0)
                return;

            if (lists.Count == 1)
            {
                foreach (var item in lists[0].Item1)
                {
                    object[] input = new object[Utility.lengthInput];
                    Array.Copy(original, 0, input, 0, original.Length);

                    

                    input[lists[0].Item2] = item;
                    input[57] = name;
                    input[59] = "" + number;

                    inputs.Add(input);
                    number++;
                }
                return;
            }
            Tuple<List<string>,int> first = lists[0];
            lists.RemoveAt(0);
            foreach (var item in first.Item1)
            {
                object[] input = new object[Utility.lengthInput];
                Array.Copy(original, 0, input, 0, original.Length);

                //Console.WriteLine("Populating: " + number);

                original[first.Item2] = item;
                PopulateMultiSpecialRecursive(ref lists, ref inputs, ref original, name);
            }
            lists.Insert(0, first);
        }
        public static void PopulateErrorCode(ref List<Array> inputs, ref object[] original)
        {
            Bound all = new Bound { Heightstart = 100, Heightend = 2755, Heightsteps = 5, Lengthstart = 100, Lengthend = 4205, Lengthsteps = 5 };  //H:551 L:841

            for (int i = all.Heightstart; i <= all.Heightend; i += all.Heightsteps)
            {
                //totalHeight.Add("" + i);
            }
            totalHeight.Add("2750");

            for (int i = all.Lengthstart; i <= all.Lengthend; i += all.Lengthsteps)
            {
                //totalLength.Add("" + i);
            }
            totalLength.Add("3360");

            for (int i = 0; i < 22; i++)
            {
                //numberOfRows.Add(""+1);
            }   //NR: 22
            numberOfRows.Add("12");

            for (int i = 0; i < 95; i++)
            {
                //numberOfCircuits.Add("" + i);
            }
            numberOfCircuits.Add("38");

            finSpacings.Add("15");  //not existing
            finSpacings.Add("18");
            finSpacings.Add("21");
            finSpacings.Add("25");
            finSpacings.Add("30");      //FS: 12
            finSpacings.Add("40");
            finSpacings.Add("45");  //not existing
            finSpacings.Add("50");
            finSpacings.Add("60");
            finSpacings.Add("70");   //not existing
            finSpacings.Add("10000"); //only for 5/8" tube with 0.89 mm thickness
            finSpacings.Add("20000");   //not existing

            for (int i = 0; i <= 20; i += 1)
            {
                finThicknesses.Add("" + i);
            }   //FT: 20
            //finThicknesses.Add("1");

            tubeThicknesses.Add("0.15");
            tubeThicknesses.Add("0.25");
            tubeThicknesses.Add("0.35");    //TT: 6
            tubeThicknesses.Add("0.40");
            tubeThicknesses.Add("0.5");
            tubeThicknesses.Add("1");

            int numberOfComputations = totalHeight.Count * totalLength.Count * finSpacings.Count * finThicknesses.Count * tubeThicknesses.Count * numberOfRows.Count * numberOfCircuits.Count;

            foreach (var height in totalHeight)
            {
                foreach (var length in totalLength)
                {
                    foreach(var finSpacing in finSpacings)
                    {
                        foreach(var finthickness in finThicknesses)
                        {
                            foreach(var tubeThickness in tubeThicknesses)
                            {
                                foreach(var rowNumber in numberOfRows)
                                {
                                    foreach (var circuitNumber in numberOfCircuits)
                                    {
                                        object[] input = new object[Utility.lengthInput];
                                        Array.Copy(original, 0, input, 0, original.Length);

                                        Console.WriteLine("Populating: " + number+ " / "+ numberOfComputations);

                                        input[3] = height;
                                        input[4] = length;
                                        input[9] = tubeThickness;
                                        input[10] = circuitNumber;
                                        input[11] = finSpacing;
                                        input[12] = rowNumber;
                                        input[33] = finthickness;
                                        input[59] = "" + number;

                                        inputs.Add(input);
                                        errorCodes.Add("" + number);
                                        number++;
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}