using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.PathPlan.SampleBased
{
    public delegate bool CollisionCheck(double[] p);

    public class CSpace
    {
        private Random rand;

        private int dimensionCount;
        private double[] dimensionLowLimit;
        private double[] dimensionHighLimit;
        private double[] dimensionWeight;
        private CollisionCheck collisionCheck;

        public int DimensionCount
        {
            get { return dimensionCount; }
        }

        public CSpace(int dimensionCount, double[] dimensionLowLimit, double[] dimensionHighLimit, double[] dimensionWeight, CollisionCheck collisionCheck, int randomSeed)
        {
            
            if (dimensionLowLimit.Length != dimensionCount)
            {
                throw new ArgumentException ("The dimensionLowLimit must have the same length of the dimensionCount value");
            }
            
            if (dimensionHighLimit.Length != dimensionCount)
            {
                throw new ArgumentException("The dimensionHighLimit must have the same length of the dimensionCount value");
            }

            if (dimensionWeight.Length != dimensionCount)
            {
                throw new ArgumentException("The dimensionWeight must have the same length of the dimensionCount value");
            }
            
            if (dimensionWeight.Length != dimensionCount)
            {
                throw new ArgumentNullException("collisionCheck");
            }

            this.dimensionCount = dimensionCount;
            this.dimensionLowLimit = dimensionLowLimit;
            this.dimensionHighLimit = dimensionHighLimit;
            this.dimensionWeight = dimensionWeight;
            this.collisionCheck = collisionCheck;
            this.rand = new Random(randomSeed);
        }

        public bool CheckCollision(double[] p)
        {
            return collisionCheck(p);
        }

        public Queue<double[]> GenerateLatticeSampleList(int sampleCount)
        {
            double a = (Math.Sqrt(5) + 1) / 2;

            Queue<double[]> sampleList = new Queue<double[]>(sampleCount);


            for (int i = 1; i <= sampleCount; i++)
            {
                double[] p = new double[dimensionCount];
                double coord = (double)i / (double)sampleCount * (dimensionHighLimit[0] - dimensionLowLimit[0]) + dimensionLowLimit[0];

                p[0] = coord;
                for (int j = 1; j < dimensionCount; j++)
                {
                    double fnb = Math.Pow(a, j);

                    coord = (i * fnb - Math.Floor(i * fnb)) * (dimensionHighLimit[i] - dimensionLowLimit[i]) + dimensionLowLimit[i];
                    p[j] = coord;

                }

                if (CheckCollision(p))
                {
                    sampleList.Enqueue(p);
                }
            }

            return sampleList;
        }

        public Queue<double[]> GenerateRandomSampleList(int sampleCount)
        {
            Queue<double[]> sampleList = new Queue<double[]>(sampleCount);
         
            for (int i = 1; i <= sampleCount; i++)
            {
                double[] p = new double[dimensionCount];

                for (int j = 0; j < dimensionCount; j++)
                {

                    p[j] = rand.NextDouble() * (dimensionHighLimit[i] - dimensionLowLimit[i]) + dimensionLowLimit[i];
                }

                if (CheckCollision(p))
                {
                    sampleList.Enqueue(p);
                }
            }

            return sampleList;
        }

        public double[] GenerateSample()
        {
            double[] p = new double[dimensionCount];

            for (int i = 0; i < dimensionCount; i++)
            {
                p[i] = rand.NextDouble() * (dimensionHighLimit[i] - dimensionLowLimit[i]) + dimensionLowLimit[i];
            }

            return p;
        }
        /*
        public bool CheckPath(Node node1, ref Node node2, float maxEdgeSize)
        {
            Boolean collision = false;
            double dist = CalcDist(node1, node2);
            int step = 1;
            double[] p = new double[node1.p.Length];
            double[] lastP = new double[node1.p.Length];

            for (int j = 0; j < p.Length; j++)
            {
                p[j] = node1.p[j];
                lastP[j] = p[j];
            }

            for (int i = step; i < dist; i = i + step)
            {
                double stepPercent = i / dist;
                for (int j = 0; j < p.Length; j++)
                {
                    double dimValue = node1.p[j] + (node2.p[j] - node1.p[j]) * stepPercent;
                    p[j] = dimValue;
                }

                collision = CheckCollision(p);

                if (collision)
                {
                    node2 = new Node(lastP);
                    //dist = CalcDist(node1, node2);
                    return true;
                }
                else if (i > maxEdgeSize)
                {
                    node2 = new Node(p);
                    return false;

                }
                else
                {
                    for (int j = 0; j < p.Length; j++)
                    {
                        lastP[j] = p[j];
                    }

                }
            }
            return false;
        }
        
        public bool CheckPath(Node node1, ref Node node2)
        {
            CheckPath(node1, ref node2, float.PositiveInfinity);
        }
         * */
    
        public bool CheckPath(Node node1, ref Node node2)
        {
            Boolean collision = false;
            double dist = CalcDist(node1, node2);
            int step = 1;
            double[] p = new double[node1.p.Length];
            double[] lastP = new double[node1.p.Length];

            for (int j = 0; j < p.Length; j++)
            {
                p[j] = node1.p[j];
                lastP[j] = p[j];
            }

            for (int i = step; i < dist; i = i + step)
            {
                double stepPercent = i / dist;
                for (int j = 0; j < p.Length; j++)
                {
                    double dimValue = node1.p[j] + (node2.p[j] - node1.p[j]) * stepPercent;
                    p[j] = dimValue;
                }

                collision = CheckCollision(p);

                if (collision)
                {
                    node2 = new Node(lastP);
                    dist = CalcDist(node1, node2);
                    return true;
                }
                else
                {
                    for (int j = 0; j < p.Length; j++)
                    {
                        lastP[j] = p[j];
                    }

                }
            }
            return false;
        }

        public static double CalcDist(Node node1, Node node2)
        {
            if (node1.p.Length != node2.p.Length)
            {
                throw new ArgumentException("node1.p must have the same length that node2.p");
            }
            double sum = 0;

            for (int i = 0; i < node1.p.Length; i++)
            {
                double diff = node1.p[i] - node2.p[i];
                sum = sum + Math.Pow(diff, 2);
            }

            return Math.Sqrt(sum);
        }

        public double CalcWeightedDist(Node node1, Node node2)
        {
            return CalcDist(node1, node2);
        }

        public double CalcWeightedDist(Edge edge)
        {
            return CalcWeightedDist(edge.Node1, edge.Node2);
        }

        
        public static double CalcDist(Edge edge)
        {
            return CalcDist(edge.Node1, edge.Node2);
        }
         
        
        public Edge CreateEdge(Node node1, Node node2)
        {
            EdgeState state;

            Boolean freePath = !(CheckPath(node1, ref node2));

            if (freePath)
            {
                state = EdgeState.Free;
            }
            else
            {
                state = EdgeState.Obstacle;
            }

            return new Edge(node1, node2, state);
        }        
        
        
        private static void searchAndInsert(List<Node> nodeList, Node node)
        {
            int index = nodeList.BinarySearch(node, NodeComparer.nc);

            if (index < 0)
            {
                nodeList.Insert(~index, node);
            }
        }
        
        public static void A_Star(Node originNode, Node destNode)
        {
            List<Node> closedSet = new List<Node>();
            List<Node> openSet = new List<Node>();

            Node x;
            Node y;

            originNode.aDist = 0;
            originNode.aTotalDist = CalcDist(originNode, destNode);

            openSet.Add(originNode);

            while (openSet.Count != 0)
            {
                x = openSet[0];

                if (x == destNode)
                    return;

                openSet.Remove(x);
                closedSet.Add(x);

                foreach (Edge yEdge in x.childs)
                {
                    y = yEdge.GetOtherNode(x);
                    if (closedSet.Contains(y))
                        continue;

                    double score = x.aDist + yEdge.Distance;
                    Boolean scoreBetter = false;
                    if (!openSet.Contains(y))
                    {
                        y.aTotalDist = score + CalcDist(y, destNode);
                        searchAndInsert(openSet, y);
                        scoreBetter = true;
                    }
                    else if (score < y.aDist)
                    {
                        scoreBetter = true;
                    }

                    if (scoreBetter)
                    {
                        y.aCameFrom = x;
                        y.aDist = score;
                        y.aTotalDist = y.aDist + CalcDist(y, destNode);
                    }
                }
            }
        }

    }
}
