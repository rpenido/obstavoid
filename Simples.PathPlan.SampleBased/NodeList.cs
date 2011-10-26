using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  Simples.PathPlan.SampleBased
{
    public class NodeList
    {
        bool useKdTree;
        List<Node> nodeList;
        List<Edge> edgeList;
        KdTree kdTree;
        CSpace cSpace;
        GrowConnectionType connectionType;

        public int Size
        {
            get { return nodeList.Count; }
        }

        public NodeList(CSpace cSpace, GrowConnectionType connectionType, bool useKdTree)
        {
            this.cSpace = cSpace;
            this.useKdTree = useKdTree;
            this.connectionType = connectionType;
            if (useKdTree)
            {
                kdTree = new KdTree(cSpace);
            }
            this.nodeList = new List<Node>();
            if (connectionType == GrowConnectionType.Edge)
            {
                this.edgeList = new List<Edge>();
            }
        }
        
        ~NodeList()
        {
            foreach (Node node in nodeList)
            {
                node.Dispose();
            }
            if (connectionType == GrowConnectionType.Edge)
            {
                foreach (Edge edge in edgeList)
                {
                    edge.Dispose();
                }
                edgeList.Clear();
            }
        }
 
        public void AddNode(Node node)
        {
            nodeList.Add(node);
            if (useKdTree)
            {
                kdTree.Add(node);
            }
        }
        
        public void AddEdge(Edge edge)
        {
            if (connectionType == GrowConnectionType.Edge)
            {
                edgeList.Add(edge);
            }
        }

        public Node GetNearestByNode(Node node)
        {
            if (useKdTree)
            {
                return kdTree.GetNearestNode(node);
            }
            else
            {
                double minDist = double.MaxValue;
                Node nearestNode = null;

                foreach (Node n in nodeList)
                {
                    double dist = cSpace.CalcWeightedDist(node, n);
                    if (dist < minDist)
                    {
                        nearestNode = n;
                    }
                }

                return nearestNode;
            }
        }

        public Node GetNearestByEdge(Node node)
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

    }
}
