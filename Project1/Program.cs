// See https://aka.ms/new-console-template for more information
using Project1.Solutions;
namespace Project1
{
    class Program
    {
       
        public static async Task Main()
        {
        //    double x0 = 0.5980;
        //    double v0 = -1.1815;
        //    List<double> XE = new List<double>();
        //    List<double> VE = new List<double>();
        //    List<double> XV = new List<double>();
        //    List<double> VV = new List<double>();
        //    List<double> XRK4 = new List<double>();
        //    List<double> VRK4 = new List<double>();
        //    var TIMES = new List<double> { 1e-5, 1e-4, 1e-3, 1e-2,0.03,0.05,0.08, 1e-1 }; //Enumerable.Range(1, 10).Select(x => (double)x * 0.000001).ToList();
        //    foreach (var time in TIMES)
        //    {
                //Console.WriteLine($"Calculating time {time}");
                var ex1 = new Exercise1();
                var dE = await ex1.CalculateExplicitEulerSolution();
                var dV = await ex1.CalculateVerlet();
                var drk4 =  await ex1.CalculateRK4();
            //XE.Add(Math.Abs(dE["X"] - x0));
            //VE.Add(Math.Abs(dE["V"] - v0));
            //XV.Add(Math.Abs(dV["X"] -x0 ));
            //VV.Add(Math.Abs(dV["V"] - v0));
            //XRK4.Add(Math.Abs(drk4["X"] - x0));
            //VRK4.Add(Math.Abs(drk4["V"] - v0));
            //}
            //ScottPlot.Plot plt = new ();

            //var s1 = plt.Add.ScatterLine(TIMES, XE);
            //var s2 = plt.Add.ScatterLine(TIMES, XV);
            //var s3 = plt.Add.ScatterLine(TIMES, XRK4);
            //s1.LegendText = "Euler";
            //s2.LegendText = "Verlet";
            //s3.LegendText = "RK4";
            //s1.MarkerStyle = ScottPlot.MarkerStyle.Default;
            //s2.MarkerStyle = ScottPlot.MarkerStyle.Default;
            //s3.MarkerStyle = ScottPlot.MarkerStyle.Default;
            //plt.XLabel("Krok czasowy (s)");
            //plt.YLabel("|X - Xref|");

            //plt.SaveJpeg("Diagrams/Exercise1.jpg", 500, 500);
            //plt = new();
            //s1 = plt.Add.ScatterLine(TIMES, VE);

            //s2 = plt.Add.ScatterLine(TIMES, VV);
            //s3 = plt.Add.ScatterLine(TIMES, VRK4);
            //s1.LegendText = "Euler";
            //s2.LegendText = "Verlet";
            //s3.LegendText = "RK4";
            //s1.MarkerStyle = ScottPlot.MarkerStyle.Default;
            //s2.MarkerStyle = ScottPlot.MarkerStyle.Default;
            //s3.MarkerStyle = ScottPlot.MarkerStyle.Default;
            //plt.XLabel("Krok czasowy (s)");
            //plt.YLabel("|V - Vref|");
            //plt.SaveJpeg("Diagrams/Exercise1_Velocity.jpg", 500, 500);
            //Console.WriteLine("Finished!");
            //zadanie 2
            var ex2 = new Exercise2();
            ex2.alpha = null;
            ex2.tolerance = 0.0001;
            await ex2.CalculateExplicitEulerSolutionWVT();
            await ex2.CalculateVerletSolutionWVT();
            await ex2.CalculateRK4SolutionWVT();

            //zadanie 3
            ex2.alpha = 5;
            await ex2.CalculateExplicitEulerSolutionWVT();
            await ex2.CalculateRK4SolutionWVT();
            ex2.alpha = 0.5;
            await ex2.CalculateExplicitEulerSolutionWVT();
            await ex2.CalculateRK4SolutionWVT();

            var ex4 = new Exercise4();
            await ex4.CalculateTrapzSolution();
            ex4.alpha = 5;
            await ex4.CalculateTrapzSolution();
            Console.WriteLine("Finished!");
        }
    }
}
