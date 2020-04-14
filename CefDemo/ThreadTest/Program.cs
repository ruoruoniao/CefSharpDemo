using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 20; i++)
            {
                if (i == 10)
                {
                    Thread.Sleep(10000);
                }

                OrderedThread orderedThread = new OrderedThread((o) =>
                {
                    Console.WriteLine("第一轮:当前的线程序号" + o);

                    Thread.Sleep(1000);

                    Console.WriteLine(DateTime.Now.ToString());
                });
                orderedThread.Start();
            }

            Console.WriteLine("第一轮全部开始了...");

            for (int i = 0; i < 20; i++)
            {
                OrderedThread orderedThread = new OrderedThread((o) =>
                {
                    Console.WriteLine("第二轮:当前的线程序号" + o);

                    Thread.Sleep(1000);

                    Console.WriteLine(DateTime.Now.ToString());
                });
                orderedThread.Start();
            }

            Console.WriteLine("第二轮全部开始了...");

            Console.ReadKey();
        }
    }
}
