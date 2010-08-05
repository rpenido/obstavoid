using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.SampleBased
{
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
                    int retval = edge1.dist.CompareTo(edge2.dist);
                    return retval;
                }
            }
        }
    }


    public enum EdgeState { Free, Obstacle, Unexplored };

    public class Edge
    {
        public Node node1;
        public Node node2;
        public double dist;
        public EdgeState state;
        public double[] vector;

        public Edge(Node node1, Node node2, double dist, EdgeState state)
        {
            this.node1 = node1;
            this.node2 = node2;
            this.dist = dist;
            this.state = state;

            this.vector = new double[node1.p.Length];
            calcVector();

            double check0 = Math.Sqrt(Math.Pow(node1.p[0] - node2.p[0], 2) + Math.Pow(node1.p[1] - node2.p[1], 2));
            double check = Math.Sqrt(Math.Pow(vector[0], 2) + Math.Pow(vector[1], 2));

        }

        public void Dispose()
        {
            node1 = null;
            node2 = null;

        }
        public void calcVector()
        {
            for (int i = 0; i < node1.p.Length; i++)
            {
                vector[i] = (node2.p[i] - node1.p[i]) / dist;
                if (double.IsNaN(vector[i]))
                {
                    vector[i] = -1;
                }
            }

        }

        public Node getNode(Node node)
        {
            if (node == this.node1)
                return this.node2;
            else if (node == this.node2)
                return this.node1;
            else
                return null;
        }

    }
}
