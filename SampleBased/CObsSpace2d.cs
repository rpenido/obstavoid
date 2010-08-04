using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Simples.SampledBased
{
    public class CObsSpace2d : CObsSpace
    {
        private Boolean[,] obsMatrix;
        public CObsSpace2d(Boolean[,] obsMatrix)
            : base(2, new double[2] { obsMatrix.GetLength(0), obsMatrix.GetLength(1) })
        {
            this.obsMatrix = obsMatrix;
        }

        public override Boolean CheckCollision(double[] p)
        {
            return obsMatrix[(int)Math.Round(p[0]), (int)Math.Round(p[1])];

        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

    }
}
