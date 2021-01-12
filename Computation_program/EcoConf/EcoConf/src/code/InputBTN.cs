using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoConf
{
    class InputBTN : iInput
    {
        private string[] argumentsConfiguration = new string[Utility.inputConfigurationSize];
        private List<object[]> argumentsBTN = new List<object[]>();

        public int NumberIntakesAndOuttakes { get; set; }

        public object[] GetInputBlackbox(int id)
        {
            if (id < argumentsBTN.Count)
                return argumentsBTN[id];
            return null;
        }

        public List<object[]> GetInputBlackbox()
        {
            return argumentsBTN;
        }

        public string[] GetInputConfiguration()
        {
            return argumentsConfiguration;
        }

        public void SetInput(object[] input, int id)
        {

            argumentsBTN[id] = input;

        }
        public void SetInput(List<object[]> input)
        {
            argumentsBTN = input;
        }


        /**
         * perform checks on the input
         */
        public bool CheckInputCorrectness()
        {
            //TODO
            return true;
        }

        public void Copy(iInput input)
        {
            Array.Copy(input.GetInputConfiguration(), 0, argumentsConfiguration, 0, input.GetInputConfiguration().Length);
            foreach (var item in input.GetInputBlackbox())
            {
                object[] tmp = new object[item.Length];
                Array.Copy(item,0,tmp,0,item.Length);
                argumentsBTN.Add(tmp);
            }
            NumberIntakesAndOuttakes = input.NumberIntakesAndOuttakes;
        }
    }
}
