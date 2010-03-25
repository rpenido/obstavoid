using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleBased
{

    public class CObsSpace
    {
        protected int _dimensionCount;
        protected int[] _dimensionSize;

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

            this._dimensionCount = dimensionCount;
            this._dimensionSize = dimensionSize;

        }

        public virtual bool CheckCollision(int[] p)
        {
            if (p.Length != _dimensionCount)
            {
                new Exception("The dimensionCount of p and CObsSpace msut be the same");
            }
            for (int i = 0; i < _dimensionCount; i++)
            {
                if (p[i] >= _dimensionSize[i])
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
                for (int j = 0; j < p.Length; j++)
                {
                    double dimValue = node1.p[j] + (node2.p[j] - node1.p[j]) * stepPercent;
                    p[j] = (int)dimValue;
                }

                collision = CheckCollision(p);

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


}
