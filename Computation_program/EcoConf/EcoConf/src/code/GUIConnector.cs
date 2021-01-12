using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EcoConf.GUI
{
    class GUIConnector : iGUIConnector
    {
        public GUIConnector(MainWindow mainWindow) : base(mainWindow) { }

        public override void ClosingApplication()
        {
            if (mainSystem != null)
            {
                mainSystem.ClosingApplication();
            }
        }

        public override void Computation()
        {
            mainSystem.Compute();
        }

        public override void LoadConfiguration(string filepath)
        {
            mainSystem = new MainSystem(filepath, mainWindow, this);
        }

        public override void SaveResultToPDF(string filepath)
        {
            mainSystem.SavePDFDocument(filepath);
        }

        public override void StoreConfiguration(string storeFilePath, string currentConfFilePath)
        {
            TransferInputToSystem(currentConfFilePath);
            mainSystem.SaveConfiguration(storeFilePath);
        }

        public override void TransferInputToSystem(string currentConfFilePath)
        {
            mainSystem = new MainSystem(currentConfFilePath, mainWindow, this);
        }
        public override void UseDefaultValues()
        {
            mainSystem = new MainSystem(mainWindow, this);
        }

        public override iInput GetInputFromSystem()
        {
            return mainSystem.Input;
        }

        public override iOutput GetOutputFromSystem()
        {
            return mainSystem.Output;
        }

        public override void ErrorInInputData()
        {
            mainWindow.OutputButton.IsEnabled = false;
        }

        public override void FinishComputation()
        {
            mainWindow.ChangeToOutputView();
            UpdateGUIInput();
        }

        public override void UpdateGUIInput()
        {
            mainWindow.GetInputView().ShowInput();
        }
    }

}
