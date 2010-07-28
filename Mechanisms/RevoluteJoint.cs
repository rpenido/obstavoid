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
        protected Vector3 axis;
        public Vector3 Axis
        {
            get { return axis; }
        }
        #endregion

        #region Property:Value (Override)
        protected override void setValue(double value)
        {
            base.setValue((360 + value) % 360);
        }      
        #endregion


        public double RadiansValue
        {
            get { return Math.PI * Value / 180.0; }
        }

        public RevoluteJoint(Link parentLink, Vector3 position, float angle, Vector3 axis)
            : base(parentLink, position)
        {
            this.Value = angle;
            this.axis = axis;
        }
        
        public RevoluteJoint(Matrix world, float angle, Vector3 axis)
            : base(world)
        {
            this.Value = angle;
            this.axis = axis;
        }

        protected override Matrix getTransform()
        {
            
            return Matrix.CreateFromAxisAngle(axis, Convert.ToSingle(RadiansValue)) * base.getTransform();
        }
    }
}
