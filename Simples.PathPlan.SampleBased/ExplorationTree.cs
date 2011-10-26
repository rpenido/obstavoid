using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.PathPlan.SampleBased
{
    public enum GrowConnectionType { Node, Edge };
    public enum SamplingMethod { RandomInteractive, RandomPrior, LatticePrior };

    public class ExplorationTree
    {
        private double maxEdgeSize = double.PositiveInfinity;
        //private double maxEdgeSize = 10.0;
        private GrowConnectionType growConnectionType = GrowConnectionType.Edge;
        public GrowConnectionType GrowConnectionType
        {
            get { return growConnectionType; }
            set { growConnectionType = value; }
        }

        private SamplingMethod samplingMethod = SamplingMethod.RandomInteractive;
        public SamplingMethod SamplingMethod
        {
            get { return samplingMethod; }
            set { samplingMethod = value; }
        }
        Random rand = new Random();

        private CSpace cSpace;
        
        private NodeList nodeList;

        private Queue<double[]> sampleList;
        public int Size
        {
            get { return nodeList.Size; }
        }


       

        internal ExplorationTree(CSpace cSpace, Node rootNode, GrowConnectionType connectionType, bool useKDtree, double maxEdgeSize)
        {
            this.cSpace = cSpace;
            this.nodeList = new NodeList(cSpace, growConnectionType, useKDtree);
            this.growConnectionType = connectionType;
            this.maxEdgeSize = maxEdgeSize;

            nodeList.AddNode(rootNode);
        }

        internal ExplorationTree(CSpace cSpace, Node rootNode, int sampleCount)
        {
            this.cSpace = cSpace;
            this.nodeList = new NodeList(cSpace, growConnectionType, false);

            nodeList.AddNode(rootNode);

            switch (samplingMethod)
            {
                case SamplingMethod.RandomPrior:
                    sampleList = cSpace.GenerateLatticeSampleList(sampleCount);
                    break;
                case SamplingMethod.LatticePrior:
                    sampleList = cSpace.GenerateLatticeSampleList(sampleCount);
                    break;
                case SamplingMethod.RandomInteractive:
                    throw new ArgumentException("Cannot initialize sampleCount with interactive sampling method", "sampleCount");
            }


        }
      
        private Node getNearestNode(Node node)
        {
            switch (growConnectionType)
            {
                case SampleBased.GrowConnectionType.Edge:
                    return nodeList.GetNearestByEdge(node);
                case SampleBased.GrowConnectionType.Node:
                    return nodeList.GetNearestByNode(node);
                default:
                    return null;
            }
           
        }

        internal double[] getSample()
        {
            switch (samplingMethod)
            {
                case SamplingMethod.RandomPrior:
                case SamplingMethod.LatticePrior:
                    return sampleList.Dequeue();
                case SamplingMethod.RandomInteractive:
                   return cSpace.GenerateSample();
                default:
                   return null;
            }
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
                double dist = cSpace.CalcWeightedDist(gn, gs);
                double stepPercent = maxEdgeSize / dist;

                for (double i = maxEdgeSize; i < dist; i += maxEdgeSize)
                {
                    double[] p = new double[gn.p.Length];

                    for (int j = 0; j < p.Length; j++)
                    {
                        double dimValue = gn.p[j] + (gs.p[j] - gn.p[j]) * stepPercent;
                        p[j] = dimValue;
                    }

                    Node gk = new Node(p);

                    nodeList.AddNode(gk);
                    Edge newEdge = new Edge(gn, gk, EdgeState.Free);
                    nodeList.AddEdge(newEdge);
                    gn.addChild(newEdge);
                    gk.addChild(newEdge);
                    
                    gn = gk;

                }

                nodeList.AddNode(gs);
                Edge newLastEdge = new Edge(gn, gs, EdgeState.Free);
                nodeList.AddEdge(newLastEdge);
                gn.addChild(newLastEdge);
                gs.addChild(newLastEdge);
                return gs;
            }
            else
            {
                // Cannot grow in this direction
                return null;
            }
        }

        public Node Grow()
        {            
            Node node;
            do
            {
                double[] p = getSample();
                node = Grow(new Node(p));
            } while (node == null);
            return node;
        }


    }

}
