// See https://aka.ms/new-console-template for more information
using Project1.Solutions;
namespace Project1
{
    class Program
    {
       
        public static async Task Main()
        {
            var ex1 = new Exercise1();
            await ex1.CalculateExplicitEulerSolution();
            await ex1.CalculateVerlet();
            await ex1.CalculateRK4();

            var ex2 = new Exercise2();
            await ex2.CalculateExplicitEulerSolutionWVT();
            await ex2.CalculateVerletSolutionWVT();
            await ex2.CalculateRK4SolutionWVT();
            ex2.alpha = 5;
            await ex2.CalculateExplicitEulerSolutionWVT();
            await ex2.CalculateVerletSolutionWVT();
            await ex2.CalculateRK4SolutionWVT();
            ex2.alpha = 0.5;
            await ex2.CalculateExplicitEulerSolutionWVT();
            await ex2.CalculateVerletSolutionWVT();
            await ex2.CalculateRK4SolutionWVT();
            Console.WriteLine("Finished!");
        }
    }
}
