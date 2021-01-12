using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Microsoft.Win32;

namespace EcoConf.GUI
{
    /// <summary>
    /// Interaktionslogik für InputView.xaml
    /// </summary>
    public partial class InputView : Page
    {
        MainWindow mainWindow;
        iGUIConnector connector;

        bool isLinear = false;
        bool isManuel = false;

        public ObservableCollection<IntakeOuttakeInput> Intakes { get; set; } = new ObservableCollection<IntakeOuttakeInput>();
        public ObservableCollection<IntakeOuttakeInput> Outtakes { get; set; } = new ObservableCollection<IntakeOuttakeInput>();
        public ObservableCollection<string> Configuration { get; set; } = new ObservableCollection<string>();

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
        public Dictionary<string, int> hcConversionIndex = new Dictionary<string, int>()
            {
                {"HC4",1},
                {"HC6",2},
                {"HC9",3},
                {"HC12",4},
                {"HC16",5},
                {"HC20",6},
                {"HC25",7},
                {"HC30",8},
                {"HC36",9},
                {"HC42",10},
                {"HC49",11},
                {"HC56",12},
                {"HC64",13},
                {"HC72",14},
                {"HC80",15},
                {"HC90",16},
                {"HC100",17},
                {"HC110",18}
            };

        public int transMissiondegree = 0;

        List<int> waterMixChoice = new List<int>() { 10, 15, 20, 25, 30, 35 };
        List<int> waterMixStartEnd = new List<int>() { 0 , 6};

        public class WaterMix
        {
            /**
             * Indices:
             *      1: MEG
             *      2: MPG
             */
            public int dllIndex { get; }
            public int percentage { get; }
            public WaterMix(int dllIndex, int percentage)
            {
                this.dllIndex = dllIndex;
                this.percentage = percentage;
            }
        }

        public WaterMix waterMix = new WaterMix(0, 25);
        public int version = 0;
        public InputView(MainWindow mW, iGUIConnector guiConnector)
        {
            InitializeComponent();
            mainWindow = mW;
            DataContext = this;
            connector = guiConnector;
            //IntakeOuttakeInput init = new IntakeOuttakeInput(true);
            //init.Name = defaultNameIntake+'_' + init.ID;
            //Array.Copy(MainSystem.DefaultIntake, init.inputFields, MainSystem.DefaultIntake.Length); //TODO use some defaults?
            //if (init.inputFields.Length > 2) init.inputFields[init.inputFields.Length - 2] = "" + 0;
            //if (init.inputFields.Length > 2) init.inputFields[init.inputFields.Length - 1] = ""+init.ID;
            //Intakes.Add(init);
            //init = new IntakeOuttakeInput(false);
            //init.Name = defaultNameOuttake+'_'+ init.ID;
            //Array.Copy(MainSystem.DefaultOuttake, init.inputFields, MainSystem.DefaultOuttake.Length); //TODO use some defaults?
            //if (init.inputFields.Length > 2) init.inputFields[init.inputFields.Length - 2] = "" + 1;
            //if (init.inputFields.Length > 2) init.inputFields[init.inputFields.Length - 1] = "" + init.ID;
            //Outtakes.Add(init);
            Configuration = new ObservableCollection<string>(MainSystem.DefaultConfiguration);
            //ShowInput();
            ClearInput(null,null);
        }


        //is called if the compute button is pressed
        private void Compute_Click(object sender, RoutedEventArgs e)
        {
            Utility.computeWithNomad = !computeWithNomad.IsChecked.HasValue ? false : computeWithNomad.IsChecked.Value;
            foreach (var window in Application.Current.Windows)
            {
                if(window.GetType() == typeof(IntakeOuttakeWindow))
                {
                    ((IntakeOuttakeWindow)window).UpdateIntakeOuttakeFields();
                }
            }
            connector.TransferInputToSystem(PrepareStoringTheConfig());
            connector.Computation();
        }


