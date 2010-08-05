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

        private ManualResetEvent stopEvent;
        private Thread[] threadPool;
        private CObsSpace[] mechanismPool;

        public double MinDist
        {
            get
            {
                    return minDist;
            }
        }

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
            this.threadPool = new Thread[threadCount];
            this.mechanismPool = new CObsSpace[threadCount];

            fs = new FileStream("result.csv", FileMode.Append);
            sw = new StreamWriter(fs);

            stopEvent = new ManualResetEvent(false);

            for (int i = 0; i < threadCount; i++)
            {
                mechanismPool[i] = cObsSpace.Clone() as CObsSpace;          
            }
        }

        private void calcLoop(object cObsSpace)
        {
            while (true)
            {
                if (stopEvent.WaitOne(0))
                {
                    break;
                }
                else
                {
                    calc(cObsSpace as CObsSpace);
                }
            }
        }

        private void calc(CObsSpace cObsSpace)
        {

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

        }

        public void Start()
        {
            for (int i = 0; i < threadCount; i++ )
            {
                threadPool[i] = new Thread(calcLoop);
                threadPool[i].Start(mechanismPool[i]);
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
