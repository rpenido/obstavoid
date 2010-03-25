using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Simples.Robotics.Collision;


namespace Simples.Robotics.Mechanisms
{
    public class NArticulatedPlanar
    {
        private Mechanism _mechanism;
        public Mechanism Mechanism
        {
            get { return _mechanism; }
        }

        private Matrix _world;
      
        private Vector3 _linkTranslation;

        public NArticulatedPlanar(Vector3 linkTranslation, int linkCount, Matrix world)
        {
            this._mechanism = new Mechanism();
            this._world = world;
            this._linkTranslation = linkTranslation;

            RevoluteJoint nextJoint = new RevoluteJoint(world, 0, world.Up);
            _mechanism.Joints.Add(nextJoint);
            for (int i = 0; i < linkCount; i++)
            {
                Vector3 min = new Vector3(-10, -5, -10);
                Vector3 max = new Vector3(120, 5, 10);
                OrientedBoundingBox obb = new OrientedBoundingBox(min, max);
                Link link = new Link(nextJoint, obb);
                _mechanism.Links.Add(link);
                if (i != linkCount - 1)
                {
                    nextJoint = new RevoluteJoint(link, _linkTranslation, 0, world.Up);
                    _mechanism.Joints.Add(nextJoint);
                }                
            }
        }
    }
}
