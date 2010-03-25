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
        protected Matrix _transform;
        protected bool _calcPending;
    

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
