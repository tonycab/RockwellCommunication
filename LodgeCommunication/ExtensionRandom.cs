using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exemple
{
	public static  class ExtensionRandom
	{

		
		public static double RandonRangeDouble(this Random Value,double min, double max)
		{
			return Value.NextDouble() * (max - min) + min;
		}

		public static int RandonRangeInt(this Random Value, int min, int max)
		{
			return (int)(Value.NextDouble() * (max - min) + min);
		}



	}
}
