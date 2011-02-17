using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.PathPlan.SamplesBased
{

    public abstract class CObsSpace2: ICloneable
    {
        protected int dimensionCount;
        protected double[] dimensionSize;

        public CObsSpace2(int dimensionCount, double[] dimensionSize)
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





        public abstract object Clone();
    }


}
