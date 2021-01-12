using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Text;

using MigraDoc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes.Charts;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;

using EcoConf.GUI;
using DOCX = Xceed.Words.NET;
using DOC = Xceed.Document.NET;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using MathNet.Numerics;
using System.Windows.Media.Converters;
using System.Net.Sockets;
using System.Data.SqlClient;

namespace EcoConf
{

	public class MainSystem
	{
        //public static readonly string defaultFileName = @"./masken/default.txt"; //TODO make installer that install these 
        //public static readonly string defaultCoilDatabase = @"./coil/";          //TODO make installer that install these    

        public static readonly string defaultFileName = @"C:/EtaWin/masken/default.txt"; //TODO make installer that install these 
        public static readonly string defaultCoilDatabase = @"C:/EtaWin/coil/";          //TODO make installer that install these    

        string currentConfigurationPath;            //path to the current tmp file of the configuration

        public static string[] DefaultConfiguration { get; set; }
        public static string[] DefaultIntake { get; set; }
        public static string[] DefaultOuttake { get; set; }

        public static bool hasError = false;

        iInput input = new InputBTN();
        iOutput output= new OutputBTN();

        iDLL dll = new DLLBTN();
        public iOutput Output { get { return output; } }
        public iInput Input { get { return input; } }
        
        MainWindow mainWindow;

        iGUIConnector connector = null;

        DocumentCreator documentCreator;

        Graph graph = new Graph();

        public Dictionary<string, Tuple<int, int>> hcConversion = new Dictionary<string, Tuple<int, int>>()
            {
                {"HC4",Tuple.Create(642, 606)},
                {"HC6",Tuple.Create(642, 912)},
                {"HC9",Tuple.Create(948, 912)},
                {"HC12",Tuple.Create(948, 1218)},
                {"HC16",Tuple.Create(1254, 1218)},
                {"HC20",Tuple.Create(1254, 1524)},
                {"HC25",Tuple.Create(1560, 1524)},
                {"HC30",Tuple.Create(1560, 1830)},
                {"HC36",Tuple.Create(1866, 1830)},
                {"HC42",Tuple.Create(1866, 2136)},
                {"HC49",Tuple.Create(2172, 2136)},
                {"HC56",Tuple.Create(2172, 2442)},
                {"HC64",Tuple.Create(2478, 2442)},
                {"HC72",Tuple.Create(2478, 2748)},
                {"HC80",Tuple.Create(2750, 2748)},
                {"HC90",Tuple.Create(2750, 3054)},
                {"HC100",Tuple.Create(2750, 3360)},
                {"HC110",Tuple.Create(2750, 3666)}
            };

        /**
         * constructor without filename, will look up the default-input,
         * otherwise use the given input
         */
        public MainSystem(MainWindow mainWindow, iGUIConnector connector)
        {
            documentCreator = new DocumentCreator(this);
            this.mainWindow = mainWindow;
            this.connector = connector;
            string filename = defaultFileName;
            InputParser.ParseData(filename, ref input);
            for(int i =0; i < input.NumberIntakesAndOuttakes; i++)
            {
                output.GetOutput().Add(new object[(int)System.Windows.Application.Current.FindResource("outputDllSize")]);
            }
            currentConfigurationPath = filename;
        }
        public MainSystem(string filename, MainWindow mainWindow, iGUIConnector connector)
        {
            documentCreator = new DocumentCreator(this);
            this.mainWindow = mainWindow;
            this.connector = connector;
            InputParser.ParseData(filename, ref input);
            for (int i = 0; i < input.NumberIntakesAndOuttakes; i++)
            {
                Debug.Assert(input.GetInputBlackbox(i).Length == Utility.inputDllSize);
                //fill everything of the input, that is not specified with some default values, otherwise we can not use the DLL
                if (Convert.ToString(input.GetInputBlackbox(i)[input.GetInputBlackbox(i).Length - 2]) == "0")
                {
                    for (int j = 0; j < input.GetInputBlackbox(i).Length; j++)
                    {
                        input.GetInputBlackbox(i)[j] = input.GetInputBlackbox(i)[j] == null || Convert.ToString(input.GetInputBlackbox(i)[j]) == "" ? DefaultIntake[j] : input.GetInputBlackbox(i)[j];
                    }
                }
                else if (Convert.ToString(input.GetInputBlackbox(i)[input.GetInputBlackbox(i).Length - 2]) == "1")
                {
                    for (int j = 0; j < input.GetInputBlackbox(i).Length; j++)
                    {
                        input.GetInputBlackbox(i)[j] = input.GetInputBlackbox(i)[j] == null || Convert.ToString(input.GetInputBlackbox(i)[j]) == "" ? DefaultOuttake[j] : input.GetInputBlackbox(i)[j];
                    }
                }
                else
                {
                    //TODO not an intake or outtake?
                }
                output.GetOutput().Add(new object[(int)System.Windows.Application.Current.FindResource("outputDllSize")]);
            }
            currentConfigurationPath = filename;

            //Feed the graph with data
            InitializeGraph();
        }

        #region graphinitialization
        private void InitializeGraph()
        {
            FluidType air = FluidType.AIR;
            FluidType watermix = FluidType.WATERGLYKOL;

            Component pump = new Pump();

            Component connection_he_he = new Connection(watermix);
            Component connection_pump_pumphe = new Connection(watermix, true);
            Component connection_pumphe_pump = new Connection(watermix, true);
            connection_pumphe_pump.inputPorts.Clear();
            connection_pump_pumphe.outputPorts.Clear();

            bool skipnext = false;
            for (int i = 0; i < input.NumberIntakesAndOuttakes; i++)
            {
                if (skipnext)
                {
                    skipnext = false;
                    continue;
                }
                skipnext = Convert.ToString(input.GetInputBlackbox(i)[Utility.inputDllSize - 3]) == "1";

                Component airStart = null;

                Component fan = new Fan();


                Component heatExchanger = new HeatExchanger(input.GetInputBlackbox(i));
                Component airEnd = null;

                Component connection_start_heIN = null;
                Component connection_heIN_fan = null;
                Component connection_fan_end = null;

                Component connection_start_fan = null;
                Component connection_fan_heOUT = null;
                Component connection_heOUT_end = null;

                Component connection_he_heIN = null;
                Component connection_heOUT_he = null;

                Component connection_heIN_pumphe = null;
                Component connection_pumphe_heOUT = null;

                bool isIntake = true;
                int heatExchangerIndex = Convert.ToInt32(input.GetInputBlackbox(i)[Utility.inputDllSize - 1]);

                if (Convert.ToString(input.GetInputBlackbox(i)[Utility.inputDllSize - 2]) == "0")
                {
                    airStart = new AirStartEndPoint(Convert.ToDouble(DefaultIntake[28]), Convert.ToDouble(DefaultIntake[30]));
                    airEnd = new AirStartEndPoint(Convert.ToDouble(DefaultIntake[31]), false);



                    connection_start_heIN = new Connection(airStart, heatExchanger, air);
                    connection_heIN_fan = new Connection(heatExchanger, fan,air);
                    connection_fan_end = new Connection(fan, airEnd,air);
                    connection_heIN_pumphe = new Connection(heatExchanger, connection_pumphe_pump, watermix);
                    //connection_heIN_pumphe.connectedConnectionsOutput.Add((Connection) connection_pumphe_pump);
                    connection_he_heIN = new Connection(connection_he_he, heatExchanger, watermix);
                    //connection_he_heIN.connectedConnectionsInput.Add((Connection) connection_he_he);

                    //connection_he_he.outputPorts.Add(connection_he_heIN.inputPorts[0]);
                    connection_pumphe_pump.inputPorts[heatExchangerIndex].connectedComponent = Tuple.Create(connection_heIN_pumphe,connection_pumphe_pump);
                    connection_pumphe_pump.outputPorts[0].connectedComponent = Tuple.Create(connection_pumphe_pump, pump);

                    //pump.connectedConnectionsInput.Add((Connection) connection_pumphe_pump);
                    pump.inputPorts.Add(connection_pumphe_pump.outputPorts[0]);

                    graph.Add(connection_start_heIN);
                    graph.Add(connection_heIN_fan);
                    graph.Add(connection_fan_end);
                    graph.Add(connection_heIN_pumphe);
                    graph.Add(connection_he_heIN);

                }
                else if (Convert.ToString(input.GetInputBlackbox(i)[Utility.inputDllSize - 2]) == "1")
                {
                    airStart = new AirStartEndPoint(Convert.ToDouble(DefaultOuttake[28]), Convert.ToDouble(DefaultOuttake[30]));
                    airEnd = new AirStartEndPoint();


                    connection_start_fan = new Connection(airStart, fan, air);
                    connection_fan_heOUT = new Connection(fan, heatExchanger, air);
                    connection_heOUT_end = new Connection(heatExchanger, airEnd, air);
                    connection_pumphe_heOUT = new Connection(connection_pump_pumphe, heatExchanger, watermix);
                    //connection_pumphe_heOUT.connectedConnectionsInput.Add((Connection) connection_pump_pumphe);
                    connection_heOUT_he = new Connection(heatExchanger, connection_he_he, watermix);
                    //connection_heOUT_he.connectedConnectionsOutput.Add((Connection) connection_he_he);

                    //connection_he_he.inputPorts.Add(connection_heOUT_he.outputPorts[0]);
                    connection_pump_pumphe.inputPorts[0].connectedComponent = Tuple.Create(pump, connection_pump_pumphe);
                    connection_pump_pumphe.outputPorts[heatExchangerIndex].connectedComponent = Tuple.Create(connection_pump_pumphe,connection_pumphe_heOUT);

                    //pump.connectedConnectionsOutput.Add((Connection)connection_pump_pumphe);
                    pump.outputPorts.Add(connection_pump_pumphe.inputPorts[0]);
                    graph.Add(connection_start_fan);
       
             graph.Add(connection_fan_heOUT);
                    graph.Add(connection_heOUT_end);
                    graph.Add(connection_pumphe_heOUT);
                    graph.Add(connection_heOUT_he);
                    isIntake = false;
                }
                else
                {
                    //TODO not a intake or outtake
                    Debug.Assert(false);
                }
                ((HeatExchanger)heatExchanger).isIntake = isIntake;

                foreach (var startPort in airStart.inputPorts)
                {
                   // startPort.iterationComputed = 0;
                }
                //airStart.Compute(0);

                graph.Add(fan, isIntake);
                graph.Add(heatExchanger, isIntake);
                graph.Add(airStart, isIntake, true);
                graph.Add(airEnd,isIntake, false);
            }

            graph.Add(connection_pumphe_pump);
            graph.Add(connection_pump_pumphe);

            graph.Add(connection_he_he);
            graph.Add(pump);

        }

