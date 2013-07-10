using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiveTest.Prompt
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Test");
            ITest oTest = new BojianTest();
            oTest.MainTest();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
