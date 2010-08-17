using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.SampleBased;

namespace Simples.SampleBased
{

    public class ExplorationTree
    {

        public List<Node> nodeList;
        public List<Edge> edgeList;

        public int size
        {
            get { return nodeList.Count; }
        }

        public ExplorationTree(Node node)
        {
            this.nodeList = new List<Node>();
            this.edgeList = new List<Edge>();

            nodeList.Add(node);
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

        public static double getProjection(double[] p, Edge edge)
        {
            double[] pVector = new double[p.Length];
            double sum = 0;
            for (int i = 0; i < p.Length; i++)
            {
                pVector[i] = (p[i] - edge.node1.p[i]) * edge.vector[i];
                sum += (p[i] - edge.node1.p[i]) * edge.vector[i];
            }

            return sum;
            
        }

        public Node getNearestNode(Node a)
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
                double scalarProjection;
                Node edgeNode;
                Boolean split;

                scalarProjection = getProjection(a.p, edge);

                if (scalarProjection <= 0)
                {
                    edgeNode = edge.node1;
                    split = false;
                }
                else if (scalarProjection >= edge.dist)
                {
                    edgeNode = edge.node2;
                    split = false;
                }
                else
                {
                    double[] edgeP = new double[a.p.Length];

                    for (int i = 0; i < edgeP.Length; i++)
                    {
                        edgeP[i] = edge.node1.p[i] + scalarProjection * edge.vector[i];
                    }

                    edgeNode = new Node(edgeP);
                    split = true;
                }

                double dist;
                dist = a.calcDist(edgeNode);

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


            } // End-loop

            if (nearestEdge != null)
            {
                Node oldNode = nearestEdge.node2;
                oldNode.removeChild(nearestEdge);

                nodeList.Add(nearestNode);
                nearestEdge.node2 = nearestNode;
                nearestEdge.dist = nearestEdge.node1.calcDist(nearestNode);
                nearestNode.addChild(nearestEdge);

                Edge newEdge = new Edge(nearestNode, oldNode, nearestNode.calcDist(oldNode), EdgeState.Free);
                edgeList.Add(newEdge);
                nearestNode.addChild(newEdge);
                oldNode.addChild(newEdge);
            }

            return nearestNode;
        }

    }

    public class CSpaceRRT : CSpace
    {
        Random rand = new Random();
        public List<double[]> sampleList;
        public ExplorationTree startTree;
        public ExplorationTree goalTree;
        int k;

        public CSpaceRRT(int dimensionCount, double[] dimensionLowLimit, double[] dimensionHighLimit, CObsSpace cObsSpace, int k)
            : base(dimensionCount, dimensionLowLimit, dimensionHighLimit, cObsSpace)
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

        public Node growTree(ExplorationTree T)
        {
            double[] p = new double[dimensionCount];

            for (int i = 0; i < dimensionCount; i++)
            {
                p[i] = rand.NextDouble() * (dimensionHighLimit[i] - dimensionLowLimit[i]) + dimensionLowLimit[i];
            }

            return growTree(T, new Node(p));
        }

        public int generatePath(double[] origin, double[] dest,
            out Node originNode, out Node destNode)
        {
            originNode = new Node(origin);
            startTree = new ExplorationTree(originNode);

            destNode = new Node(dest);
            goalTree = new ExplorationTree(destNode);


            Node qs;
            Node qs2;
            //qs = growTree(startTree, destNode);

            //if (qs == destNode)
            //{
            //    return;
            //}

            ExplorationTree T1 = startTree;
            ExplorationTree T2 = goalTree;
            int i;
            for (i = 0; i < k; i++)
            {
                qs = growTree(T1);
                qs2 = growTree(T2, qs);

                if (qs == qs2)
                    break;

                if (T1.size > T2.size)
                {
                    ExplorationTree temp;
                    temp = T1;
                    T1 = T2;
                    T2 = temp;
                }

            }



            A_Star(originNode, destNode);
            return i;
        }

    }
}
