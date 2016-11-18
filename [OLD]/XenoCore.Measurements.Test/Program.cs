using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XenoCore.Measurements.Test
{

    class Abstract
    {
        public int x = 0;

        public static int testInstance = 0;

        public virtual void Method(Abstract l) { }
        public virtual void ListMethod(List<Abstract> list) { }
    }


    class Test : Abstract
    {


        public override void Method(Abstract l)
        {
            DoWork(l);
        }

        public override void ListMethod(List<Abstract> list)
        {
            foreach (var l in list)
                DoWork(l);
        }


        private void DoWork(Abstract t)
        {
            for (int a = 0; a < 100; ++a)
            {
                t.x += a;
                Abstract.testInstance += a;
            }
        }
    }


    class Program
    {
        static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        static void Main(string[] args)
        {
            Test work = new Test();
            int testsCount = 50;
            int iterationsCount = 50;
            int listSize = 1000;

            TimeSpan[] method1 = new TimeSpan[testsCount];
            TimeSpan[] method2 = new TimeSpan[testsCount];


            List<Abstract> list = new List<Abstract>();
            for (int i = 0; i < listSize; ++i)
            {
                list.Add(new Test()
                {
                    x = i
                });
            }

            for (int i = 0; i < testsCount; ++i)
            {

                watch.Restart();

                for (int j = 0; j < iterationsCount; ++j)
                    foreach (var l in list)
                        work.Method(l);

                watch.Stop();

                method1[i] = watch.Elapsed;



                watch.Restart();

                for (int j = 0; j < iterationsCount; ++j)
                    work.ListMethod(list);

                watch.Stop();

                method2[i] = watch.Elapsed;

            }


            long method1Ticks = 0, method2Ticks = 0;
            for (int i = 0; i < testsCount; ++i)
            {
                method1Ticks += method1[i].Ticks;
                method2Ticks += method2[i].Ticks;
            }

            method1Ticks /= testsCount;
            method2Ticks /= testsCount;

            Console.WriteLine($"Foreach -> Virtual: {new TimeSpan(method1Ticks)}");
            Console.WriteLine($"Virtual -> Foreach: {new TimeSpan(method2Ticks)}");

            Console.WriteLine($"Difference: {100 - (Math.Min(method1Ticks, method2Ticks) * 100) / Math.Max(method1Ticks, method2Ticks)} %");

            Console.ReadLine();
        }

    }
}
