using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EcoConf.GUI;

namespace EcoConf
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            HandleExceptions();
            //this.Dispatcher.UnhandledException += Application_DispatcherUnhandledException;
        }

        [Conditional("RELEASE")]
        private void HandleExceptions()
        {
            this.Dispatcher.UnhandledException += Application_DispatcherUnhandledException;
        }

        // catch any unhandled exception
        private void Application_DispatcherUnhandledException(object sender, global::System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        /**
         * First function that is called on start of the application
         */
        private void StartApplication(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            //TODO maybe show logo on startup
            if (e.Args.Length == 1)
                MessageBox.Show("Now opening file: \n" + e.Args[0]);
            wnd.Show();
        }

    }
}
