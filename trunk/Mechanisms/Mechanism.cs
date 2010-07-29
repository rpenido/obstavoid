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
        private List<Joint> jointList;
        public List<Joint> Joints
        {
            get { return jointList; }
        }

        private List<Link> linkList;
        public List<Link> Links
        {
            get { return linkList; }
        }

        public Mechanism()
        {
            jointList = new List<Joint>();
            linkList = new List<Link>();
        }
    }
}
