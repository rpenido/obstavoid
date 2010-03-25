using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.Robotics.Mechanisms;
using Simples.Robotics.Scene;
using SampleBased;


namespace Simples.Robotics.Mechanisms
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
                ((RevoluteJoint)_mechanism.Joints[i]).Angle = p[i];
            }
            return _scene.isColliding(_mechanism);
        }

    }
}
