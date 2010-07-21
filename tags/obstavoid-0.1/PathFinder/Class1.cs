using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsFormsApplication1
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

    class NodeComparer : IComparer<Node>
    {
        public static NodeComparer nc = new NodeComparer();

        public int Compare(Node node1, Node node2)
        {
            return node1.aTotalDist.CompareTo(node2.aTotalDist);
        }
    }

    public class Node
    {
        public int[] p;
        public List<Edge> childs;
        public double aDist;
        public double aTotalDist;
        public Node aCameFrom = null;

        public Node(int[] p)
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

        public double calcDist(Node node)
        {
            double sum = 0;

            for (int i = 0; i < node.p.Length; i++)
            {
                sum = sum + Math.Pow(p[i] - node.p[i], 2);
            }

            return Math.Sqrt(sum);
        }

        public void searchAndInsert(Edge edge)
        {
            int index = childs.BinarySearch(edge, EdgeComparer.vc);

            if (index < 0)
            {
                childs.Insert(~index, edge);
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
            for (int i = 0; i < node1.p.Length; i++)
            {
                vector[i] = (node2.p[i] - node1.p[i])/dist;
            }

            double check0 = Math.Sqrt(Math.Pow(node1.p[0] - node2.p[0], 2) + Math.Pow(node1.p[1] - node2.p[1], 2));
            double check = Math.Sqrt(Math.Pow(vector[0], 2) + Math.Pow(vector[1], 2));

        }

        public void Dispose()
        {
            node1 = null;
            node2 = null;
         
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

    public class CObsSpace
    {
        private int dimensionCount;
        private int[] dimensionSize;

        public CObsSpace(int dimensionCount, int[] dimensionSize)
        {
            if (dimensionCount != 2)
            {
                new Exception("The dimensionCount must be 2");
            }
            else if (dimensionSize.Length != dimensionCount)
            {
                new Exception("The dimensionSize must have the same length of the dimensionCount value");
            }

            this.dimensionCount = dimensionCount;
            this.dimensionSize = dimensionSize;

        }

        public virtual Boolean checkCollision(int[] p)
        {
            if (p.Length != dimensionCount)
            {
                new Exception("The dimensionCount of p and CObsSpace msut be the same");
            }
            for (int i=0; i < dimensionCount; i++)
            {
                if (p[i] >= dimensionSize[i])
                {
                    new Exception("p is out of bounds of CObsSpace");
                }
            }

            return false;


        }

        public bool checkPath(Node node1, ref Node node2, out double dist)
        {
            Boolean collision = false;
            dist = node1.calcDist(node2);
            int step = 5;
            int[] p = new int[node1.p.Length];
            int[] lastP = new int[node1.p.Length];

            for (int j = 0; j < p.Length; j++)
            {
                p[j] = node1.p[j];
                lastP[j] = p[j];
            }

            for (int i = step; i < dist; i = i + step)
            {
                double stepPercent = i / dist;
                for (int j = 0; j < p.Length ; j++)
                {
                    double dimValue = node1.p[j] + (node2.p[j] - node1.p[j]) * stepPercent;
                    p[j] = (int)dimValue;
                }
                
                collision = checkCollision(p);

                if (collision)
                {
                    node2 = new Node(lastP);
                    dist = node1.calcDist(node2);
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

        public Edge createEdge(Node node1, Node node2)
        {
            double dist;
            EdgeState state;
            Boolean freePath = !checkPath(node1, ref node2, out dist);
            if (freePath)
            {
                state = EdgeState.Free;
            }
            else
            {
                state = EdgeState.Obstacle;
            }

            return new Edge(node1, node2, dist, state);
        }


    }

    public class CObsSpace2d : CObsSpace
    {
        private Boolean[,] obsMatrix;
        public CObsSpace2d(Boolean[,] obsMatrix)
            : base(2, new int[2] { obsMatrix.GetLength(0), obsMatrix.GetLength(1) })
        {
            this.obsMatrix = obsMatrix;            
        }

        public override Boolean checkCollision(int[] p)
        {
            return obsMatrix[p[0], p[1]];

        }

    }
      

    public abstract class CSpace
    {
        protected int dimensionCount;
        protected int[] dimensionSize;
        protected CObsSpace cObsSpace;

        protected CSpace(int dimensionCount, int[] dimensionSize, CObsSpace cObsSpace)
        {
            if (dimensionCount != 2)
            {
                new Exception("The dimensionCount must be 2");
            }
            else if (dimensionSize.Length != dimensionCount)
            {
                new Exception("The dimensionSize must have the same length of the dimensionCount value");
            }

            this.dimensionCount = dimensionCount;
            this.dimensionSize = dimensionSize;
            this.cObsSpace = cObsSpace;

        }


        public Edge getEdge(List<Edge> edgeList, Node node1, Node node2)
        {
            foreach (Edge edge in edgeList)
            {
                if ((node1 == edge.node1) && (node2 == edge.node2))
                    return edge;
                else if ((node1 == edge.node2) && (node2 == edge.node1))
                    return edge;
            }

            Edge newEdge = cObsSpace.createEdge(node1, node2);
            edgeList.Add(newEdge);
            return newEdge;
        }

        private static void searchAndInsert(List<Node> nodeList, Node node)
        {
            int index = nodeList.BinarySearch(node, NodeComparer.nc);

            if (index < 0)
            {
                nodeList.Insert(~index, node);
            }
        }

        protected static void A_Star(Node originNode, Node destNode)
        {
            List<Node> closedSet = new List<Node>();
            List<Node> openSet = new List<Node>();

            Node x;
            Node y;

            originNode.aDist = 0;
            originNode.aTotalDist = originNode.calcDist(destNode);

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
                    y = yEdge.getNode(x);
                    if (closedSet.Contains(y))
                        continue;

                    double tentative_g_score = x.aDist + yEdge.dist;
                    Boolean tentative_is_better = false;
                    if (!openSet.Contains(y))
                    {
                        y.aTotalDist = tentative_g_score + y.calcDist(destNode);
                        searchAndInsert(openSet, y);
                        tentative_is_better = true;
                    }
                    else if (tentative_g_score < y.aDist)
                    {
                        tentative_is_better = true;
                    }

                    if (tentative_is_better)
                    {
                        y.aCameFrom = x;
                        y.aDist = tentative_g_score;
                        y.aTotalDist = y.aDist + y.calcDist(destNode);
                    }
                }
            }
        }



    }

    public enum PRMSampleMethod{ Random, Lattice }

    public class CSpacePRM : CSpace
    {
        public int N;
        public int k;
        public PRMSampleMethod sampleMethod;

        public List<int[]> sampleList;
        public List<Node> nodeList;
        public List<Edge> edgeList;


        public Boolean pathed = false;

        public CSpacePRM(int dimensionCount, int[] dimensionSize, CObsSpace cObsSpace, int N, int k, PRMSampleMethod sampleMethod)
            : base(dimensionCount, dimensionSize, cObsSpace)
        {
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

        public void addNode(Node node, int k)
        {
            nodeList.Add(node);
            connectNode(node, k);
        }


        public List<int[]> generateSample()
        {
            double a = (Math.Sqrt(5) + 1) / 2;

            sampleList = new List<int[]>();


            for (int i = 1; i <= N; i++)
            {
                int[] p = new int[dimensionCount];
                double coord = (double)i / (double)N * ((double)dimensionSize[0] - 1);

                p[0] = (int)Math.Round(coord);
                for (int j = 1; j < dimensionCount; j++)
                {
                    double fnb = Math.Pow(a, j);

                    coord = (i * fnb - Math.Floor(i * fnb)) * ((double)dimensionSize[j] - 1);
                    p[j] = (int)Math.Round(coord);

                }

                if (!((cObsSpace != null) && (cObsSpace.checkCollision(p))))
                {
                    sampleList.Add(p);
                }
            }

            return sampleList;

        }

        public List<int[]> generateRandomSample()
        {
            sampleList = new List<int[]>();
            Random rand = new Random();

            for (int i = 1; i <= N; i++)
            {
                int[] p = new int[dimensionCount];

                for (int j = 0; j < dimensionCount; j++)
                {
                    p[j] = rand.Next(dimensionSize[j] - 1);
                }

                if (!((cObsSpace != null) && (cObsSpace.checkCollision(p))))
                {
                    sampleList.Add(p);
                }
            }

            return sampleList;

        }        

        private void generateCFreeSpace()
        {
            foreach (int[] p in sampleList)
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

                if (edge.state != EdgeState.Free)
                    continue;

                if (node.childs.Contains(edge))
                    continue;

                if (node.childs.Count == 0)
                {
                    node.childs.Add(edge);
                    edge.getNode(node).searchAndInsert(edge);
                }
                else if (edge.dist < node.childs.Last().dist)
                {
                    if (node.childs.Count == k)
                    {
                        Edge oldEdge = node.childs.Last();
                        Node oldNode = oldEdge.getNode(node);
                        oldNode.removeChild(oldEdge);
                        node.removeChild(oldEdge);
                    }

                    node.searchAndInsert(edge);
                    edge.getNode(node).searchAndInsert(edge);
                }
            }

        }


        public void generatePath(int[] origin, int[] dest, int k,
            out Node originNode, out Node destNode)
        {
            if (sampleList == null)
            {
                switch (sampleMethod)
                {
                    case PRMSampleMethod.Lattice:
                        generateSample();
                        break;
                    case PRMSampleMethod.Random:
                        generateRandomSample();
                        break;
                }
            }

            if (!pathed)
                generateCFreeSpace();

            originNode = new Node(origin);
            addNode(originNode, k);

            destNode = new Node(dest);
            addNode(destNode, k);

            A_Star(originNode, destNode);
        }

    }

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

        /*        public Node addNode(Node node)
                {
                    nodeList.Add(node);
                    return connectNode(node);
                }
        
                public Node connectNode(Node node)
                {
                    return new Node(new int[2]);
                }
        */
        public static double getProjection(int[] p, Edge edge)
        {
            double[] pVector = new double[p.Length];
            double sum = 0;
            for (int i = 0; i < p.Length; i++)
            {
                pVector[i] = (p[i] - edge.node1.p[i]) * edge.vector[i];
                sum = sum + Math.Pow(pVector[i], 2);
            }
            return Math.Sqrt(sum);
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
                    int[] edgeP = new int[a.p.Length];

                    for (int i = 0; i < edgeP.Length; i++)
                    {
                        edgeP[i] = edge.node1.p[i] + (int)Math.Round(scalarProjection * edge.vector[i]);
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
        public List<int[]> sampleList;
        public ExplorationTree startTree;
        public ExplorationTree goalTree;
        int k;

        public CSpaceRRT(int dimensionCount, int[] dimensionSize, CObsSpace cObsSpace, int k)
            : base(dimensionCount, dimensionSize, cObsSpace)
        {
            this.k = k;
            this.sampleList = new List<int[]>();
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
            int[] p = new int[dimensionCount];

            for (int j = 0; j < dimensionCount; j++)
            {
                p[j] = rand.Next(dimensionSize[j] - 1);
            }

            return growTree(T, new Node(p));
        }

        public void generatePath(int[] origin, int[] dest,
            out Node originNode, out Node destNode)
        {
            originNode = new Node(origin);
            startTree = new ExplorationTree(originNode);

            destNode = new Node(dest);
            goalTree = new ExplorationTree(destNode);


            Node qs;
            Node qs2;
            qs = growTree(startTree, destNode);

            if (qs == destNode)
            {
                return;
            }
            
            ExplorationTree T1 = startTree;
            ExplorationTree T2 = goalTree;
            
            for (int i = 0; i < k; i++)
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
        }

    }
}

