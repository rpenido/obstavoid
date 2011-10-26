using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.PathPlan.SampleBased
{
    
    class NodeComparer : IComparer<Node>
    {
        public static NodeComparer nc = new NodeComparer();

        public int Compare(Node node1, Node node2)
        {
            return node1.aTotalDist.CompareTo(node2.aTotalDist);
        }
    }
    

    public class Node: IDisposable
    {
        public double[] p;
        public List<Edge> childs;
        public double aDist;
        public double aTotalDist;
        public Node aCameFrom = null;

        public Node(double[] p)
        {
            childs = new List<Edge>();
            this.p = p;
        }

        public void Dispose()
        {
            childs.Clear();
        }

        public void addChild(Edge child)
        {
            childs.Add(child);
        }

        public void removeChild(Edge child)
        {
            childs.Remove(child);
        }
        /*
        public void searchAndInsert(Edge edge)
        {
            int index = childs.BinarySearch(edge, EdgeComparer.vc);

            if (index < 0)
            {
                childs.Insert(~index, edge);
            }
        }
        */
    }

}
