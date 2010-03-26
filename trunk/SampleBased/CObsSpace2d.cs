using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.SampledBased.ObstacleSpace
{
    public class CObsSpace2d : CObsSpace
    {
        private Boolean[,] obsMatrix;
        public CObsSpace2d(Boolean[,] obsMatrix)
            : base(2, new int[2] { obsMatrix.GetLength(0), obsMatrix.GetLength(1) })
        {
            this.obsMatrix = obsMatrix;
        }

        public override Boolean CheckCollision(int[] p)
        {
            return obsMatrix[p[0], p[1]];

        }

    }
}
