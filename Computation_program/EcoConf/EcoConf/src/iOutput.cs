using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoConf
{
	public interface iOutput
	{
        List<object[]> GetOutput();
        object[] GetOutput(int id);
        void SetOutput(List<object[]> output);
        void SetOutput(object[] output, int id);
    }
}
