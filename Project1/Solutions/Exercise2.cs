using ScottPlot;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Solutions
{
    public class Exercise2
    {
        public double SafetyParameter { get; set; } = 0.9;
        public double MaxTime { get; set; } = 100;
        public double DeltaTime { get; set; } = 0.1;
        public double DeltaXInDerriveative { get; set; } = 0.001;
        public double? alpha { get; set; } = null;
        public double tolerance { get; set; } = 0.0001;
        //public List<double> TimePoints { get; set; } = new() { 0 };
        //public List<double> XValues { get; set; } = new() { 2.58 };
        //public List<double> VelocityValues { get; set; } = new() { 0 };
        public HashSet<double> Timesteps { get; set; } = new();
        public Exercise2()
        {
            try
            {
                if (!Path.Exists("./Diagrams"))
                    Directory.CreateDirectory("./Diagrams");
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "Diagrams");
                Console.WriteLine($"Plots will be saved in: \n{dir}");
                //TimePoints = Enumerable.Range(0, Convert.ToInt32(100.00 / DeltaTime)).Select(x => (double)x * DeltaTime).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"By some miracle this occured in Exercise1 constructor {ex.Message.ToString()}");
            }
        }
        private async Task<double> CalculatePotentialValue(double x)
        {
            return -Math.Pow(Math.E, -x * x) - 8 * Math.Pow(Math.E, -8 * (Math.Pow(x - 2, 2) * 8));
        }
        private async Task<double> CalculateAcceleration(double x,double lastVelocity)
        {
            if (alpha == null)
            {
                return -(await CalculatePotentialValue(x + DeltaXInDerriveative) - await CalculatePotentialValue(x - DeltaXInDerriveative)) / (2 * DeltaXInDerriveative);
            }
            try
            {
                return -(await CalculatePotentialValue(x + DeltaXInDerriveative) - await CalculatePotentialValue(x - DeltaXInDerriveative)) / (2 * DeltaXInDerriveative) -Convert.ToDouble(alpha) * lastVelocity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"By some miracle this occured in CalculateAcceleration {ex.Message.ToString()}");
                return 0;
            }
        }
        private async Task<double> CalculateNewDeltaTime(int MethodAccuracy,double epsilon)
        {
            return SafetyParameter *DeltaTime* Math.Pow(tolerance /  epsilon, 1.0 / (MethodAccuracy + 1));
        }
        public async Task CalculateExplicitEulerSolutionWVT()
        {
            List<double> TimePoints  = new() { 0 };
            List<double> XValues = new() { 2.58 };
            List<double> VelocityValues = new() { 0 };
            //Timesteps = new() { DeltaTime };
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (TimePoints.LastOrDefault() < MaxTime)
            {
                double? epsilon = null;
                while (epsilon== null || epsilon >tolerance ) {
                    var DoubleDtVelocity = VelocityValues.LastOrDefault() + 2 * DeltaTime * await CalculateAcceleration(XValues.LastOrDefault(),VelocityValues.LastOrDefault());
                    var DoubleDtX = XValues.LastOrDefault() + 2 * DeltaTime * VelocityValues.LastOrDefault();
                    List<double> EpsilonTemp = new();
                    var Velocity = VelocityValues.LastOrDefault() + DeltaTime * await CalculateAcceleration(XValues.LastOrDefault(),VelocityValues.LastOrDefault());
                    var x = XValues.LastOrDefault() + DeltaTime * VelocityValues.LastOrDefault();
                    Velocity = Velocity + DeltaTime * await CalculateAcceleration(x,Velocity);
                    x = x + DeltaTime * Velocity;
                    EpsilonTemp.Add(Math.Abs(DoubleDtVelocity - Velocity));
                    EpsilonTemp.Add(Math.Abs(DoubleDtX - x));
                    epsilon = EpsilonTemp.Max();
                    try
                    {
                        DeltaTime = await CalculateNewDeltaTime(1, Convert.ToDouble(epsilon));
                        //Timesteps.Add(DeltaTime);
                        if(epsilon <= tolerance)
                        {
                            VelocityValues.Add(Velocity);
                            XValues.Add(x);
                        } 
                    }
                    catch
                    {
                       
                    }
                }
                TimePoints.Add(TimePoints.LastOrDefault() + DeltaTime);
                
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
            plot.SaveJpeg($"Diagrams/explicit_eulerWT{alpha}.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(XValues.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("X");
            plot.YLabel("V");
            plot.SaveJpeg($"Diagrams/explicit_euler_phase_spaceWT{alpha}.jpg", 500, 500);
            watch.Stop();
            Console.WriteLine($"Finished {watch.ElapsedMilliseconds} ms");
        }
        public async Task CalculateVerletSolutionWVT()
        {
            List<double> TimePoints  = new() { 0 };
            DeltaTime = 0.1;
            List<double> XValues = new() { 2.58 };
            List<double> VelocityValues = new() { 0 };
            Timesteps = new() ;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            while (TimePoints.LastOrDefault() < MaxTime)
            {
                double? epsilon = null;
                while (epsilon == null || epsilon > tolerance)
                {
                    var DoubleDtX = XValues.LastOrDefault() + 2* DeltaTime * VelocityValues.LastOrDefault() + 2 * DeltaTime * DeltaTime * await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault());
                    var DoubleDtVelocity = VelocityValues.LastOrDefault() + 0.5*DeltaTime * (await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault()) + await CalculateAcceleration(DoubleDtX, VelocityValues.LastOrDefault()));
                    List<double> EpsilonTemp = new();
                    var x = XValues.LastOrDefault() + DeltaTime * VelocityValues.LastOrDefault() + 0.5*DeltaTime * DeltaTime * await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault());
                    var Velocity = VelocityValues.LastOrDefault() + 0.5 * DeltaTime * (await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault()) + await CalculateAcceleration(x,VelocityValues.LastOrDefault()));
                    x = x + Velocity*DeltaTime + 0.5 * DeltaTime*DeltaTime * (await CalculateAcceleration(x, Velocity) + await CalculateAcceleration(x, Velocity));
                    Velocity = Velocity + 0.5 * DeltaTime * (await CalculateAcceleration(x, Velocity) + await CalculateAcceleration(x, Velocity));
                    EpsilonTemp.Add(Math.Abs(DoubleDtVelocity - Velocity));
                    EpsilonTemp.Add(Math.Abs(DoubleDtX - x));
                    epsilon = EpsilonTemp.Max();
                    try
                    {
                        DeltaTime = await CalculateNewDeltaTime(1, Convert.ToDouble(epsilon));
                        if (epsilon <= tolerance)
                        {
                            VelocityValues.Add(Velocity);
                            XValues.Add(x);
                        }
                    }
                    catch
                    {

                    }
                }
                TimePoints.Add(TimePoints.LastOrDefault() + DeltaTime);
                //VelocityValues.Add(VelocityValues.LastOrDefault() + 0.5 * DeltaTime * (await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault()) + await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault())));
                //XValues.Add(XValues.LastOrDefault() + VelocityValues.LastOrDefault() * DeltaTime + 0.5 * DeltaTime * DeltaTime * await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault()));

            }
            
            Console.WriteLine($"Calculation completed in {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            Console.WriteLine("Creating plots please wait...");

            Plot plot = new();
            plot.XLabel("Time (s)");
            plot.YLabel("Value");
            var scatter1 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), XValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter1.LegendText = "X";
            var scatter2 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter2.LegendText = "V";
            plot.SaveJpeg($"Diagrams/verletWVT{alpha}.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(XValues.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("X");
            plot.YLabel("V");
            plot.SaveJpeg($"Diagrams/verlet_phase_spaceWVT{alpha}.jpg", 500, 500);
            watch.Stop();
            Console.WriteLine($"Finished in {watch.ElapsedMilliseconds} ms");
        }
        public async Task CalculateRK4SolutionWVT()
        {
            List<double> TimePoints = new() { 0 };
            DeltaTime = 0.5;
            List<double> XValues = new() { 2.58 };
            List<double> VelocityValues = new() { 0 };
            Timesteps = new();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            while (TimePoints.LastOrDefault() < MaxTime)
            {
                double? epsilon = null;
                while (epsilon == null || epsilon > tolerance)
                {
                    List<double> DoubledtXkv = new();
                    List<double> DoubledtVkv = new();

                    // 4 steps of RK4 for double time step -- źle liczy 4 k
                    DoubledtXkv.Add(VelocityValues.LastOrDefault());
                    DoubledtVkv.Add(await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault()));
                    for (int i = 1; i < 4; i++)
                    {
                        var consant = (i==3) ? 2: 1;
                        //var xh = VelocityValues.LastOrDefault() +  DeltaTime * DoubledtXkv[i - 1]*consant;
                        //var Velocityh = await CalculateAcceleration(XValues.LastOrDefault() + DeltaTime * DoubledtXkv[i - 1]*consant, VelocityValues.LastOrDefault()); 
                        DoubledtXkv.Add(VelocityValues.LastOrDefault() + DeltaTime*consant* DoubledtVkv[i-1]);
                        DoubledtVkv.Add(await CalculateAcceleration(XValues.LastOrDefault() + DeltaTime * DoubledtXkv[i-1] , VelocityValues.LastOrDefault()));
                    }

                    var DoubleDtX = XValues.LastOrDefault() + 2 * DeltaTime  * (DoubledtXkv[0] + 2 * DoubledtXkv[1] + 2 * DoubledtXkv[2] + DoubledtXkv[3])/6;
                    var DoubleDtVelocity = VelocityValues.LastOrDefault() + 2 * DeltaTime * (DoubledtVkv[0] + 2 * DoubledtVkv[1] + 2 * DoubledtVkv[2] + DoubledtVkv[3]) / 6;

                    List<double> Xkv = new();
                    List<double> Vkv = new();
                    // 4 steps of RK4 for normal time step
                    Xkv.Add(VelocityValues.LastOrDefault());
                    Vkv.Add(await CalculateAcceleration(XValues.LastOrDefault(), VelocityValues.LastOrDefault()));
                    for (int i = 1; i < 4; i++)
                    {
                        var consant = (i == 3) ? 1 : 0.5;
                        Xkv.Add(VelocityValues.LastOrDefault() + DeltaTime * consant * DoubledtVkv[i - 1]);
                        Vkv.Add(await CalculateAcceleration(XValues.LastOrDefault() + DeltaTime * DoubledtXkv[i - 1], VelocityValues.LastOrDefault()));
                    }
                    var x = XValues.LastOrDefault() + DeltaTime * (Xkv[0] + 2 * Xkv[1] + 2 * Xkv[2] + Xkv[3]) / 6;
                    var Velocity = VelocityValues.LastOrDefault() + DeltaTime * (Vkv[0] + 2 * Vkv[1] + 2 * Vkv[2] + Vkv[3]) / 6;

                    // 4 steps of RK4 for normal time step second time
                    Xkv = new();
                    Vkv = new();
                    Xkv.Add(Velocity);
                    Vkv.Add(await CalculateAcceleration(x, Velocity));
                    for (int i = 1; i < 4; i++)
                    {
                        var consant = (i == 3) ? 1 : 0.5;
                        Xkv.Add(Velocity + DeltaTime * consant * DoubledtVkv[i - 1]);
                        Vkv.Add(await CalculateAcceleration(x + DeltaTime * DoubledtXkv[i - 1], VelocityValues.LastOrDefault()));
                    }
                    x = x + DeltaTime * (Xkv[0] + 2 * Xkv[1] + 2 * Xkv[2] + Xkv[3]) / 6;
                    Velocity = Velocity + DeltaTime * (Vkv[0] + 2 * Vkv[1] + 2 * Vkv[2] + Vkv[3]) / 6;

                    // Calculate epsilon
                    List<double> EpsilonTemp = new();
                    EpsilonTemp.Add(Math.Abs(DoubleDtVelocity - Velocity));
                    EpsilonTemp.Add(Math.Abs(DoubleDtX - x));
                    epsilon = EpsilonTemp.Max();
                    try
                    {
                        DeltaTime = await CalculateNewDeltaTime(1, Convert.ToDouble(epsilon));
                        if (epsilon <= tolerance)
                        {
                            VelocityValues.Add(Velocity);
                            XValues.Add(x);
                        }
                    }
                    catch
                    {
                    }

                }
                TimePoints.Add(TimePoints.LastOrDefault() + DeltaTime);
            }
            Plot plot = new();
            plot.XLabel("Time (s)");
            plot.YLabel("Value");
            var scatter1 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), XValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter1.LegendText = "X";
            var scatter2 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter2.LegendText = "V";
            plot.SaveJpeg($"Diagrams/RK4WVT{alpha}.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(XValues.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("X");
            plot.YLabel("V");
            plot.SaveJpeg($"Diagrams/RK4_phase_spaceWVT{alpha}.jpg", 500, 500);
            watch.Stop();
            Console.WriteLine($"Finished in {watch.ElapsedMilliseconds} ms");
            Console.WriteLine(DeltaTime);
        }
    }
}
