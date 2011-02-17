using System;
using System.Collections.Generic;
using System.Text;

namespace Simples.PathPlan.SamplesBased
{
    /*
    class EdgeComparer : IComparer<Edge>
    {
        public static EdgeComparer vc = new EdgeComparer();

        public int Compare(Edge edge1, Edge edge2)
        {
            if (edge1 == null)
            {
                if (edge2 == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (edge2 == null)
                {
                    return 1;
                }
                else
                {
                    int retval = edge1.WeightedDistance.CompareTo(edge2.WeightedDistance);
                    return retval;
                }
            }
        }
    }
    */

    public enum EdgeState { Free, Obstacle, Unexplored };

    public class Edge
    {
        private Node node1;
        private Node node2;

        public EdgeState State;
        public double[] vector;

        public Node Node1
        {
            get { return node1; }
            set { node1 = value; }
        }

        public Node Node2
        {
            get { return node2; }
            set { node2 = value; }
        }

        public double Distance
        {
            get { return CSpace.CalcDist(this); }
        }

        internal Edge(Node node1, Node node2, EdgeState state)
        {
            this.Node1 = node1;
            this.Node2 = node2;
            this.State = state;

            this.vector = new double[node1.p.Length];
            calcVector();

            double check0 = Math.Sqrt(Math.Pow(node1.p[0] - node2.p[0], 2) + Math.Pow(node1.p[1] - node2.p[1], 2));
            double check = Math.Sqrt(Math.Pow(vector[0], 2) + Math.Pow(vector[1], 2));

        }

        public void Dispose()
        {
            Node1 = null;
            Node2 = null;

        }
        public void calcVector()
        {
            for (int i = 0; i < Node1.p.Length; i++)
            {
                vector[i] = (Node2.p[i] - Node1.p[i]) / CSpace.CalcDist(node1, node2);
                if (double.IsNaN(vector[i]))
                {
                    vector[i] = -1;
                }
            }

        }
        
        public Node GetOtherNode(Node node)
        {
            if (node == this.Node1)
                return this.Node2;
            else if (node == this.Node2)
                return this.Node1;
            else
                return null;
        }
        
        private double getProjection(double[] p)
        {
            double[] pVector = new double[p.Length];
            double sum = 0;
            for (int i = 0; i < p.Length; i++)
            {
                pVector[i] = (p[i] - Node1.p[i]) * vector[i];
                sum += (p[i] - Node1.p[i]) * vector[i];
            }

            return sum;

        }

        public Node GetNearestNode(Node node, out double nodeDist, out bool split)
        {
            double scalarProjection;
            Node edgeNode;

            scalarProjection = getProjection(node.p);

            if (scalarProjection <= 0)
            {
                edgeNode = Node1;
                split = false;
            }
            else if (scalarProjection >= Distance)
            {
                edgeNode = Node2;
                split = false;
            }
            else
            {
                double[] edgeP = new double[node.p.Length];

                for (int i = 0; i < edgeP.Length; i++)
                {
                    edgeP[i] = Node1.p[i] + scalarProjection * vector[i];
                }

                edgeNode = new Node(edgeP);
                split = true;
            }

            nodeDist = CSpace.CalcDist(node, edgeNode);
            return edgeNode;
        }

        public Edge Split(Node midNode)
        {
            Node tempNode = Node2;
            tempNode.removeChild(this);

            Node2 = midNode;
            midNode.addChild(this);

            Edge newEdge = new Edge(midNode, tempNode, EdgeState.Free);
            midNode.addChild(newEdge);
            tempNode.addChild(newEdge);

            return newEdge;
        }

    }
}
