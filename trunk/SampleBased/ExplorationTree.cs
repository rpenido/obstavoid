using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.SampleBased
{

    public class ExplorationTree
    {
        Random rand = new Random();
        private CObsSpace cObsSpace;

        private int dimensionCount;
        private double[] dimensionLowLimit;
        private double[] dimensionHighLimit;
        private double[] dimensionWeight;
        
        public List<Node> nodeList;
        public List<Edge> edgeList;

        public int size
        {
            get { return nodeList.Count; }
        }

        private double[] generateSample()
        {
            double[] p = new double[dimensionCount];

            for (int i = 0; i < dimensionCount; i++)
            {
                p[i] = rand.NextDouble() * (dimensionHighLimit[i] - dimensionLowLimit[i]) + dimensionLowLimit[i];
            }

            return p;
        }

        public ExplorationTree(CObsSpace cObsSpace, int dimensionCount, double[] dimensionLowLimit,
            double[] dimensionHighLimit, double[] dimensionWeight, Node rootNode)
        {
            this.cObsSpace = cObsSpace;

            this.dimensionCount = dimensionCount;
            this.dimensionLowLimit = dimensionLowLimit;
            this.dimensionHighLimit = dimensionHighLimit;
            this.dimensionWeight = dimensionWeight;

            this.nodeList = new List<Node>();
            this.edgeList = new List<Edge>();

            nodeList.Add(rootNode);
        }

        ~ExplorationTree()
        {
            foreach (Node node in nodeList)
            {
                node.Dispose();
            }

            foreach (Edge edge in edgeList)
            {
                edge.Dispose();
            }

        }

   
        public Node getNearestNode(Node node)
        {
            double minDist = double.MaxValue;
            Edge nearestEdge;
            Node nearestNode = null;
           


            if (edgeList.Count == 0)
            {
                return nodeList[0];
            }
            nearestEdge = null;

            foreach (Edge edge in edgeList)
            {
                double dist;
                bool split;
                Node edgeNode = edge.GetNearestNode(node, out dist, out split);

                if (dist < minDist)
                {
                    if (split)
                    {
                        nearestEdge = edge;
                    }
                    else
                    {
                        nearestEdge = null;
                    }

                    minDist = dist;
                    nearestNode = edgeNode;

                }
            }

            if (nearestEdge != null)
            {
                nodeList.Add(nearestNode);

                Edge newEdge = nearestEdge.Split(nearestNode);
                edgeList.Add(newEdge);
            }

            return nearestNode;
        }

        public Node Grow(Node node)
        {
            //sampleList.Add(a.p);
            Node gn = getNearestNode(node);
            Node gs = node;


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
                nodeList.Add(gs);
                Edge newEdge = new Edge(gn, gs, dist, EdgeState.Free);
                edgeList.Add(newEdge);
                gn.addChild(newEdge);
                gs.addChild(newEdge);
                return gs;
            }
            else
            {
                return gn;
            }
        }

        public Node Grow()
        {
            double[] p = generateSample();

            return Grow(new Node(p));
        }
    }

}
