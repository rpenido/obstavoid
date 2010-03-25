using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Robotics.Mechanisms
{
    public class RevoluteJoint: Joint
    {
        protected Vector3 _axis;
        public Vector3 Axis
        {
            get { return _axis; }
        }


        protected float _angle;
        
        public float Angle
        {
            get { return _angle; }
            set
            {
                _angle = value;
                _calcPending = true;
            }
        }

        public RevoluteJoint(Link parentLink, Vector3 position, float angle, Vector3 axis)
            : base(parentLink, position)
        {
            this._angle = angle;
            this._axis = axis;
        }
        
        public RevoluteJoint(Matrix world, float angle, Vector3 axis)
            : base(world)
        {
            this._angle = angle;
            this._axis = axis;
        }

        protected override Matrix getTransform()
        {
            return Matrix.CreateFromAxisAngle(_axis, MathHelper.ToRadians(_angle)) * base.getTransform();
        }
    }
}
