using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
    public partial class ShowResultForm : Form
    {

        private SpriteBatch spriteBatch;
        private Texture2D cSpace;
        private Texture2D pixel;

        private int width;
        private int height;
        public List<Vector2> edges;
        
        private BasicEffect effect;
        private Matrix projectionMatrix;

        private List<VertexPositionColor> vertices;
        private List<VertexPositionColor> path;
        private VertexPositionColor[] origin;
        private VertexPositionColor[] dest;
        public void ClearEdges()
        {
            vertices.Clear();
            path.Clear();
        }
        public void AddEdge(int[] p1, int[] p2, Color color)
        {
            vertices.Add(new VertexPositionColor(new Vector3(p1[1], p1[0], 0), color));
            vertices.Add(new VertexPositionColor(new Vector3(p2[1], p2[0], 0), color));
        }

        public void AddPointToPath(int[] p1, Color color)
        {
            return;
            color.A = 100;
            vertices.Add(new VertexPositionColor(new Vector3(p1[1], p1[0], -0.5f), color));
        }

        public int[] Origin
        {
            set {
                origin = new VertexPositionColor[1];
                origin[0] = new VertexPositionColor(new Vector3(value[1], value[0], 0), Color.Blue);
            }
        }
        
        public int[] Dest
        {
            set
            {
                dest = new VertexPositionColor[1];
                dest[0] = new VertexPositionColor(new Vector3(value[1]+180, value[0], 0), Color.Blue);
            }
        }

        public GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }
        public Texture2D CSpace
        {
            get { return cSpace; }
        }

        PresentationParameters parameters;

        public ShowResultForm(int width, int height)
        {
            this.width = width;
            this.height = height;
            edges = new List<Vector2>();

            InitializeComponent();        
        }

     
        public void Draw()
        {
            spriteBatch.Begin();

            spriteBatch.Draw(cSpace, new Vector2(0, 0), Color.White);
            /*
            for (int i = 1; i < edges.Count; i++)
            {
                Vector2 vector1 = (Vector2)edges[i - 1];
                Vector2 vector2 = (Vector2)edges[i];

                // calculate the distance between the two vectors
                float distance = Vector2.Distance(vector1, vector2);

                // calculate the angle between the two vectors
                float angle = (float)Math.Atan2((double)(vector2.Y - vector1.Y),
                    (double)(vector2.X - vector1.X));

                // stretch the pixel between the two vectors
                spriteBatch.Draw(pixel,
                    vector1,
                    null,
                    Color.Green,
                    angle,
                    Vector2.Zero,
                    new Vector2(distance, 1),
                    SpriteEffects.None,
                    0);
            }
            */

            spriteBatch.End();

            effect.Begin();
            foreach (EffectPass p in effect.CurrentTechnique.Passes)
            {
                p.Begin();
                if (origin != null)
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList, origin, 0, 1);
                }
                if (dest != null)
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList, dest, 0, 1);
                }
                if (vertices.Count >= 2)
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
                }
                if (path.Count >= 2)
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices.ToArray(), 0, vertices.Count -1);
                }

                
                p.End();
            }
            effect.End();

            GraphicsDevice.Present(this.pnlPanel.Handle);
        }

        private void pnlPanel_Paint(object sender, PaintEventArgs e)
        {
            Draw();
        }

        private void ShowResultForm_Load(object sender, EventArgs e)
        {
            parameters = new PresentationParameters();
            parameters.BackBufferWidth = Math.Max(width, 1);
            parameters.BackBufferHeight = Math.Max(height, 1);
            parameters.BackBufferFormat = SurfaceFormat.Color;

            parameters.EnableAutoDepthStencil = true;
            parameters.AutoDepthStencilFormat = DepthFormat.Depth24;

            graphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter,
                                                DeviceType.Hardware,
                                                pnlPanel.IsHandleCreated ? pnlPanel.Handle : IntPtr.Zero,
                                                parameters);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            cSpace = new Texture2D(GraphicsDevice, width, height, 0, TextureUsage.None, SurfaceFormat.Color);
            pixel = new Texture2D(GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] pixels = new Color[1];
            pixels[0] = Color.Green;
            pixel.SetData<Color>(pixels);

            effect = new BasicEffect(GraphicsDevice, null);
            //vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);

            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                                                                   GraphicsDevice.Viewport.Height, 0, 0, 1f);

            effect.World = Matrix.Identity;
            effect.View = Matrix.Identity;
            effect.Projection = projectionMatrix;
            effect.VertexColorEnabled = true;

            vertices = new List<VertexPositionColor>();
            path = new List<VertexPositionColor>();
        }
    }
}
