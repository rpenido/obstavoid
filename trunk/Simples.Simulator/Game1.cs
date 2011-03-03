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
using Simples.Camera;
using Simples.Mechanisms;
using Simples.Mechanisms.SampleBased;
using Simples.PathPlan.SamplesBased;
using Simples.PathPlan.SamplesBased.RRT;
using System.Runtime.InteropServices;
using Simples.Mechanisms.NArticulatedPlanar;
using Simples.Mechanisms.Draw;

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

        private OrbitCamera camera;

        private static float _STEP = 1;

        private Mechanism robot;
        private DrawableMechanism  drawableRobot;

        private MechanismEnviroment enviroment;
        private DrawableEnviroment drawableEnviroment;

        bool reset = true;
        private float oldMouseX, oldMouseY;


        private double[] origin;
        private double[] dest;

        bool flagOptmize;
        //OctreeNode oct;

        private Model test;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            graphics.PreferredBackBufferHeight = 600;            
            graphics.PreferredBackBufferWidth = 800;

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


        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Matrix world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            //Matrix world = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ);
            camera = new OrbitCamera();
           
            Model linkModel = Content.Load<Model>("link");
            Vector3 minBound = new Vector3(-10, -10, -5);
            Vector3 maxBound = new Vector3(120, 10, 5);

            robot = NArticulatedPlanarMechanism.Create(linkModel, new Vector3(100, 0, 0), minBound,
                maxBound, 6, world);
            drawableRobot = new DrawableMechanism(this, robot, camera);
            Components.Add(drawableRobot);

        
            Model sceneModel = Content.Load<Model>("scene");
            
            enviroment = new MechanismEnviroment(sceneModel);
            drawableEnviroment = new DrawableEnviroment(this, enviroment, camera);
            Components.Add(drawableEnviroment);
           
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
            return enviroment.IsColliding(robot);
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

            camera.Zoom = 2500 + mState.ScrollWheelValue;
            if (state.IsKeyDown(Keys.Home))
            {
                camera.cameraAngleX = 0;
                camera.cameraAngleY = 0;
            }

            if (state.IsKeyDown(Keys.D1) || state.IsKeyDown(Keys.Down))
            {
                robot.Joints[0].Value += _STEP;
                setPending(0, robot.Joints);
            }
            if (state.IsKeyDown(Keys.D2) || state.IsKeyDown(Keys.Up))
            {

                robot.Joints[0].Value -= _STEP;
                setPending(0, robot.Joints);

            }

            if (state.IsKeyDown(Keys.D3) || state.IsKeyDown(Keys.Right))
            {
                robot.Joints[1].Value += _STEP;
                setPending(1, robot.Joints);
            }
            if (state.IsKeyDown(Keys.D4) || state.IsKeyDown(Keys.Left))
            {

                robot.Joints[1].Value -= _STEP;
                setPending(1, robot.Joints);

            }

            if (state.IsKeyDown(Keys.D5))
            {
                robot.Joints[2].Value += _STEP;
                setPending(2, robot.Joints);
            }
            if (state.IsKeyDown(Keys.D6))
            {
                robot.Joints[2].Value -= _STEP;
                setPending(2, robot.Joints);
            }
            
            if (state.IsKeyDown(Keys.D7))
            {
                robot.Joints[3].Value += _STEP;
                setPending(3, robot.Joints);
            }
            if (state.IsKeyDown(Keys.D8))
            {
                robot.Joints[3].Value -= _STEP;
                setPending(3, robot.Joints);
            }
            
            if (state.IsKeyDown(Keys.D9))
            {
                robot.Joints[4].Value += _STEP;
                setPending(4, robot.Joints);
            }
            if (state.IsKeyDown(Keys.D0))
            {
                robot.Joints[4].Value -= _STEP;
                setPending(4, robot.Joints);
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
                camera.cameraAngleX += (oldMouseY - mState.Y);
                camera.cameraAngleY += (oldMouseX - mState.X);

                oldMouseX = mState.X;
                oldMouseY = mState.Y;
            }
            
            if (state.IsKeyDown(Keys.F1))
            {
                origin = new double[robot.Joints.Count];
                for (int i = 0; i < robot.Joints.Count; i++)
                {
                    origin[i] = robot.Joints[i].Value;
                }
                if (resultForm != null)
                {
                    resultForm.Origin = origin;
                }
            }

            if (state.IsKeyDown(Keys.F2))
            {
                dest = new double[robot.Joints.Count];
                for (int i = 0; i < robot.Joints.Count; i++)
                {
                    dest[i] = robot.Joints[i].Value;
                }
            }
            
            if (state.IsKeyDown(Keys.F3))
            {
                if (flagOptmize == false)
                {
                    flagOptmize = true;
                    optmize();
                    
                }
            }
            else
            {
                flagOptmize = false;
            }
            /*
           
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
            */

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
                robot.Joints[0].Value = i;
                robot.Joints[0].setPending();
                for (int j = 0; j < 360; j++)
                {
                    robot.Joints[1].Value = j;
                    robot.Joints[1].setPending();
                    if (enviroment.IsColliding(robot))
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
        private void optmize()
        {
            int threadCount = 7;
            MechanismCSpace[] cMechanismPool = new MechanismCSpace[threadCount];
            CSpace[] cSpacePool = new CSpace[threadCount];
                
                
            
            for (int i = 0; i < threadCount; i++)
            {
                cMechanismPool[i] = new MechanismCSpace(robot, enviroment);
                cSpacePool[i] = cMechanismPool[i].CSpace;

            }
            
            RRTOptimizer opt = new RRTOptimizer(origin, dest, cSpacePool, threadCount);
            controller = new NArticulatedPlanarController(robot);
            OptmizeForm form = new OptmizeForm(opt, controller);
            form.Show();
        }

        private void move()
        {
            controller = new NArticulatedPlanarController(robot);
            controller.Clear();
            robot.Joints[0].Value = dest[0];
            robot.Joints[1].Value = dest[1];
            robot.Joints[2].Value = dest[2];
            controller.AddPoint(new double[] {90, 328, 24});
            
            controller.AddPoint(new double[] { 121.16, 316.06, 19.78 });
            controller.AddPoint(new double[] { 143, 233.87, 127.61 });
            controller.AddPoint(new double[] { 124, 191, 99 });
            controller.AddPoint(new double[] { 116, 184, 61 });
            controller.AddPoint(new double[] { 64, 287, 50 });
            controller.AddPoint(new double[] { 62, 292, 48 });
            controller.AddPoint(new double[] { 61, 293, 48 });
            controller.AddPoint(new double[] { 61, 295, 47 });
            controller.AddPoint(new double[] { 34, 334, 48 });
            
            controller.AddPoint(new double[] { 90, 328, 24 });
            controller.AddPoint(new double[] { 116.82, 272.72, 048.88 });
            controller.AddPoint(new double[] { 155.69, 192.62, 084.94 });
            controller.AddPoint(new double[] { 112, 171, 84 });
            controller.AddPoint(new double[] { 46, 323, 55 });
            controller.AddPoint(new double[] { 46, 324, 55 });
            controller.AddPoint(new double[] { 41, 333, 52});
            controller.AddPoint(new double[] { 41, 334, 53 });
            controller.AddPoint(new double[] { 34, 334, 48 });
            
            
            controller.running = true;

       
        }
   
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (isColliding())
            {
                GraphicsDevice.Clear(Color.Yellow);
            }
            else
            {
                GraphicsDevice.Clear(Color.BlueViolet);
            }

            // TODO: Add your drawing code here
            base.Draw(gameTime);
            //scene.Draw(gameTime);
            //drawableRobot.Draw(gameTime, GraphicsDevice);
            /*
            foreach (OrientedBoundingBox obb in enviroment.BoundingBoxList)
            {
                obb.Draw(GraphicsDevice, _camera.Projection, _camera.View);
            }
            */
            //oct.Draw(this, _camera.Projection, _camera.View);

            /*
            Matrix[] transforms = new Matrix[teste.Bones.Count];
            teste.CopyBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in teste.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.View = _camera.View;

                    effect.Projection = _camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index];
                }
                mesh.Draw();
            }        
            */
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
