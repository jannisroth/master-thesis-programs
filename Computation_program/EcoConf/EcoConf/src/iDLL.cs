using System;
using System.Collections.Generic;
using System.Text;

namespace EcoConf
{
    interface iDLL
    {
        bool BlackBoxComputing(ref iInput arguments,ref iOutput result, int idx=0);
        bool BlackBoxComputingPIPE(ref iInput arguments, ref iOutput result, int idx = 0);
    }
}
