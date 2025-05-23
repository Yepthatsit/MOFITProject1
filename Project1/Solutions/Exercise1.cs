﻿using ScottPlot;
using ShellProgressBar;
using System.Security.Cryptography;
namespace Project1.Solutions
{
    class Exercise1
    {
        public double MaxTime { get; set; } = 100;
        public double DeltaTime { get; set; } = 0.00001;
        public double DeltaXInDerriveative { get; set; } = 0.001;
        public List<double> TimePoints { get; set; } = new();
        public List<double> XValues { get; set; } = new() {2.58};
        public List<double> VelocityValues { get; set; } = new() { 0 };
        public Exercise1(double DeltaTime = 0.00001)
        {
            try
            {
                this.DeltaTime = DeltaTime;
                if (!Path.Exists("./Diagrams"))
                    Directory.CreateDirectory("./Diagrams");
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "Diagrams");
                Console.WriteLine($"Plots will be saved in: \n{dir}");
                TimePoints = Enumerable.Range(0, Convert.ToInt32(100.00/DeltaTime)).Select(x => (double)x *DeltaTime).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"By some miracle this occured in Exercise1 constructor {ex.Message.ToString()}");
            }
        }
        private async Task<double> CalculatePotentialValue(double x)
        {
            return -Math.Pow(Math.E,-x*x) -  8* Math.Pow(Math.E, -8*(Math.Pow(x-2,2)*8));
        }
        private async Task<double> CalculateAcceleration(double x)
        {
            return -(await CalculatePotentialValue(x + DeltaXInDerriveative) - await CalculatePotentialValue(x - DeltaXInDerriveative))/(2*DeltaXInDerriveative);
        }
        public async Task<Dictionary<string,double>> CalculateExplicitEulerSolution()
        {
            XValues = new() { 2.58 };
            VelocityValues = new() { 0 };
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.Black,
                ProgressCharacter = '*',
                ProgressBarOnBottom = true
            };
            var progress = new ProgressBar(TimePoints.Count(), "Calculating explicit Euler solution", options);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 1; i< TimePoints.Count(); i++)
            {
                
                XValues.Add(XValues[i - 1] + DeltaTime * VelocityValues[i-1]);
                VelocityValues.Add(VelocityValues[i - 1] + DeltaTime * await CalculateAcceleration(XValues[i - 1]));
                progress.Tick();
            }
            progress.Dispose();
            Console.WriteLine($"Calculation completed in {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            Console.WriteLine("Creating plots please wait...");
            ScottPlot.Plot plot = new();
            plot.XLabel("Czas (s)");
            plot.YLabel("Wartość");
            var scatter1 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), XValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter1.LegendText = "X [m]";
            var scatter2 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter2.LegendText = "V [m/s]";
            plot.SaveJpeg("Diagrams/explicit_euler.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(XValues.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("X [m]");
            plot.YLabel("V [m/s]");
            plot.SaveJpeg("Diagrams/explicit_euler_phase_space.jpg", 500, 500);
            watch.Stop();
            Console.WriteLine($"Finished {watch.ElapsedMilliseconds} ms");


            //energy plot
            var energy = new List<double>();
            for (int i = 0; i < XValues.Count(); i++)
            {
                energy.Add(0.5 * Math.Pow(VelocityValues[i], 2) + await CalculatePotentialValue(XValues[i]));
            }
            plot = new();
            plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), energy.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("Czas [s]");
            plot.YLabel("Energia całkowita [J]");
            plot.SaveJpeg("Diagrams/explicit_euler_Energy.jpg", 500, 500);
            var index = TimePoints.IndexOf(TimePoints.FirstOrDefault(v => Math.Abs(v - 10) < 0.000001));
            Console.WriteLine($"Euler X={XValues[index]}  V={VelocityValues[index]}");
            return new Dictionary<string, double>
            {
                { "X", XValues[index] },
                { "V", VelocityValues[index] }
            };
            //var a = await CalculatePotentialValue(5);
            //Console.WriteLine(a);
        }
        public async Task<Dictionary<string,double>> CalculateVerlet()
        {
            XValues = new() { 2.58 };
            VelocityValues = new() { 0 };
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.Black,
                ProgressCharacter = '*',
                ProgressBarOnBottom = true
            };
            var progress = new ProgressBar(TimePoints.Count(), "Calculating verlet solution", options);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 1; i < TimePoints.Count(); i++)
            {
                XValues.Add(XValues[i - 1] + DeltaTime * VelocityValues[i - 1] + 0.5 * DeltaTime * DeltaTime * await CalculateAcceleration(XValues[i - 1]));
                VelocityValues.Add(VelocityValues[i - 1] + 0.5 * DeltaTime * (await CalculateAcceleration(XValues[i - 1]) + await CalculateAcceleration(XValues[i])));
                progress.Tick();
            }
            progress.Dispose();
            Console.WriteLine($"Calculation completed in {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            Console.WriteLine("Creating plots please wait...");

            Plot plot = new();
            plot.XLabel("Czas (s)");
            plot.YLabel("Wartość");
            var scatter1 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), XValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter1.LegendText = "X [m]";
            var scatter2 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter2.LegendText = "V [m/s]";
            plot.SaveJpeg("Diagrams/verlet.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(XValues.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("X [m]");
            plot.YLabel("V [m/s]");
            plot.SaveJpeg("Diagrams/verlet_phase_space.jpg", 500, 500);
            watch.Stop();
            Console.WriteLine($"Finished in {watch.ElapsedMilliseconds} ms");
            var energy = new List<double>();
            for (int i = 0; i < XValues.Count(); i++)
            {
                energy.Add(0.5 * Math.Pow(VelocityValues[i], 2) + await CalculatePotentialValue(XValues[i]));
            }
            plot = new();
            plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), energy.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("Czas [s]");
            plot.YLabel("Energia całkowita [J]");
            plot.SaveJpeg("Diagrams/Verlet_Energy.jpg", 500, 500);

            var index = TimePoints.IndexOf(TimePoints.FirstOrDefault(v => Math.Abs(v - 10) < 0.000001));
            Console.WriteLine($"Verlet X={XValues[index]}  V={VelocityValues[index]}");
            return new Dictionary<string, double>
            {
                { "X", XValues[index] },
                { "V", VelocityValues[index] }
            };
        }
        public async Task<Dictionary<string,double>> CalculateRK4()
        {
            XValues = new() { 2.58 };
            VelocityValues = new() { 0 };
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.Black,
                ProgressCharacter = '*',
                ProgressBarOnBottom = true
            };
            var progress = new ProgressBar(TimePoints.Count(), "Calculating RK4 solution", options);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 1; i < TimePoints.Count(); i++)
            {
                var kx1 = VelocityValues[i - 1];
                var kv1 = await CalculateAcceleration(XValues[i - 1]);
                var kx2 = VelocityValues[i - 1] + DeltaTime * kv1 / 2;
                var kv2 = await CalculateAcceleration(XValues[i - 1] + DeltaTime * kx1 / 2);
                var kx3 = VelocityValues[i - 1] + DeltaTime * kv2 / 2;
                var kv3 = await CalculateAcceleration(XValues[i - 1] + DeltaTime * kx2 / 2);
                var kx4 = VelocityValues[i - 1] + DeltaTime * kv3;
                var kv4 = await CalculateAcceleration(XValues[i - 1] + DeltaTime * kx3);
                XValues.Add(XValues[i - 1] + DeltaTime * (kx1 + 2 * kx2 + 2 * kx3 + kx4) / 6);
                VelocityValues.Add(VelocityValues[i - 1] + DeltaTime * (kv1 + 2 * kv2 + 2 * kv3 + kv4) / 6);
                progress.Tick();
            }
            progress.Dispose();
            Console.WriteLine($"Calculation completed in {watch.ElapsedMilliseconds} ms");
            watch.Restart();
            Console.WriteLine("Creating plots please wait...");

            Plot plot = new();
            plot.XLabel("Czas [s]");
            plot.YLabel("Wartość");
            var scatter1 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), XValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter1.LegendText = "X [m]";
            var scatter2 = plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            scatter2.LegendText = "V [m/s]";
            plot.SaveJpeg("Diagrams/RK4.jpg", 500, 500);
            plot = new();
            plot.Add.ScatterLine(XValues.Where((v, ind) => ind % 10 == 0).ToList(), VelocityValues.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("X [m]");
            plot.YLabel("V [m/s]");
            plot.SaveJpeg("Diagrams/RK4_phase_space.jpg", 500, 500);
            watch.Stop();
            Console.WriteLine($"Finished in {watch.ElapsedMilliseconds} ms");
            var energy = new List<double>();
            for (int i = 0; i < XValues.Count(); i++)
            {
                energy.Add(0.5 * Math.Pow(VelocityValues[i], 2) + await CalculatePotentialValue(XValues[i]));
            }
            plot = new();
            plot.Add.ScatterLine(TimePoints.Where((v, ind) => ind % 10 == 0).ToList(), energy.Where((v, ind) => ind % 10 == 0).ToList());
            plot.XLabel("Czas [s]");
            plot.YLabel("Energia całkowita [J]");
            plot.SaveJpeg("Diagrams/Rk4_Energy.jpg", 500, 500);

            var index = TimePoints.IndexOf(TimePoints.FirstOrDefault(v => Math.Abs(v - 10) < 0.000001));
            Console.WriteLine($"RK4 X={XValues[index]}  V={VelocityValues[index]}");
            return new Dictionary<string, double>
            {
                { "X", XValues[index] },
                { "V", VelocityValues[index] }
            };
        }
    }
}
