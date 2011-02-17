using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.SampleBased;

namespace Simples.SampleBased
{
    public abstract class CSpace
    {
        protected int dimensionCount;
        protected double[] dimensionLowLimit;
        protected double[] dimensionHighLimit;
        protected double[] dimensionWeight;
        protected CObsSpace cObsSpace;

        protected CSpace(int dimensionCount, double[] dimensionLowLimit, double[] dimensionHighLimit, double[] dimensionWeight, CObsSpace cObsSpace)
        {
            if (dimensionCount != 2)
            {
                new Exception("The dimensionCount must be 2");
            }
            else if (dimensionLowLimit.Length != dimensionCount)
            {
                new Exception("The dimensionLowLimit must have the same length of the dimensionCount value");
            }
            else if (dimensionHighLimit.Length != dimensionCount)
            {
                new Exception("The dimensionHighLimit must have the same length of the dimensionCount value");
            }

            this.dimensionCount = dimensionCount;
            this.dimensionLowLimit = dimensionLowLimit;
            this.dimensionHighLimit = dimensionWeight;
            this.cObsSpace = cObsSpace;

        }


        public Edge getEdge(List<Edge> edgeList, Node node1, Node node2)
        {
            foreach (Edge edge in edgeList)
            {
                if ((node1 == edge.Node1) && (node2 == edge.Node2))
                    return edge;
                else if ((node1 == edge.Node2) && (node2 == edge.Node1))
                    return edge;
            }

            Edge newEdge = cObsSpace.createEdge(node1, node2);
            edgeList.Add(newEdge);
            return newEdge;
        }

        private static void searchAndInsert(List<Node> nodeList, Node node)
        {
            int index = nodeList.BinarySearch(node, NodeComparer.nc);

            if (index < 0)
            {
                nodeList.Insert(~index, node);
            }
        }

        protected static void A_Star(Node originNode, Node destNode)
        {
            List<Node> closedSet = new List<Node>();
            List<Node> openSet = new List<Node>();

            Node x;
            Node y;

            originNode.aDist = 0;
            originNode.aTotalDist = originNode.calcDist(destNode);

            openSet.Add(originNode);

            while (openSet.Count != 0)
            {
                x = openSet[0];

                if (x == destNode)
                    return;

                openSet.Remove(x);
                closedSet.Add(x);

                foreach (Edge yEdge in x.childs)
                {
                    y = yEdge.getNode(x);
                    if (closedSet.Contains(y))
                        continue;

                    double score = x.aDist + yEdge.WeightedDistance;
                    Boolean scoreBetter = false;
                    if (!openSet.Contains(y))
                    {
                        y.aTotalDist = score + y.calcDist(destNode);
                        searchAndInsert(openSet, y);
                        scoreBetter = true;
                    }
                    else if (score < y.aDist)
                    {
                        scoreBetter = true;
                    }

                    if (scoreBetter)
                    {
                        y.aCameFrom = x;
                        y.aDist = score;
                        y.aTotalDist = y.aDist + y.calcDist(destNode);
                    }
                }
            }
        }



    }
}
