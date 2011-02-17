using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.PathPlan.SamplesBased
{
    public class CObsSpace2d
    {
        private Boolean[,] obsMatrix;
        public CObsSpace2d(Boolean[,] obsMatrix)
        {
            this.obsMatrix = obsMatrix;
        }

        public Boolean CheckCollision(double[] p)
        {
            return obsMatrix[(int)Math.Round(p[0]), (int)Math.Round(p[1])];

        }

    }
}