        //is called when the file browser button is pressed
        private void Filebrowser_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //string[] help = AppDomain.CurrentDomain.BaseDirectory.Split('\\');
            //for (int i = 0; i < help.Length - 3; i++)
            //{
            //    path += help[i] + @"\";
            //}
            path = @"C:\EtaWin\masken\";
            openFileDialog.InitialDirectory = path;
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ClearInput(new object(), new RoutedEventArgs());
                connector.LoadConfiguration(openFileDialog.FileName);
                Compute_Button.IsEnabled = true;
                ShowInput();
            }
        }


        //is called if the default button is pressed, loads default values in the input fields
        private void Default_Click(object sender, RoutedEventArgs e)
        {
            ClearInput(new object(), new RoutedEventArgs());
            connector.UseDefaultValues();
            Compute_Button.IsEnabled = true;
            ShowInput();

        }


        /**
         * shows the input in the GUI
         */
         public void ShowInput()
         {
            Intakes.Clear();
            Outtakes.Clear();
            IntakeOuttakeInput.Reset();

            iInput input = connector.GetInputFromSystem();

            airmass.Text = input.GetInputConfiguration()[3];
            degree.Text = input.GetInputConfiguration()[2];

            //TODO check if there are more than two intakes/outtakes
            // should be good now, as otherwise there will be custom 
            if (hcConversionIndex.ContainsKey(input.GetInputConfiguration()[5].ToString()))
            {
                HCSelecetion.SelectedIndex = hcConversionIndex[input.GetInputConfiguration()[5].ToString()];
                height.Text = input.GetInputBlackbox()[0][3].ToString();
                length.Text = input.GetInputBlackbox()[0][4].ToString();
            }
            else
            {
                HCSelecetion.SelectedIndex = 0;
                height.Text = "---";
                length.Text = "---";
            }

            versionSelecetion.SelectedIndex = Convert.ToInt32(input.GetInputConfiguration()[6] == ""? "0": input.GetInputConfiguration()[6]);
            watermixSelecetion.SelectedIndex = waterMixStartEnd[Convert.ToInt32(input.GetInputConfiguration()[7] == ""? ""+waterMix.dllIndex : input.GetInputConfiguration()[7])] + waterMixChoice.FindIndex(x => x == Convert.ToInt32(input.GetInputConfiguration()[8] == ""? ""+waterMix.percentage : input.GetInputConfiguration()[8]));


            foreach (var item in input.GetInputBlackbox())
            {
                IntakeOuttakeInput tmp = null;
                Debug.Assert(item.Length > 2);
                if (Convert.ToInt32(item[item.Length-2]) == 0)
                {
                    tmp = new IntakeOuttakeInput(true);
                    tmp.Name = defaultNameIntake+'_' + item.Last();
                    Array.Copy(item, tmp.inputFields, item.Length); //TODO use some defaults?
                    if (tmp.inputFields.Length > 2) tmp.inputFields[tmp.inputFields.Length - 2] = "" + 0;
                    if (tmp.inputFields.Length > 2) tmp.inputFields[tmp.inputFields.Length - 1] = "" + tmp.ID;
                    Intakes.Add(tmp);
                }else if (Convert.ToInt32(item[item.Length - 2]) == 1)
                {
                    tmp = new IntakeOuttakeInput(false);
                    tmp.Name = defaultNameOuttake+'_'+ item.Last();
                    Array.Copy(item, tmp.inputFields, item.Length); //TODO use some defaults?
                    if (tmp.inputFields.Length > 2) tmp.inputFields[tmp.inputFields.Length - 2] = "" + 1;
                    if (tmp.inputFields.Length > 2) tmp.inputFields[tmp.inputFields.Length - 1] = "" + tmp.ID;
                    Outtakes.Add(tmp);
                }
                Debug.Assert(tmp != null);

            }
         }

        //clears the the input fields
        private void ClearInput(object sender, RoutedEventArgs e)
        {
            degree.Text = "68";
            airmass.Text = "60000";
            if (mainWindow != null) mainWindow.OutputButton.IsEnabled = false;
            Compute_Button.IsEnabled = false;
            //foreach (var item in Intakes) Intakes.Remove(item);
            //foreach (var item in Outtakes) Outtakes.Remove(item);
            Intakes.Clear();
            Outtakes.Clear();
            foreach (var window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(IntakeOuttakeWindow))
                {
                    ((IntakeOuttakeWindow)window).Close();
                }
            }
            IntakeOuttakeInput.Reset();
            if (mainWindow != null) mainWindow.NewOutputView(); //TODO not needed if the view does not present anything
            AddIntake_Click(null,null);
            AddOuttake_Click(null, null);
        }

        /**
         * keep the global configuration complete
         */
        private void ClearHeatexchangerInput()
        {
            if (mainWindow != null) mainWindow.OutputButton.IsEnabled = false;
            Compute_Button.IsEnabled = false;
            int skip = 1;
            while (Intakes.Count - skip  > 0)
            {
                if (Intakes[Intakes.Count - skip].inputFields[Utility.blackBoxPartWasSplit] == "1")
                {
                    skip++;
                    continue;
                }
                Intakes.RemoveAt(Intakes.Count - skip);
            }
            skip = 1;
            while (Outtakes.Count - skip > 0)
            {
                if (Outtakes[Intakes.Count - skip].inputFields[Utility.blackBoxPartWasSplit] == "1")
                {
                    skip++;
                    continue;
                }
                Outtakes.RemoveAt(Outtakes.Count - skip);
            }
            foreach (var window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(IntakeOuttakeWindow))
                {
                    ((IntakeOuttakeWindow)window).Close();
                }
            }
            IntakeOuttakeInput.Reset();
            if (mainWindow != null) mainWindow.NewOutputView(); //TODO not needed if the view does not present anything
        }


        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //string[] help = AppDomain.CurrentDomain.BaseDirectory.Split('\\');
            //for (int i = 0; i < help.Length - 3; i++)
            //{
            //    path += help[i] + @"\";
            //}
            path = @"C:\EtaWin\masken\";
            saveFileDialog.InitialDirectory = path;
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            string prep = PrepareStoringTheConfig();
            if (saveFileDialog.ShowDialog() == true)
            {
                connector.StoreConfiguration(saveFileDialog.FileName, prep);
            }
        }


        private string PrepareStoringTheConfig()
        {
            // can use this a C:/EtaWin/tmp/tmpfile???
            //string tmpfile = HandleTmpFiles.CreateTmpFile();
            string tmpfile = Utility.guiConfigFile;
            List<string> inputs = new List<string>();

            Debug.Assert(Configuration.Count == Utility.inputConfigurationSize);

            Configuration[Utility.configPartIntakeCount] = ""+Intakes.Count;
            Configuration[Utility.configPartOuttakeCount] = "" + Outtakes.Count;
            Configuration[Utility.configPartTemperaturetransmissionDegree] = degree.Text;
            Configuration[Utility.configPartAirmass] = airmass.Text;
            Configuration[Utility.configPartWatermixMass] = watermixmass.Text;
            Configuration[Utility.configPartHC] = ((ComboBoxItem)HCSelecetion.SelectedItem).Content.ToString();
            Configuration[Utility.configPartVersion] = "" + version;
            Configuration[Utility.configPartWatermixType] = "" + waterMix.dllIndex;
            Configuration[Utility.configPartWatermixPercentage] = "" + waterMix.percentage;
            Configuration[Utility.configPartisLinear] = isLinear ? "1" : "0";

            for (int i = 0; i < Configuration.Count; i++)
            {
                inputs.Add("ConParameter_in("+i+")="+Configuration[i]);
            }
            inputs.Add("---");
            foreach (var intake in Intakes)
            {
                for (int i = 0;i < intake.inputFields.Length; i++)
                {
                    if (i == 13)
                        inputs.Add(intake.Name + "(" + i + ")=" + waterMix.dllIndex);
                    else if (i == 16)
                        inputs.Add(intake.Name + "(" + i + ")=" + waterMix.percentage);
                    else if(i == 31)
                        if (Convert.ToInt32(Configuration[Utility.configPartTemperaturetransmissionDegree]) >= 68)
                            inputs.Add(intake.Name + "(" + i + ")=" + Utility.TransmissionDegreeToTemperature(Convert.ToDouble(Configuration[Utility.configPartTemperaturetransmissionDegree]), Convert.ToDouble(intake.inputFields[Utility.blackBoxPartAirTempIn]), 25.0)); //TODO let the user input the airout globally, othewise we do not know, which one to use
                        else
                            inputs.Add(intake.Name + "(" + i + ")=" + Configuration[Utility.configPartTemperaturetransmissionDegree]);
                    else
                        inputs.Add(intake.Name+ "(" + i + ")=" + intake.inputFields[i]);
                }
                inputs.Add("+++");
            }
            foreach(var outtake in Outtakes)
            {
                for (int i = 0; i < outtake.inputFields.Length; i++)
                {
                    if (i == 13)
                        inputs.Add(outtake.Name + "(" + i + ")=" + waterMix.dllIndex);
                    else if (i == 16)
                        inputs.Add(outtake.Name + "(" + i + ")=" + waterMix.percentage);
                    else
                        inputs.Add(outtake.Name+ "(" + i + ")=" + outtake.inputFields[i]);
                }
                inputs.Add("+++");
            }

            try
            {

                #region WriteToFile
                StreamWriter storefile;

                storefile = new StreamWriter(tmpfile);
                foreach (string line in inputs)
                {
                    storefile.WriteLine(line);
                }
                storefile.Flush();
                storefile.Close();
                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write tmpfile, Error: " + e.Message);
            }
            return tmpfile;
        }

        #region HandleIntake

        string defaultNameIntake = (string)Application.Current.FindResource("nameIntake");
        private void AddIntake_Click(object sender, RoutedEventArgs e)
        {
            IntakeOuttakeInput intake = new IntakeOuttakeInput(true);
            intake.Name = defaultNameIntake+'_' + intake.ID;
            Array.Copy(MainSystem.DefaultIntake, intake.inputFields, MainSystem.DefaultIntake.Length); // TODO use some defaults?
            if (intake.inputFields.Length > 2) intake.inputFields[intake.inputFields.Length - 2] = "" + 0;
            if (intake.inputFields.Length > 2) intake.inputFields[intake.inputFields.Length - 1] = ""+intake.ID;
            intake.inputFields[23] = airmass.Text;
            intake.inputFields[37] = watermixmass.Text;
            intake.inputFields[3] = height.Text;
            intake.inputFields[4] = length.Text;
            Intakes.Add(intake);
            Compute_Button.IsEnabled = true;
        }

        private void RemoveIntake_Click(object sender, RoutedEventArgs e)
        {
            //TODO remove real intake
            List<IntakeOuttakeInput> currentSelected = new List<IntakeOuttakeInput>();
            foreach (IntakeOuttakeInput x in IntakeList.SelectedItems)
            {
                currentSelected.Add(x);
            }

            if (currentSelected == null) return;
            foreach (IntakeOuttakeInput item in currentSelected.Reverse<IntakeOuttakeInput>())
            {
                Intakes.Remove(item);
            }
        }
      
        private void MoveUpIntake_Click(object sender, RoutedEventArgs e)
        {
            List<int> currentIndices = new List<int>();
            for (int i = 0; i < IntakeList.SelectedItems.Count; i++)
            {
                currentIndices.Add(IntakeList.Items.IndexOf(IntakeList.SelectedItems[i]));
            }
            if (currentIndices.Count <= 0) return;
            currentIndices.Sort();
            for (int i = 0; i < IntakeList.SelectedItems.Count && currentIndices.First() > 0; i++)
            {
                Utility.MoveItemUp<IntakeOuttakeInput>(Intakes, currentIndices[i]);
            }
        }

        private void MoveDownIntake_Click(object sender, RoutedEventArgs e)
        {
            List<int> currentIndices = new List<int>();
            for (int i = 0; i < IntakeList.SelectedItems.Count; i++)
            {
                currentIndices.Add(IntakeList.Items.IndexOf(IntakeList.SelectedItems[i]));
            }
            if (currentIndices.Count <= 0) return;
            currentIndices.Sort();
            for (int i = IntakeList.SelectedItems.Count; i > 0 && currentIndices.Last() < IntakeList.Items.Count - 1; i--)
            {
                Utility.MoveItemDown<IntakeOuttakeInput>(Intakes, currentIndices[i - 1]);
            }
        }

        #endregion


        #region HandleOuttake

        string defaultNameOuttake = (string)Application.Current.FindResource("nameOuttake");
        private void AddOuttake_Click(object sender, RoutedEventArgs e)
        {
            IntakeOuttakeInput outtake = new IntakeOuttakeInput(false);
            outtake.Name = defaultNameOuttake+'_'+ outtake.ID;
            Array.Copy(MainSystem.DefaultOuttake, outtake.inputFields, MainSystem.DefaultOuttake.Length); //TODO use some defaults?
            if (outtake.inputFields.Length > 2) outtake.inputFields[outtake.inputFields.Length - 2] = "" + 1;
            if (outtake.inputFields.Length > 2) outtake.inputFields[outtake.inputFields.Length - 1] = ""+outtake.ID;
            outtake.inputFields[23] = airmass.Text;
            outtake.inputFields[37] = watermixmass.Text;
            outtake.inputFields[3] = height.Text;
            outtake.inputFields[4] = length.Text;
            Outtakes.Add(outtake);
            Compute_Button.IsEnabled = true;
        }

        private void RemoveOuttake_Click(object sender, RoutedEventArgs e)
        {
            //TODO remove real Outtake
            List<IntakeOuttakeInput> currentSelected = new List<IntakeOuttakeInput>();
            foreach (IntakeOuttakeInput str in OuttakeList.SelectedItems)
            {
                currentSelected.Add(str);
            }

            if (currentSelected == null) return;
            foreach (IntakeOuttakeInput item in currentSelected.Reverse<IntakeOuttakeInput>())
            {
                Outtakes.Remove(item);
            }
        }
        private void MoveUpOuttake_Click(object sender, RoutedEventArgs e)
        {
            List<int> currentIndices = new List<int>();
            for (int i = 0; i < OuttakeList.SelectedItems.Count; i++)
            {
                currentIndices.Add(OuttakeList.Items.IndexOf(OuttakeList.SelectedItems[i]));
            }
            if (currentIndices.Count <= 0) return;
            currentIndices.Sort();
            for (int i = 0; i < OuttakeList.SelectedItems.Count && currentIndices.First() > 0; i++)
            {
                Utility.MoveItemUp<IntakeOuttakeInput>(Outtakes, currentIndices[i]);

            }

        }

        private void MoveDownOuttake_Click(object sender, RoutedEventArgs e)
        {
            List<int> currentIndices = new List<int>();
            for (int i = 0; i < OuttakeList.SelectedItems.Count; i++)
            {
                currentIndices.Add(OuttakeList.Items.IndexOf(OuttakeList.SelectedItems[i]));
            }
            if (currentIndices.Count <= 0) return;
            currentIndices.Sort();
            for (int i = OuttakeList.SelectedItems.Count; i > 0 && currentIndices.Last() < OuttakeList.Items.Count - 1; i--)
            {

                Utility.MoveItemDown<IntakeOuttakeInput>(Outtakes, currentIndices[i - 1]);
            }
        }


        #endregion


        #region handleNewWindowsIntakeOuttake

        public static List<int> currentOpenIntake = new List<int>();
        public static List<int> currentOpenOuttake = new List<int>();
        private void IntakeList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != IntakeList)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    if (currentOpenIntake.Contains(((IntakeOuttakeInput)IntakeList.SelectedItem).ID))
                    {
                        break;
                    }
                    currentOpenIntake.Add(((IntakeOuttakeInput)IntakeList.SelectedItem).ID);
                    IntakeOuttakeWindow subWindow = new IntakeOuttakeWindow(mainWindow,connector,((IntakeOuttakeInput)IntakeList.SelectedItem), this);
                    subWindow.Show();

                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
        }

        private void OuttakeList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = (DependencyObject)e.OriginalSource;

            while (obj != null && obj != IntakeList)
            {
                if (obj.GetType() == typeof(ListBoxItem))
                {
                    if (currentOpenOuttake.Contains(((IntakeOuttakeInput)OuttakeList.SelectedItem).ID))
                    {
                        break;
                    }
                    currentOpenOuttake.Add(((IntakeOuttakeInput)OuttakeList.SelectedItem).ID);
                    IntakeOuttakeWindow subWindow = new IntakeOuttakeWindow(mainWindow, connector,((IntakeOuttakeInput)OuttakeList.SelectedItem), this);
                    subWindow.Show();

                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }

        }
        #endregion

        private void Heigth_TextChanged(object sender, TextChangedEventArgs e)
        {
            for(int i = 0; i < Intakes.Count; i++)
            {
                Intakes[i].inputFields[3] = height.Text;
            }

            for (int i = 0; i < Outtakes.Count; i++)
            {
                Outtakes[i].inputFields[3] = height.Text;
            }

            if (height != null && length != null)
            {
                foreach (KeyValuePair<string, Tuple<int,int>> entry in hcConversion)
                {
                    if(IsTextAllowed(height.Text) && entry.Value.Item1 == Convert.ToInt32(height.Text == ""? "0": height.Text))
                    {
                        return;
                    }
                }
                HCSelecetion.SelectedIndex = 0;
            }
        }

        private void Length_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (HCSelecetion == null) return;

            for (int i = 0; i < Intakes.Count; i++)
            {
                Intakes[i].inputFields[4] = length.Text;
            }

            for (int i = 0; i < Outtakes.Count; i++)
            {
                Outtakes[i].inputFields[4] = length.Text;
            }
            if (height != null && length != null)
            {
                foreach (KeyValuePair<string, Tuple<int, int>> entry in hcConversion)
                {
                    if (IsTextAllowed(length.Text))
                    {
                        if (entry.Value.Item2 == Convert.ToInt32(length.Text == "" ? "0" : length.Text))
                        {
                            return;
                        }
                    }
                }
                HCSelecetion.SelectedIndex = 0;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            isLinear = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            isLinear = false;
        }
        

        private void Airmass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (airmass.Text == "") return;

            double testDouble;
            if (!double.TryParse(airmass.Text, out testDouble))
            {
                // TODO make a messagebox at the compute button with everything that ist not good
                // MessageBox.Show("Bitte nur korrekte Zahlen eingeben");
                return;
            }
            
            double watermass = Convert.ToDouble(airmass.Text)*1.2 / (Utility.specificHeatWaterMix * Utility.densityWaterMix) * 1000;
            double watermassAdjusted = Math.Round(watermass*1.05/ 100.0, 0) * 100.0;
            watermixmass.Text = "" + watermassAdjusted;

            for (int i = 0; i < Intakes.Count; i++)
            {
                Intakes[i].inputFields[23] = airmass.Text;
            }

            for (int i = 0; i < Outtakes.Count; i++)
            {
                Outtakes[i].inputFields[23] = airmass.Text;
            }
        }

        private void Watermixmass_TextChanged(object sender, TextChangedEventArgs e)
        {
            for (int i = 0; i < Intakes.Count; i++)
            {
                Intakes[i].inputFields[37] = watermixmass.Text;
            }

            for (int i = 0; i < Outtakes.Count; i++)
            {
                Outtakes[i].inputFields[37] = watermixmass.Text;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBoxItem)HCSelecetion.SelectedItem).Content == null)
            {
                return;
            }
            int selectionIndex = HCSelecetion.SelectedIndex;
            var item = ((ComboBoxItem)HCSelecetion.SelectedItem).Content.ToString();
            Console.WriteLine(item);

            if (!hcConversion.ContainsKey(item))
            {
                return;
            }
            heightLable.Content= "" + hcConversion[item].Item1;
            lengthLable.Content= "" + hcConversion[item].Item2;

            height.Text = "" + hcConversion[item].Item1;
            length.Text = "" + hcConversion[item].Item2;
            HCSelecetion.SelectedIndex = selectionIndex;
        }

        private void ComboBox_watermixSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /**
             * Indices: 0-5 MEG
             *      0: 10%
             *      1: 15%
             *      2: 20%
             *      3: 25%
             *      4: 30%
             *      5: 35%
             */

            if (watermixSelecetion.SelectedIndex >= 0 && watermixSelecetion.SelectedIndex <= 5)
            {
                waterMix = new WaterMix(1, waterMixChoice[watermixSelecetion.SelectedIndex]);
            }
        }
        private void ComboBox_Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /**
             * Indices:
             *      0: Basis
             *      1: Vollversion
             *      2:EcoCond+
             */
            version = versionSelecetion.SelectedIndex;

            if (version == 0)
            {
                addIntakeButton.Visibility = Visibility.Hidden;
                addOuttakeButton.Visibility = Visibility.Hidden;
                deleteIntakeButton.Visibility = Visibility.Hidden;
                deleteOuttakeButton.Visibility = Visibility.Hidden;

                ClearHeatexchangerInput();
            }
            else
            {
                addIntakeButton.Visibility = Visibility.Visible;
                addOuttakeButton.Visibility = Visibility.Visible;
                deleteIntakeButton.Visibility = Visibility.Visible;
                deleteOuttakeButton.Visibility = Visibility.Visible;

            }
        }

        //TODO allow scientific input with e????
        private static readonly Regex regex = new Regex("[^0-9.,-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !regex.IsMatch(text);
        }
        private void PreviewTextInput_Airmass(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void PreviewTextInput_Watermix(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void PreviewTextInput_Degree(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        public void ExitInptakeOuttakeWindow()
        {
            bool isCustomInput = true;
            bool isCustomOutput = true;
            foreach (var inp in Intakes)
            {
                foreach (KeyValuePair<string, Tuple<int, int>> entry in hcConversion)
                {
                    if (entry.Value.Item1 == Convert.ToInt32(inp.inputFields[3] == "" ? "0" : inp.inputFields[3]) && entry.Value.Item2 == Convert.ToInt32(inp.inputFields[4] == "" ? "0" : inp.inputFields[4]))
                    {
                        isCustomInput = false;
                    }
                }
            }

            foreach (var oup in Outtakes)
            {
                foreach (KeyValuePair<string, Tuple<int, int>> entry in hcConversion)
                {
                    if (entry.Value.Item1 == Convert.ToInt32(oup.inputFields[3] == "" ? "0" : oup.inputFields[3]) && entry.Value.Item2 == Convert.ToInt32(oup.inputFields[4] == "" ? "0" : oup.inputFields[4]))
                    {
                        isCustomOutput = false;
                    }
                }
            }

            if (isCustomInput || isCustomOutput)
            {
                HCSelecetion.SelectedIndex = 0;
                heightLable.Content = "---";
                lengthLable.Content = "---";
            }

        }

    }




    public class IntakeOuttakeInput
    {
        public int ID { get; } =  0;
        public string Name { get; set; }

        static int nextIn = 0;
        static int nextOut = 0;

        public bool IsIntake { get; }

        public string[] inputFields = new string[Utility.inputDllSize];

        public IntakeOuttakeInput(bool isIntake)
        {
            if (isIntake)
            {
                ID = nextIn++;
            }
            else
            {
                ID = nextOut++;
            }
            IsIntake = isIntake;
        }

        public static void Reset()
        {
            nextIn = 0;
            nextOut = 0;
        }
    }

}
