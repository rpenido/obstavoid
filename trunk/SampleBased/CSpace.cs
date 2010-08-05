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
        protected double[] dimensionSize;
        protected CObsSpace cObsSpace;

        protected CSpace(int dimensionCount, double[] dimensionSize, CObsSpace cObsSpace)
        {
            if (dimensionCount != 2)
            {
                new Exception("The dimensionCount must be 2");
            }
            else if (dimensionSize.Length != dimensionCount)
            {
                new Exception("The dimensionSize must have the same length of the dimensionCount value");
            }

            this.dimensionCount = dimensionCount;
            this.dimensionSize = dimensionSize;
            this.cObsSpace = cObsSpace;

        }


        public Edge getEdge(List<Edge> edgeList, Node node1, Node node2)
        {
            foreach (Edge edge in edgeList)
            {
                if ((node1 == edge.node1) && (node2 == edge.node2))
                    return edge;
                else if ((node1 == edge.node2) && (node2 == edge.node1))
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

                    double tentative_g_score = x.aDist + yEdge.dist;
                    Boolean tentative_is_better = false;
                    if (!openSet.Contains(y))
                    {
                        y.aTotalDist = tentative_g_score + y.calcDist(destNode);
                        searchAndInsert(openSet, y);
                        tentative_is_better = true;
                    }
                    else if (tentative_g_score < y.aDist)
                    {
                        tentative_is_better = true;
                    }

                    if (tentative_is_better)
                    {
                        y.aCameFrom = x;
                        y.aDist = tentative_g_score;
                        y.aTotalDist = y.aDist + y.calcDist(destNode);
                    }
                }
            }
        }



    }
}
