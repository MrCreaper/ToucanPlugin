using System.Collections.Generic;

namespace ToucanPlugin.API
{
    public static class Extensions
    {
        public static double Max(this List<double> input)
        {
            double Max = 0;
            input.ForEach(x => {
                if (x > Max)
                    Max = x;
            });
            return Max;
        }
        public static double Min(this List<double> input)
        {
            double Min = 0;
            input.ForEach(x => {
                if (x > Min)
                    Min = x;
            });
            return Min;
        }
        public static double Avg(this List<double> input)
        {
            double Total = 0;
            input.ForEach(x => {
                Total += x;
            });
            return Total / input.Count;
        }
    }
    public class Features
    {
        public void CassieProtocol() => CassieProtocol();
    }
}