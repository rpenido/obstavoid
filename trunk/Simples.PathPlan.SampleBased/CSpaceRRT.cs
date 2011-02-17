using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.PathPlan.SamplesBased;

namespace Simples.PathPlan.SamplesBased.RRT
{

    public class CSpaceRRT
    {
        
        public List<double[]> sampleList;
        public ExplorationTree startTree;
        public ExplorationTree goalTree;
        int k;

        private CSpace cSpace;

        public CSpaceRRT(CSpace cSpace, int k)
        {
            this.cSpace = cSpace;

            this.k = k;
            this.sampleList = new List<double[]>();
        }

        public int generatePath(double[] origin, double[] dest,
            out Node originNode, out Node destNode)
        {
            originNode = new Node(origin);
            startTree = new ExplorationTree(cSpace, originNode);

            destNode = new Node(dest);
            goalTree = new ExplorationTree(cSpace, destNode);


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

                if (T1.size > T2.size)
                {
                    ExplorationTree temp = T1;
                    T1 = T2;
                    T2 = temp;
                }

            }


            CSpace.A_Star(originNode, destNode);
            return i;
        }

    }
}
