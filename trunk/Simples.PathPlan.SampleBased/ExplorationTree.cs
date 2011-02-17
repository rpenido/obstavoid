using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.PathPlan.SamplesBased
{

    public class ExplorationTree
    {
        Random rand = new Random();

        private CSpace cSpace;
        
        private List<Node> nodeList;
        private List<Edge> edgeList;

        public int size
        {
            get { return nodeList.Count; }
        }

       

        internal ExplorationTree(CSpace cSpace, Node rootNode)
        {
            this.cSpace = cSpace;

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
            Node gn = getNearestNode(node);
            Node gs = node;


            cSpace.CheckPath(gn, ref gs);

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
                Edge newEdge = new Edge(gn, gs, EdgeState.Free);
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
            double[] p = cSpace.GenerateSample();

            return Grow(new Node(p));
        }


    }

}
