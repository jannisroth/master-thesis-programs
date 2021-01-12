using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoConf
{
	class OutputBTN : iOutput
	{
        List<object[]> result = new List<object[]>();

        //new object[(int) Application.Current.FindResource("outputDllSize")]
        public List<object[]> GetOutput()
        {
            return result;
        }
        public object[] GetOutput(int id)
        {
            return result[id];
        }

        public void SetOutput(List<object[]> output)
        {
            result = output;
        }
        public void SetOutput(object[] output, int id)
        {
            result[id] = output;
        }
    }
}
