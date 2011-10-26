using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using Simples.PathPlan.SampleBased;

namespace Simples.PathPlan.SampleBased.RRT
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
        private int maxIterations = 1000;
        private double[] origin;
        private double[] dest;

        private double minDist = double.PositiveInfinity;
        private int noResultCount = 0;
        private ExplorationTree t1, t2;
        public Node bestDestNode;
        private int threadCount;

        private ManualResetEvent stopEvent;
        private Thread[] threadPool;
        private CSpace[] cSpacePool;

        public double MinDist
        {
            get { return minDist; }        
        }

        public CSpace CSpace
        {
            get
            {
                if (cSpacePool.Length == 0)
                {
                    throw new Exception("cSpacePool.Length == 0");
                }
                return cSpacePool[0];
            }
        }

        private FileStream fs;
        private StreamWriter sw;

        public RRTOptimizer(double[] origin, double[] dest, CSpace[] cSpacePool, int threadCount)
        {
            if (cSpacePool.Length != threadCount)
            {
                throw new ArgumentException("The cSpacePool must have the same length of the threadCount value");
            }
            this.cSpacePool = cSpacePool;

            this.origin = origin;
            this.dest = dest;

            this.threadCount = threadCount;
            this.threadPool = new Thread[threadCount];

            fs = new FileStream("result.csv", FileMode.Append);
            sw = new StreamWriter(fs);

            stopEvent = new ManualResetEvent(false);
        }

        private void calcLoop(object cSpace)
        {
            while (true)
            {
                if (stopEvent.WaitOne(0))
                {
                    break;
                }
                else
                {
                    calc(cSpace as CSpace);
                }
            }
        }

        private void calc(CSpace cSpace)
        {
            CSpaceRRT RRT = new CSpaceRRT(cSpace, maxIterations);
            
            Node originNode, destNode;
            int iterations = RRT.generatePath(origin, dest, out originNode, out destNode, GrowConnectionType.Node, true, 10.0, stopEvent);

            double distance = destNode.aTotalDist;
            if (distance != 0)
            {
                noResultCount = 0;
                lock (sw)
                {
                    if (distance < minDist)
                    {
                        minDist = distance;
                        //maxIterations = iterations;
                        bestDestNode = destNode;
                        t1 = RRT.startTree;
                        t2 = RRT.goalTree;
                        //sw.WriteLine(iterations.ToString() + ";" + distance.ToString());
                        //sw.Flush();
                    }
                    //results.Add(new Result(iterations, distance));

                }
            }
            else
            {
                lock (sw)
                {
                    noResultCount++;
                    if (noResultCount > 50)
                    {
                        //maxIterations *= 2;
                        noResultCount = 0;
                    }                    
                }
            }

        }

        public void Start()
        {
            for (int i = 0; i < threadCount; i++ )
            {
                threadPool[i] = new Thread(calcLoop);
                threadPool[i].Start(cSpacePool[i]);
            }
        }

        public void Stop()
        {
            stopEvent.Set();
            for (int i = 0; i < threadCount; i++)
            {
                threadPool[i].Join();
            }
            stopEvent.Reset();
        }


        
    }
}
