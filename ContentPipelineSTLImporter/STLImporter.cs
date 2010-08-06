using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

// TODO: replace this with the type you want to import.
using TImport = System.String;

namespace SceneRendering
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".stl", DisplayName = "STLImporter", DefaultProcessor = "ModelProcessor")]
    public class STLImporter : ContentImporter<NodeContent>
    {

        // The current mesh being constructed
        private MeshBuilder meshBuilder;

        public override NodeContent Import(string filename, ContentImporterContext context)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            BinaryReader sr = new BinaryReader(fs);
            
            NodeContent root = new NodeContent();
            sr.ReadBytes(80);
            int faceCount = sr.ReadInt32();
            /***/
            meshBuilder = MeshBuilder.StartMesh("");
            meshBuilder.SwapWindingOrder = true;
            /***/
            int[] verticesIndex = new int[faceCount*3];                
            for (int i = 0; i < faceCount; i++)
            {                
                
                Vector3 normal = new Vector3(sr.ReadSingle(),
                    sr.ReadSingle(), sr.ReadSingle());
                
                for (int j = i*3; j < (i*3)+3; j++)
                {
                    Vector3 vertice = new Vector3(sr.ReadSingle(),
                        sr.ReadSingle(), sr.ReadSingle());

                    verticesIndex[j] = meshBuilder.CreatePosition(vertice);                    
                }
                sr.ReadInt16();
            }
            for (int i = 0; i < faceCount; i++)
            {
                for (int j = i*3; j < (i*3)+3; j++)
                {
                    meshBuilder.AddTriangleVertex(verticesIndex[j]);
                }

            }

            MeshContent mesh = meshBuilder.FinishMesh();
            root.Children.Add(mesh);


                


           return root;
        }
    }
}
