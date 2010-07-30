using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace Simples.Robotics.Mechanisms
{
    [Serializable]
    public class Joint
    {
        protected Matrix world;
        protected Link parentLink;
        protected Vector3 position;
        
        protected bool calcPending;
        public void setPending()
        {
            calcPending = true;
        }

        #region Property:Value
        protected double value;

        protected virtual void setValue(double value)
        {
            this.value = value;
            setPending();
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
        #endregion

        #region Property:Transform
        protected Matrix transform;
        public Matrix Transform
        {
            get { return getTransform();}
        }

        protected virtual Matrix getTransform()
        {
            if (calcPending)
            {
                if (parentLink != null)
                {
                    transform = Matrix.CreateTranslation(position) * parentLink.Transform;
                }
                else
                {
                    transform = world;
                }
                calcPending = false;
            }
            return transform;
        }
        #endregion

        public Joint()
        {
        }

        public Joint(Link parentLink, Vector3 position)
        {
            this.parentLink = parentLink;
            this.position = position;
            if (parentLink == null)
            {
                
            }
            calcPending = true;
        }

        public Joint(Matrix world)
        {
            this.position = world.Translation;
            this.world = world;
        }


    }
}
