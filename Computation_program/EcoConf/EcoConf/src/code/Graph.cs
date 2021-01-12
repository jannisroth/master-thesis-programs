using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace EcoConf
{
    public enum humidityUnit { PERCENT, SPECIFICHUMIDITY, DEWPOINT };
    public enum FluidType {AIR, WATERGLYKOL};
    public enum ValveType { };

    class ComponentList : IEnumerable
    {
        private List<Component> components = new List<Component>();  // all components

        private List<List<Component>> typeComponents = new List<List<Component>>();

        private Dictionary<Type, List<int>> typeDictionary = new Dictionary<Type, List<int>>();

        public Connection middleConnection { get; private set; } = null;

        public int Count { get { return components.Count; } }

        /*
         * with isIntake we have 
         * 0-> is an Intake
         * 1-> is an Outtake
         * 
         * with isIntake and isStart we have
         * 0-> is an intake and is at the Start
         * 1-> is an intake and is at the End
         * 2-> is an outtake and is at the Start
         * 3-> is an outtake and is at the End
         */

        public void Add(Component component)
        {
            CheckDictionary(component,1);
            typeComponents[typeDictionary[component.GetType()][0]].Add(component);
            components.Add(component);
        }
        public void Add(Component component, bool isIntake)
        {
            CheckDictionary(component, 2);
            typeComponents[typeDictionary[component.GetType()][isIntake? 0: 1]].Add(component);
            components.Add(component);
        }
        public void Add(Component component, bool isIntake, bool isStart)
        {
            CheckDictionary(component,4);
            if (isIntake)
            {
                if (isStart)
                {
                    typeComponents[typeDictionary[component.GetType()][0]].Add(component);
                }
                else
                {
                    typeComponents[typeDictionary[component.GetType()][1]].Add(component);
                }
            }
            else
            {
                if (isStart)
                {
                    typeComponents[typeDictionary[component.GetType()][2]].Add(component);
                }
                else
                {
                    typeComponents[typeDictionary[component.GetType()][3]].Add(component);
                }
            }
            components.Add(component);
        }

        public List<Component> Get(Type type)
        {
            if (!typeDictionary.ContainsKey(type))
            {
                return new List<Component>();
            }
            List<Component> res = new List<Component>();
            for (int i = 0; i < typeDictionary[type].Count; i++)
            {
                res = res.Concat(typeComponents[typeDictionary[type][i]]).ToList();
            }
            return res; ;
        }
        public List<Component> Get(Type type, bool isIntake)
        {
            List<Component> res = new List<Component>();
            Debug.Assert(typeDictionary[type].Count == 2);
            res = res.Concat(typeComponents[typeDictionary[type][isIntake? 0 : 1]]).ToList();

            return res; ;
        }
        public List<Component> Get(Type type, bool isIntake, bool isStart)
        {
            List<Component> res = new List<Component>();
            Debug.Assert(typeDictionary[type].Count == 4);

            res = res.Concat(typeComponents[typeDictionary[type][isIntake? (isStart?0:1) : (isStart? 2:3)]]).ToList();

            return res; ;
        }

        public bool Remove(Component component)
        {
            if (components.Remove(component))
            {
                Console.WriteLine("component could not be deleted");
            }

            foreach (var idx in typeDictionary[component.GetType()])
            {
                if (typeComponents[idx].Remove(component))
                    return true;
            }
            return false;
        }

        public List<Component> GetIntakeHeatExchanger()
        {
            return typeComponents[typeDictionary[typeof(HeatExchanger)][0]];
        }
        public List<Component> GetOuttakeHeatExchanger()
        {
            return typeComponents[typeDictionary[typeof(HeatExchanger)][1]];
        }
        public List<Component> GetOuttakeAirStart()
        {
            return typeComponents[typeDictionary[typeof(AirStartEndPoint)][2]];
        }
        public List<Component> GetIntakeAirStart()
        {
            return typeComponents[typeDictionary[typeof(AirStartEndPoint)][0]];
        }
        public List<Component> GetIntakeAirEnd()
        {
            return typeComponents[typeDictionary[typeof(AirStartEndPoint)][1]];
        }
        public List<Component> GetConnections()
        {
            return typeComponents[typeDictionary[typeof(Connection)][0]];
        }
        public List<Component> GetComponents()
        {
            return components;
        }

        public IEnumerator GetEnumerator()
        {
            foreach(var component in components)
            {
                yield return component;
            }
        }

        private void CheckDictionary(Component component, int number)
        {
            if (!typeDictionary.ContainsKey(component.GetType()))
            {
                typeDictionary[component.GetType()] = new List<int>();
                for (int i = 0; i < number; i++)
                {
                    typeComponents.Add(new List<Component>());
                    typeDictionary[component.GetType()].Add(typeComponents.Count - 1);
                }
            }
        }

        public void UpdateMiddleConnection()
        {
            foreach(var connection in Get(typeof(Connection)))
            {
                bool isConnection = true;

                foreach(var input in connection.inputPorts)
                {
                    if(input.connectedComponent.Item1.GetType() != typeof(Connection))
                    {
                        isConnection = false;
                        break;
                    }
                }
                if (!isConnection) continue;
                foreach (var output in connection.outputPorts)
                {
                    if (output.connectedComponent.Item2.GetType() != typeof(Connection))
                    {
                        isConnection = false;
                        break;
                    }
                }
                if (isConnection)
                {
                    middleConnection = (Connection)connection;
                    break;
                }
            }
        }

    }

    class Graph
    {
  
        public static DLLBTN dll = new DLLBTN();

        //TODO reformat this, dont use a static member
        public static CurrentConfig currentConfig;


        //public List<Component> components = new List<Component>();  // all components
        public ComponentList components = new ComponentList();
        public List<Component> computationSequence = new List<Component>();
        public List<HeatExchanger> componentIntakes { get { return components.GetIntakeHeatExchanger().Cast<HeatExchanger>().ToList(); } }
        //public List<HeatExchanger> componentIntakes = new List<HeatExchanger>();
        public List<HeatExchanger> componentOuttakes { get { return components.GetOuttakeHeatExchanger().Cast<HeatExchanger>().ToList(); } }
        //public List<HeatExchanger> componentOuttakes = new List<HeatExchanger>();
        public List<AirStartEndPoint> componentAirStartPointsIntake { get { return components.GetIntakeAirStart().Cast<AirStartEndPoint>().ToList(); } }
        //public List<AirStartEndPoint> componentAirStartPointsIntake = new List<AirStartEndPoint>();
        public List<AirStartEndPoint> componentAirEndPointsIntake { get { return components.GetIntakeAirEnd().Cast<AirStartEndPoint>().ToList(); } }
        //public List<AirStartEndPoint> componentAirEndPointsIntake = new List<AirStartEndPoint>();
        public List<AirStartEndPoint> componentAirStartPointsOuttake { get { return components.GetOuttakeAirStart().Cast<AirStartEndPoint>().ToList(); } }
        //public List<AirStartEndPoint> componentAirStartPointsOuttake = new List<AirStartEndPoint>();
        public List<Connection> connections { get {  return components.GetConnections().Cast<Connection>().ToList(); } }
        //public List<Connection> connections= new List<Connection>();

        int idCounter = 0;

        public static int portIDcounter = 0;

        public bool isHeating { get {return  components.GetOuttakeAirStart().Cast<AirStartEndPoint>().ToList().First().air.temperature < components.GetOuttakeAirStart().Cast<AirStartEndPoint>().ToList().First().air.temperature; } }

        public bool isEcoCondBasic = true;

        public bool isDinComputation = true;

        int boundNumberOfIterations = 0;

        bool calculateWater = false;

        int iterationCount = 0;

        List<double> tempsLastIteration = new List<double>();
        List<List<double>> tmpHistory = new List<List<double>>();

        public bool SimulateWithRealTemps(CurrentConfig currentConfig, bool isDINComputation)
        {

            if (currentConfig == null)
            {
                return false;
            }
            Stopwatch sw = new Stopwatch();
            //sw.Start();
            Graph.currentConfig = currentConfig;
            this.isDinComputation = isDINComputation;
            tempsLastIteration.Clear();
            boundNumberOfIterations = 20;
            tmpHistory.Clear();

            InitialzeConfig(currentConfig);     //put user input in the system

            //sw.Stop();
            //Console.WriteLine("Time init: " + sw.Elapsed);
            //sw.Restart();
            FindComputationSequence();

            for (int i = 0; i < componentIntakes.Count; i++)
            {
                tempsLastIteration.Add(0.0);
            }
            for (int i = 0; i < componentOuttakes.Count; i++)
            {
                tempsLastIteration.Add(0.0);
            }
            tmpHistory.Add(tempsLastIteration);

            //sw.Stop();
            //Console.WriteLine("Time computation sequence: "+sw.Elapsed);
            int count = 0;
            for (count = 0;  !isSteadyPoint(); count++)
            {
                sw.Restart();
                foreach(var comp in computationSequence)
                {
                    //Stopwatch stopwatch = new Stopwatch();
                    //stopwatch.Restart();
                    comp.Compute(count);
                    //stopwatch.Stop();
                    //Console.WriteLine("     Time " + comp.GetType().ToString() + ": " + stopwatch.Elapsed);
                }
                //sw.Stop();
                //Console.WriteLine("Time iteration "+count+": "+sw.Elapsed);
            }

            if (HasToBeSplit() && !currentConfig.isLinear && false) //TODO TEST 
            {
                tempsLastIteration.Clear();
                foreach (var he in components.Get(typeof(HeatExchanger)))
                {
                    SplitHeatExchanger((HeatExchanger)he);
                }

                FindComputationSequence();

                foreach (var connection in connections)
                {
                    tempsLastIteration.Add(0.0);
                }

                for (int i = 0; !isSteadyPoint(); i++)
                {
                    foreach (var comp in computationSequence)
                    {
                        comp.Compute(i);
                    }
                }
            }

            return !HasError();
        }

        double waterTempTop = 0.0;
        double waterTempBot = 0.0;
        /**
         * check if the steady point is not reached 
         * return true if it is not
         * return false if it is
         */
        private bool isSteadyPoint()
        {
            Debug.Assert(components.Get(typeof(Pump)).Count == 1);
            if(boundNumberOfIterations-- < 0) { return true; } //TODO Print warning, that iterations are reached
            bool isSteadyState = true;
            //TODO change to just tmpHistory and maybe make it a circular buffer
            for (int k = tmpHistory.Count - 1; k >= 0; k--)
            {
                isSteadyState = true;
                for (int i = 0; i < componentIntakes.Count; i++)
                {
                    if (!Utility.EqualDouble(tmpHistory[k][i], Convert.ToDouble(componentIntakes[i].DLLoutput[33] == "" ? "0" : componentIntakes[i].DLLoutput[33])))
                    {
                        isSteadyState = false;
                        break;
                    }
                }
                for (int i = componentIntakes.Count; i < componentIntakes.Count + componentOuttakes.Count; i++)
                {
                    if (!Utility.EqualDouble(tmpHistory[k][i], Convert.ToDouble(componentOuttakes[i- componentIntakes.Count].DLLoutput[33] == "" ? "0" : componentOuttakes[i - componentIntakes.Count].DLLoutput[33])))
                    {
                        isSteadyState = false;
                        break;
                    }
                }
                if (isSteadyState) break;
                //if(!Utility.EqualDouble(tempsLastIteration[i], connections[i].fluid.temperature))
                //{
                //    isSteadyState = false;
                //    break;
                //}
            }

            for (int i = 0; i < componentIntakes.Count; i++)
            {
                tempsLastIteration[i] = Convert.ToDouble(componentIntakes[i].DLLoutput[33] == "" ? "0" : componentIntakes[i].DLLoutput[33]);
            }
            for (int i = componentIntakes.Count; i < componentIntakes.Count + componentOuttakes.Count; i++)
            {
                tempsLastIteration[i] = Convert.ToDouble(componentOuttakes[i - componentIntakes.Count].DLLoutput[33] == "" ? "0" : componentOuttakes[i - componentIntakes.Count].DLLoutput[33]);
            }

            tmpHistory.Add(new List<double>(tempsLastIteration));

            return isSteadyState;
        }

        public bool HasError()
        {
            foreach(var intake in componentIntakes)
            {
                if(intake.DLLoutput[0] != null && intake.DLLoutput[0] != "")
                {
                    return true;
                }
            }
            foreach(var outtake in componentOuttakes)
            {
                if (outtake.DLLoutput[0] != null && outtake.DLLoutput[0] != "")
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeIntakes()
        {
            // air intake
            for (int i = 0; i < currentConfig.intakeConfigs.Count; i++)
            {
                componentIntakes[i].InitizeCurrentConfig(currentConfig.intakeConfigs[i], currentConfig);

                if (currentConfig.intakeConfigs[i].waterMixTemperatureIN >= 1000)
                {
                    calculateWater = true;
                    ResetWaterTemperatures();
                    if (isDinComputation)
                    {
                        componentIntakes[i].DLLconfig.WaterMixTempIn = 21;
                    }
                    else
                    {
                        componentIntakes[i].DLLconfig.WaterMixTempIn = componentAirStartPointsOuttake[0].air.temperature;
                    }

                    foreach (var inputPort in componentIntakes[i].inputPorts)
                        if (inputPort.fluid.type == FluidType.WATERGLYKOL)
                            inputPort.fluid.temperature = componentIntakes[i].DLLconfig.WaterMixTempIn;
                }
                else
                {
                    calculateWater |= false;
                    componentIntakes[i].DLLconfig.WaterMixTempIn = currentConfig.intakeConfigs[i].waterMixTemperatureIN;
                }
                if (currentConfig.intakeConfigs[i].airTemperatureOUT >= 1000)
                {
                    //TODO set some airTemperature 
                }
                else
                {
                    componentIntakes[i].DLLconfig.AirTempOut = currentConfig.intakeConfigs[i].airTemperatureOUT;
                }

                componentAirStartPointsIntake[i].air = new Air(currentConfig.intakeConfigs[i].airTemperatureIN, currentConfig.intakeConfigs[i].airHumidityIN);


                if (componentIntakes[i].DLLconfig.NumberRows >= 16)
                {
                    componentIntakes[i].DLLconfig.Config[54] = "" + 2; //TODO IMPORTANT use some representaition, such that the program can be used with multiple DLLs (actually need to rework other code too, especially the heatexchanger class)
                }

                if (currentConfig.intakeConfigs[i].splitConfig != null) //TODO is actually mulptiple air intakes and outtakes
                {
                    SplitHeatExchanger(currentConfig, componentIntakes[i], currentConfig.intakeConfigs[i], currentConfig.intakeConfigs[i].splitConfig);
                }
            }
        }
        private void InitializeOuttakes()
        {
            // air outtake
            for (int i = 0; i < currentConfig.outtakeConfigs.Count; i++)
            {
                componentOuttakes[i].InitizeCurrentConfig(currentConfig.outtakeConfigs[i], currentConfig);
                if (currentConfig.outtakeConfigs[i].waterMixTemperatureIN >= 1000)
                {
                    //TODO set some waterTemperature
                    calculateWater = true;
                    ResetWaterTemperatures();
                }
                else
                {
                    calculateWater |= false;
                    componentOuttakes[i].DLLconfig.WaterMixTempIn = currentConfig.outtakeConfigs[i].waterMixTemperatureIN;
                }

                if (Utility.EqualDouble(currentConfig.outtakeConfigs[i].power, 0))
                {
                    //TODO set some Power
                }
                else
                {
                    componentOuttakes[i].DLLconfig.Power = currentConfig.outtakeConfigs[i].power;
                }

                componentAirStartPointsOuttake[i].air = new Air(currentConfig.outtakeConfigs[i].airTemperatureIN, currentConfig.outtakeConfigs[i].airHumidityIN);

                if (componentOuttakes[i].DLLconfig.NumberRows >= 16)
                {
                    componentOuttakes[i].DLLconfig.Config[54] = "" + 2; //TODO IMPORTANT use some representaition, such that the program can be used with multiple DLLs (actually need to rework other code too, especially the heatexchanger class)
                }

                if (currentConfig.outtakeConfigs[i].splitConfig != null)//TODO is actually mulptiple air intakes and outtakes
                {
                    SplitHeatExchanger(componentOuttakes[i], currentConfig.outtakeConfigs[i], currentConfig.outtakeConfigs[i].splitConfig);
                }
            }
        }

        /**
         * initialize the intakes and outtakes according to the useinput
         */
        private void InitialzeConfig(CurrentConfig currentConfig)
         {
            Debug.Assert(currentConfig.intakeConfigs.Count == components.GetIntakeHeatExchanger().Count && currentConfig.intakeConfigs.Count == components.GetIntakeAirStart().Count);
            Debug.Assert(currentConfig.outtakeConfigs.Count == components.GetOuttakeHeatExchanger().Count && currentConfig.outtakeConfigs.Count == components.GetOuttakeAirStart().Count);

            InitializeIntakes();

            InitializeOuttakes();


            //find the middle connection, the connections from heOUT to heIN,
            components.UpdateMiddleConnection();
         }

        /**
         * To start the computation, we have to find valid sequence of components, that we can compute
         * this step also perform the first computation
         */
        private void FindComputationSequence()
        {
            Stopwatch sw = new Stopwatch();
            //sw.Start();
            computationSequence.Clear();
            List<int> wasPortSeen = new List<int>();
            foreach(var airIn in componentAirStartPointsIntake)
            {
                foreach(Port port in airIn.outputPorts)
                {
                    wasPortSeen.Add(port.ID);
                }
                airIn.Compute(0);
                computationSequence.Add(airIn);
            }
            foreach (var airIn in componentAirStartPointsOuttake)
            {
                foreach (Port port in airIn.outputPorts)
                {
                    wasPortSeen.Add(port.ID);
                }
                airIn.Compute(0);
                computationSequence.Add(airIn);
            }

            foreach (var intake in componentIntakes)
            {
                foreach (Port port in intake.inputPorts)
                {
                    if(port.fluid.type == FluidType.WATERGLYKOL)
                    {
                        wasPortSeen.Add(port.ID);
                    }
                }
            }
            //sw.Stop();
            //Console.WriteLine("     Time computationsequence start: "+sw.Elapsed);
            //sw.Restart();
            int testingNumberOfComponents = componentAirStartPointsIntake.Count + componentAirStartPointsOuttake.Count;
            int iter = 1;
            while (testingNumberOfComponents < components.Count)
            {
                iter++;
                foreach (Component component in components)
                {
                    if (component.ReadyToComputed(iterationCount, wasPortSeen))
                    {
                        //component.Compute(iterationCount);
                        foreach (Port port in component.outputPorts)
                        {
                            wasPortSeen.Add(port.ID);
                        }
                        if (computationSequence.Contains(component)) continue;
                        computationSequence.Add(component);
                        //Console.WriteLine("Iteration: "+ iter+ " and Component: "+component.ID);
                        if (component.GetType() == typeof(HeatExchanger) && !isEcoCondBasic)
                        {
                            ((HeatExchanger)component).computeWithTemperature = true;
                        }
                        testingNumberOfComponents++;
                    }
                }
            }
            iterationCount++;

            //sw.Stop();
            //Console.WriteLine("     Time computationsequence main: " + sw.Elapsed);
            //sw.Restart();
            foreach (var component in computationSequence)
            {
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Restart();
                component.Compute(iterationCount);
                //stopwatch.Stop();
                //Console.WriteLine("     Time " + component.GetType().ToString() + ": " + stopwatch.Elapsed);
            }

            //sw.Stop();
            //Console.WriteLine("     Time computationsequence computation: " + sw.Elapsed);
        }

        private void ResetWaterTemperatures()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if(connections[i].fluid.type != FluidType.AIR)
                {
                    connections[i].fluid = new WaterMix();
                }
            }
        }

        public void Add(Component component, bool isIntake, bool isStart)
        {
            component.SetID(idCounter++);
            components.Add(component, isIntake, isStart);
        }

        public void Add(Component component, bool isIntake)
        {
            component.SetID(idCounter++);
            components.Add(component, isIntake);
        }
        public void Add(Component component)
        {
            component.SetID(idCounter++);
            components.Add(component);
        }

        public void AddHeatPump()
        {
            Debug.Assert(components.GetOuttakeHeatExchanger().Count > 0);
            Connection waterOutputConnection = null;
            foreach (var conn in components.GetOuttakeHeatExchanger()[0].ConnectedConnectionsOutput)
            {
                if (conn.fluid.type == FluidType.WATERGLYKOL)
                {
                    waterOutputConnection = conn;
                    break;
                }
            }
            _ = waterOutputConnection ?? throw new NullReferenceException();
            Debug.Assert(waterOutputConnection.ConnectedConnectionsOutput.Count > 0);
            var middleConnection = waterOutputConnection.ConnectedConnectionsOutput[0];

            Debug.Assert(components.Get(typeof(Pump)).Count == 1);
            Debug.Assert(components.Get(typeof(Pump))[0].ConnectedConnectionsOutput.Count == 1);
            var connection_pump_pumphe = components.Get(typeof(Pump))[0].ConnectedConnectionsOutput[0];
            AddHeatPump(connection_pump_pumphe, middleConnection);


        }
        public void AddHeatPump(Connection connectionBottom, Connection connectionTop)
        {
            Debug.Assert(components.GetOuttakeHeatExchanger().Count > 0);
            HeatPumpTop heatPumpTop = new HeatPumpTop(isHeating);
            HeatPumpBottom heatPumpBottom = new HeatPumpBottom(isHeating);
            Port entryForward = new Port(FluidType.WATERGLYKOL);
            Port exitBackwards = new Port(FluidType.WATERGLYKOL);
            Connection waterOutputConnection = null;
            foreach (var conn in components.GetOuttakeHeatExchanger()[0].ConnectedConnectionsOutput)
            {
                if (conn.fluid.type == FluidType.WATERGLYKOL)
                {
                    waterOutputConnection = conn;
                    break;
                }
            }
            _ = waterOutputConnection ?? throw new NullReferenceException();
            Debug.Assert(waterOutputConnection.ConnectedConnectionsOutput.Count > 0);
            var connectionTopSplit = SplitConnection(connectionTop);

            Debug.Assert(components.Get(typeof(Pump)).Count == 1);
            Debug.Assert(components.Get(typeof(Pump))[0].ConnectedConnectionsOutput.Count == 1);
            var connectionBottomSplit = SplitConnection(connectionBottom);

            //heatPumpBottom.ConnectedConnectionsInput.Add(connectionBottom);
            //heatPumpBottom.ConnectedConnectionsOutput.Add(connectionBottomSplit);
            //connectionBottom.ConnectedConnectionsOutput.Remove(connectionBottomSplit);
            //connectionBottomSplit.ConnectedConnectionsInput.Remove(connectionBottom);
            //heatPumpTop.ConnectedConnectionsInput.Add(connectionTop);
            //heatPumpTop.ConnectedConnectionsOutput.Add(connectionTopSplit);
            //connectionTop.ConnectedConnectionsOutput.Remove(connectionTopSplit);
            //connectionTopSplit.ConnectedConnectionsInput.Remove(connectionTop);

            Debug.Assert(connectionTop.outputPorts.Count == 1);
            //Debug.Assert(connectionTop.inputPorts.Count == 1);
            //Debug.Assert(connectionBottomSplit.outputPorts.Count == 1);
            Debug.Assert(connectionBottomSplit.inputPorts.Count == 1);
            entryForward.connectedComponent = Tuple.Create((Component)connectionBottom,(Component) heatPumpBottom);
            heatPumpBottom.inputPorts.Add(entryForward);
            heatPumpBottom.outputPorts.Add(connectionBottomSplit.inputPorts[0]);
            heatPumpTop.inputPorts.Add(connectionTop.outputPorts[0]);
            exitBackwards.connectedComponent = Tuple.Create((Component)heatPumpTop, (Component)connectionTopSplit);
            heatPumpTop.outputPorts.Add(exitBackwards);

            connectionTopSplit.inputPorts = new List<Port> { exitBackwards };
            connectionTop.outputPorts[0].connectedComponent = Tuple.Create((Component)connectionTop, (Component)heatPumpTop);
            connectionBottom.outputPorts = new List<Port> { entryForward };
            connectionBottomSplit.inputPorts[0].connectedComponent = Tuple.Create((Component)heatPumpBottom, (Component)connectionBottomSplit);

            this.Add(heatPumpBottom);
            this.Add(heatPumpTop);

        }

        public Connection SplitConnection(Connection connection)
        {
            Connection newPart = new Connection(connection.fluid.type, true);
            newPart.outputPorts.Clear();
            //newPart.ConnectedConnectionsInput.Add(connection);
            //newPart.ConnectedConnectionsOutput = new List<Connection>(connection.ConnectedConnectionsOutput);
            foreach (var outputPort in connection.outputPorts)
            {
                outputPort.connectedComponent = Tuple.Create((Component)newPart,outputPort.connectedComponent.Item2);
                newPart.outputPorts.Add(outputPort);
            }
            //foreach (var con in connection.ConnectedConnectionsOutput)
            //{
            //    con.ConnectedConnectionsInput.Remove(connection);
            //    con.ConnectedConnectionsInput.Add(newPart);
            //}
            Debug.Assert(newPart.inputPorts.Count == 1);
            foreach (var inputPort in newPart.inputPorts)
            {
                inputPort.connectedComponent = Tuple.Create((Component)connection, (Component)newPart);
            }
            //connection.ConnectedConnectionsOutput.Clear();
            //connection.ConnectedConnectionsOutput.Add(newPart);
            connection.outputPorts = new List<Port>(newPart.inputPorts);
            this.Add(newPart);
            return newPart;
        }

        public bool SplitHeatExchanger(CurrentConfig currentConfig, HeatExchanger heatExchanger, CurrentConfig.IntakeConfig conf1, CurrentConfig.IntakeConfig conf2)
        {
            heatExchanger.InitizeCurrentConfig(conf1, currentConfig);
            heatExchanger.DLLconfig.Config[56] = "1";               //Indicate that this heatexchanger was split at all
            object[] newConfig = new object[Utility.inputDllSize];
            Array.Copy(heatExchanger.DLLconfig.Config, newConfig, heatExchanger.DLLconfig.Config.Length);
            HeatExchanger newHeatExchanger = new HeatExchanger(newConfig); //number of rows get copied
            newHeatExchanger.InitizeCurrentConfig(conf2, currentConfig);
            heatExchanger.DLLconfig.Config[57] = "1";               //Indicate that this heatexchanger was split and the next one is his splitted twin
            newHeatExchanger.DLLconfig.Config[57] = "0";               //Indicate that this heatexchanger was the splitted twin and has no one else
            Add(newHeatExchanger, heatExchanger.isIntake);
            heatExchanger.splitedHeatexchanger = newHeatExchanger;
            newHeatExchanger.wasSplitByHeatexchanger = heatExchanger;


            //update the existing connections to go to the correct heat exchanger
            newHeatExchanger.outputPorts = new List<Port>(heatExchanger.outputPorts);
            heatExchanger.outputPorts = new List<Port>() { };
            foreach (var outport in newHeatExchanger.outputPorts)
            {
                outport.connectedComponent = Tuple.Create((Component)newHeatExchanger, outport.connectedComponent.Item2);
            }

            //need 2 extra connections between the heat exchangers
            Connection he1_he2_air = new Connection(heatExchanger, newHeatExchanger, FluidType.AIR);
            Connection he1_he2_watermix = new Connection(heatExchanger, newHeatExchanger, FluidType.WATERGLYKOL);
            Add(he1_he2_air);
            Add(he1_he2_watermix);

            return true;
        }
        public bool SplitHeatExchanger(HeatExchanger heatExchanger, CurrentConfig.OuttakeConfig conf1, CurrentConfig.OuttakeConfig conf2)
        {
            heatExchanger.InitizeCurrentConfig(conf1, currentConfig);
            heatExchanger.DLLconfig.Config[56] = "1";               //Indicate that this heatexchanger was split at all
            object[] newConfig = new object[Utility.inputDllSize];
            Array.Copy(heatExchanger.DLLconfig.Config, newConfig, heatExchanger.DLLconfig.Config.Length);
            HeatExchanger newHeatExchanger = new HeatExchanger(newConfig); //number of rows get copied
            newHeatExchanger.InitizeCurrentConfig(conf2, currentConfig);
            heatExchanger.DLLconfig.Config[57] = "1";               //Indicate that this heatexchanger was split and the next one is his splitted twin
            newHeatExchanger.DLLconfig.Config[57] = "0";               //Indicate that this heatexchanger was the splitted twin and has no one else
            Add(newHeatExchanger, heatExchanger.isIntake);
            heatExchanger.splitedHeatexchanger = newHeatExchanger;
            newHeatExchanger.wasSplitByHeatexchanger = heatExchanger;


            //update the existing connections to go to the correct heat exchanger
            newHeatExchanger.outputPorts = new List<Port>(heatExchanger.outputPorts);
            heatExchanger.outputPorts = new List<Port>() { };
            foreach (var outport in newHeatExchanger.outputPorts)
            {
                outport.connectedComponent = Tuple.Create((Component)newHeatExchanger, outport.connectedComponent.Item2);
            }

            //need 2 extra connections between the heat exchangers
            Connection he1_he2_air = new Connection(heatExchanger, newHeatExchanger, FluidType.AIR);
            Connection he1_he2_watermix = new Connection(heatExchanger, newHeatExchanger, FluidType.WATERGLYKOL);
            Add(he1_he2_air);
            Add(he1_he2_watermix);

            return true;
        }

        public bool SplitHeatExchanger(HeatExchanger heatExchanger)
        {
            heatExchanger.DLLconfig.NumberRows = 8;                 // TODO use a clever startpoint, not fix 8?
            heatExchanger.DLLconfig.Airmass = heatExchanger.DLLconfig.Airmass / 2; // TODO make double??? check for undividable numbers
            heatExchanger.DLLconfig.Config[37] = "" + Convert.ToInt32(heatExchanger.DLLconfig.Config[37]) / 2;
            heatExchanger.DLLconfig.Config[56] = "1";               //Indicate that this heatexchanger was split at all
            object[] newConfig = new object[Utility.inputDllSize];
            Array.Copy(heatExchanger.DLLconfig.Config, newConfig,heatExchanger.DLLconfig.Config.Length);
            HeatExchanger newHeatExchanger = new HeatExchanger(newConfig); //number of rows get copied
            heatExchanger.DLLconfig.Config[57] = "1";               //Indicate that this heatexchanger was split and the next one is his splitted twin
            newHeatExchanger.DLLconfig.Config[57] = "0";               //Indicate that this heatexchanger was the splitted twin and has no one else
            Add(newHeatExchanger, heatExchanger.isIntake);
            heatExchanger.splitedHeatexchanger = newHeatExchanger;
            newHeatExchanger.wasSplitByHeatexchanger = heatExchanger;

            //update the existing connections to go to the correct heat exchanger
            newHeatExchanger.outputPorts = new List<Port>(heatExchanger.outputPorts);
            heatExchanger.outputPorts = new List<Port>() { };
            foreach (var outport in newHeatExchanger.outputPorts)
            {
                outport.connectedComponent = Tuple.Create((Component)newHeatExchanger, outport.connectedComponent.Item2);
            }

            //need 2 extra connections between the heat exchangers
            Connection he1_he2_air = new Connection(heatExchanger, newHeatExchanger, FluidType.AIR);
            Connection he1_he2_watermix = new Connection(heatExchanger, newHeatExchanger, FluidType.WATERGLYKOL);
            Add(he1_he2_air);
            Add(he1_he2_watermix);

            return true;
        }

        public bool SplitHeatExchanger(List<HeatExchanger> heatExchangers)
        {
            bool result = true;
            foreach(var heatExchanger in heatExchangers)
            {
                result &= SplitHeatExchanger(heatExchanger);
            }
            return result;
        }


        public bool HasToBeSplit()
        {
            foreach (var intake in componentIntakes)
            {
                if (intake.DLLoutput[0] != null && intake.DLLoutput[0] != "")
                    return true;
                if (Convert.ToInt32(intake.DLLoutput[28]) > 200)
                    return true;
                if (Convert.ToInt32(intake.DLLoutput[37]) > 185) //TODO make variable for ecoCond basic, Vollversion and plus
                    return true;
            }
            foreach (var outtake in componentOuttakes)
            {
                if (outtake.DLLoutput[0] != null && outtake.DLLoutput[0] != "")
                    return true;
                if (Convert.ToInt32(outtake.DLLoutput[28]) > 200)
                    return true;
                if (Convert.ToInt32(outtake.DLLoutput[37]) > 185) //TODO make variable for ecoCond basic, Vollversion and plus
                    return true;
            }
            return false;
        }

    }

    public class CurrentConfig
    {
        public int airMass; //TODO check if we need this anymore
        public int watermixMass;    //TODO check if we need this anymore
        public bool isLinear;
        public int waterMixDLLindex;
        public int waterMixPercentage;
        public CurrentConfig(int airMass, int watermixMass, int waterMixDLLindex, int waterMixPercentage, bool isLinear = false)
        {
            this.airMass = airMass;
            this.isLinear = isLinear;
            this.watermixMass = watermixMass;
            this.waterMixDLLindex = waterMixDLLindex;
            this.waterMixPercentage = waterMixPercentage;
            intakeConfigs = new List<IntakeConfig>();
            outtakeConfigs = new List<OuttakeConfig>();
        }
        public class IntakeConfig
        {
            public int length;
            public int height;
            public int numberRows;
            public int numberCircuits;
            public int finSpacing;
            public int finThickness;
            public int airmass;
            public int watermixMass;
            public double airTemperatureIN;
            public double airHumidityIN;
            public double airTemperatureOUT;
            public double waterMixTemperatureIN;
            public CurrentConfig.IntakeConfig splitConfig;
            public IntakeConfig(int length, int height, int numberRows, int numberCircuits, int finSpacing, int finThickness, double airTemperatureIN, double airHumidityIN, int airmass, int watermixMass, double airTemperatureOUT = 10000, double waterMixTemperatureIN = 1000)
            {
                this.length = length;
                this.height = height;
                this.numberRows = numberRows;
                this.numberCircuits = numberCircuits;
                this.finSpacing = finSpacing;
                this.finThickness = finThickness;
                this.airTemperatureIN = airTemperatureIN;
                this.airHumidityIN = airHumidityIN;
                this.airmass = airmass;
                this.watermixMass = watermixMass;
                this.airTemperatureOUT = airTemperatureOUT;
                this.waterMixTemperatureIN = waterMixTemperatureIN;
            }
        }
        public List<IntakeConfig> intakeConfigs = new List<IntakeConfig>();

        public class OuttakeConfig
        {
            public int length;
            public int height;
            public int numberRows;
            public int numberCircuits;
            public int finSpacing;
            public int finThickness;
            public int airmass;
            public int watermixMass;
            public double airTemperatureIN;
            public double airHumidityIN;
            public double power;
            public double waterMixTemperatureIN;
            public CurrentConfig.OuttakeConfig splitConfig;

            public OuttakeConfig(int length, int height, int numberRows, int numberCircuits, int finSpacing, int finThickness, double airTemperatureIN, double airHumidityIN, int airmass, int watermixMass, double power = 0, double waterMixTemperatureIN = 1000)
            {
                this.length = length;
                this.height = height;
                this.numberRows = numberRows;
                this.numberCircuits = numberCircuits;
                this.finSpacing = finSpacing;
                this.finThickness = finThickness;
                this.airTemperatureIN = airTemperatureIN;
                this.airHumidityIN = airHumidityIN;
                this.airmass = airmass;
                this.watermixMass = watermixMass;
                this.power = power;
                this.waterMixTemperatureIN = waterMixTemperatureIN; 
            }
        }
        public List<OuttakeConfig> outtakeConfigs = new List<OuttakeConfig>();

        public CurrentConfig Clone(CurrentConfig currentConfig)
        {
            CurrentConfig newConf = new CurrentConfig(currentConfig.airMass,currentConfig.watermixMass,currentConfig.waterMixDLLindex,currentConfig.waterMixPercentage,currentConfig.isLinear);
            foreach (var intake in currentConfig.intakeConfigs)
            {
                newConf.intakeConfigs.Add(new IntakeConfig(intake.length,intake.height, intake.numberRows, intake.numberCircuits, intake.finSpacing, intake.finThickness, intake.airTemperatureIN, intake.airHumidityIN,intake.airmass, intake.watermixMass,intake.airTemperatureOUT, intake.waterMixTemperatureIN));
            }
            foreach (var outtake in currentConfig.outtakeConfigs)
            {
                newConf.outtakeConfigs.Add(new OuttakeConfig(outtake.length, outtake.height, outtake.numberRows, outtake.numberCircuits, outtake.finSpacing, outtake.finThickness, outtake.airTemperatureIN, outtake.airHumidityIN, outtake.airmass, outtake.watermixMass, outtake.power, outtake.waterMixTemperatureIN));
            }

            return newConf;
        }
    }

    class Port
    {
        //TODO remove connectedComponent and make it something easier, maybe remove Port all together 
        public Fluid fluid;
        public int ID = 0;
        public Tuple<Component, Component> connectedComponent;

        public Port()
        {
            ID = Graph.portIDcounter++;
        }
        public Port(FluidType fluid)
        {
            ID = Graph.portIDcounter++;
            switch (fluid)
            {
                case FluidType.AIR: this.fluid = new Air();
                    break;
                case FluidType.WATERGLYKOL: this.fluid = new WaterMix();
                    break;
                default:
                    break;

            }
        }
    }

    #region Components
    abstract class Component
    {
        public List<Port> inputPorts = new List<Port>();
        public List<Port> outputPorts = new List<Port>();
        public double pressureDropAir;
        public double pressureDropWaterMix;
        //public List<Connection> ConnectedConnectionsInput = new List<Connection>();
        public List<Connection> ConnectedConnectionsInput { get 
            {
                List<Connection> res = new List<Connection>();
                foreach (var port in inputPorts)
                {
                    if (port.connectedComponent.Item1.GetType() == typeof(Connection))
                    {
                        res.Add((Connection) port.connectedComponent.Item1);
                    }
                }
                return res;
            } 
        }
        public List<Connection> ConnectedConnectionsOutput
        {
            get
            {
                List<Connection> res = new List<Connection>();
                foreach (var port in outputPorts)
                {
                    if (port.connectedComponent.Item2.GetType() == typeof(Connection))
                    {
                        res.Add((Connection)port.connectedComponent.Item2);
                    }
                }
                return res;
            }
        }
        //public List<Connection> ConnectedConnectionsOutput = new List<Connection>();

        public int iterationComputed = -1;  // TODO remove iterationComputed

        public int ID { get; private set; }

        public void SetID(int id)
        {
            ID = id;
        }

        abstract public bool Compute(int iterationComputed);
        public bool ReadyToComputed(int iterationCount, List<int> wasSeen) // TODO remove iterationComputed
        {
            foreach (var inputPort in inputPorts)
            {
                if (!wasSeen.Contains(inputPort.ID))
                {
                    return false;
                }
            }
            return true;
        }
        
    }

    #region Heat Exchanger
    class HeatExchanger : Component
    {
        public HeatExchangerConfig DLLconfig;
        public object[] DLLoutput = new object[Utility.outputDllSize];
        public bool isIntake;
        public bool computeWithTemperature = false;
        public HeatExchanger splitedHeatexchanger = null;
        public HeatExchanger wasSplitByHeatexchanger = null;
        public int Error { get { return DLLoutput[0] == null || Convert.ToString(DLLoutput[0]) == "" ? 0 : Convert.ToInt32(DLLoutput[0]); } }
        public double OutputPower { get { return Convert.ToDouble(DLLoutput[16] ?? "0"); } }
        public double OutputAirTempOut { get { return Convert.ToDouble(DLLoutput[21] ?? "0"); } }

        public HeatExchanger(object[] input)
        {
            DLLconfig = new HeatExchangerConfig(input);
            isIntake = DLLconfig.IsIntake;

        }
        public override bool Compute(int iterationComputed)
        {
            for(int i = 0; i < inputPorts.Count; i++)
            {
                if (inputPorts[i].fluid.type == FluidType.AIR)
                {
                    DLLconfig.AirTempIn = inputPorts[i].fluid.temperature;
                    DLLconfig.AirHumidity = ((Air)inputPorts[i].fluid).humidity;
                }
                else
                {
                    if (inputPorts[i].fluid.temperature < 100 || computeWithTemperature)
                    {
                        DLLconfig.WaterMixTempIn = inputPorts[i].fluid.temperature;
                    }
                    if (!Utility.EqualDouble(((WaterMix)inputPorts[i].fluid).power, 0) && !computeWithTemperature)
                        //if (!isIntake || computeWithTemperature)
                    {
                        DLLconfig.Power = ((WaterMix)inputPorts[i].fluid).power;
                        DLLconfig.AirTempOut = 0.0;
                    }



                }
            }

            var input = DLLconfig.Config;
            var output = this.DLLoutput;

            Graph.dll.BlackBoxComputing(ref input, ref output);

            for (int i = 0; i < outputPorts.Count; i++)
            {
                if (outputPorts[i].fluid.type== FluidType.AIR)
                {
                    outputPorts[i].fluid.temperature = Convert.ToDouble(this.DLLoutput[21] ==""? "0" : this.DLLoutput[21]);
                    ((Air)outputPorts[i].fluid).humidity = Convert.ToDouble(this.DLLoutput[22] =="" ? "0" : this.DLLoutput[22]);
                }
                if (outputPorts[i].fluid.type == FluidType.WATERGLYKOL)
                {
                    outputPorts[i].fluid.temperature = Convert.ToDouble(this.DLLoutput[34] == ""? "0" : this.DLLoutput[34]);
                    ((WaterMix)outputPorts[i].fluid).power = Convert.ToDouble(this.DLLoutput[16] == "" ? "0" : this.DLLoutput[16]);
                }
                pressureDropAir = Convert.ToDouble(this.DLLoutput[28] == "" ? "0" : this.DLLoutput[28]);
                pressureDropWaterMix = Convert.ToDouble(this.DLLoutput[37] == "" ? "0" : this.DLLoutput[37]);
            }

            this.iterationComputed = iterationComputed;
            return true;
        }

        public void InitizeCurrentConfig(CurrentConfig.IntakeConfig intakeConfig, CurrentConfig currentConfig)
        {
            DLLconfig.Finspacing = intakeConfig.finSpacing;
            DLLconfig.Finthickness = intakeConfig.finThickness;
            DLLconfig.NumberRows = intakeConfig.numberRows;
            DLLconfig.NumberCircuits = intakeConfig.numberCircuits;
            DLLconfig.Length = intakeConfig.length;
            DLLconfig.Height = intakeConfig.height;
            DLLconfig.Airmass = intakeConfig.airmass;
            DLLconfig.Config[13] = "" + currentConfig.waterMixDLLindex; //use a string as it is direct to the DLL config
            DLLconfig.Config[16] = "" + currentConfig.waterMixPercentage; //use a string as it is direct to the DLL config

        }

        public void InitizeCurrentConfig(CurrentConfig.OuttakeConfig outtakeConfig, CurrentConfig currentConfig)
        {
            DLLconfig.Finspacing = outtakeConfig.finSpacing;
            DLLconfig.Finthickness = outtakeConfig.finThickness;
            DLLconfig.NumberRows = outtakeConfig.numberRows;
            DLLconfig.NumberCircuits = outtakeConfig.numberCircuits;
            DLLconfig.Length = outtakeConfig.length;
            DLLconfig.Height = outtakeConfig.height;
            DLLconfig.Airmass = outtakeConfig.airmass;
            DLLconfig.Config[13] = "" + currentConfig.waterMixDLLindex; //use a string as it is direct to the DLL config
            DLLconfig.Config[16] = "" + currentConfig.waterMixPercentage; //use a string as it is direct to the DLL config
        }
    }

    class HeatExchangerConfig
    {
        private object[] config = new object[Utility.inputDllSize];

        public object[] Config { get { return config; } set { config = value; } }

        public int Height { get { return Convert.ToInt32(config[3]); } set { config[3] = "" + value; } }
        public int Length { get { return Convert.ToInt32(config[4]); } set { config[4] = "" + value; } }
        public int NumberRows { get { return Convert.ToInt32(config[12]); } set { config[12] = "" + value; } }
        public int NumberCircuits { get { return Convert.ToInt32(config[10]); } set { config[10] = "" + value; } }
        public int Finspacing { get { return Convert.ToInt32(config[11]); } set { config[11] = "" + value; } }
        public int Finthickness { get { return Convert.ToInt32(config[33]); } set { config[33] = "" + value; } }
        public int Airmass { get { return Convert.ToInt32(config[33]); } set { config[23] = "" + value; } }

        public double AirTempIn { get { return Convert.ToDouble(config[28]); } set { config[28] = "" + value; } }
        public double AirTempOut { get { return Convert.ToDouble(config[31]); } set { config[31] = "" + value; } }
        public double AirHumidity { get { return Convert.ToDouble(config[30]); } set { config[30] = "" + value; } }
        public double WaterMixTempIn { get { return Convert.ToDouble(config[14]); } set { config[14] = "" + value; } }
        public double Power { get { return Convert.ToDouble(config[26]); } set { config[26] = "" + value; } }
        
        public bool IsIntake { get { return Convert.ToString(config[config.Length - 1]) == "0"; } }

        public HeatExchangerConfig(object[] input)
        {
            Debug.Assert(input.Length == config.Length);
            Array.Copy(input, 0, config, 0, input.Length);
        }
    }
    #endregion

    class Fan : Component
    {
        public double power;
        public double energyTransmissionRate;

        public Fan()
        {

        }
        public override bool Compute(int iterationComputed)
        {
            //TODO make something actually usefull
            if (inputPorts.Count == outputPorts.Count && inputPorts.Count == 1)
            {
                outputPorts[0].fluid = inputPorts[0].fluid;
                this.iterationComputed = iterationComputed;
            }
            return true;
        }

    }

    class Pump : Component
    {
        public double power;
        public double energyTransmissionRate;

        public Pump()
        {

        }
        public override bool Compute(int iterationComputed)
        {
            //TODO make something actually usefull
            if (inputPorts.Count == outputPorts.Count && inputPorts.Count == 1)
            {
                outputPorts[0].fluid = inputPorts[0].fluid;
                this.iterationComputed = iterationComputed;
            }
            return true;
        }

    }

    class PlateHeatExchanger : Component
    {
        public override bool Compute(int iterationComputed)
        {
            throw new NotImplementedException();
        }
    }

    #region Heat Pump
    class HeatPump : Component
    {
        public double power;
        public double energyTransmissionRate;
        public bool isHeating;
        public decimal[] test_coolingCapacity = new decimal[] { 30787.4m, 1061.66m, -418.7266m, 11.05278m, -10.06807m, 1.984894m, 0.0009370273m, -0.05694705m, 0.03001729m, -0.0000004184396m};
        public decimal[] test_powerDrain = new decimal[] { };
        public CompressionPolynom polynom;

        public override bool Compute(int iterationComputed)
        {
            throw new NotImplementedException();
        }

    }

    class HeatPumpTop : HeatPump
    {
        public HeatPumpTop(bool isHeating)
        {
             polynom= new CompressionPolynom(1.0m, 32.0m, test_coolingCapacity, test_powerDrain); //TEST
            this.isHeating = isHeating;
            //TODO someone has to set the evaporation temperature and the condensation temperature 
        }
        public override bool Compute(int iterationComputed)
        {
            Debug.Assert(inputPorts.Count == outputPorts.Count && inputPorts.Count == 1);
            for (int i = 0; i < inputPorts.Count; i++)
            {
                if (isHeating)
                {
                    double deltaT = Convert.ToDouble(polynom.power);
                    outputPorts[0].fluid = inputPorts[0].fluid;
                    outputPorts[0].fluid.temperature -= deltaT;
                }
                else
                {
                    double deltaT = Convert.ToDouble(polynom.power + polynom.electricEnergy);
                    outputPorts[0].fluid = inputPorts[0].fluid;
                    outputPorts[0].fluid.temperature += deltaT;
                }
                this.iterationComputed = iterationComputed;
            }
            return true;
        }
    }

    class HeatPumpBottom : HeatPump
    {
        public HeatPumpBottom(bool isHeating)
        {
            polynom = new CompressionPolynom(1.0m, 32.0m, test_coolingCapacity, test_powerDrain); //TEST
            this.isHeating = isHeating;
            //TODO someone has to set the evaporation temperature and the condensation temperature 
        }
        public override bool Compute(int iterationComputed)
        {
            Debug.Assert(inputPorts.Count == outputPorts.Count && inputPorts.Count == 1);
            if (isHeating)
            {
                double deltaT = Convert.ToDouble(polynom.power + polynom.electricEnergy);
                outputPorts[0].fluid = inputPorts[0].fluid;
                outputPorts[0].fluid.temperature += deltaT;
            }
            else
            {
                double deltaT = Convert.ToDouble(polynom.power);
                outputPorts[0].fluid = inputPorts[0].fluid;
                outputPorts[0].fluid.temperature -= deltaT;
            }
            this.iterationComputed = iterationComputed;
            return true;
        }
    }

    class CompressionPolynom
    {
        public decimal S; // evaporation temperature
        public decimal D; // condensation temperature
        public decimal[] C_coolingCapacity = new decimal[10];
        public decimal[] C_powerDrain = new decimal[10];
        public decimal[] C_currentDrain = new decimal[10];
        public decimal[] C_materialTroughput = new decimal[10];

        //Carefull, different polynomy for different compressors
        // X = (C0) + (C1*S) + (C2*D) + (C3*S^2) + (C4*S*D) + (C5*D^2) + (C6*S^3) + (C7*D*S^2) +(C8*S*D^2) + (C9*D^3)
        public decimal power { get { return C_coolingCapacity[0] + C_coolingCapacity[1]*S + C_coolingCapacity[2]*D + C_coolingCapacity[3]*S*S + C_coolingCapacity[4]*S*D + C_coolingCapacity[5]*D*D + C_coolingCapacity[6]*S*S*S + C_coolingCapacity[7]*D*S*S + C_coolingCapacity[8]*S*D*D + C_coolingCapacity[9]*D*D*D; } }
        public decimal electricEnergy { get { return C_powerDrain[0] + C_powerDrain[1] * S + C_powerDrain[2] * D + C_powerDrain[3] * S * S + C_powerDrain[4] * S * D + C_powerDrain[5] * D * D + C_powerDrain[6] * S * S * S + C_powerDrain[7] * D * S * S + C_powerDrain[8] * S * D * D + C_powerDrain[9] * D * D * D; } }

        public CompressionPolynom(decimal S, decimal D, decimal[] C_coolingCapacity, decimal [] C_powerDrain)
        {
            this.S = S;
            this.D = D;
            Debug.Assert(C_coolingCapacity.Length == this.C_coolingCapacity.Length);
            Array.Copy(C_coolingCapacity, this.C_coolingCapacity, C_coolingCapacity.Length);
            Array.Copy(C_powerDrain, this.C_powerDrain, C_powerDrain.Length);


        }



    }

    #endregion

    class Valvle : Component
    {
        public List<double> distribution = new List<double>();
        public ValveType type;

        public Valvle()//TODO choose type
        {

        }
        public override bool Compute(int iterationComputed)
        {
            throw new NotImplementedException();
        }

    }

    class EvaporativeCooling : Component
    {
        public double waterAmount;
        public double power;
        public override bool Compute(int iterationComputed)
        {
            throw new NotImplementedException();
        }

    }

    class Connection : Component
    {
        public Fluid fluid; //TODO specify what this component should be
        public double length;
        public int diameter;
        public List<double> distribution = new List<double>();

        public Connection(Component start, Component end, FluidType type)
        {
            List<Component> starts = new List<Component> { start };
            List<Component> ends = new List<Component> { end };
            SetFluidofConnection(type);
            initialize(starts, ends, type);
        }
        public Connection(FluidType type)
        {
            SetFluidofConnection(type);
        }
        public Connection(FluidType type, bool initializePorts)
        {
            SetFluidofConnection(type);
            Port input = new Port(type);
            inputPorts.Add(input);
            Port output = new Port(type);
            outputPorts.Add(output);

        }
        public Connection(Component start, List<Component> ends, FluidType type)
        {
            List<Component> starts = new List<Component> { start };
            SetFluidofConnection(type);
            initialize(starts, ends, type);
        }
        public Connection(List<Component> starts, List<Component> ends, FluidType type)
        {
            SetFluidofConnection(type);
            initialize(starts, ends, type);
        }
        private void initialize(List<Component> starts, List<Component> ends, FluidType type)
        {
            foreach (var start in starts)
            {
                Port input = new Port();
                //start.ConnectedConnectionsOutput.Add(this);
                input.connectedComponent = Tuple.Create(start, (Component)this);
                SetFluidOfPort(input, type);
                inputPorts.Add(input);
                start.outputPorts.Add(input);
            }

            foreach (var end in ends)
            {
                Port output = new Port();
                //end.ConnectedConnectionsInput.Add(this);
                output.connectedComponent = Tuple.Create((Component)this, end);
                SetFluidOfPort(output, type);
                outputPorts.Add(output);
                end.inputPorts.Add(output);
            }
        }
        private void SetFluidofConnection(FluidType type)
        {
            switch (type)
            {
                case FluidType.AIR:
                    fluid = new Air();
                    break;
                case FluidType.WATERGLYKOL:
                    fluid = new WaterMix();
                    break;
                default:
                    break;
            }
        }
        private void SetFluidOfPort(Port port, FluidType type)
        {
            switch (type)
            {
                case FluidType.AIR:
                    port.fluid = new Air();
                    break;
                case FluidType.WATERGLYKOL:
                    port.fluid = new WaterMix();
                    break;
                default:
                    break;
            }
        }
        public override bool Compute(int iterationComputed)
        {
            if (inputPorts.Count == outputPorts.Count && inputPorts.Count == 1)
            {
                outputPorts[0].fluid = inputPorts[0].fluid;
                fluid = inputPorts[0].fluid;

                if (outputPorts[0].connectedComponent.Item2.GetType() == typeof(AirStartEndPoint))
                {
                    ((AirStartEndPoint)outputPorts[0].connectedComponent.Item2).air = (Air)inputPorts[0].fluid;
                }
            }
            else
            {
                //TODO compute a mixing point
                double mixingPoint = inputPorts[0].fluid.temperature;
                if (inputPorts.Count > 1)
                {
                    mixingPoint = 0;
                    foreach (var connection in ConnectedConnectionsInput)
                    {
                        //compute fluidtemperature * watermixMass of the corresponding heatExchanger
                        //TODO make more easily accessible
                        mixingPoint += connection.fluid.temperature * Convert.ToDouble(((HeatExchanger)connection.inputPorts[0].connectedComponent.Item1).DLLconfig.Config[37]);
                    }

                    mixingPoint = mixingPoint / Graph.currentConfig.watermixMass;
                }
                foreach (var outputPort in outputPorts)
                {
                    //outputPort.fluid = inputPorts[0].fluid;
                    outputPort.fluid = new WaterMix((WaterMix)inputPorts[0].fluid);
                    outputPort.fluid.temperature = mixingPoint;
                    //TODO remove from forloop, make it a reference to the port fluid??
                    fluid = outputPort.fluid;
                }
            }

            this.iterationComputed = iterationComputed;
            return true;
        }

    }

    class AirStartEndPoint : Component
    {
        public Air air = new Air();
        public bool canChange = true;

        public AirStartEndPoint(double temperature, double humidity, humidityUnit unit = humidityUnit.PERCENT)
        {
            air = new Air(temperature, humidity, unit);
            canChange = false;
        }
        public AirStartEndPoint()
        {
            air.temperature = 1000.0;
        }
        public AirStartEndPoint(double temperature, bool canChange = true)
        {
            this.canChange = canChange;
            air.temperature = temperature;
        }
        public override bool Compute(int iterationComputed)
        {
            for (int i = 0; i < outputPorts.Count; i++)
            {
                if (canChange)
                {
                    if (inputPorts.Count == 0)
                    {
                        ((Air)outputPorts[i].fluid).humidity = air.humidity;
                    }
                    else
                    {
                        air.humidity = ((Air)inputPorts[i].fluid).humidity;
                    }
                    this.iterationComputed = iterationComputed;
                    return true;
                }
                if (inputPorts.Count == 0)
                {
                    ((Air)outputPorts[i].fluid).temperature = air.temperature;
                    ((Air)outputPorts[i].fluid).humidity= air.humidity;
                }
                else
                {
                    air.temperature = ((Air)inputPorts[i].fluid).temperature;
                    air.humidity = ((Air)inputPorts[i].fluid).humidity;
                }

            }

            this.iterationComputed = iterationComputed;
            return true;
        }


    }

    #endregion

    #region Fluid
    abstract class Fluid
    {
        public double specificHeat;
        public double density;
        public double temperature = 10000;
        public double pressure;
        public FluidType type;

    }

    class WaterMix : Fluid
    {
        public double mixingRelation;
        public double power;

        public WaterMix(double temperature, double power, double mixingRelation = 25)
        {
            this.temperature = temperature;
            this.power = power;
            this.type = FluidType.WATERGLYKOL;
            this.density = Utility.densityWaterMix;
            this.mixingRelation = mixingRelation;
            this.specificHeat = Utility.specificHeatWaterMix;
        }
        public WaterMix()
        {
            this.power = 0;
            this.type = FluidType.WATERGLYKOL;
            this.density = Utility.densityWaterMix;
            this.mixingRelation = 25;
            this.specificHeat = Utility.specificHeatWaterMix;
        }
        public WaterMix(WaterMix waterMix)
        {
            this.specificHeat = waterMix.specificHeat;
            this.density = waterMix.density;
            this.temperature = waterMix.temperature;
            this.pressure = waterMix.pressure;
            this.power = waterMix.power;
            this.mixingRelation = waterMix.mixingRelation;
        }

    }

    class Air : Fluid
    {
        public double humidity;
        public humidityUnit unit;

        public Air(double temperature, double humidity, humidityUnit unit = humidityUnit.PERCENT)
        {
            this.temperature = temperature;
            this.humidity = humidity;
            this.unit = unit;
        }
        public Air(Air air)
        {
            this.specificHeat = air.specificHeat;
            this.density = air.density;
            this.temperature = air.temperature;
            this.pressure = air.pressure;
            this.humidity = air.humidity;
            this.unit = air.unit;
        }
        public Air()
        {
            this.type = FluidType.AIR;
        }
    }
    #endregion
}
