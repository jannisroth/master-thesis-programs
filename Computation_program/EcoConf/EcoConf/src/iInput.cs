using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoConf
{
	public interface iInput
	{
        List<object[]> GetInputBlackbox();
        object[] GetInputBlackbox(int id);

        int NumberIntakesAndOuttakes { get; set; }
        void SetInput(List<object []> input);
        void SetInput(object[] input, int id);

        string[] GetInputConfiguration();
        /**
         * perform checks on the input
         */
        bool CheckInputCorrectness();

        void Copy(iInput input);
    }
}
