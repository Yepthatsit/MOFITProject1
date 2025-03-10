using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Solutions
{
    class Exercise1
    {
        public double MaxTime { get; set; } = 100;
        public double DeltaTime { get; set; } = 0.001;
        public List<double> TimePoints { get; set; } = new();
        public List<double> XValues { get; set; } = new() {2.58};
        public List<double> VelocityValues { get; set; } = new() { 0 };
        public Exercise1()
        {
            try
            {
                TimePoints = Enumerable.Range(0, (int)(100.00/0.001)).Select(x => (double)x / 1000).ToList();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"By some miracle this occured in Exercise1 constructor {ex.Message.ToString()}");
            }
        }
        private async Task<double> CalculatePotentialValue(double x)
        {
            return -Math.Pow(Math.E,-x*x) -  8* Math.Pow(Math.E, -8*(Math.Pow(x-2,2)));
        }
        public async Task CalculateExplicitEulerSolution()
        {
            for(int i = 1; i< TimePoints.Count(); i++)
            {
                
            }
            //var a = await CalculatePotentialValue(5);
            //Console.WriteLine(a);
        }
    }
}
