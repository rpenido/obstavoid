using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Simples.PathPlan.SamplesBased
{
    public class PathSmoothing
    {
        private ManualResetEvent stopEvent;
        private Thread workerThread;
        private Node destNode;
        private CSpace cSpace;

        public Double MinDist
        {
            get { return destNode.aTotalDist; }
        }

        public PathSmoothing(Node destNode, CSpace cSpace)
        {
            this.destNode = destNode;
            this.cSpace = cSpace;

            workerThread = new Thread(calcLoop);
            stopEvent = new ManualResetEvent(false);
            
            workerThread.Start();
        }

        public void Stop()
        {
            stopEvent.Set();
            workerThread.Join();
            stopEvent.Reset();
        }

        private void calcLoop()
        {
            while (true)
            {
                if (stopEvent.WaitOne(0))
                {
                    break;
                }
                else
                {
                    Smooth(destNode, cSpace);
                }
            }
        }


        private static List<Edge> GetEdgeList(Node destNode)
        {
            List<Edge> edgeList = new List<Edge>();

            Node currentNode;

            currentNode = destNode;
            while (currentNode.aCameFrom != null)
            {
                Edge edge = new Edge(currentNode.aCameFrom, currentNode, EdgeState.Free);
                edgeList.Insert(0, edge);

                currentNode = currentNode.aCameFrom;
            }
            return edgeList;
        }

        public static List<Edge> Smooth(Node destNode, CSpace cSpace)
        {
            List<Edge> edgeList = GetEdgeList(destNode);
            Smooth(destNode, cSpace, edgeList);
            return edgeList;
        }

        public static List<Edge> Smooth(Node destNode, CSpace cSpace, List<Edge> edgeList)
        {
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
                p1[i] = edge1.Node1.p[i] + edge1.vector[i] * edge1.Distance * factor;
            }
            node1 = new Node(p1);
            if (CSpace.CalcDist(node1, edge1.Node1) < 0.1)
            {
                return edgeList;
            }


            factor = random.NextDouble();
            double[] p2 = new double[destNode.p.Length];
            for (int i = 0; i < p2.Length; i++)
            {
                p2[i] = edge2.Node1.p[i] + edge2.vector[i] * edge2.Distance * factor;
            }
            node2 = new Node(p2);
            if (CSpace.CalcDist(node2, edge2.Node2) < 0.1)
            {
                return edgeList;
            }

            //double dist;
            if (!cSpace.CheckPath(node1, ref node2) && CSpace.CalcDist(node1, node2) > 0.1)
            {
                node2.aCameFrom = node1;

                node1.aCameFrom = edge1.Node1;
                edge1.Node2 = node1;
                //edge1.Dist = edge1.Node1.calcDist(node1);


                edge2.Node1 = node2;
                edge2.Node2.aCameFrom = node2;
                //edge2.Dist = node2.calcDist(edge2.Node2);


                edgeList.RemoveRange(rand1 + 1, rand2 - (rand1 + 1));

                Edge newEdge = new Edge(node1, node2, EdgeState.Free);


                edgeList.Insert(rand1 + 1, newEdge);

                double totalDist = 0;
                foreach (Edge edge in edgeList)
                {
                    totalDist += edge.Distance;
                }
                destNode.aTotalDist = totalDist;
            }
            return edgeList;

        }
    }
}
