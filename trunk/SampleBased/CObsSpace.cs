using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.SampleBased;

namespace Simples.SampleBased
{

    public abstract class CObsSpace: ICloneable
    {
        protected int dimensionCount;
        protected double[] dimensionSize;

        public CObsSpace(int dimensionCount, double[] dimensionSize)
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

        public virtual bool CheckCollision(double[] p)
        {
            if (p.Length != dimensionCount)
            {
                new Exception("The dimensionCount of p and CObsSpace msut be the same");
            }
            for (int i = 0; i < dimensionCount; i++)
            {
                if (p[i] >= dimensionSize[i])
                {
                    new Exception("p is out of bounds of CObsSpace");
                }
            }

            return false;


        }

        public bool checkPath(Node node1, ref Node node2)
        //public bool checkPath(Node node1, ref Node node2, out double dist)
        {
            Boolean collision = false;
            double dist = node1.calcDist(node2);
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
            //double dist;
            EdgeState state;
            //Boolean freePath = !checkPath(node1, ref node2, out dist);
            Boolean freePath = !checkPath(node1, ref node2);
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

        public abstract object Clone();
    }


}
