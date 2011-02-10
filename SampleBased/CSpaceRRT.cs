using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.SampleBased;

namespace Simples.SampleBased
{

    public class CSpaceRRT : CSpace
    {
        
        public List<double[]> sampleList;
        public ExplorationTree startTree;
        public ExplorationTree goalTree;
        int k;

        public CSpaceRRT(int dimensionCount, double[] dimensionLowLimit, double[] dimensionHighLimit, double[] dimensionWeight, CObsSpace cObsSpace, int k)
            : base(dimensionCount, dimensionLowLimit, dimensionHighLimit, dimensionWeight, cObsSpace)
        {
            this.k = k;
            this.sampleList = new List<double[]>();
        }

        public Node growTree(ExplorationTree T, Node a)
        {
            //sampleList.Add(a.p);
            Node gn = T.getNearestNode(a);
            Node gs = a;


            double dist;

            cObsSpace.checkPath(gn, ref gs, out dist);

            Boolean sameNode;
            sameNode = true;
            for (int i = 0; i < gs.p.Length; i++)
            {
                sameNode = (gs.p[i] == gn.p[i]);
                if (!sameNode)
                    break;
            }

            if (!sameNode)
            {
                T.nodeList.Add(gs);
                Edge newEdge = new Edge(gn, gs, dist, EdgeState.Free);
                T.edgeList.Add(newEdge);
                gn.addChild(newEdge);
                gs.addChild(newEdge);
                return gs;
            }
            else
            {
                return gn;
            }


        }
        /*
        public Node growTree(ExplorationTree T)
        {
            double[] p = new double[dimensionCount];

            for (int i = 0; i < dimensionCount; i++)
            {
                p[i] = rand.NextDouble() * (dimensionHighLimit[i] - dimensionLowLimit[i]) + dimensionLowLimit[i];
            }

            return growTree(T, new Node(p));
        }
        */
        public int generatePath(double[] origin, double[] dest,
            out Node originNode, out Node destNode)
        {
            originNode = new Node(origin);
            startTree = new ExplorationTree(cObsSpace, dimensionCount, dimensionLowLimit,
                dimensionHighLimit, dimensionWeight, originNode);

            destNode = new Node(dest);
            goalTree = new ExplorationTree(cObsSpace, dimensionCount, dimensionLowLimit,
                dimensionHighLimit, dimensionWeight, destNode);


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


            A_Star(originNode, destNode);
            return i;
        }

    }
}
