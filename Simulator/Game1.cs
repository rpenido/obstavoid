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
using System.Diagnostics;
using Simples.Scene.Camera;
using Simples.Robotics.Mechanisms;
using Simples.SampledBased.ConfigurationSpace;
using Simples.SampledBased.ObstacleSpace;
using Simples.SampledBased.Util;
using Simples.Simulation.Planar2D;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice mainGraphicDevice;

        ShowResultForm resultForm;

        NArticulatedPlanarController controller;

        Matrix _world;
        private OrbitCamera _camera;
        //private Model linkModel;

        private static float _STEP = 1;
        
        //private Model cube;

        private NArticulatedPlanar robot;

        private SceneBoxes scene;

        bool reset = true;
        private float oldMouseX, oldMouseY;

        //private int[] origin = new int[3] { 258, 113 , 0};
        //private int[] dest = new int[3] { 22, 344, 0};

        private int[] origin = new int[3] { 34, 334, 48 };
        private int[] dest = new int[3] { 90, 328, 24 };
        bool flag;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            graphics.PreferredBackBufferHeight = 1000;            
            graphics.PreferredBackBufferWidth = 1200;

            mainGraphicDevice = GraphicsDevice;
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here


            base.Initialize();


            _world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            _camera = new OrbitCamera();
            _camera.cameraAngleX = -90f;
            scene = new SceneBoxes(this, _camera);

            //Debug.Assert(linkModel.Bones.Count == 2);
            robot = new NArticulatedPlanar(this.Services, new Vector3(100, 0, 0), 3, _world, _camera);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //linkModel = Content.Load<Model>("model1");
            //cube = Content.Load<Model>("cube");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
           if (Keyboard.GetState().IsKeyDown(Keys.Escape))
               this.Exit();

            // TODO: Add your update logic here            
            base.Update(gameTime);
            UpdateGamePad();

            if (controller != null)
            {
                controller.Update(gameTime);
            }
        }

        private bool isColliding()
        {
            return scene.isColliding(robot.Mechanism);
        }

        private void setPending(int index, List<Joint> joints)
        {
            for (int i = index; i < joints.Count; i++)
            {
                joints[i].setPending();
            }
        }

        private void UpdateGamePad()
        {
            KeyboardState state = Keyboard.GetState();
            MouseState mState = Mouse.GetState();

            _camera.Zoom = 2500 + mState.ScrollWheelValue;

            if (state.IsKeyDown(Keys.D1) || state.IsKeyDown(Keys.Down))
            {
                robot.Mechanism.Joints[0].Value += _STEP;
                setPending(0, robot.Mechanism.Joints);
            }
            if (state.IsKeyDown(Keys.D2) || state.IsKeyDown(Keys.Up))
            {

                robot.Mechanism.Joints[0].Value -= _STEP;
                setPending(0, robot.Mechanism.Joints);

            }

            if (state.IsKeyDown(Keys.D3) || state.IsKeyDown(Keys.Right))
            {
                robot.Mechanism.Joints[1].Value += _STEP;
                setPending(1, robot.Mechanism.Joints);
            }
            if (state.IsKeyDown(Keys.D4) || state.IsKeyDown(Keys.Left))
            {

                robot.Mechanism.Joints[1].Value -= _STEP;
                setPending(1, robot.Mechanism.Joints);

            }

            if (state.IsKeyDown(Keys.D5))
            {
                robot.Mechanism.Joints[2].Value += _STEP;
                setPending(2, robot.Mechanism.Joints);
            }
            if (state.IsKeyDown(Keys.D6))
            {
                robot.Mechanism.Joints[2].Value -= _STEP;
                setPending(2, robot.Mechanism.Joints);
            }

            if (state.IsKeyDown(Keys.D7))
            {
                robot.Mechanism.Joints[3].Value += _STEP;
                setPending(3, robot.Mechanism.Joints);
            }
            if (state.IsKeyDown(Keys.D8))
            {
                robot.Mechanism.Joints[3].Value -= _STEP;
                setPending(3, robot.Mechanism.Joints);
            }

            if (state.IsKeyDown(Keys.D9))
            {
                robot.Mechanism.Joints[4].Value += _STEP;
                setPending(4, robot.Mechanism.Joints);
            }
            if (state.IsKeyDown(Keys.D0))
            {
                robot.Mechanism.Joints[4].Value -= _STEP;
                setPending(4, robot.Mechanism.Joints);
            }

            if (reset)
            {
                oldMouseX = mState.X;
                oldMouseY = mState.Y;
                reset = false;
            }

            if (mState.MiddleButton == ButtonState.Released)
            {
                reset = true;
            }
            else
            {
                _camera.cameraAngleX += (oldMouseY - mState.Y);
                _camera.cameraAngleY += (oldMouseX - mState.X);

                oldMouseX = mState.X;
                oldMouseY = mState.Y;
            }

            if (state.IsKeyDown(Keys.F1))
            {
                origin[0] = (int)robot.Mechanism.Joints[0].Value;
                origin[1] = (int)robot.Mechanism.Joints[1].Value;
                origin[2] = (int)robot.Mechanism.Joints[2].Value;
                if (resultForm != null)
                {
                    resultForm.Origin = origin;
                }
            }
            if (state.IsKeyDown(Keys.F2))
            {
                dest[0] = (int)robot.Mechanism.Joints[0].Value;
                dest[1] = (int)robot.Mechanism.Joints[1].Value;
                dest[2] = (int)robot.Mechanism.Joints[2].Value;
                if (resultForm != null)
                {
                    resultForm.Dest = dest;
                }
            }
            if (state.IsKeyDown(Keys.F5))
            {
                if (flag == false)
                {
                    flag = true;
                    calc();
                }
            }
            else
            {
                flag = false;
            }

            if (state.IsKeyDown(Keys.F9))
            {
                if (flag == false)
                {
                    flag = true;
                    obs();
                }
            }
            else
            {
                flag = false;
            }
            aa();

        }
        private void obs()
        {
            Color[] colorData = new Color[360 * 360];
            if (resultForm == null)
            {
                resultForm = new ShowResultForm(360, 360);
                resultForm.Show();
                resultForm.graphicsDevice.Textures[0] = null;    
            }

            resultForm.Origin = origin;
            resultForm.Dest = dest;
            resultForm.CSpace.GetData<Color>(colorData);

            for (int i = 0; i < 360; i++)
            {
                robot.Mechanism.Joints[0].Value = i;
                robot.Mechanism.Joints[0].setPending();
                for (int j = 0; j < 360; j++)
                {
                    robot.Mechanism.Joints[1].Value = j;
                    robot.Mechanism.Joints[1].setPending();
                    if (scene.isColliding(robot.Mechanism))
                    {
                        colorData[(i)*360+j] = Color.Black;
                    }
                    else
                    {
                        colorData[(i) * 360 + j] = Color.White;
                    }
                    
                }
            }
            resultForm.GraphicsDevice.Textures[0] = null;
            resultForm.CSpace.SetData<Color>(colorData);
            
        }

        private void aa()
        {
            /*
            Color[] colorData = new Color[360 * 360];
            cSpace.GetData<Color>(colorData);

            
            int x = Math.Abs((int)robot.Mechanism.Joints[0].Value % 360);
            int y = Math.Abs((int)robot.Mechanism.Joints[1].Value % 360);
            colorData[x * 360 + y] = Color.Blue;

            GraphicsDevice.Textures[0] = null;
            cSpace.SetData<Color>(colorData);
             * */
        }
        private void calc()
        {
            controller = new NArticulatedPlanarController(robot);

            CObsSpace cObsSpace = new MechanismCObsSpace(robot.Mechanism, scene);

            // Inicializa par�metros
            int k = 50;

            CSpaceRRT tst = new CSpaceRRT(3, new int[] {360, 360, 360}, cObsSpace, k);

            Node originNode;
            Node destNode;

            // Chamada � fun��o do algoritmo
            tst.generatePath(origin, dest, out originNode, out destNode);


            Node previousNode = destNode;
            Node currentNode = destNode.aCameFrom;

            resultForm.ClearEdges();

            foreach (Edge edge in tst.goalTree.edgeList)
            {
                resultForm.AddEdge(edge.node1.p, edge.node2.p, Color.LimeGreen);
            }

            foreach (Edge edge in tst.startTree.edgeList)
            {
                resultForm.AddEdge(edge.node1.p, edge.node2.p, Color.Red);
            }

            controller.Clear();
            robot.Mechanism.Joints[0].Value = dest[0];
            robot.Mechanism.Joints[1].Value = dest[1];
            robot.Mechanism.Joints[2].Value = dest[2];
            //List<Vector2> list = new List<Vector2>();
            resultForm.AddPointToPath(dest, Color.Blue);
            while (currentNode != null)
            {
                //list.Add(new Vector2(currentNode.p[0], currentNode.p[1]));
                //resultForm.AddPointToPath(currentNode.p, Color.Blue);
                controller.AddPoint(currentNode.p);
                previousNode = currentNode;
                currentNode = currentNode.aCameFrom;
            }
            controller.running = true;

            
            Color[] colorData = new Color[360 * 360];
            resultForm.CSpace.GetData<Color>(colorData);
            /*
            foreach (Node node in tst.goalTree.nodeList)
            {
                int i = node.p[0];
                int j = node.p[1];
                colorData[i*360+j] = Color.Red;
            }

            foreach (Node node in tst.startTree.nodeList)
            {
                int i = node.p[0];
                int j = node.p[1];
                colorData[i * 360 + j] = Color.Red;
            }
             */
            resultForm.GraphicsDevice.Textures[0] = null;
            resultForm.CSpace.SetData<Color>(colorData);
            resultForm.Draw();

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (isColliding())
            {
                GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.Yellow);
            }
            else
            {
                GraphicsDevice.Clear(Microsoft.Xna.Framework.Graphics.Color.Black);
            }

            // TODO: Add your drawing code here
            base.Draw(gameTime);

            scene.Draw(gameTime);
            robot.Draw(gameTime);
            /*
            if (resultForm != null)
                resultForm.Draw(gameTime);
            */
            /*
             * 
            Matrix[] transforms = new Matrix[linkModel.Bones.Count];
            linkModel.CopyBoneTransformsTo(transforms);
            
            foreach (OrientedBoundingBox Bb in mechanism.BoundingBoxList)
            {
                
                BoundingBoxRenderer.Render(Bb, transforms[1], graphics.GraphicsDevice, _camera.View, _camera.Projection,
                    Color.Red);
                 
            }

            foreach (OrientedBoundingBox Bb in scene.BoundingBoxList)
            {
                
                BoundingBoxRenderer.Render(Bb, transforms[1], graphics.GraphicsDevice, _camera.View, _camera.Projection,
                    Color.Red);
                
            }
            */
        }

    }
}