using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel; // CancelEventArgs
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Microsoft.Win32;

namespace EcoConf.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        InputView inputView;
        OutputView outputView;

        MainSystem mainSystem;

        iGUIConnector guiConnector;
        public MainWindow()
        {
            Utility.inputConfigurationSize = (int)Application.Current.FindResource("inputConfigurationSize");
            Utility.inputDllSize = (int)Application.Current.FindResource("inputDllSize");

            Utility.outputDllSize = (int)Application.Current.FindResource("outputDllSize"); 
            Utility.outputConfigSize = (int)Application.Current.FindResource("outputConfigurationSize");


            InitializeComponent();
            guiConnector = new GUIConnector(this);
            if (! Directory.Exists(@"C:\EtaWin\masken")){
                MessageBoxResult result = MessageBox.Show("Need to add folder masken to C:\\EtaWin\\masken with a default.txt");
                System.Windows.Application.Current.Shutdown();
                return;
            }

            MainSystem.InitializeDefaults();

            inputView = new InputView(this, guiConnector);
            outputView = new OutputView(this, guiConnector);
            Main.Content = inputView;
            mainSystem = new MainSystem(this, guiConnector);
            guiConnector.SetMainSystem(mainSystem);
            guiConnector.UseDefaultValues();

        }


        #region handleViews
        //Button Input click
        private void ChangeToInput(object sender, RoutedEventArgs e)
        {
            this.Title = "Eingabe";
            Main.Content = inputView;
        }

        //Button output click
        private void ChangeToOutput(object sender, RoutedEventArgs e)
        {
            //outputView = new OutputView(this);  //Use this to reset the results
            this.Title = "Ausgabe";
            Main.Content = outputView;

        }

        // public accessable, can be used from the computation
        public void ChangeToInputView()
        {
            this.Title = "Eingabe";
            Main.Content = inputView;
        }

        // public accessable, can be used from the computation
        public void ChangeToOutputView()
        {
            if (!MainSystem.hasError)
            {
                this.Title = "Ausgabe";
                Main.Content = outputView;
                outputView.outputList = guiConnector.GetOutputFromSystem().GetOutput();
                outputView.ShowOutput();
                OutputButton.IsEnabled = true;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Keine Konfiguration gefunden!");
                OutputButton.IsEnabled = false;
            }
        }

        //used to clear the output, after the input is cleared
        public void NewOutputView()
        {
            this.Title = "Eingabe";
            outputView = new OutputView(this, guiConnector);
        }

        public InputView GetInputView()
        {
            return inputView;
        }

        #endregion


        void WindowClose(object sender, EventArgs e)
        {
            guiConnector.ClosingApplication();
        }


        /**
         * close all the windows if the main window is closed
         */
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }
    }
}