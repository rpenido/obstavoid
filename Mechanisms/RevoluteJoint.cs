using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Robotics.Mechanisms
{
    public class RevoluteJoint: Joint
    {
        #region Property:Axis
        protected Vector3 _axis;
        public Vector3 Axis
        {
            get { return _axis; }
        }
        #endregion

        public RevoluteJoint(Link parentLink, Vector3 position, float angle, Vector3 axis)
            : base(parentLink, position)
        {
            this.Value = angle;
            this._axis = axis;
        }
        
        public RevoluteJoint(Matrix world, float angle, Vector3 axis)
            : base(world)
        {
            this.Value = angle;
            this._axis = axis;
        }

        protected override Matrix getTransform()
        {
            return Matrix.CreateFromAxisAngle(_axis, MathHelper.ToRadians(_value)) * base.getTransform();
        }
    }
}
