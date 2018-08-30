using System;
using System.Reflection;
namespace Retriever4.Tests.UnmeasurableTests
{
    public class RetrieverMethodsTests
    {
        public static void TestAllQueries()
        {
            Console.WriteLine("TESTOWANIE POPRAWNOŚCI ZAPYTAŃ Z KLASY RETRIEVER.CS");
            Console.WriteLine();
            var obj = new Gatherer();
            var myType = obj.GetType();
            var info = myType.GetMethods();
            var mess = "--------------==========Passed!==========--------------";
            var color = ConsoleColor.Green;
            foreach (var z in info)
            {
                if (z.Name == "GetType" || z.Name == "ToString" || z.Name == "GetHashCode" || z.Name == "Equals")
                    continue;
                try
                {
                    Console.Write(z.Name);
                    z.Invoke(obj, null);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(50, Console.CursorTop);
                    Console.Write("Work!");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.SetCursorPosition(50, Console.CursorTop);
                    Console.WriteLine(e.InnerException.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    mess = "--------------==========Failed!==========--------------";
                    color = ConsoleColor.Red;
                }
            }

            Console.ForegroundColor = color;
            Console.Write(mess);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
