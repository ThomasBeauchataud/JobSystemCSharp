using System;
using System.Collections.Generic;
using System.Threading;

namespace JobSystem
{
    public class JobSystem
    {
        private static JobSystem instance;
        private static int threadNumber = 1;

        private List<Thread> threads;
        private Queue<Job> queue;
        private bool running;
        private Int32 x = 0;

        public JobSystem()
        {
            threads = new List<Thread>();
            queue = new Queue<Job>();
            running = false;
        }

        public void Start()
        {
            Stop();
            if(threads.Count < threadNumber)
            {
                while(threads.Count < threadNumber)
                {
                    threads.Add(new Thread(Handle));
                }
            } else
            {
                while (threads.Count > threadNumber)
                {
                    threads.RemoveAt(threads.Count - 1);
                }
            }
            running = true;
        }

        public void Stop()
        {
            running = false;
            Join();
        }

        public void Join()
        {
            foreach(Thread thread in threads)
            {
                thread.Join();
            }
        }

        public void Queue(Job job)
        {
            int expected = 0;
            while (Interlocked.CompareExchange(ref x, 1, expected) == 0)
            {
                Thread.Sleep(1);
            }
            queue.Enqueue(job);
            x = 0;
        }

        private void Handle()
        {
            while(running)
            {
                if (queue.Count > 0)
                {
                    while (Interlocked.CompareExchange(ref x, 1, 0) == 0)
                    {
                        Thread.Sleep(1);
                    }
                    if (queue.Count > 0)
                    {
                        Job job = queue.Dequeue();
                        x = 0;
                        job.Execute();
                    }
                }
            }
        }

        public static JobSystem GetInstance()
        {
            if(instance == null)
            {
                instance = new JobSystem();
            }
            return instance;
        }

        public static void SetThreadNumber(int number)
        {
            threadNumber = number;
        }
    }
}
