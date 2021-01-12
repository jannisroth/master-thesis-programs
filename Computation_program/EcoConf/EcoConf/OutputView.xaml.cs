using Microsoft.Win32;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EcoConf.GUI
{
    /// <summary>
    /// Interaktionslogik für OutputView.xaml
    /// </summary>
    public partial class OutputView : Page
    {
        MainWindow mainWindow;
        iGUIConnector connector;

        public List<object[]> outputList = new List<object[]>();

        List<Tuple<int, string>> assignment = new List<Tuple<int, string>> 
        {

            Tuple.Create(62, "Höhe"),
            Tuple.Create(65, "Länge"),
            Tuple.Create(13, "Luftmenge Sm^3/h"),
            Tuple.Create(16, "Leistungs kW"),
            Tuple.Create(18, "EingangsTemp"), 
            Tuple.Create(21, "AusgangsTemp"),
            Tuple.Create(25, "Geschwindigkeit m/s"), 
            Tuple.Create(28, "Druckverlust"), 
            Tuple.Create(30, "Mediummenge kg/h"),
            Tuple.Create(31, "Mediummenge m^3/h"),
            Tuple.Create(33, "MediumTemp Ein"), 
            Tuple.Create(34, "MediumTemp Aus"), 
            Tuple.Create(36, "Geschwindikeit Medium"), 
            Tuple.Create(37, "Druckverlust Medium"),
            Tuple.Create(2,"Typ"),
            Tuple.Create(53, "Preis in Euro"),
            Tuple.Create(141, "Anzahl")
        };

        public OutputView(MainWindow mW, iGUIConnector guiConnector)
        {
            InitializeComponent();
            mainWindow = mW;
            connector = guiConnector;
        }

        //print the output
        public void ShowOutput()
        {
            textInput.Text = "Zuluftgeräte:\n\n";
            textOutput.Text = "Abluftgeräte:\n\n";
            foreach (var output in outputList)
            {
                string helper = "";
                if (output[0] != null && output[0].ToString() != "") helper += output[0].ToString() + " " +output[1].ToString()+"\n";
                helper += "Angaben Luft:\n";
                int count = 0;
                foreach (var item in assignment)
                {
                    helper +="    " + item.Item2+ " = " + output[item.Item1] + "\n";
                    count++;
                    if (count == 8)
                    {
                        helper += "\nAngaben Medium:\n";
                    }
                }

                if (output[output.Length - 2] != null && output[output.Length - 2].ToString() == "0")
                {
                    textInput.Text += "Input:"+ output[output.Length - 1].ToString() + (Convert.ToInt32(output[Utility.outputBlackBoxNumberOfCoils]) > 1? " (VDI Split x" + output[Utility.outputBlackBoxNumberOfCoils].ToString() + ") " : "") + "\n";
                    textInput.Text += helper;
                    textInput.Text += "\n\n";
                }
                else
                if (output[output.Length - 2] != null && output[output.Length - 2].ToString() == "1")
                {
                    textOutput.Text += "Output:"+ output[output.Length - 1].ToString() + (Convert.ToInt32(output[Utility.outputBlackBoxNumberOfCoils]) > 1 ? " (VDI Split x" + output[Utility.outputBlackBoxNumberOfCoils].ToString() + ") " : "") + "\n";
                    textOutput.Text += helper;
                    textOutput.Text += "\n\n";
                }
            }

            //foreach (var output in outputList)
            //{
            //    string helper = "";
            //    if (output[output.Length-2].ToString() == "0")
            //    {
            //        helper = "";
            //        int count = 0;
            //        foreach (var item in output)
            //            helper += "output "+count++ +" = " + item + "\n";
            //        textInput.Text += helper;
            //        textInput.Text +="\n\n";
            //    }
            //    else
            //    if (output[output.Length - 2].ToString() == "1")
            //    {
            //        helper = "";
            //        int count = 0;
            //        foreach (var item in output)
            //            helper += "output " + count++ + " = " + item + "\n";
            //        textOutput.Text += helper;
            //        textOutput.Text += "\n\n";
            //    }
            //}
        }

        private void SavePdfToLocation(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog1.DefaultExt = "pdf";
            saveFileDialog1.FileName = "Dokument";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == true)
            {
                connector.SaveResultToPDF(saveFileDialog1.FileName);
            }
        }
    }
}
