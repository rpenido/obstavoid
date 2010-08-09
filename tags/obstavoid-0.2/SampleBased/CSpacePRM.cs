﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.SampledBased.Util;
using Simples.SampledBased.ObstacleSpace;

namespace Simples.SampledBased.ConfigurationSpace
{
    public enum PRMSampleMethod { Random, Lattice }

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

                if (!((cObsSpace != null) && (cObsSpace.CheckCollision(p))))
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

                if (!((cObsSpace != null) && (cObsSpace.CheckCollision(p))))
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

}