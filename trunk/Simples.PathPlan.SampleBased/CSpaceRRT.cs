using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Simples.PathPlan.SampleBased;

namespace Simples.PathPlan.SampleBased.RRT
{

    public class CSpaceRRT
    {
        
        private Queue<double[]> sampleList;
        public ExplorationTree startTree;
        public ExplorationTree goalTree;
        int k;

        private CSpace cSpace;

        public CSpaceRRT(CSpace cSpace, int k)
        {
            this.cSpace = cSpace;

            this.k = k;
        }

        public int generatePath(double[] origin, double[] dest,
            out Node originNode, out Node destNode, GrowConnectionType connectionType, bool useKdTree, double maxEdgeSize, EventWaitHandle stopSignal)
        {
            originNode = new Node(origin);
            startTree = new ExplorationTree(cSpace, originNode, connectionType, useKdTree, maxEdgeSize);

            destNode = new Node(dest);
            goalTree = new ExplorationTree(cSpace, destNode, connectionType, useKdTree, maxEdgeSize);


            Node qs;
            Node qs2;

            ExplorationTree T1 = startTree;
            ExplorationTree T2 = goalTree;
            int i;
            for (i = 0; i < k; i++)
            {
                qs = T1.Grow();
                qs2 = T2.Grow(qs);

                if (qs == qs2)
                    break;

                if (stopSignal.WaitOne(0))
                {
                    break;
                }

                if (T1.Size > T2.Size)
                {
                    ExplorationTree temp = T1;
                    T1 = T2;
                    T2 = temp;
                }

            }


            CSpace.A_Star(originNode, destNode);
            T1 = null;
            T2 = null;
            return i;
        }

    }
}
