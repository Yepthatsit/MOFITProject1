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
        public double MaxNewtonIterations { get; set; } = 5;
        public double DeltaTime { get; set; } = 0.5;
        public double DeltaXInDerriveative { get; set; } = 0.001;
        public double tolerance { get; set; } = 0.001;
        public double MinDeltaTime { get; set; } = 0.0000001;
        public double MaxDeltaTime { get; set; } = 0.5;
        public double NewtonTol { get; set; } = 0.0001;
        public double SafetyParameter { get; set; } = 0.9;
        private async Task<double> CalculateNewDeltaTime(int MethodAccuracy, double epsilon)
        {
            double dtNew = SafetyParameter * DeltaTime * Math.Pow(tolerance / epsilon, 1.0 / (MethodAccuracy + 1));
            return Math.Clamp(dtNew, MinDeltaTime, MaxDeltaTime);
        }
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
        private async Task<double> F1(double xnp1, double vnp1, double xn,double vn, int devide = 2)
        {
            return xnp1  - xn - (DeltaTime/devide) * vnp1 - (DeltaTime/devide)* vn;
        }
        private async Task<double> F2(double xnp1, double vnp1, double xn, double vn,int devide = 2)
        {
            return vnp1 - vn - (DeltaTime / devide) * (await CalculateAcceleration(xnp1, vnp1) + await CalculateAcceleration(xn, vn));
        }
        public async Task CalculateTrapzSolution()
        {
            List<double> TimePoints = new() { 0 };
            List<double> XValues = new() { 2.58 };
            List<double> VelocityValues = new() { 0 };
            List<double> deltatimes = new() { DeltaTime };
            var watch = System.Diagnostics.Stopwatch.StartNew();

            while (TimePoints.LastOrDefault() < MaxTime)
            {
                double xPrev = XValues.LastOrDefault();
                double vPrev = VelocityValues.LastOrDefault();
                double epsilon = 1;
                Console.WriteLine($"Calculating time {TimePoints.LastOrDefault()}");
                while (epsilon >= tolerance)
                {
                    // Calculate trapzoid solution
                    int NewtonItersations = 1;
                    double xIterDoubledt = xPrev; // single step double dt
                    double vIterDoubleDt = vPrev; // single step double dt
                    while (NewtonItersations < MaxNewtonIterations)
                    {
                        try
                        {
                            double f1_val = await F1(xIterDoubledt, vIterDoubleDt, xPrev, vPrev, 1);
                            double f2_val = await F2(xIterDoubledt, vIterDoubleDt, xPrev, vPrev, 1);


                            double j11 = 1.0;
                            double j12 = -DeltaTime;
                            double j21 = (DeltaTime) * await CalculatePotSecDerr(xIterDoubledt);
                            double j22 = 1.0 + DeltaTime * Convert.ToDouble(alpha); // Simplified: 1.0 + Alpha * Dt / 2.0



                            double detJ = j11 * j22 - j12 * j21;



                            double delta_x = (j22 * (-f1_val) - j12 * (-f2_val)) / detJ;
                            double delta_v = (-j21 * (-f1_val) + j11 * (-f2_val)) / detJ;



                            xIterDoubledt += delta_x;
                            vIterDoubleDt += delta_v;


                            if (Math.Abs(delta_x) < NewtonTol && Math.Abs(delta_v) < NewtonTol)
                            {
                                break; // Exit Newton loop
                            }

                        }
                        catch
                        {
                            Console.WriteLine($"By some miracle this occured in CalculateTrapzSolution {NewtonItersations}");
                            break;
                        }
                    }

                    double xIter = xPrev; // double step single dt
                    double vIter = vPrev; // double step single dt
                    for (int i = 0; i < 2; i++)
                    {
                        NewtonItersations = 1;
                        while (NewtonItersations < MaxNewtonIterations)
                        {
                            try
                            {

                                double f1_val = await F1(xIter, vIter, xPrev, vPrev);
                                double f2_val = await F2(xIter, vIter, xPrev, vPrev);


                                double j11 = 1.0;
                                double j12 = -DeltaTime / 2.0;
                                double j21 = -(DeltaTime / 2.0) * (-(1.0 / 1) * await CalculatePotSecDerr(xIter));
                                double j22 = 1.0 - (DeltaTime / 2.0) * (-Convert.ToDouble(alpha)); // Simplified: 1.0 + Alpha * Dt / 2.0



                                double detJ = j11 * j22 - j12 * j21;



                                double delta_x = (j22 * (-f1_val) - j12 * (-f2_val)) / detJ;
                                double delta_v = (-j21 * (-f1_val) + j11 * (-f2_val)) / detJ;



                                xIter += delta_x;
                                vIter += delta_v;


                                if (Math.Abs(delta_x) < NewtonTol && Math.Abs(delta_v) < NewtonTol)
                                {
                                    break; // Exit Newton loop
                                }


                            }
                            catch (Exception ex)
                            {
                                {
                                    Console.WriteLine($"By some miracle this occured in CalculateTrapzSolution {NewtonItersations}  {ex.Message.ToString()}");
                                    break;
                                }
                            }
                        }
                        List<double> EpsilonTemp = new();
                        EpsilonTemp.Add(Math.Abs(xIterDoubledt - xIter));
                        EpsilonTemp.Add(Math.Abs(vIterDoubleDt - vIter));
                        epsilon = EpsilonTemp.Max();

                        DeltaTime = await CalculateNewDeltaTime(2, Convert.ToDouble(epsilon));
                        if (epsilon < tolerance)
                        {
                            XValues.Add(xIter);
                            VelocityValues.Add(vIter);
                            TimePoints.Add(TimePoints.LastOrDefault() + DeltaTime);
                            deltatimes.Add(DeltaTime);
                            break;
                        }
                    }
                }
            }
            Console.WriteLine($"Calculation completed in {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            Console.WriteLine("Creating plots please wait...");
            ScottPlot.Plot plot = new();
            plot.XLabel("Time (s)");
            plot.YLabel("Value");
            var scatter1 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), XValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter1.LegendText = "X";
            var scatter2 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter2.LegendText = "V";
            plot.SaveJpeg($"Diagrams/Trapzoid{alpha}.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(XValues.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("X");
            plot.YLabel("V");
            plot.SaveJpeg($"Diagrams/Trapzoid_Phaze_Space{alpha}.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), deltatimes.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("Czas [s]");
            plot.YLabel("Krok czasowy [s]");
            plot.SaveJpeg($"Diagrams/Trapz_Times{alpha}.jpg", 500, 500);
            watch.Stop();
            watch.Stop();
            Console.WriteLine($"Finished {watch.ElapsedMilliseconds} ms");

        }
    }
}
