using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.PathPlan.SampleBased;

namespace Simples.PathPlan.SampleBased.PRM
{
    public enum PRMSampleMethod { Random, Lattice }

    public class CSpacePRM
    {
        public int N;
        public int k;
        public PRMSampleMethod sampleMethod;

        private Queue<double[]> sampleList;
        private List<Node> nodeList;
        private List<Edge> edgeList;

        private CSpace cSpace;


        public Boolean pathed = false;

        public CSpacePRM(CSpace cSpace, int N, int k, PRMSampleMethod sampleMethod)
        {
            this.cSpace = cSpace;

            this.N = N;
            this.k = k;

            this.sampleMethod = sampleMethod;

            this.nodeList = new List<Node>();
            this.edgeList = new List<Edge>();
        }

        ~CSpacePRM()
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

        public Edge getEdge(List<Edge> edgeList, Node node1, Node node2)
        {
            foreach (Edge edge in edgeList)
            {
                if ((node1 == edge.Node1) && (node2 == edge.Node2))
                    return edge;
                else if ((node1 == edge.Node2) && (node2 == edge.Node1))
                    return edge;
            }

            Edge newEdge = cSpace.CreateEdge(node1, node2);
            edgeList.Add(newEdge);
            return newEdge;
        }

        public void addNode(Node node, int k)
        {
            nodeList.Add(node);
            connectNode(node, k);
        }



        private void generateCFreeSpace()
        {
            foreach (double[] p in sampleList)
            {
                Node node = new Node(p);
                addNode(node, k);
            }

            pathed = true;
        }


        public void connectNode(Node node, int k)
        {
            foreach (Node nodeCSpace in this.nodeList)
            {
                if (nodeCSpace == node) // self;
                {
                    continue;
                }

                Edge edge = this.getEdge(edgeList, node, nodeCSpace);

                if (edge.State != EdgeState.Free)
                    continue;

                if (node.childs.Contains(edge))
                    continue;

                if (node.childs.Count == 0)
                {
                    node.childs.Add(edge);
                    edge.GetOtherNode(node).addChild(edge);
                }
                else if (edge.Distance < node.childs.Last().Distance)
                {
                    if (node.childs.Count == k)
                    {
                        Edge oldEdge = node.childs.Last();
                        Node oldNode = oldEdge.GetOtherNode(node);
                        oldNode.removeChild(oldEdge);
                        node.removeChild(oldEdge);
                    }

                    node.addChild(edge);
                    edge.GetOtherNode(node).addChild(edge);
                }
            }

        }


        public void generatePath(double[] origin, double[] dest, int k,
            out Node originNode, out Node destNode)
        {
            if (sampleList == null)
            {
                switch (sampleMethod)
                {
                    case PRMSampleMethod.Lattice:
                        sampleList = cSpace.GenerateLatticeSampleList(N);
                        break;
                    case PRMSampleMethod.Random:
                        sampleList = cSpace.GenerateRandomSampleList(N);
                        break;
                }
            }

            if (!pathed)
                generateCFreeSpace();

            originNode = new Node(origin);
            addNode(originNode, k);

            destNode = new Node(dest);
            addNode(destNode, k);

            CSpace.A_Star(originNode, destNode);
        }

    }

}
