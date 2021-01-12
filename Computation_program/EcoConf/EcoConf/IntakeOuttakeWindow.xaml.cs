using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EcoConf.GUI
{
    /// <summary>
    /// Interaktionslogik für IntakeOuttakeWindow.xaml
    /// </summary>
    public partial class IntakeOuttakeWindow : Window
    {
        MainWindow mainWindow;
        iGUIConnector connector;
        IntakeOuttakeInput input;
        InputView inputView;

        Dictionary<string, string> finThicknessTOIndex = new Dictionary<string, string>()
        {
            {"0.2","1"},
            {"0.15","2"},
            {"0.4","3"},
            {"0,2","1"},
            {"0,15","2"},
            {"0,4","3"},
            {"0.20","1"},
            {"0.40","3"},
            {"0,20","1"},
            {"0,40","3"}
        };
        Dictionary<string, string> finThicknessFromIndex = new Dictionary<string, string>()
        {
            {"1","0.2"},
            {"2","0.15"},
            {"3","0.4"}
        };

        private Dictionary<string, int> assignmentIDX = new Dictionary<string, int>() 
        { 
            {"length", 0},
            {"height", 1},
            {"numberRows", 2},
            {"numberCircuits", 3},
            {"finSpacing", 4},
            {"finThickness", 5},
            {"airMass", 6},
            {"airTempIn", 7},
            {"airHumidity", 8},
            {"watermixTempIn", 9},
            {"watermixMass", 10},
            {"power",11}
        };

        List<Tuple<int, string>> assignment = new List<Tuple<int, string>>
        {
            Tuple.Create(3,"Höhe"),
            Tuple.Create(4,"Länge"),
            Tuple.Create(12,"Rohreihen"),
            Tuple.Create(10,"Anzahl Kreisl."),
            Tuple.Create(11,"Lamellenabstand"),
            Tuple.Create(33,"Lemellenstärke"),
            Tuple.Create(23,"Luftmenge"),
            Tuple.Create(28,"Lufttemp Ein"),
            Tuple.Create(30,"Luftfeuchte"),
            Tuple.Create(14,"Wassertemp Ein"),
            Tuple.Create(37,"Massenstrom"),
            Tuple.Create(26,"Leistung")
        };

        public ObservableCollection<ItemString> Items { get; set; } = new ObservableCollection<ItemString>();
        public string NameWindow { get; set; }
        public IntakeOuttakeWindow(MainWindow mW, iGUIConnector guiConnector, IntakeOuttakeInput input, InputView iv)
        {
            InitializeComponent();
            NameWindow = string.Join("__",input.Name.Split(new char[] { '_' }))+':';
            mainWindow = mW;
            inputView = iv;
            DataContext = this;
            connector = guiConnector;
            this.input = input;

            int counter = 0;
            foreach (var item in assignment)
            {
                if (counter == assignmentIDX["finThickness"])
                {
                    Items.Add(new ItemString(finThicknessFromIndex[input.inputFields[item.Item1]], item.Item2 + " = "));
                }
                else
                if (counter == assignmentIDX["finSpacing"])
                {
                    Items.Add(new ItemString(input.inputFields[item.Item1][0] + (input.inputFields[item.Item1].Length > 1?"." + input.inputFields[item.Item1].Substring(1) : ".0"), item.Item2 + " = "));
                }
                else
                if (counter == assignmentIDX["watermixTempIn"] && Convert.ToDouble(input.inputFields[item.Item1].Replace(',','.')) >= 1000)
                {
                    Items.Add(new ItemString("", item.Item2 + " = "));
                }
                else
                {
                    Items.Add(new ItemString(input.inputFields[item.Item1],item.Item2+" = "));
                }
                counter++;
            }
        }

        void WindowClose(object sender, EventArgs e)
        {
            UpdateIntakeOuttakeFields();
            if (input.IsIntake)
            {
                InputView.currentOpenIntake.Remove(input.ID);
            }
            else
            {
                InputView.currentOpenOuttake.Remove(input.ID);
            }
            inputView.ExitInptakeOuttakeWindow();
        }

        public void UpdateIntakeOuttakeFields()
        {

            for (int i = 0; i < assignment.Count; i++)
            {
                if (i == assignmentIDX["finThickness"])
                {
                    input.inputFields[assignment[i].Item1] = finThicknessTOIndex[Items[i].Value];
                }
                else
                if (i == assignmentIDX["finSpacing"])
                {
                    input.inputFields[assignment[i].Item1] = Items[i].Value.Remove(1,1);
                }
                else
                if (i == assignmentIDX["watermixTempIn"] && Items[i].Value == "" )
                {
                    input.inputFields[assignment[i].Item1] = ""+10000;
                }
                else
                {
                    input.inputFields[assignment[i].Item1] = Items[i].Value;
                }
            }

        }

    }

    public class ItemString
    {
        public ItemString(string val, string name = "")
        {
            Value = val;
            Name = name;
        }
        public string Value { get; set; }
        public string Name { get; set; }

    }
}