        #endregion

        public static void InitializeDefaults()
        {
            iInput defaultValues  =new InputBTN();
            InputParser.ParseData(defaultFileName, ref defaultValues);
            Debug.Assert(defaultValues.GetInputBlackbox().Count == 2);
            //TODO what then?
            if (defaultValues.GetInputBlackbox().Count != 2) return;

            DefaultOuttake = new string[Utility.inputDllSize];

            //change object[] to string[]
            DefaultIntake = defaultValues.GetInputBlackbox()[0].OfType<object>().Select(o => o.ToString()).ToArray();
            DefaultOuttake= defaultValues.GetInputBlackbox()[1].OfType<object>().Select(o => o.ToString()).ToArray();

            DefaultConfiguration = defaultValues.GetInputConfiguration();

        }


        public double ScoreGraph()
        {
            double score = 0.0;
            double[] scaling = { 0.05, -0.0054, 0.0005, 0.5}; //apd fpd p fs

            score = scaling[0] * Convert.ToDouble(graph.componentIntakes[0].DLLoutput[28]) +    //apd
                    scaling[1] * Convert.ToDouble(graph.componentIntakes[0].DLLoutput[37]) +    //fpd
                    scaling[2] * Convert.ToDouble(graph.componentIntakes[0].DLLoutput[53]) +    //p
                    scaling[3] * Convert.ToDouble(graph.componentIntakes[0].DLLoutput[36]);     //fs

            score +=scaling[0] * Convert.ToDouble(graph.componentOuttakes[0].DLLoutput[28]) +    //apd
                    scaling[1] * Convert.ToDouble(graph.componentOuttakes[0].DLLoutput[37]) +    //fpd
                    scaling[2] * Convert.ToDouble(graph.componentOuttakes[0].DLLoutput[53]) +    //p
                    scaling[3] * Convert.ToDouble(graph.componentOuttakes[0].DLLoutput[36]);     //fs


            if (Convert.ToInt32(graph.componentIntakes[0].DLLoutput[7]) >= 40 || Convert.ToInt32(graph.componentOuttakes[0].DLLoutput[7]) >= 40)
            {
                score *= 1.5;
                //score = 100000;
            }

            if (Convert.ToDouble(graph.componentIntakes[0].DLLoutput[36]) > 1.26 || Convert.ToDouble(graph.componentOuttakes[0].DLLoutput[36]) > 1.26)
            {
                //score *= 2;
                score = 100000;
            }
            if (Convert.ToDouble(graph.componentIntakes[0].DLLoutput[21]) <= 18.6)
            {
                //score /= 3;
                score = 100000;
            }

            return score;
        }

