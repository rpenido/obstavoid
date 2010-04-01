using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simples.Simulation.Planar2D;
using Microsoft.Xna.Framework;
using Simples.Scene.Camera;

namespace _2dPlanarSimulation
{
    class SceneView : GraphicsDeviceControl
    {
        private Matrix _world;
        private SceneBoxes _scene;
        private NArticulatedPlanar _robot;

        public SceneView(NArticulatedPlanar robot, SceneBoxes scene, Matrix world)
        {
            this._scene = scene;
            this._robot = robot;
            this._world = world;
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        protected override void Initialize()
        {
            // Create our vertex declaration.

            /*
            vertexDeclaration = new VertexDeclaration(GraphicsDevice,
                                                VertexPositionColor.VertexElements);

            // Create our effect.
            effect = new BasicEffect(GraphicsDevice, null);

            effect.VertexColorEnabled = true;

            // Start the animation timer.
            timer = Stopwatch.StartNew();

            // Hook the idle event to constantly redraw our animation.
            Application.Idle += delegate { Invalidate(); };
             * */
        }

        protected override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
