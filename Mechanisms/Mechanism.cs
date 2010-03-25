using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Simples.Robotics.Mechanisms
{
    public class Mechanism
    {
        private List<Joint> _jointList;
        public List<Joint> Joints
        {
            get { return _jointList; }
        }

        private List<Link> _linkList;
        public List<Link> Links
        {
            get { return _linkList; }
        }

        public Mechanism()
        {
            _jointList = new List<Joint>();
            _linkList = new List<Link>();
        }
    }
}
