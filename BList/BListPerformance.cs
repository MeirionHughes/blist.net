using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Diagnostics;

namespace Testing
{
    [TestFixture]
    public class BListPerformance
    {
        private Random _random = new Random();

        [Test]
        public void performance()
        {
            var stopwatch = new Stopwatch();

            var count = 2 << 16;

            Console.WriteLine($"Count: {count}");
            

            stopwatch.Reset();
            stopwatch.Start();
            {
                var blist = new BList<int>();

                for (int i = 0; i < count; i++)
                {
                    blist.Add(_random.Next());
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"B-List - Add Last = {stopwatch.Elapsed.TotalMilliseconds:0.0}ms");


            stopwatch.Reset();
            stopwatch.Start();
            {
                var blist = new BList<int>();

                for (int i = 0; i < count; i++)
                {
                    blist.Insert(0, _random.Next());
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"B-List - Add First = {stopwatch.Elapsed.TotalMilliseconds:0.0}ms");
            

            stopwatch.Reset();
            stopwatch.Start();
            {
                var list = new List<int>();

                for (int i = 0; i < count; i++)
                {
                    list.Add(_random.Next());
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"List - Add Last = {stopwatch.Elapsed.TotalMilliseconds:0.0}ms");


            stopwatch.Reset();
            stopwatch.Start();
            {
                var list = new List<int>();

                for (int i = 0; i < count; i++)
                {
                    list.Insert(0, _random.Next());
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"List - Add First = {stopwatch.Elapsed.TotalMilliseconds:0.0}ms");


            stopwatch.Reset();
            stopwatch.Start();
            {
                var list = new LinkedList<int>();

                for (int i = 0; i < count; i++)
                {
                    list.AddFirst(_random.Next());
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Linked-List - Add Last = {stopwatch.Elapsed.TotalMilliseconds:0.0}ms");


            stopwatch.Reset();
            stopwatch.Start();
            {
                var list = new LinkedList<int>();

                for (int i = 0; i < count; i++)
                {
                    list.AddLast(_random.Next());
                }
            }

            stopwatch.Stop();
            Console.WriteLine($"Linked-List - Add First = {stopwatch.Elapsed.TotalMilliseconds:0.0}ms");

        }
    }
}