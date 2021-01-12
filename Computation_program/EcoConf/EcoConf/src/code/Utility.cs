using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;

namespace EcoConf
{
    public static class  Utility
    {
        public static int inputConfigurationSize = 0;
        public static int inputDllSize = 0;
        public static int outputDllSize = 0;
        public static int outputConfigSize = 0;
        public static List<int> smallestNumberOfCircuits = new List<int>();
        public static List<int> finSpacings = new List<int> { 18, 21, 25, 30, 40, 50, 60 }; //TODO send to the other program, or set somewhere as default

        public static double specificHeatWaterMix = 3.73; //TODO check different concentrations
        public static double specificHeatAir;

        public static string guiConfigFile = @"C:\EtaWin\tmp\tmpfile.txt";

        public static CultureInfo culture = new CultureInfo("en-US");

        public static double densityWaterMix = 1037.49;


        //TEST
        public static bool computeWithNomad = true;







        /**
         *  Indices for the configuration input:
         */

        public static int configPartIntakeCount = 0;
        public static int configPartOuttakeCount = 1;
        public static int configPartTemperaturetransmissionDegree = 2;
        public static int configPartAirmass = 3;
        public static int configPartWatermixMass = 4;
        public static int configPartHC = 5;
        public static int configPartVersion = 6;
        public static int configPartWatermixType = 7;
        public static int configPartWatermixPercentage = 8;

        public static int configPartisLinear = 29;

        /**
         *  Indices for the BlackBoxPart input:
         */

        public static int blackBoxPartheight = 3;
        public static int blackBoxPartlength = 4;
        public static int blackBoxPartNumberCircuits = 10;
        public static int blackBoxPartFinSpacing = 11;
        public static int blackBoxPartNumberRows = 12;
        public static int blackBoxPartWatermixType = 13;
        public static int blackBoxPartTemperatureIn = 14;
        public static int blackBoxPartWatermixConcentration = 16;
        public static int blackBoxPartAirmass = 23;
        public static int blackBoxPartPower = 26;
        public static int blackBoxPartCoilMode = 27;
        public static int blackBoxPartAirTempIn = 28;
        public static int blackBoxPartAirHumidity = 30;
        public static int blackBoxPartDemandedAirOut = 31;
        public static int blackBoxPartFinThickness = 33;
        public static int blackBoxPartWatermixMass = 37;
        public static int blackBoxPartWasSplit = 56;
        public static int blackBoxPartNextHEBelongsToSplit = 57;
        public static int blackBoxPartIsIntakeOrOuttake = 58;
        public static int blackBoxPartID = 59;
        public static int blackBoxVDISplit = 54;


        /*
         *  Indices for the BlackBoxPart input:
         */
        public static int outputBlackBoxNumberOfCoils = 141;






        //Comparing doubles
        public const double error = 0.01;
        public static bool EqualDouble(double first, double second)
        {
            if (Math.Abs(first - second) < error)
                return true;
            else
                return false;
        }


        /**
         * removes any whitespace of the string
         */
        public static void EatWhitespaces(ref string s)
        {
            s = new string(s.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        /**
         * convert pressure to meter above sea level and vice versa
         */
		internal static string ConvertPressureSealevel(string value, bool v)
		{
			/// https://en.wikipedia.org/wiki/Atmospheric_pressure
			double g = 9.80665; // m/s^2
			double M = 0.0289644; // kg/mol
			double R0 = 8.314462618; // J/(mol*K)
			double p0 = 101325; //Pa
			double L = 0.00976; // K/m
			double T0 = 288.16; // K
			double num = Convert.ToDouble(value);
			if (v)
			{
				double res = (1-Math.Pow(num/p0,R0*L/g/M))*T0/L;
				return Convert.ToString(res/100); //hPa
			}
			else
			{
				double res = p0*Math.Pow(1-L*num/T0,g*M/R0/L);
				return Convert.ToString(res); //m
			}
		}


        public static double TransmissionDegreeToTemperature(double transmissionDegree, double airTempIn = 5, double airTempOut = 25)
        {
            double result = airTempIn + (transmissionDegree < 1? transmissionDegree : transmissionDegree/100)*(airTempOut - airTempIn);
            return result;
        }

        public static void MoveItemUp<T>(this ObservableCollection<T> baseCollection, int selectedIndex)
        {
            //# Check if move is possible
            if (selectedIndex <= 0)
                return;

            //# Move-Item
            baseCollection.Move(selectedIndex - 1, selectedIndex);

        }

        public static void MoveItemDown<T>(this ObservableCollection<T> baseCollection, int selectedIndex)
        {
            //# Check if move is possible
            if (selectedIndex < 0 || selectedIndex + 1>= baseCollection.Count)
                return;

            //# Move-Item
            baseCollection.Move(selectedIndex + 1, selectedIndex);

        }

        public static void MoveItemDown<T>(this ObservableCollection<T> baseCollection, T selectedItem)
        {
            //# MoveDown based on Item
            baseCollection.MoveItemDown(baseCollection.IndexOf(selectedItem));
        }

        public static void MoveItemUp<T>(this ObservableCollection<T> baseCollection, T selectedItem)
        {
            //# MoveUp based on Item
            baseCollection.MoveItemUp(baseCollection.IndexOf(selectedItem));
        }


        public static T CutFront<T>(List<T> list)
        {
            if (list.Count <= 0) return default(T);
            T item = list.First();
            list.RemoveAt(0);
            return item;
        }

        //extension Method
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

    }

}
