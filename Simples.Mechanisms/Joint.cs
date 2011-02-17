using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace Simples.Mechanisms
{
    [Serializable]
    public class Joint
    {
        //protected Matrix world;
        protected double minValue;
        protected double maxValue;
        protected double velocity;
        protected Link parentLink;
        protected Vector3 position;
        
        protected bool calcPending;
        public void setPending()
        {
            calcPending = true;
        }

        protected double value;

        protected virtual void setValue(double value)
        {
            if ((value <= maxValue) && (value >= minValue))
            {
                this.value = value;
                setPending();
            }
        }        

        protected double getValue()
        {
            return value;
        }
        
        public double Value
        {
            get { return getValue(); }
            set { setValue(value); }
        }
        

        protected Matrix transform;
        public Matrix Transform
        {
            get { return getTransform();}
        }

        protected virtual Matrix getTransform()
        {
            if (calcPending)
            {
                transform = Matrix.CreateTranslation(position) * parentLink.Transform;
                calcPending = false;
            }
            return transform;
        }

        private Joint()
        {
        }

        public Joint(Link parentLink, Vector3 position, double minValue, double maxValue, double velocity)
        {
            this.parentLink = parentLink;
            this.position = position;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.velocity = velocity;
            setPending();
        }
    }
}
