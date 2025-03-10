// See https://aka.ms/new-console-template for more information
using Project1.Solutions;
namespace Project1
{
    class Program
    {
       
        public static async Task Main()
        {
            var ex1 = new Exercise1();
            await ex1.CalculateSolution();
            Console.WriteLine("something");
        }
    }
}