        public bool ComputeLinear()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> availablePrograms = new List<string>();
            DirectoryInfo d = new DirectoryInfo("C:\\EtaWin\\");
            foreach (var folder in d.GetDirectories("HC*"))
                availablePrograms.Add(folder.Name);
            string currentHC = "";
            if (hcConversion.ContainsKey(input.GetInputConfiguration()[5].ToString()))
            {
                currentHC = input.GetInputConfiguration()[5].ToString();
            }
            else
            {
                double minDistance = 100000;
                double height = Convert.ToDouble(input.GetInputBlackbox()[0][Utility.blackBoxPartheight]);
                double length = Convert.ToDouble(input.GetInputBlackbox()[0][Utility.blackBoxPartlength]);
                foreach (KeyValuePair<string, Tuple<int,int>> kvp in hcConversion)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                    double distance = height - kvp.Value.Item1 + length - kvp.Value.Item2;
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        currentHC = kvp.Key;
                    }
                }

            }
            List<CurrentConfig> currentConfigsLinear = LinearOptimization.Optimize(this, currentHC);
            sw.Stop();
            Console.WriteLine("Time Elapsed Linear: " + sw.Elapsed);

            if (currentConfigsLinear.Count() == 0) //TODO
            {
                Console.WriteLine("No solutions found!");
                connector.FinishComputation();
                return false;
            }

            List<int> rowsList = new List<int>() { 6 ,8, 10, 12, 14};
            List<int> circuitsList = new List<int>();
            for (int i = 0; i < 60; i++)
            {
                circuitsList.Add(i);
            }
            List<int> finSpacingList = new List<int>() { 25, 30, 40, 50, 60};
            List<int> finThicknessList = new List<int>() { 1, 2, 3};

            sw.Restart();
            foreach (var item in currentConfigsLinear)
            {
                List<double> scoreList= new List<double>();
                List<int[]> configList = new List<int[]>();
                if (graph.SimulateWithRealTemps(item, true))
                {
                    int numberRowsIntake = item.intakeConfigs[0].numberRows;
                    int numberRowsOuttake = item.outtakeConfigs[0].numberRows;
                    int numberCircuitsIntake = item.intakeConfigs[0].numberCircuits;
                    int numberCircuitsOuttake = item.outtakeConfigs[0].numberCircuits;
                    int finSpacingintake = item.intakeConfigs[0].finSpacing;
                    int finSpacingOuttake = item.outtakeConfigs[0].finSpacing;
                    int finThicknessIntake = item.intakeConfigs[0].finThickness;
                    int finThicknessOuttake= item.outtakeConfigs[0].finThickness;


                    scoreList.Add(ScoreGraph());
                    Console.WriteLine("START: " + scoreList[scoreList.Count - 1]);
                    Console.WriteLine("config: " + numberRowsIntake + " " + numberRowsOuttake + " " + numberCircuitsIntake + " " + numberCircuitsOuttake + " " + finSpacingintake + " " + finSpacingOuttake + " " + finThicknessIntake + " " + finThicknessOuttake);
                    configList.Add(new int[] { numberRowsIntake, numberRowsOuttake, numberCircuitsIntake, numberCircuitsOuttake, finSpacingintake, finSpacingOuttake,finThicknessIntake, finThicknessOuttake});

                    int rowIntakeIdx = rowsList.IndexOf(numberRowsIntake);
                    int rowOuttakeIdx = rowsList.IndexOf(numberRowsOuttake);

                    int circuitIntakeIdx = circuitsList.IndexOf(numberCircuitsIntake);
                    int circuitOuttakeIdx = circuitsList.IndexOf(numberCircuitsOuttake);

                    int finSpacingIntakeIdx = finSpacingList.IndexOf(finSpacingintake);
                    int finSpacingOuttakeIdx = finSpacingList.IndexOf(finSpacingOuttake);

                    int finThicknessIntakeIdx = finThicknessList.IndexOf(finThicknessIntake);
                    int finThicknessOuttakeIdx = finThicknessList.IndexOf(finThicknessOuttake);

                    int i1 = 0;
                    for (i1 = 0; i1 <= 1; i1++)
                    {
                        //if (i1 == 0) continue;
                        if ((rowIntakeIdx == 0 && i1 < 0) || (rowIntakeIdx == rowsList.Count - 1 && i1 > 0)) continue;
                        item.intakeConfigs[0].numberRows = rowsList[rowIntakeIdx + i1];

                        int i2 = 0;
                        for (i2 = -1; i2 <= 0; i2++)
                        {
                            //if (i2 != 0) continue;
                            if ((circuitIntakeIdx == 0 && i2 < 0) || (circuitIntakeIdx == circuitsList.Count - 1 && i2 > 0)) continue;
                            item.intakeConfigs[0].numberCircuits= circuitsList[circuitIntakeIdx + i2];

                            int i3 = 0;
                            for (i3 = 0; i3 <= 1; i3++)
                            {
                                //if (i3 == 0) continue;
                                //if ((finSpacingIntakeIdx == 0 && i3 < 0) || (finSpacingIntakeIdx == finSpacingList.Count - 1 && i3 > 0)) continue;
                                //item.intakeConfigs[0].finSpacing = finSpacingList[finSpacingIntakeIdx + i3];
                                item.intakeConfigs[0].finSpacing = finSpacingList[i3];
                                int i4 = 0;
                                //for (i4 = -1; i4 <= 1; i4++)
                                for (i4 = 1; i4 <= 3; i4++)
                                {
                                    if (i4 == 3 && i3 == 0)
                                    {
                                        continue;
                                    }
                                    //if (i4 == 0) continue;
                                    //if ((finThicknessIntakeIdx == 0 && i4 < 0) || (finThicknessIntakeIdx == finThicknessList.Count - 1 && i4 > 0)) continue;
                                    //item.intakeConfigs[0].finThickness = finThicknessList[finThicknessIntakeIdx + i4];

                                    item.intakeConfigs[0].finThickness = i4;

                                    int o1 = 0;
                                    for (o1 = 0; o1 <= 1; o1++)
                                    {
                                        //if (o1 == 0) continue;
                                        if ((rowOuttakeIdx == 0 && o1 < 0) || (rowOuttakeIdx == rowsList.Count - 1 && o1 > 0)) continue;
                                        item.outtakeConfigs[0].numberRows = rowsList[rowOuttakeIdx + o1];

                                        int o2 = 0;
                                        for (o2 = -1; o2 <= 0; o2++)
                                        {
                                            //if (o2 != 0) continue;
                                            if ((circuitOuttakeIdx == 0 && o2 < 0) || (circuitOuttakeIdx == circuitsList.Count - 1 && o2 > 0)) continue;
                                            item.outtakeConfigs[0].numberCircuits = circuitsList[circuitOuttakeIdx + o2];

                                            int o3 = 0;
                                            for (o3 = 0; o3 <= 1; o3++)
                                            {
                                                //if (o3 == 0) continue;
                                                //if ((finSpacingOuttakeIdx == 0 && o3 < 0) || (finSpacingOuttakeIdx == finSpacingList.Count - 1 && o3 > 0)) continue;
                                                //item.outtakeConfigs[0].finSpacing = finSpacingList[finSpacingOuttakeIdx + o3];
                                                item.outtakeConfigs[0].finSpacing = finSpacingList[o3];
                                                int o4 = 0;
                                                //for (o4 = -1; o4 <= 1; o4++)
                                                for (o4 = 1; o4 <= 3; o4++)
                                                {
                                                    if (o4 == 3 && o3 == 0)
                                                    {
                                                        continue;
                                                    }
                                                    //if (o4 == 0) continue;
                                                    //if ((finThicknessOuttakeIdx == 0 && o4 < 0) || (finThicknessOuttakeIdx == finThicknessList.Count - 1 && o4 > 0)) continue;
                                                    //item.outtakeConfigs[0].finThickness = finThicknessList[finThicknessOuttakeIdx + o4];

                                                    item.outtakeConfigs[0].finThickness = o4;

                                                    if (graph.SimulateWithRealTemps(item, true))
                                                    {
                                                        //if (Convert.ToDouble(graph.componentIntakes[0].DLLoutput[21]) >= Utility.TransmissionDegreeToTemperature(Convert.ToDouble(input.GetInputConfiguration()[Utility.configPartTemperaturetransmissionDegree])))
                                                        {
                                                            scoreList.Add(ScoreGraph());
                                                            configList.Add(new int[] { rowsList[rowIntakeIdx + i1], rowsList[rowOuttakeIdx + o1], circuitsList[circuitIntakeIdx + i2], circuitsList[circuitOuttakeIdx + o2], finSpacingList[i3], finSpacingList[o3], i4, o4});
                                                            //Console.WriteLine("Scores: " + scoreList[scoreList.Count - 1]);
                                                            //Console.WriteLine("config: " + configList[configList.Count - 1][0] + " " + configList[configList.Count - 1][1] + " " + configList[configList.Count - 1][2] + " " + configList[configList.Count - 1][3] + " " + configList[configList.Count - 1][4] + " " + configList[configList.Count - 1][5] + " " + configList[configList.Count - 1][6] + " " + configList[configList.Count - 1][7]);
                                                            //configList.Add(new int[] { rowsList[rowIntakeIdx + i1], rowsList[rowOuttakeIdx + o1], circuitsList[circuitIntakeIdx + i2], circuitsList[circuitOuttakeIdx + o2], finSpacingList[finSpacingIntakeIdx + i3], finSpacingList[finSpacingOuttakeIdx + o3], finThicknessList[finThicknessIntakeIdx + i4], finThicknessList[finThicknessOuttakeIdx + o4]});

                                                        }
                                                    }
                                                }//o4
                                            }//o3
                                        }//o2
                                    }//o1
                                }//i4
                            }//i3
                        }//i2
                    }//i1

                    int minIndex = 0;
                    for (int i = 0; i < scoreList.Count; i++)
                    {
                        if (scoreList[i] < scoreList[minIndex])
                        {
                            minIndex = i;
                            Console.WriteLine("MIN:  " + scoreList[minIndex]);
                            Console.WriteLine("config: " + configList[minIndex][0] + " " + configList[minIndex][1] + " " + configList[minIndex][2] + " " + configList[minIndex][3] + " " + configList[minIndex][4] + " " + configList[minIndex][5] + " " + configList[minIndex][6] + " " + configList[minIndex][7]);
                        }
                    }

                    if (scoreList[minIndex] == 100000) //backup
                    {
                        Console.WriteLine("Backup");
                        for (i1 = 1; i1 <= 2; i1++)
                        {
                            //if (i1 == 0) continue;
                            if ((rowIntakeIdx == 0 && i1 < 0) || (rowIntakeIdx + i1 >= rowsList.Count && i1 > 0)) continue;
                            item.intakeConfigs[0].numberRows = rowsList[rowIntakeIdx + i1];

                            int i2 = 0;
                            for (i2 = -1; i2 <= 0; i2++)
                            {
                                //if (i2 != 0) continue;
                                if ((circuitIntakeIdx == 0 && i2 < 0) || (circuitIntakeIdx == circuitsList.Count - 1 && i2 > 0)) continue;
                                item.intakeConfigs[0].numberCircuits = circuitsList[circuitIntakeIdx + i2];

                                int i3 = 0;
                                for (i3 = 0; i3 <= 1; i3++)
                                {
                                    //if (i3 == 0) continue;
                                    //if ((finSpacingIntakeIdx == 0 && i3 < 0) || (finSpacingIntakeIdx == finSpacingList.Count - 1 && i3 > 0)) continue;
                                    //item.intakeConfigs[0].finSpacing = finSpacingList[finSpacingIntakeIdx + i3];
                                    item.intakeConfigs[0].finSpacing = finSpacingList[i3];
                                    int i4 = 0;
                                    //for (i4 = -1; i4 <= 1; i4++)
                                    for (i4 = 1; i4 <= 3; i4++)
                                    {
                                        if (i4 == 3 && i3 == 0)
                                        {
                                            continue;
                                        }
                                        //if (i4 == 0) continue;
                                        //if ((finThicknessIntakeIdx == 0 && i4 < 0) || (finThicknessIntakeIdx == finThicknessList.Count - 1 && i4 > 0)) continue;
                                        //item.intakeConfigs[0].finThickness = finThicknessList[finThicknessIntakeIdx + i4];

                                        item.intakeConfigs[0].finThickness = i4;

                                        int o1 = 0;
                                        for (o1 = 1; o1 <= 2; o1++)
                                        {
                                            //if (o1 == 0) continue;
                                            if ((rowOuttakeIdx == 0 && o1 < 0) || (rowOuttakeIdx + o1 >= rowsList.Count && o1 > 0)) continue;
                                            item.outtakeConfigs[0].numberRows = rowsList[rowOuttakeIdx + o1];

                                            int o2 = 0;
                                            for (o2 = -1; o2 <= 0; o2++)
                                            {
                                                //if (o2 != 0) continue;
                                                if ((circuitOuttakeIdx == 0 && o2 < 0) || (circuitOuttakeIdx == circuitsList.Count - 1 && o2 > 0)) continue;
                                                item.outtakeConfigs[0].numberCircuits = circuitsList[circuitOuttakeIdx + o2];

                                                int o3 = 0;
                                                for (o3 = 0; o3 <= 1; o3++)
                                                {
                                                    //if (o3 == 0) continue;
                                                    //if ((finSpacingOuttakeIdx == 0 && o3 < 0) || (finSpacingOuttakeIdx == finSpacingList.Count - 1 && o3 > 0)) continue;
                                                    //item.outtakeConfigs[0].finSpacing = finSpacingList[finSpacingOuttakeIdx + o3];
                                                    item.outtakeConfigs[0].finSpacing = finSpacingList[o3];
                                                    int o4 = 0;
                                                    //for (o4 = -1; o4 <= 1; o4++)
                                                    for (o4 = 1; o4 <= 3; o4++)
                                                    {
                                                        if (o4 == 3 && o3 == 0)
                                                        {
                                                            continue;
                                                        }
                                                        //if (o4 == 0) continue;
                                                        //if ((finThicknessOuttakeIdx == 0 && o4 < 0) || (finThicknessOuttakeIdx == finThicknessList.Count - 1 && o4 > 0)) continue;
                                                        //item.outtakeConfigs[0].finThickness = finThicknessList[finThicknessOuttakeIdx + o4];

                                                        item.outtakeConfigs[0].finThickness = o4;

                                                        if (graph.SimulateWithRealTemps(item, true))
                                                        {
                                                            //if (Convert.ToDouble(graph.componentIntakes[0].DLLoutput[21]) >= Utility.TransmissionDegreeToTemperature(Convert.ToDouble(input.GetInputConfiguration()[Utility.configPartTemperaturetransmissionDegree])))
                                                            {
                                                                scoreList.Add(ScoreGraph());
                                                                configList.Add(new int[] { rowsList[rowIntakeIdx + i1], rowsList[rowOuttakeIdx + o1], circuitsList[circuitIntakeIdx + i2], circuitsList[circuitOuttakeIdx + o2], finSpacingList[i3], finSpacingList[o3], i4, o4 });
                                                                //Console.WriteLine("Scores: " + scoreList[scoreList.Count - 1]);
                                                                //Console.WriteLine("config: " + configList[configList.Count - 1][0] + " " + configList[configList.Count - 1][1] + " " + configList[configList.Count - 1][2] + " " + configList[configList.Count - 1][3] + " " + configList[configList.Count - 1][4] + " " + configList[configList.Count - 1][5] + " " + configList[configList.Count - 1][6] + " " + configList[configList.Count - 1][7]);
                                                                //configList.Add(new int[] { rowsList[rowIntakeIdx + i1], rowsList[rowOuttakeIdx + o1], circuitsList[circuitIntakeIdx + i2], circuitsList[circuitOuttakeIdx + o2], finSpacingList[finSpacingIntakeIdx + i3], finSpacingList[finSpacingOuttakeIdx + o3], finThicknessList[finThicknessIntakeIdx + i4], finThicknessList[finThicknessOuttakeIdx + o4]});

                                                            }
                                                        }
                                                    }//o4
                                                }//o3
                                            }//o2
                                        }//o1
                                    }//i4
                                }//i3
                            }//i2
                        }//i1
                        for (int i = 0; i < scoreList.Count; i++)
                        {
                            if (scoreList[i] < scoreList[minIndex])
                            {
                                minIndex = i;
                                Console.WriteLine("MIN:  " + scoreList[minIndex]);
                                Console.WriteLine("config: " + configList[minIndex][0] + " " + configList[minIndex][1] + " " + configList[minIndex][2] + " " + configList[minIndex][3] + " " + configList[minIndex][4] + " " + configList[minIndex][5] + " " + configList[minIndex][6] + " " + configList[minIndex][7]);
                            }
                        }
                    }
                    if (scoreList.Count == 0)
                    {
                        Console.WriteLine("No sulution found");
                        break;
                        //TODO what then if nothing works??
                    }

                    item.intakeConfigs[0].numberRows = configList[minIndex][0];
                    item.outtakeConfigs[0].numberRows = configList[minIndex][1];
                    item.intakeConfigs[0].numberCircuits = configList[minIndex][2];
                    item.outtakeConfigs[0].numberCircuits = configList[minIndex][3];
                    item.intakeConfigs[0].finSpacing = configList[minIndex][4];
                    item.outtakeConfigs[0].finSpacing = configList[minIndex][5];
                    item.intakeConfigs[0].finThickness = configList[minIndex][6];
                    item.outtakeConfigs[0].finThickness = configList[minIndex][7];
                    graph.SimulateWithRealTemps(item, true);
                    //Console.WriteLine("Scores:");
                    //foreach (var score in scoreList)
                    //{
                    //    Console.WriteLine("sc:  "+score);
                    //}
                    break;
                }
                //graph.SimulateWithRealTemps(item, true);
                //{
                //    Console.WriteLine("Error: " + graph.HasError() + " Air out Temperature : "+ graph.componentIntakes[0].DLLoutput[21] + "  Water Temperatures: " + graph.componentIntakes[0].DLLoutput[33] + " - " + graph.componentOuttakes[0].DLLoutput[33]);
                //    Console.WriteLine("Config for graph:");
                //    Console.WriteLine
                //        (
                //            "intake: " + 
                //            graph.componentIntakes[0].DLLconfig.NumberRows + " " +
                //            graph.componentIntakes[0].DLLconfig.NumberCircuits + " " +
                //            graph.componentIntakes[0].DLLconfig.Finspacing + " " +
                //            graph.componentIntakes[0].DLLconfig.Finthickness + "    " +
                //            "outtake: " +
                //            graph.componentOuttakes[0].DLLconfig.NumberRows + " " +
                //            graph.componentOuttakes[0].DLLconfig.NumberCircuits + " " +
                //            graph.componentOuttakes[0].DLLconfig.Finspacing + " " +
                //            graph.componentOuttakes[0].DLLconfig.Finthickness

                //        );
                //    Console.WriteLine("\n");
                //} 
            }
            sw.Stop();
            Console.WriteLine("Time Elapsed: " + sw.Elapsed);
            return true;
        }

        public bool ComputeNomad()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //TEST
            CurrentConfig currentConfig = new CurrentConfig(Convert.ToInt32(input.GetInputConfiguration()[3]), Convert.ToInt32(input.GetInputConfiguration()[4]), Convert.ToInt32(input.GetInputConfiguration()[7]), Convert.ToInt32(input.GetInputConfiguration()[8]));

            bool nextIsSplit = false;
            for (int i = 0; i < input.NumberIntakesAndOuttakes; i++)
            {
                nextIsSplit = Convert.ToString(input.GetInputBlackbox(i)[input.GetInputBlackbox(i).Length - 3]) == "1";
                if (Convert.ToString(input.GetInputBlackbox(i)[input.GetInputBlackbox(i).Length - 2]) == "0")
                {
                    CurrentConfig.IntakeConfig intakeConfig = new CurrentConfig.IntakeConfig(
                        Convert.ToInt32(input.GetInputBlackbox(i)[4]),  //length
                        Convert.ToInt32(input.GetInputBlackbox(i)[3]),  //height
                        Convert.ToInt32(input.GetInputBlackbox(i)[12]), //number of rows
                        Convert.ToInt32(input.GetInputBlackbox(i)[10]), //number of circuits
                        Convert.ToInt32(input.GetInputBlackbox(i)[11]), //fin spacing
                        Convert.ToInt32(input.GetInputBlackbox(i)[33]), //fin thickness
                        Convert.ToDouble(input.GetInputBlackbox(i)[28]), //air temperature in
                        Convert.ToDouble(input.GetInputBlackbox(i)[30]), //air humidity in
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[3] : input.GetInputBlackbox(i)[23]), //airmass
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[4] : input.GetInputBlackbox(i)[37]), //watermixMass
                                                                                                                                                 //Convert.ToDouble(Convert.ToDouble(input.GetInputConfiguration()[2])*(Convert.ToDouble(ABL - AUL) + AUL) //air temperature out
                        10000 //waterTempIn
                        );

                    if (nextIsSplit) //TODO actually fo another use case, see split if heat exchanger is too large
                    {
                        i++;
                        CurrentConfig.IntakeConfig intakeSplitConfig = new CurrentConfig.IntakeConfig(
                        Convert.ToInt32(input.GetInputBlackbox(i)[4]),  //length
                        Convert.ToInt32(input.GetInputBlackbox(i)[3]),  //height
                        Convert.ToInt32(input.GetInputBlackbox(i)[12]), //number of rows
                        Convert.ToInt32(input.GetInputBlackbox(i)[10]), //number of circuits
                        Convert.ToInt32(input.GetInputBlackbox(i)[11]), //fin spacing
                        Convert.ToInt32(input.GetInputBlackbox(i)[33]), //fin thickness
                        Convert.ToDouble(input.GetInputBlackbox(i)[28]), //air temperature in
                        Convert.ToDouble(input.GetInputBlackbox(i)[30]), //air humidity in
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[3] : input.GetInputBlackbox(i)[23]), //airmass
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[4] : input.GetInputBlackbox(i)[37]), //watermixMass
                                                                                                                                                 //Convert.ToDouble(Convert.ToDouble(input.GetInputConfiguration()[2])*(Convert.ToDouble(ABL - AUL) + AUL) //air temperature out
                        10000 //waterTempIn
                        );
                        intakeConfig.splitConfig = intakeSplitConfig;
                    }

                    currentConfig.intakeConfigs.Add(intakeConfig);
                }
                else if (Convert.ToString(input.GetInputBlackbox(i)[input.GetInputBlackbox(i).Length - 2]) == "1")
                {
                    CurrentConfig.OuttakeConfig outtakeConfig = new CurrentConfig.OuttakeConfig(
                        Convert.ToInt32(input.GetInputBlackbox(i)[4]),  //length
                        Convert.ToInt32(input.GetInputBlackbox(i)[3]),  //height
                        Convert.ToInt32(input.GetInputBlackbox(i)[12]), //number of rows
                        Convert.ToInt32(input.GetInputBlackbox(i)[10]), //number of circuits
                        Convert.ToInt32(input.GetInputBlackbox(i)[11]), //fin spacing
                        Convert.ToInt32(input.GetInputBlackbox(i)[33]), //fin thickness
                        Convert.ToDouble(input.GetInputBlackbox(i)[28]),//air temperature in
                        Convert.ToDouble(input.GetInputBlackbox(i)[30]), //air humidity in
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[3] : input.GetInputBlackbox(i)[23]), //airmass
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[4] : input.GetInputBlackbox(i)[37]) //watermixMass
                        );

                    if (nextIsSplit) //TODO actually fo another use case, see split if heat exchanger is too large
                    {
                        i++;
                        CurrentConfig.OuttakeConfig outtakeSplitConfig = new CurrentConfig.OuttakeConfig(
                        Convert.ToInt32(input.GetInputBlackbox(i)[4]),  //length
                        Convert.ToInt32(input.GetInputBlackbox(i)[3]),  //height
                        Convert.ToInt32(input.GetInputBlackbox(i)[12]), //number of rows
                        Convert.ToInt32(input.GetInputBlackbox(i)[10]), //number of circuits
                        Convert.ToInt32(input.GetInputBlackbox(i)[11]), //fin spacing
                        Convert.ToInt32(input.GetInputBlackbox(i)[33]), //fin thickness
                        Convert.ToDouble(input.GetInputBlackbox(i)[28]), //air temperature in
                        Convert.ToDouble(input.GetInputBlackbox(i)[30]), //air humidity in
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[3] : input.GetInputBlackbox(i)[23]), //airmass
                        Convert.ToInt32(input.NumberIntakesAndOuttakes == 2 ? input.GetInputConfiguration()[4] : input.GetInputBlackbox(i)[37]) //watermixMass
                        );
                        outtakeConfig.splitConfig = outtakeSplitConfig;
                    }

                    currentConfig.outtakeConfigs.Add(outtakeConfig);
                }
                else
                {
                    //TODO no intake or Outtake
                }
            }

            if (Utility.computeWithNomad)
            {
                string[] startValues = { "startPointOptimized.txt", "startPoint1.txt", "startPoint2.txt", "startPoint3.txt", "startPoint4.txt", "startPoint5.txt", "startPoint6.txt", "startPointSplitOptimized.txt", "test.txt" };
                string useDefaultNomadStart = input.GetInputConfiguration()[3].ToString();
                if (useDefaultNomadStart.Length >= 4) //TODO use a finer grid? use some other method for finding such a start point?
                {
                    useDefaultNomadStart.Remove(useDefaultNomadStart.Length - 3);
                    useDefaultNomadStart += "000";
                }
                else
                {
                    useDefaultNomadStart = "1000";
                }
                CurrentConfig testConfig = LaunchNomadApp(useDefaultNomadStart, startValues[0], currentConfig);        //TODO use actual startvalue, this is only for testing purposes

                if (false && testConfig== null)
                {
                    testConfig = LaunchNomadApp(useDefaultNomadStart, startValues[8], currentConfig, true);
                }


                //if (testConfig != null)
                    currentConfig = testConfig;

            }



            bool fine = graph.SimulateWithRealTemps(currentConfig, true);
            sw.Stop();
            Console.WriteLine("Time SimulateWithRealTemps Elapsed: " + sw.Elapsed);
            Console.WriteLine("Final Score: "+ ScoreGraph());


            //TODO NEXT check how many heat exchangers are used in one config (since this is now the BTN representation)
            // but then what next? use this information in the output to show how the things are split


            /*
             * TODO maybe the next part can be used for the multiple case
             * 
            for (int i = currentConfig.intakeConfigs.Count; i < graph.componentIntakes.Count; i++)
            {
                object[] split = new object[Utility.inputDllSize];
                Array.Copy(input.GetInputBlackbox(0), split, input.GetInputBlackbox(0).Length);
                split[split.Length - 1] = "" + i;
                split[split.Length - 2] = "" + 0;
                graph.componentIntakes[i].DLLconfig.Config[graph.componentIntakes[i].DLLconfig.Config.Length - 1] = "" + i;
                input.GetInputBlackbox().Add(split);
            }
            for (int i = currentConfig.outtakeConfigs.Count; i < graph.componentOuttakes.Count; i++)
            {
                object[] split = new object[Utility.inputDllSize];
                Array.Copy(input.GetInputBlackbox(0), split, input.GetInputBlackbox(0).Length);
                split[split.Length - 1] = "" + i;
                split[split.Length - 2] = "" + 1;
                graph.componentOuttakes[i].DLLconfig.Config[graph.componentOuttakes[i].DLLconfig.Config.Length - 1] = "" + i;
                input.GetInputBlackbox().Add(split);
            }
            */
            return fine;
        }

        /**
         * main entry point of the calculation and simulation
         */
        public void Compute()
        {
            hasError = false;
            //TODO only use graphical progression?
            if (input.GetInputConfiguration()[29] == "1") // isLinear computation?
            {
                for(int i = 0; i < 10; i++)
                hasError = !ComputeLinear();
            }
            else // not linear computation
            {
                hasError = !ComputeNomad();
            }

            List<object[]> computedOutputs = new List<object[]>();

            for(int i = 0; i < graph.componentIntakes.Count; i++)
            {
                Array.Copy(graph.componentIntakes[i].DLLconfig.Config, input.GetInputBlackbox()[i], 55);
            }
            for (int i = 0; i < graph.componentOuttakes.Count; i++)
            {
                Array.Copy(graph.componentOuttakes[i].DLLconfig.Config, input.GetInputBlackbox()[i+graph.componentIntakes.Count], 55);
            }

            foreach (var output in graph.componentIntakes)
            {
                output.DLLoutput[output.DLLoutput.Length - 1] = output.DLLconfig.Config[output.DLLconfig.Config.Length  - 1];
                computedOutputs.Add(output.DLLoutput);
            }

            foreach (var output in graph.componentOuttakes)
            {
                output.DLLoutput[output.DLLoutput.Length - 1] = output.DLLconfig.Config[output.DLLconfig.Config.Length - 1];
                computedOutputs.Add(output.DLLoutput);
            }


            output.SetOutput(computedOutputs);

            //-- Create Output files
            if (false) // TODO do if user wants to
            {
                DocumentCreator dc = new DocumentCreator(this);
                for(int i = 0; i < output.GetOutput().Count; i++)
                {
                    if (input.GetInputBlackbox()[i].Length > 2 && input.GetInputBlackbox()[i][input.GetInputBlackbox()[i].Length - 2].ToString() == "0")
                    {
                        dc.CreateDocument(i,"Intake"+ input.GetInputBlackbox()[i].Last().ToString());
                    }
                    else if (input.GetInputBlackbox()[i].Length > 2 && input.GetInputBlackbox()[i][input.GetInputBlackbox()[i].Length - 2].ToString() == "1")
                    {
                        dc.CreateDocument(i, "Outtake" + input.GetInputBlackbox()[i].Last().ToString());
                    }
                }
            }
            //---

            connector.FinishComputation();


            return;
        }

        static CurrentConfig LaunchNomadApp(string hc, string startpoint, CurrentConfig currentConfig, bool useSplit = false)
        {
            Dictionary<object, List<object>> finSpacing = new Dictionary<object, List<object>>()
            {
                {"0", new List<object>{"25", "30", "40", "50", "60"}},      //6r
                {"1", new List<object>{"25", "30", "40", "50", "60"}},      //8r
                {"2", new List<object>{"25", "30", "40", "50", "60"}},      //10r
                {"3", new List<object>{"26", "30", "40", "50", "60"}},      //12r
                {"4", new List<object>{"31", "40", "50", "60", "60"}},      //14r
                {"5", new List<object>{"25", "30", "40", "50", "60"}},      //16r
                {"6", new List<object>{"25", "30", "40", "50", "60"}},      //20r
                {"7", new List<object>{"26", "30", "40", "50", "60"}}       //24r
            };
            Dictionary<object, object> rows = new Dictionary<object, object>()
            {   {"0", "6"},
                {"1", "8"},
                {"2", "10"},
                {"3", "12"},
                {"4", "14"},
                {"5", "16"},
                {"6", "20"},
                {"7", "24"}
            };

            if (useSplit)
            {
                finSpacing = new Dictionary<object, List<object>>()
            {
                {"0", new List<object>{"25", "30", "40", "50", "60"}},    //16r
                {"1", new List<object>{"25", "30", "40", "50", "60"}},    //18r TODO VDI splitting does not work with 18 or 22 at least if both of them need to be equal??
                {"2", new List<object>{"25", "30", "40", "50", "60"}},    //20r
                {"3", new List<object>{"26", "30", "40", "50", "60"}},    //22r TODO VDI splitting does not work with 18 or 22??
                {"4", new List<object>{"26", "30", "40", "50", "60"}}     //24r
            };
                rows = new Dictionary<object, object>()
            {   {"0", "16"},
                {"1", "18"},
                {"2", "20"},
                {"3", "22"},
                {"4", "24"}
            };
            }


            string[] values = null;
            try
            {
                // TODO make all startpoints computable
                string pathToNomad = "C:\\EtaWin\\nomad.3.9.1\\bin\\nomad.exe";
                string pathToStartPoint = "C:\\EtaWin\\NomadStart\\" + currentConfig.airMass + "\\";

                Process exeProcess = Process.Start(pathToNomad, pathToStartPoint + startpoint);
                exeProcess.WaitForExit();

                Thread.Sleep(200);
                using (StreamReader file = new StreamReader(pathToStartPoint+"solOpt.0.txt")) 
                {
                    string ln; 
                    while ((ln = file.ReadLine()) != null)
                    {
                        if (ln == "")
                        {
                            Console.WriteLine("Was an Empty line!");
                            break;
                        }

                        ln = ln.Replace("\t", " ");
                        string[] split = ln.Split(new char[] { ' ' });
                        values = split.SubArray(split.Length - 9, 8); // TODO replace 9 by a variable number for the ouputfile of the BB
                    }
                    file.Close();


                    //foreach (var value in values)
                    //{
                    //    Console.Write(value);
                    //}
                    //Console.WriteLine("");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (values == null || values[0] == "no")
            {
                return null;
            }

            currentConfig.intakeConfigs[0].numberRows = Convert.ToInt32(rows[values[0]]);
            currentConfig.intakeConfigs[0].numberCircuits= Convert.ToInt32(values[1]);
            currentConfig.intakeConfigs[0].finSpacing = Convert.ToInt32(finSpacing[values[0]][Convert.ToInt32(values[2])]);
            currentConfig.intakeConfigs[0].finThickness = Convert.ToInt32(values[3]);

            currentConfig.outtakeConfigs[0].numberRows = Convert.ToInt32(rows[values[4]]);
            currentConfig.outtakeConfigs[0].numberCircuits = Convert.ToInt32(values[5]);
            currentConfig.outtakeConfigs[0].finSpacing = Convert.ToInt32(finSpacing[values[4]][Convert.ToInt32(values[6])]);
            currentConfig.outtakeConfigs[0].finThickness= Convert.ToInt32(values[7]);
            return currentConfig;
        }
            /**
             * Find the smallest number of circuits, which does not give an error
             */
            private int FindSmallestNumberOfCircuits(int idx = 0, int start = 1, int end = -1)
        {
            if (end == -1)
                end = 40;

            int middle = (end - start) / 2 + start;

            if (end - start <=1)
            {
                input.GetInputBlackbox()[idx][10] = "" + start;
                dll.BlackBoxComputing(ref input, ref output, idx);
                if (output.GetOutput()[idx][0] == null || Convert.ToString(output.GetOutput()[idx][0]) == "")
                {
                    return start;
                }

                input.GetInputBlackbox()[idx][10] = "" + end;
                dll.BlackBoxComputing(ref input, ref output, idx);
                if (output.GetOutput()[idx][0] == null || Convert.ToString(output.GetOutput()[idx][0]) == "")
                {
                    return end;
                }
            }
            else
            {
                input.GetInputBlackbox()[idx][10] = "" + middle;
                dll.BlackBoxComputing(ref input, ref output, idx);
                if (output.GetOutput()[idx][0] == null || Convert.ToString(output.GetOutput()[idx][0]) == "") {
                    return FindSmallestNumberOfCircuits(idx, start, middle);
                }
                else
                {
                    return FindSmallestNumberOfCircuits(idx, middle, end);
                }
            }

            return -1;
        }

        /**
         * save the generated pdf file
         */
        public void SavePDFDocument(string filepath)
        {
            documentCreator.SavePDFDocument(filepath);
        }

        /**
         * save the current Configuration in an configuration file
         */
        public void SaveConfiguration(string filepath)
        {
            StreamWriter storefile;
            try
            {
                storefile = new StreamWriter(filepath);
                for (int counter = 0; counter < input.GetInputConfiguration().Length; counter++)
                {
                    storefile.WriteLine("ConfParameter_in({0})={1}", counter, input.GetInputConfiguration()[counter]);
                }
                storefile.WriteLine("---");
                List<string> intakes = new List<string>();
                List<string> outtakes = new List<string>();
                int countIntakes = 0;
                int countOuttakes = 0;
                foreach (var item in input.GetInputBlackbox())
                {
                    string help = "";

                    if (item.Length > 2 && Convert.ToInt32(item[item.Length - 2]) == 0)
                    {
                        help += "# Intake "+countIntakes+"\n";
                        for (int counter = 0; counter < item.Length; counter++)
                        {
                            help += "intake_"+countIntakes+"_(" + counter + ")=" + item[counter] + "\n";
                        }
                        countIntakes++;
                        intakes.Add(help);
                    }
                    else if(item.Length > 2 && Convert.ToInt32(item[item.Length - 2]) == 1)
                    {
                        help += "# Outtake " + countOuttakes + "\n";
                        for (int counter = 0; counter < item.Length; counter++)
                        {
                            help += "outtake_"+countOuttakes+"_(" + counter + ")=" + item[counter] + "\n";
                        }
                        countOuttakes++;
                        outtakes.Add(help);
                    }
                }
                storefile.WriteLine();
                storefile.WriteLine("# ---Intakes---");
                foreach (var item in intakes)
                {
                    storefile.WriteLine(item);
                    storefile.WriteLine("+++");
                }
                storefile.WriteLine();
                storefile.WriteLine("# ---Outtakes---");
                foreach (var item in outtakes)
                {
                    storefile.WriteLine(item);
                    storefile.WriteLine("+++");
                }

                storefile.Flush();
                storefile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write file, Error: " + e.Message);
            }
        }

        /**
         * this is called if the application is closed, it deletes all tmp files, that are left
         */
        public void ClosingApplication()
        {
            foreach (var tmpfile in HandleTmpFiles.tmpFiles.Reverse<string>())
            {
                HandleTmpFiles.DeleteTmpFile(tmpfile);  
            }
        }

        
    }





    /**
     * this class parses the input, and check it
     */
    class InputParser
    {

        /**
         * used by the contructor to get the input numbers
         */
        public static bool ParseData(string filename, ref iInput input)
        {
            StreamReader file;
            try
            {
                file = new StreamReader(filename);

                ParseConfiguratorPart(ref file, ref input);

                ParseBlackboxPart(ref file, ref input);

                file.Close();
                return input.CheckInputCorrectness();
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Could not load file, Error: " + e.Message);
                return false;
            }
        }

        /**
         * parse the non BTN part, things like number of air intakes, outtakes
         */
        static void ParseConfiguratorPart(ref StreamReader file, ref iInput input)
        {
            string line;

            //TODO fix do not use fixx value 60 for the counter
            for (int counter = 0; (line = file.ReadLine()) != null && counter < Utility.inputConfigurationSize; counter++)
            {
                Utility.EatWhitespaces(ref line);

                if (line.Contains("---")) break;           //stop if the end of the configuration input is reached

                if (line == "" || line[0] == '#')
                {
                    // this is a comment line
                    counter--;
                    continue;
                }

                string value = line.Split('=')[1];
                value = value.Split('#')[0];
                input.GetInputConfiguration()[counter] = value;
            }
            if (Convert.ToInt32(input.GetInputConfiguration()[2]) <= 0 || Convert.ToInt32(input.GetInputConfiguration()[2]) > 100) 
                input.GetInputConfiguration()[2] = "" + 68;
            input.NumberIntakesAndOuttakes = Int32.Parse(input.GetInputConfiguration()[0]) + Int32.Parse(input.GetInputConfiguration()[1]);
        }

        /**
         * parse the part that is important for the BTN blackbox
         */
        static void ParseBlackboxPart(ref StreamReader file, ref iInput input)
        {
            while (ParseBlackboxSingle(ref file, ref input)) {}

            //TODO throw error
            Debug.Assert(input.GetInputBlackbox().Count == input.NumberIntakesAndOuttakes);
        }


        private static bool ReadOneHeatExchanger(ref StreamReader file, ref object[] intakeOuttake, ref bool wasSplit)
        {
            string line;
            bool continueparsing = false;
            bool notSeen = true;

            for (int counter = 0; counter < Utility.inputDllSize && (line = file.ReadLine()) != null; counter++)
            {
                if (line.Contains("\""))
                {
                    int start = line.IndexOf('\"') + 1;
                    int end = line.LastIndexOf('\"');
                    string help = "";
                    if (start != -1 && end != -1)
                        help = line.Substring(start, end - start);
                    Utility.EatWhitespaces(ref line);
                    line = line.Split('=')[0] + "=" + help + "#";
                }
                else
                {
                    Utility.EatWhitespaces(ref line);
                }
                continueparsing = true; //at the end of the file

                //TODO make abort condition more robust
                if (line.Contains("+++"))
                {
                    notSeen = false;
                    break;           //stop if the end of the one part of Blackboxinput is reached
                }

                if (line == "" || line[0] == '#' || line.Contains("---"))
                {
                    // this is a comment line
                    counter--;
                    continue;
                }


                string value = line.Split('=')[1];
                value = value.Split('#')[0];
                intakeOuttake[counter] = value;
                if (counter == 42) intakeOuttake[counter] = MainSystem.defaultCoilDatabase;
                if (counter == 57 && value == "1") wasSplit = true;

            }
            //kill all lines after the 60th line
            while (notSeen && (line = file.ReadLine()) != null)
            {
                if (line == "+") break;
                if (line == "+++") break;
            }

            return continueparsing;
        }


        /**
         * use to parse single intake or outtake
         */
         private static bool ParseBlackboxSingle(ref StreamReader file, ref iInput input)
        {
            object[] intakeOuttake= new object[Utility.inputDllSize];
            bool wasSplit = false;

            bool continueparsing = ReadOneHeatExchanger(ref file, ref intakeOuttake, ref wasSplit);

            if (continueparsing)        //we parsed one, check if we can parse one more
            {
                input.GetInputBlackbox().Add(intakeOuttake);
                if (wasSplit)
                {
                    object[] intakeOuttakeSplit = new object[Utility.inputDllSize];
                    continueparsing = ReadOneHeatExchanger(ref file, ref intakeOuttakeSplit, ref wasSplit);
                    input.GetInputBlackbox().Add(intakeOuttakeSplit);
                }
            }
            return continueparsing;

        }

    }






    /**
     * this class is used to create a pdf file, if one wants to export the result
     */ 
    class DocumentCreator
    {
        public Document pdf;
        MainSystem mainSystem;

        public DocumentCreator(MainSystem ms)
        {
            mainSystem =ms;
        }
        /**
         * print the output in some way (text, pdf)
         */
        public void MakeOutput(bool simple = false)
        {
            Document document;

            if (simple)
                document = CreateDocumentSimple();
            else
                document = CreateDocumentComplex();
            document.UseCmykColor = true;
            pdf = document;

            CreateDocument();

            TESTFILE(); //TEST
        }

        void TESTFILE()
        {
            //string tmpfile = @"../../../../../TESTOUTPUT.txt"; //TODO make in installer? or not because it is a test file?
            string tmpfile = @"C:/EtaWin/TESTOUTPUT.txt"; //TODO make in installer? or not because it is a test file?
            StreamWriter storefile;
            var output = mainSystem.Output.GetOutput()[0];
            var input = mainSystem.Input.GetInputBlackbox()[0];

            storefile = new StreamWriter(tmpfile);
            int count = 0;
            foreach (var line in output)
            {
                storefile.WriteLine("Parameter_Out("+count+") "+line);
                count++;
            }
            storefile.Flush();
            storefile.Close();
        }

        //Create document method  
        public void CreateDocument()
        {
            CreateDocument(0,"");
        }
        public void CreateDocument(int index,string name)
        {
            var output = mainSystem.Output.GetOutput()[index];
            var input = mainSystem.Input.GetInputBlackbox()[index];

            // Title Formatting:
            var titleFormat = new DOC.Formatting();
            titleFormat.FontFamily = new DOC.Font("Arial");
            titleFormat.Bold = true;
            titleFormat.Size = 14D;
            titleFormat.Position = 12;
            

            // Body Formatting
            var paraFormat = new DOC.Formatting();
            paraFormat.FontFamily = new DOC.Font("Arial");
            paraFormat.Size = 10D;
            titleFormat.Position = 12;


            //using (DOCX.DocX doc = DOCX.DocX.Create(@"../../../../../document"+name+".docx")) //TODO make a routine for saving this?
            using (DOCX.DocX doc = DOCX.DocX.Create(@"C:/EtaWin/document" + name + ".docx")) //TODO make a routine for saving this?
            {
                //var title = doc.InsertParagraph("Bestimmung der Energieeffizienz nach DIN EN 13053 : 2012 - 02", false, titleFormat);

                //CreateFirstTable(doc);

                //CreateSecondTable(doc);

                //var p = doc.InsertParagraph("Gesamt Leistung Ventilatoren für WRG:");
                //p.SpacingAfter(25d);

                var title = doc.InsertParagraph("Angaben zur Luft:", false, titleFormat);
                var p = doc.InsertParagraph("");
                p.SpacingAfter(10d);
                var t = doc.AddTable(1, 7);
                t.Design = DOC.TableDesign.None;
                t.Alignment = DOC.Alignment.left;

                var r = t.Rows[0];
                r.Cells[0].Width = 440;
                r.Cells[1].Width = 60;
                r.Cells[2].Width = 60;
                r.Cells[3].Width = 60;
                r.Cells[4].Width = 60;
                r.Cells[5].Width = 60;
                r.Cells[6].Width = 60;
                r.Cells[0].Paragraphs[0].Append("Luftmenge");
                r.Cells[1].Paragraphs[0].Append(""+output[13]);
                r.Cells[2].Paragraphs[0].Append("Sm3/h");
                r.Cells[3].Paragraphs[0].Append("" + output[14]);
                r.Cells[4].Paragraphs[0].Append("kg/h");
                r.Cells[5].Paragraphs[0].Append("" + output[15]);
                r.Cells[6].Paragraphs[0].Append("m3/h");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Leistungs Gesammt/Sensibel");
                r.Cells[1].Paragraphs[0].Append("" + output[16]);
                r.Cells[2].Paragraphs[0].Append("kW");
                r.Cells[3].Paragraphs[0].Append("" + output[124]);
                r.Cells[4].Paragraphs[0].Append("kW");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Eingangs-T°");
                r.Cells[1].Paragraphs[0].Append("" + output[18]);
                r.Cells[2].Paragraphs[0].Append("°C");
                r.Cells[3].Paragraphs[0].Append("" + output[19]);
                r.Cells[4].Paragraphs[0].Append("%");
                r.Cells[5].Paragraphs[0].Append("" + output[20]);
                r.Cells[6].Paragraphs[0].Append("g/kg");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Ausgangs-T°");
                r.Cells[1].Paragraphs[0].Append("" + output[21]);
                r.Cells[2].Paragraphs[0].Append("°C");
                r.Cells[3].Paragraphs[0].Append("" + output[22]);
                r.Cells[4].Paragraphs[0].Append("%");
                r.Cells[5].Paragraphs[0].Append("" + output[23]);
                r.Cells[6].Paragraphs[0].Append("g/kg");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Geschw. Norm./Ein/Aus");
                r.Cells[1].Paragraphs[0].Append("" + output[25]);
                r.Cells[2].Paragraphs[0].Append("m/s");
                r.Cells[3].Paragraphs[0].Append("" + output[26]);
                r.Cells[4].Paragraphs[0].Append("m/s");
                r.Cells[5].Paragraphs[0].Append("" + output[27]);
                r.Cells[6].Paragraphs[0].Append("m/s");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Kondensation");
                r.Cells[1].Paragraphs[0].Append("" );
                r.Cells[2].Paragraphs[0].Append("l/h");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Druckverluste (Luft)");
                r.Cells[1].Paragraphs[0].Append("" + output[28]);
                r.Cells[2].Paragraphs[0].Append("Pa");
                t = doc.InsertTable(t);
                p = doc.InsertParagraph("");
                p.SpacingAfter(25d);

                title = doc.InsertParagraph("Angaben zum Medium:", false, titleFormat);
                p = doc.InsertParagraph("");
                p.SpacingAfter(10d);
                t = doc.AddTable(1, 7);
                t.Design = DOC.TableDesign.None;
                t.Alignment = DOC.Alignment.left;

                r = t.Rows[0];
                r.Cells[0].Width = 440;
                r.Cells[1].Width = 60;
                r.Cells[2].Width = 60;
                r.Cells[3].Width = 60;
                r.Cells[4].Width = 60;
                r.Cells[5].Width = 60;
                r.Cells[6].Width = 60;
                r.Cells[0].Paragraphs[0].Append("Art des Mediums");
                r.Cells[1].Paragraphs[0].Append("" + output[29]);
                r.Cells[5].Paragraphs[0].Append("" + output[35]);
                r.Cells[6].Paragraphs[0].Append("°C");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Mediummenge");
                r.Cells[1].Paragraphs[0].Append("" + output[30]);
                r.Cells[2].Paragraphs[0].Append("kg/h");
                r.Cells[3].Paragraphs[0].Append("" + output[31]);
                r.Cells[4].Paragraphs[0].Append("m3/h");
                r.Cells[5].Paragraphs[0].Append("" + output[32]);
                r.Cells[6].Paragraphs[0].Append("l/s");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Medium Temp. Ein/Aus");
                r.Cells[1].Paragraphs[0].Append("" + output[33]);
                r.Cells[2].Paragraphs[0].Append("°C");
                r.Cells[3].Paragraphs[0].Append("" + output[34]);
                r.Cells[4].Paragraphs[0].Append("°C");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Geschw. Medium");
                r.Cells[1].Paragraphs[0].Append("" + output[36]);
                r.Cells[2].Paragraphs[0].Append("m/h");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Druckverluste Medium");
                r.Cells[1].Paragraphs[0].Append("" + output[37]);
                r.Cells[2].Paragraphs[0].Append("kPa");
                t = doc.InsertTable(t);
                p = doc.InsertParagraph("");
                p.SpacingAfter(25d);

                title = doc.InsertParagraph("Angaben zum Tauscher:", false, titleFormat);
                p = doc.InsertParagraph("");
                p.SpacingAfter(10d);
                t = doc.AddTable(1,2);
                t.Design = DOC.TableDesign.None;
                t.Alignment = DOC.Alignment.left;

                r = t.Rows[0];
                r.Cells[0].Width = 440;
                r.Cells[1].Width = 360;
                r.Cells[0].Paragraphs[0].Append("Typ");
                r.Cells[1].Paragraphs[0].Append("" + output[4]+" "+output[2]);

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Materialien");
                char[] delimiter = { '/' };
                var help = Convert.ToString(output[3]).Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                string tmp_back="";
                string[] tmp_array= { "" };
                string tmp_front="";
                if (help.Count() > 0)
                {
                    tmp_back = help.Last();
                    tmp_array = new string[help.Length - 1];
                    Array.Copy(help, 0, tmp_array, 0, help.Length - 1);
                    tmp_front = String.Join("/", tmp_array);
                }
                r.Cells[1].Paragraphs[0].Append(tmp_front + " "+output[68] + " "+tmp_back+ " " + output[12] /*+  " RAMEN SAMMLER????"*/);
                t = doc.InsertTable(t);

                t = doc.AddTable(1, 7);
                t.Design = DOC.TableDesign.None;
                t.Alignment = DOC.Alignment.left;

                r = t.Rows[0];
                r.Cells[0].Width = 440;
                r.Cells[1].Width = 60;
                r.Cells[2].Width = 60;
                r.Cells[3].Width = 60;
                r.Cells[4].Width = 60;
                r.Cells[5].Width = 60;
                r.Cells[6].Width = 60;
                r.Cells[0].Paragraphs[0].Append("Berippte H*L*W, Gewicht");
                r.Cells[1].Paragraphs[0].Append("" + output[6]+"*");
                r.Cells[2].Paragraphs[0].Append(""+output[5]+"*");
                r.Cells[3].Paragraphs[0].Append("" + output[127]);
                r.Cells[4].Paragraphs[0].Append("mm");
                r.Cells[5].Paragraphs[0].Append("" + output[49]);
                r.Cells[6].Paragraphs[0].Append("kg");

                r = t.InsertRow();
                r.Cells[0].Paragraphs[0].Append("Sa, Si, Inhalt");
                r.Cells[1].Paragraphs[0].Append("" + output[50]);
                r.Cells[2].Paragraphs[0].Append("m²");
                r.Cells[3].Paragraphs[0].Append("" + output[51]);
                r.Cells[4].Paragraphs[0].Append("m²");
                r.Cells[5].Paragraphs[0].Append("" + output[52]);
                r.Cells[6].Paragraphs[0].Append("l");

                t = doc.InsertTable(t);
                p = doc.InsertParagraph("");
                p.SpacingAfter(25d);
                try
                {
                    doc.InsertParagraph("Preis pro Tauscher: "+ output[53] +" Euro            S = "+ (Convert.ToInt32(output[24])-100)+"%\n");

                }
                catch(Exception e)
                {
                    System.Windows.MessageBox.Show("Wrong Format of: " + output[24]  + "\n");  //TEST DONT USE
                }
                // Save the doc.
                doc.Save();
                }
        }

        #region PartsOfCreatingDocument
        /**
         * used for the create document
         */
        private void CreateFirstTable(DOCX.DocX doc)
        {
            iOutput output = mainSystem.Output;
            iInput input = mainSystem.Input;

            var t = doc.AddTable(1, 2);
            t.Design = DOC.TableDesign.TableGrid;
            t.Alignment = DOC.Alignment.left;

            // Add a row at the end of the table and sets its values.
            var r = t.Rows[0];
            r.Cells[0].Width = 500;
            r.Cells[1].Width = 300;
            r.Cells[0].Paragraphs[0].Append("Temperaturen / Luftzustände nach DIN EN 308");
            r.Cells[0].FillColor = System.Drawing.Color.LimeGreen;
            r.Cells[1].Paragraphs[0].Append("");
            r.Cells[1].FillColor = System.Drawing.Color.LimeGreen;

            r = t.InsertRow();
            r.Cells[0].Paragraphs[0].Append("AußenLuft");
            r.Cells[1].Paragraphs[0].Append(input.GetInputBlackbox()[0][29]+@"/"+ input.GetInputBlackbox()[0][31]);

            r = t.InsertRow();
            r.Cells[0].Paragraphs[0].Append("Abluft");
            r.Cells[1].Paragraphs[0].Append(output.GetOutput()[0][21] + @"/" + output.GetOutput()[0][22]);

            r = t.InsertRow();
            r.Cells[0].Paragraphs[0].Append("Zuluft");
            r.Cells[1].Paragraphs[0].Append(output.GetOutput()[0][18] + @"/" + output.GetOutput()[0][19]);

            t = doc.InsertTable(t);

            var p = doc.InsertParagraph("");
            p.SpacingAfter(25d);
        }

        /**
         * used for the create document
         */
        private void CreateSecondTable(DOCX.DocX doc)
        {
            // Add a Table of 5 rows and 2 columns into the document and sets its values.
            var t = doc.AddTable(1, 3);
            t.Design = DOC.TableDesign.TableGrid;
            t.Alignment = DOC.Alignment.left;

            // Add a row at the end of the table and sets its values.
            var r = t.Rows[0];
            r.Cells[0].Width = 400;
            r.Cells[1].Width = 200;
            r.Cells[2].Width = 200;
            r.Cells[0].Paragraphs[0].Append("Luftströme");
            r.Cells[0].FillColor = System.Drawing.Color.LimeGreen;
            r.Cells[1].Paragraphs[0].Append("Zuluft");
            r.Cells[1].FillColor = System.Drawing.Color.LimeGreen;
            r.Cells[2].Paragraphs[0].Append("Abluft");
            r.Cells[2].FillColor = System.Drawing.Color.LimeGreen;

            r = t.InsertRow();
            r.Cells[0].Paragraphs[0].Append("Volumenstrom");
            r.Cells[1].Paragraphs[0].Append("");
            r.Cells[2].Paragraphs[0].Append("");


            r = t.InsertRow();
            r.Cells[0].Paragraphs[0].Append("\u0394 p WRG-Wärmeüberträger");
            r.Cells[1].Paragraphs[0].Append("");
            r.Cells[2].Paragraphs[0].Append("");


            r = t.InsertRow();
            r.Cells[0].Paragraphs[0].Append("\u03B7 Ventilator");
            r.Cells[1].Paragraphs[0].Append("");
            r.Cells[2].Paragraphs[0].Append("");


            r = t.InsertRow();
            r.Cells[0].Paragraphs[0].Append("benötigte Leistung Ventilator");
            r.Cells[1].Paragraphs[0].Append("");
            r.Cells[2].Paragraphs[0].Append("");

            t = doc.InsertTable(t);

            var p = doc.InsertParagraph("");
            p.SpacingAfter(25d);
        }
        #endregion

        /**
         * used to write content with a single heat exchanger
         */
        private Document CreateDocumentSimple()
        {
            // Create a new MigraDoc document
            Document document = new Document();

            // Add a section to the document
            Section section = document.AddSection();

            // Add a paragraph to the section
            Paragraph paragraph = section.AddParagraph();

            paragraph.Format.Font.Color = MigraDoc.DocumentObjectModel.Color.FromCmyk(100, 30, 20, 50);


            // Add some text to the paragraph
            paragraph.AddFormattedText("Bestimmung der Energieeffizienz nach DIN EN 13053 : 2012-02\n", TextFormat.Bold);

            iOutput output = mainSystem.Output;
            iInput input = mainSystem.Input;

            //Add a simple table
            section.AddParagraph();
            Table table = new Table();
            table.Borders.Width = 0.75;

            Column column = table.AddColumn(Unit.FromCentimeter(8));
            column.Format.Alignment = ParagraphAlignment.Left;
            column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Left;


            Row row = table.AddRow();
            row.Shading.Color = Colors.LawnGreen;

            Cell cell = row.Cells[0];
            cell.AddParagraph(@"Temperaturen / Luftzustände nach DIN EN 308");
            cell = row.Cells[0];
            cell.AddParagraph("");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Außenluft");
            cell = row.Cells[1];
            cell.AddParagraph(""+input.GetInputBlackbox()[0][29]+@"/"+ input.GetInputBlackbox()[0][31]);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Abluft");
            cell = row.Cells[1];
            cell.AddParagraph("" + output.GetOutput()[0][22] + @"/" + output.GetOutput()[0][23]);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("Zuluft");
            cell = row.Cells[1];
            cell.AddParagraph("" + output.GetOutput()[0][21] + @"/" + output.GetOutput()[0][22]);

            ////Add a simple table
            //section.AddParagraph();
            //table = new Table();
            //table.Borders.Width = 0.75;

            //column = table.AddColumn(Unit.FromCentimeter(2));
            //column.Format.Alignment = ParagraphAlignment.Center;
            //table.AddColumn(Unit.FromCentimeter(5));
            //table.AddColumn(Unit.FromCentimeter(5));

            //row = table.AddRow();
            //row.Shading.Color = Colors.LawnGreen;

            //cell = row.Cells[0];
            //cell.AddParagraph("Luftströme");

            document.LastSection.Add(table);
            return document;
        }

        /**
         * used to write the content of the pdf
         */
        private Document CreateDocumentComplex()
        {
            // Create a new MigraDoc document
            Document document = new Document();

            // Add a section to the document
            Section section = document.AddSection();

            // Add a paragraph to the section
            Paragraph paragraph = section.AddParagraph();

            paragraph.Format.Font.Color = MigraDoc.DocumentObjectModel.Color.FromCmyk(100, 30, 20, 50);


            // Add some text to the paragraph
            paragraph.AddFormattedText("Hello, World!\n", TextFormat.Bold);
            paragraph.AddFormattedText("This is a test and this text should go over the sample table and i want to knwo what happens if this text is very long, so that i can see what happns at a line break.\n");

            //Add a simple table
            section.AddParagraph();
            document.LastSection.AddParagraph("Simple Table", "Heading2");

            Table table = new Table();
            table.Borders.Width = 0.75;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;
            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            row.Shading.Color = Colors.LawnGreen;

            Cell cell = row.Cells[0];
            cell.AddParagraph("Itemus");
            cell = row.Cells[1];
            cell.AddParagraph("Descriptum");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph("TEST");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph("TEST");

            document.LastSection.Add(table);
            return document;
        }

        /**
         * save the pdf file to the given path
         */
        public void SavePDFDocument(string filepath)
        {
            // A flag indicating whether to create a Unicode PDF or a WinAnsi PDF file.
            const bool unicode = false;
            const PdfFontEmbedding embedding = PdfFontEmbedding.Always;
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(unicode, embedding);
            if (pdf == null) return;
            // Associate the MigraDoc document with a renderer
            pdfRenderer.Document = pdf.Clone();

            // Layout and render document to PDF
            pdfRenderer.RenderDocument();

            // Save the document...
            pdfRenderer.PdfDocument.Save(filepath);


            // ...and start a viewer.
            //Process.Start(filename);
        }

    }
}
