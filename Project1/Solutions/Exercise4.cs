using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Solutions
{
    public class Exercise4
    {
        public double? alpha { get; set; } = 0.5;
        public double MaxTime { get; set; } = 100;
        public double DeltaXInDerriveative { get; set; } = 0.001;

        private async Task<double> CalculatePotentialValue(double x)
        {
            return -Math.Pow(Math.E, -x * x) - 8 * Math.Pow(Math.E, -8 * (Math.Pow(x - 2, 2) * 8));
        }
        private async Task<double> CalculateAcceleration(double x, double lastVelocity)
        {
            if (alpha == null)
            {
                return -(await CalculatePotentialValue(x + DeltaXInDerriveative) - await CalculatePotentialValue(x - DeltaXInDerriveative)) / (2 * DeltaXInDerriveative);
            }
            try
            {
                return -(await CalculatePotentialValue(x + DeltaXInDerriveative) - await CalculatePotentialValue(x - DeltaXInDerriveative)) / (2 * DeltaXInDerriveative) - Convert.ToDouble(alpha) * lastVelocity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"By some miracle this occured in CalculateAcceleration {ex.Message.ToString()}");
                return 0;
            }
        }
        private async Task<double> CalculatePotSecDerr(double xn)
        {
            return (await CalculatePotentialValue(xn + DeltaXInDerriveative) - 2 * await CalculatePotentialValue(xn) + await CalculatePotentialValue(xn - DeltaXInDerriveative)) / (DeltaXInDerriveative * DeltaXInDerriveative);
        }

        public async Task CalculateTrapzSolution()
        {

        }
    }
}
