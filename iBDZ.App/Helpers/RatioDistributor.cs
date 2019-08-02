using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBDZ.App.Helpers
{
    public class RatioDistributor
    {
		public RatioDistributor(params double[] ratios)
		{
			Ratios = new List<double>(ratios.Length);
			Ratios.AddRange(ratios);
			FixRatios();
		}

		public List<int> DistributeInt(int i)
		{
			List<int> res = new List<int>(Ratios.Count);

			double sum = Ratios.Sum();

			int remaining = i;

			foreach(var r in Ratios)
			{
				int part = Math.Min(remaining, (int) Math.Round(i / sum * r));
				res.Add(part);
				remaining -= part;
			}

			return res;
		}

		public List<double> DistributeDouble(double d)
		{
			List<double> res = new List<double>(Ratios.Count);
			double sum = Ratios.Sum();

			foreach (var r in Ratios)
			{
				double part = d / sum * r;
				res.Add(part);
			}

			return res;
		}

		private void FixRatios()
		{
			double min = Ratios.Min();
			double conversion = 1.0 / min;
			for(int i = 0; i < Ratios.Count(); i++)
			{
				Ratios[i] *= conversion;
			}
		}

		private List<double> Ratios;
    }
}
