using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoConf.GUI
{
    public abstract class iGUIConnector
    {
        protected MainSystem mainSystem;
        protected MainWindow mainWindow;
        public iGUIConnector(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        public void SetMainSystem(MainSystem mainSystem)
        {
            this.mainSystem = mainSystem;
        }

        abstract public void TransferInputToSystem(string currentConfFilePath);

        abstract public void StoreConfiguration(string storeFilePath, string currentConfFilePath);

        abstract public void LoadConfiguration(string filepath);

        abstract public void UseDefaultValues();

        abstract public void Computation();

        abstract public void SaveResultToPDF(string filepath);

        abstract public void ClosingApplication();

        abstract public iInput GetInputFromSystem();

        abstract public iOutput GetOutputFromSystem();

        abstract public void ErrorInInputData();

        abstract public void FinishComputation();

        abstract public void UpdateGUIInput();

    }
}
