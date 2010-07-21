using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.Robotics.Mechanisms;
using Simples.SampledBased.ObstacleSpace;


namespace Simples.Simulation.Planar2D
{
    public class MechanismCObsSpace : CObsSpace
    {
        Mechanism _mechanism;

        SceneBoxes _scene;
        public MechanismCObsSpace(Mechanism mechanism, SceneBoxes scene)
            :base(mechanism.Joints.Count, new int[mechanism.Joints.Count])
        {
            this._mechanism = mechanism;
            this._scene = scene;

            for (int i = 0; i < _dimensionCount; i++)
            {
                _dimensionSize[i] = 359;
            }
        }

        public override bool CheckCollision(int[] p)
        {
            base.CheckCollision(p);
            for (int i = 0; i < p.Length; i++)
            {
                _mechanism.Joints[i].Value = p[i];
            }
            return _scene.isColliding(_mechanism);
        }

    }
}
