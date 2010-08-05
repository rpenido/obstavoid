using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.SampleBased
{
    public class PathSmoothing
    {
        private PathSmoothing()
        {
        }

        private static List<Edge> GetEdgeList(Node destNode)
        {
            List<Edge> edgeList = new List<Edge>();

            Node currentNode;

            currentNode = destNode;
            while (currentNode.aCameFrom != null)
            {
                Edge edge = new Edge(currentNode.aCameFrom, currentNode, currentNode.calcDist(currentNode.aCameFrom), EdgeState.Free);
                edgeList.Insert(0, edge);

                currentNode = currentNode.aCameFrom;
            }
            return edgeList;
        }


        public static void Smooth(Node destNode, CObsSpace cObsSpace)
        {
            List<Edge> edgeList = GetEdgeList(destNode);
            Random random = new Random();

            int rand1, rand2;
            rand1 = random.Next(edgeList.Count - 1);

            do
            {
                rand2 = random.Next(edgeList.Count);
            } while (rand1 == rand2);

            Edge edge1, edge2;
            if (rand1 > rand2)
            {
                int temp = rand1;
                rand1 = rand2;
                rand2 = temp;

                edge1 = edgeList[rand1];
                edge2 = edgeList[rand2];
            }

            edge1 = edgeList[rand1];
            edge2 = edgeList[rand2];


            double factor;

            Node node1, node2;

            factor = random.NextDouble();
            double[] p1 = new double[destNode.p.Length];
            for (int i = 0; i < p1.Length; i++)
            {
                p1[i] = edge1.node1.p[i] + edge1.vector[i] * edge1.dist * factor;
            }
            node1 = new Node(p1);
            if (node1.calcDist(edge1.node1) < 0.1)
            {
                return;
            }


            factor = random.NextDouble();
            double[] p2 = new double[destNode.p.Length];
            for (int i = 0; i < p2.Length; i++)
            {
                p2[i] = edge2.node1.p[i] + edge2.vector[i] * edge2.dist * factor;
            }
            node2 = new Node(p2);
            if (node2.calcDist(edge2.node2) < 0.1)
            {
                return;
            }

            double dist;
            if (!cObsSpace.checkPath(node1, ref node2, out dist) && dist > 0.1)
            {
                node2.aCameFrom = node1;

                node1.aCameFrom = edge1.node1;
                edge1.node2 = node1;
                edge1.dist = edge1.node1.calcDist(node1);


                edge2.node1 = node2;
                edge2.node2.aCameFrom = node2;
                edge2.dist = node2.calcDist(edge2.node2);


                edgeList.RemoveRange(rand1 + 1, rand2 - (rand1 + 1));

                Edge newEdge = new Edge(node1, node2, dist, EdgeState.Free);


                edgeList.Insert(rand1 + 1, newEdge);

                double totalDist = 0;
                foreach (Edge edge in edgeList)
                {
                    totalDist += edge.dist;
                }
                destNode.aTotalDist = totalDist;
            }

        }
    }
}
