using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Simples.PathPlan.SampleBased
{
    public class KdTree
    {
        CSpace cSpace;
        public int Level;
        public Node RootNode;
        public KdTree LeftTree;
        public KdTree RightTree;

        public KdTree(CSpace cSpace)
        {
            this.cSpace = cSpace;
            this.RootNode = null;
            this.Level = 0;
        }

        private KdTree(CSpace cSpace, Node node, int level)
        {
            this.cSpace = cSpace;
            this.RootNode = node;
            this.Level = level;
        }

        public void Add(Node node)
        {
            if (RootNode == null)
            {
                RootNode = node;
            }
            else
            {

                int currentDimension = Level % cSpace.DimensionCount;
                if (node.p[currentDimension] < RootNode.p[currentDimension])
                {
                    if (LeftTree != null)
                    {
                        LeftTree.Add(node);
                    }
                    else
                    {
                        LeftTree = new KdTree(cSpace, node, Level + 1);
                    }
                }
                else
                {
                    if (RightTree != null)
                    {
                        RightTree.Add(node);
                    }
                    else
                    {
                        RightTree = new KdTree(cSpace, node, Level + 1);
                    }
                }
            }
        }

        public Node GetNearestNode(Node node)
        {
            Stack<KdTree> nodeStack = new Stack<KdTree>();
            HashSet<KdTree> visitedNodes = new HashSet<KdTree>();
            bool deepSearch = true;
            KdTree currentNode = this;
            double bestDistance = double.PositiveInfinity;
            Node bestNode = null;
            while (deepSearch)
            {
                int currentDimension = currentNode.Level % cSpace.DimensionCount;
                nodeStack.Push(currentNode);
                if (node.p[currentDimension] < currentNode.RootNode.p[currentDimension])
                {
                    if (currentNode.LeftTree != null)
                    {
                        currentNode = currentNode.LeftTree;
                    }
                    else
                    {
                        deepSearch = false;
                        bestDistance = cSpace.CalcWeightedDist(node, currentNode.RootNode);
                        bestNode = currentNode.RootNode;
                    }
                }
                else
                {
                    if (currentNode.RightTree != null)
                    {
                        currentNode = currentNode.RightTree;
                    }
                    else
                    {
                        deepSearch = false;
                        bestDistance = cSpace.CalcWeightedDist(node, currentNode.RootNode);
                        bestNode = currentNode.RootNode;
                    }
                }
            }

            while (nodeStack.Count > 0)
            {
                currentNode = nodeStack.Pop();
                int currentDimension = currentNode.Level % cSpace.DimensionCount;
                double dist = cSpace.CalcWeightedDist(node, currentNode.RootNode);
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestNode = currentNode.RootNode;
                }

                visitedNodes.Add(currentNode);

                if (node.p[currentDimension] - dist < currentNode.RootNode.p[currentDimension] && currentNode.LeftTree != null && !visitedNodes.Contains(currentNode.LeftTree))
                {
                    nodeStack.Push(currentNode.LeftTree);

                }

                if (node.p[currentDimension] + dist >= currentNode.RootNode.p[currentDimension] && currentNode.RightTree != null && !visitedNodes.Contains(currentNode.RightTree))
                {
                    nodeStack.Push(currentNode.RightTree);
                }
            }

            return bestNode;

        }
    }
}
