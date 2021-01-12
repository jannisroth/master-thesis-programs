using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using lpsolve55;
using PuppeteerSharp.PageCoverage;
using System.Windows.Markup;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace EcoConf
{

    class LinearOptimization
    {

        static int release = 0, Major = 0, Minor = 0, build = 0;
        public static MainSystem ms;

        //Name -- height,length
        static public Dictionary<string, Tuple<int, int>> HCassignment = new Dictionary<string, Tuple<int, int>>()
        {
            { "HC4", Tuple.Create(642,606)},
            { "HC6", Tuple.Create(642,912)},
            { "HC9", Tuple.Create(948,912)},
            { "HC12", Tuple.Create(948,1218)},
            { "HC16", Tuple.Create(1254,1218)},
            { "HC20", Tuple.Create(1254,1524)},
            { "HC25", Tuple.Create(1560,1524)},
            { "HC30", Tuple.Create(1560,1830)},
            { "HC36", Tuple.Create(1866,1830)},
            { "HC42", Tuple.Create(1866,2136)},
            { "HC49", Tuple.Create(2172,2136)},
            { "HC56", Tuple.Create(2172,2442)},
            { "HC64", Tuple.Create(2478,2442)},
            { "HC72", Tuple.Create(2478,2748)},
            { "HC80", Tuple.Create(2750,2748)},
            { "HC90", Tuple.Create(2750,3054)},
            { "HC100", Tuple.Create(2750,3360)},
            { "HC110", Tuple.Create(2750,3666)}
        };

        [STAThread]
        public static Tuple<double,CurrentConfig> ComputeSingleLP(string hcpath)
        {
            //Console.WriteLine(Environment.CurrentDirectory);
            //Console.WriteLine(hcpath);
            lpsolve.Init(".");

            lpsolve.lp_solve_version(ref Major, ref Minor, ref release, ref build);

            //System.Diagnostics.Debug.WriteLine(System.Environment.CurrentDirectory);

            IntPtr lp;

            /* Read LP model */
            lp = lpsolve.read_LP(hcpath, 0, ""+hcpath);
            if (lp == (IntPtr)0)
            {
                Console.WriteLine("Can't find LP, stopping");
                return null;
            }

            //add height intake
            lpsolve.set_lowbo(lp, 9, Convert.ToDouble(ms.Input.GetInputBlackbox()[0][3]));
            lpsolve.set_upbo(lp, 9, Convert.ToDouble(ms.Input.GetInputBlackbox()[0][3]));

            //add length intake
            lpsolve.set_lowbo(lp, 10, Convert.ToDouble(ms.Input.GetInputBlackbox()[0][4]));
            lpsolve.set_upbo(lp, 10, Convert.ToDouble(ms.Input.GetInputBlackbox()[0][4]));
            //add height outtake

            lpsolve.set_lowbo(lp, 20, Convert.ToDouble(ms.Input.GetInputBlackbox()[1][3]));
            lpsolve.set_upbo(lp, 20, Convert.ToDouble(ms.Input.GetInputBlackbox()[1][3]));

            //add length outtake
            lpsolve.set_lowbo(lp, 21, Convert.ToDouble(ms.Input.GetInputBlackbox()[1][4]));
            lpsolve.set_upbo(lp, 21, Convert.ToDouble(ms.Input.GetInputBlackbox()[1][4]));

            //add airmass intake
            lpsolve.set_lowbo(lp, 16, Convert.ToDouble(ms.Input.GetInputConfiguration()[3]));
            lpsolve.set_upbo(lp, 16, Convert.ToDouble(ms.Input.GetInputConfiguration()[3]));


            //add airmass outtake
            lpsolve.set_lowbo(lp, 27, Convert.ToDouble(ms.Input.GetInputConfiguration()[3]));
            lpsolve.set_upbo(lp, 27, Convert.ToDouble(ms.Input.GetInputConfiguration()[3]));

            //add bound to temperaturetransmissionrate
            //lpsolve.set_lowbo(lp, 18, Convert.ToDouble(ms.Input.GetInputConfiguration()[2]) * (25.0 - 5.0) + 5.0);
            //lpsolve.set_lowbo(lp, 18, 15.0);
            //lpsolve.set_upbo(lp, 29, 15.0);

            //lpsolve.set_lowbo(lp, 19, 10.0);
            //lpsolve.set_upbo(lp, 30, 14.0);


            //lpsolve.print_lp(lp);

            lpsolve.solve(lp); ;

            double[] variables = new double[51];
            lpsolve.get_variables(lp, variables);
            double objectiveValue = lpsolve.get_objective(lp);

            //Console.WriteLine(variables[18] + " " + variables[29] + " : "+objectiveValue);

            lpsolve.delete_lp(lp);

            double watermass = Convert.ToDouble(Convert.ToDouble(variables[26])) * 1.2 / (Utility.specificHeatWaterMix * Utility.densityWaterMix) * 1000;
            double watermassAdjusted = Math.Round(watermass * 1.05 / 100.0, 0) * 100.0;
            CurrentConfig newConfig = new CurrentConfig(0,0,1,25,true); // defaults
            newConfig.intakeConfigs.Add(new CurrentConfig.IntakeConfig
                (
                    Convert.ToInt32(variables[9]),  //length
                    Convert.ToInt32(variables[8]),  //height
                    Convert.ToInt32(variables[10]),  //number of rows
                    Convert.ToInt32(variables[11]),  //number of circuits
                    Convert.ToInt32(variables[12]), //fin spacing
                    Convert.ToInt32(variables[13]), //fin thickness
                    5,                              //airTemp in
                    80,                             //air humidity
                    Convert.ToInt32(variables[15]), //airmass
                    Convert.ToInt32(watermassAdjusted), //watermixMass
                    10000                           //airTemp out
                )
            );


            newConfig.outtakeConfigs.Add(new CurrentConfig.OuttakeConfig
                (
                    Convert.ToInt32(variables[20]),  //length
                    Convert.ToInt32(variables[19]),  //height
                    Convert.ToInt32(variables[21]),  //number of rows
                    Convert.ToInt32(variables[22]),  //number of circuits
                    Convert.ToInt32(variables[23]), //fin spacing
                    Convert.ToInt32(variables[24]), //fin thickness
                    25,                              //airTemp in
                    20,                             //air humidity
                    Convert.ToInt32(variables[26]), //airmass
                    Convert.ToInt32(watermassAdjusted) //watermixMass
                )
            );

            return Tuple.Create(objectiveValue,newConfig);
        }

        private static int Compare(Tuple<double,CurrentConfig> conf1, Tuple<double, CurrentConfig> conf2)
        {
            if (conf1.Item1 < conf2.Item1)
                return -1;
            if (conf1.Item1 == conf2.Item1)
                return 0;
            if (conf1.Item1 > conf2.Item1)
                return 1;

            throw new Exception();
        }

        public static List<CurrentConfig> Optimize(MainSystem ms,string hc)
        {
            CurrentConfig result = null;
            string hcpath = hc;

            LinearOptimization.ms = ms;

            List<Tuple<double, CurrentConfig>> resultList = new List<Tuple<double, CurrentConfig>>();

            //DirectoryInfo d = new DirectoryInfo(".\\"+ hcpath + "\\");
            DirectoryInfo d = new DirectoryInfo("C:\\EtaWin\\" + hcpath + "\\");
            foreach (var file in d.GetFiles("*.lp"))
                resultList.Add(ComputeSingleLP(file.FullName));

            resultList.Sort(Compare);

            //double minValue = Double.MaxValue;
            //CurrentConfig minConfig = null;
            //foreach (var item in resultList)
            //{
            //    if (item == null) continue;
            //    if(item.Item1 < minValue)
            //    {
            //        minValue = item.Item1;
            //        minConfig = item.Item2;
            //    }
            //}

            List<CurrentConfig> feasibleResults = new List<CurrentConfig>();
            foreach(var item in resultList)
            {
                //Console.WriteLine(item.Item1);
                if (item.Item1 < 1000000000)
                {
                    feasibleResults.Add(item.Item2);
                }
            }

            return feasibleResults;
        }
    }
}
