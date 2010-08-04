using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Simples.SampledBased;

namespace Simples.SampledBased
{
    struct Result
    {
        int Iterations;
        double Distance;

        Result(int iterations, double distance)
        {
            this.Iterations = iterations;
            this.Distance = distance;
        }
    }

    public class RRTOptimizer
    {
        private int dimensionCount;
        private double[] dimensionSize;
        private int maxIterations = 1000;
        private double[] origin;
        private double[] dest;

        private double minDist = double.PositiveInfinity;
        private ExplorationTree t1, t2;
        public Node bestDestNode;
        private int threadCount;
        private Semaphore semaphore;

        private ManualResetEvent stopEvent;
        private Thread loopThread;
        private Stack<CObsSpace> mechanismStack;

        public double MinDist
        {
            get
            {
                    return minDist;
            }
        }

        //private List<Result> results = new List<Result>();

        private FileStream fs;
        private StreamWriter sw;

        public RRTOptimizer(int dimensionCount, double[] dimensionSize, CObsSpace cObsSpace,
            double[] origin, double[] dest, int threadCount)
        {
            this.dimensionCount = dimensionCount;
            this.dimensionSize = dimensionSize;
            this.origin = origin;
            this.dest = dest;

            this.threadCount = threadCount;
           

            fs = new FileStream("result.csv", FileMode.Append);
            sw = new StreamWriter(fs);

            mechanismStack = new Stack<CObsSpace>(threadCount);

            for (int i = 0; i < threadCount; i++)
            {
                mechanismStack.Push(cObsSpace.Clone() as CObsSpace);
            }

            stopEvent = new ManualResetEvent(false);
            semaphore = new Semaphore(threadCount, threadCount);
            loopThread = new Thread(calcLoop);
         
         
        }

        private void calcLoop()
        {
            while (true)
            {
                if (stopEvent.WaitOne(0))
                {
                    break;
                }
                else if (semaphore.WaitOne(0))
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(Calc));
                }
                else
                {
                    Thread.Sleep(500);
                }
            }
        }

        private void Calc(Object stateInfo)
        {
            CObsSpace cObsSpace;
            lock (mechanismStack)
            {
                cObsSpace = mechanismStack.Pop();
            }

            CSpaceRRT RRT = new CSpaceRRT(dimensionCount, dimensionSize, cObsSpace, maxIterations);
            
            Node originNode, destNode;

            int iterations = RRT.generatePath(origin, dest, out originNode, out destNode);

            double distance = destNode.aTotalDist;
            if (distance != 0)
            {
                lock (sw)
                {
                    if (distance < minDist)
                    {
                        minDist = distance;
                        maxIterations = iterations;
                        bestDestNode = destNode;
                        t1 = RRT.startTree;
                        t2 = RRT.goalTree;
                        //sw.WriteLine(iterations.ToString() + ";" + distance.ToString());
                        //sw.Flush();
                    }
                    //results.Add(new Result(iterations, distance));

                }
            }

            lock (mechanismStack)
            {
                mechanismStack.Push(cObsSpace);
            }
          
            int tst = semaphore.Release();
            tst += 0;
         

        }

        public void Start()
        {
            loopThread = new Thread(calcLoop);
            loopThread.Start();
        }

        public void Stop()
        {
            stopEvent.Set();
            for (int i = 0; i < threadCount; i++)
            {
                semaphore.WaitOne();
            }
            loopThread.Join();
            semaphore.Release(threadCount);
            stopEvent.Reset();
        }

        
    }
}
