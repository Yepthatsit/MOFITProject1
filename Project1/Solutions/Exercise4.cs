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
        public double MaxNewtonIterations { get; set; } = 1000;
        public double DeltaTime { get; set; } = 0.1;
        public double DeltaXInDerriveative { get; set; } = 0.001;
        public double tolerance { get; set; } = 0.0001;
        public double DeltaXNewton { get; set; } = 0.01;
        public double DeltaVNewton { get; set; } = 0.01;
        public double SafetyParameter { get; set; } = 0.9;
        private async Task<double> CalculateNewDeltaTime(int MethodAccuracy, double epsilon)
        {
            return SafetyParameter * DeltaTime * Math.Pow(tolerance / epsilon, 1.0 / (MethodAccuracy + 1));
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
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (TimePoints.LastOrDefault() < MaxTime)
            {
                double epsilon = 1;
                do
                {
                    // Calculate trapzoid solution
                    int NewtonItersations = 1;
                    double xnp1muDoubledt = XValues.LastOrDefault(); // single step double dt
                    double vnp1muDoubleDt = VelocityValues.LastOrDefault(); // single step double dt
                    while (NewtonItersations < MaxNewtonIterations)
                    {
                        try
                        {
                            double xnp1mup1 = xnp1muDoubledt + Math.Pow(DeltaXNewton, NewtonItersations);
                            double vnp1mup1 = vnp1muDoubleDt + Math.Pow(DeltaVNewton, NewtonItersations);

                            double L1 = Math.Pow(DeltaXNewton, NewtonItersations) - (DeltaTime) * Math.Pow(DeltaVNewton, NewtonItersations);
                            double L2 = (DeltaTime) * await CalculatePotSecDerr(xnp1muDoubledt) * Math.Pow(DeltaXNewton, NewtonItersations)
                                        + (1 + Convert.ToDouble(alpha) * DeltaTime) * Math.Pow(DeltaVNewton, NewtonItersations);
                            var F1Val = -await F1(xnp1muDoubledt, vnp1muDoubleDt, XValues.LastOrDefault(), VelocityValues.LastOrDefault(),1);
                            var F2Val = -await F2(xnp1muDoubledt, vnp1muDoubleDt, XValues.LastOrDefault(), VelocityValues.LastOrDefault(),1);
                            if (L1 == F1Val 
                                && L2 == F2Val)
                            {
                                
                                break;
                            }
                            xnp1muDoubledt = xnp1mup1;
                            vnp1muDoubleDt = vnp1mup1;

                            NewtonItersations++;
                            if (NewtonItersations >= MaxNewtonIterations)
                            {
                                Console.WriteLine($"Max iterations reached: {NewtonItersations}");
                                break;
                            }
                        }
                        catch
                        {
                            Console.WriteLine($"By some miracle this occured in CalculateTrapzSolution {NewtonItersations}");
                            break;
                        }
                    }

                    double xnp1mu = XValues.LastOrDefault(); // double step single dt
                    double vnp1mu = VelocityValues.LastOrDefault(); // double step single dt
                    for (int i = 0; i < 2; i++)
                    {
                        NewtonItersations = 1;
                        while (NewtonItersations < MaxNewtonIterations)
                        {
                            try
                            {
                                double xnp1mup1 = xnp1mu + Math.Pow(DeltaXNewton, NewtonItersations);
                                double vnp1mup1 = vnp1mu + Math.Pow(DeltaVNewton, NewtonItersations);

                                double L1 = Math.Pow(DeltaXNewton, NewtonItersations) - (DeltaTime / 2) * Math.Pow(DeltaVNewton, NewtonItersations);
                                double L2 = (DeltaTime / 2) * await CalculatePotSecDerr(xnp1mu) * Math.Pow(DeltaXNewton, NewtonItersations)
                                            + (1 + Convert.ToDouble(alpha) * DeltaTime / 2)*Math.Pow(DeltaVNewton,NewtonItersations);
                                if(L1 == - await F1(xnp1mu,vnp1mu,XValues.LastOrDefault(),VelocityValues.LastOrDefault()) 
                                    && L2 == - await F2(xnp1mu, vnp1mu, XValues.LastOrDefault(), VelocityValues.LastOrDefault()))
                                {
                                    
                                    break;
                                }
                                xnp1mu = xnp1mup1;
                                vnp1mu = vnp1mup1;

                                NewtonItersations++;
                                if (NewtonItersations >= MaxNewtonIterations)
                                {
                                    Console.WriteLine($"Max iterations reached: {NewtonItersations}");
                                    break;
                                }
                            }
                            catch
                            {
                                Console.WriteLine($"By some miracle this occured in CalculateTrapzSolution {NewtonItersations}");
                                break;
                            }
                        }
                    }
                    List<double> EpsilonTemp = new();
                    EpsilonTemp.Add(Math.Abs(vnp1muDoubleDt - vnp1mu));
                    EpsilonTemp.Add(Math.Abs(xnp1muDoubledt - xnp1mu));
                    epsilon = EpsilonTemp.Max();

                    DeltaTime = await CalculateNewDeltaTime(1, Convert.ToDouble(epsilon));
                    if (epsilon < tolerance)
                    {
                        XValues.Add(xnp1mu);
                        VelocityValues.Add(vnp1mu);
                        TimePoints.Add(TimePoints.LastOrDefault() + DeltaTime);
                        break;
                    }
                } while (epsilon >= tolerance);
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
            watch.Stop();
            Console.WriteLine($"Finished {watch.ElapsedMilliseconds} ms");
        }
    }
}
