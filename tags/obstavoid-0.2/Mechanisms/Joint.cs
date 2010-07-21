using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Robotics.Mechanisms
{
    public class Joint
    {
        protected Matrix world;
        protected Link parentLink;
        protected Vector3 position;
        
        protected bool calcPending;
        
        #region Property:Value
        protected float value;

        protected virtual void setValue(float value)
        {
            this.value = value;
            setPending();
        }        
        protected float getValue()
        {
            return value;
        }
        public float Value
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

        public void setPending()
        {
            calcPending = true;
        }
    }
}
