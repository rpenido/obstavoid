using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Simples.Robotics.Mechanisms
{
    public class Joint
    {
        protected Matrix _world;
        protected Link _parentLink;
        protected Vector3 _position;
        
        protected bool _calcPending;
        
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
        protected Matrix _transform;
        public Matrix Transform
        {
            get { return getTransform();}
        }

        protected virtual Matrix getTransform()
        {
            if (_calcPending)
            {
                if (_parentLink != null)
                {
                    _transform = Matrix.CreateTranslation(_position) * _parentLink.Transform;
                }
                else
                {
                    _transform = _world;
                }
                _calcPending = false;
            }
            return _transform;
        }
        #endregion

        public Joint(Link parentLink, Vector3 position)
        {
            this._parentLink = parentLink;
            this._position = position;
            if (_parentLink == null)
            {
                
            }
            _calcPending = true;
        }

        public Joint(Matrix world)
        {
            this._position = world.Translation;
            this._world = world;
        }

        public void setPending()
        {
            _calcPending = true;
        }
    }
}
