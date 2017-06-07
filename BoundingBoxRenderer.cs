using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootCube
{
    public static class BoundingBoxRenderer
    {
        /// <summary>
        ///     Renders the bounding box for debugging purposes.
        /// </summary>
        /// <param name="box">The box to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use drawing the lines of the box.</param>
        public static void Render(
            BoundingBox box,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color color)
        {
            if (_effect == null)
            {
                _effect = new BasicEffect(graphicsDevice);
                _effect.VertexColorEnabled = true;
                _effect.LightingEnabled = false;
            }

            var corners = box.GetCorners();
            for (var i = 0; i < 8; i++)
            {
                Verts[i].Position = corners[i];
                Verts[i].Color = color;
            }

            _effect.View = view;
            _effect.Projection = projection;

            foreach (var pass in _effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    Verts,
                    0,
                    8,
                    Indices,
                    0,
                    Indices.Length / 2);
            }
        }


        public static BoundingBox UpdateBoundingBox(Model model, Matrix worldTransform)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (var mesh in model.Meshes)
            foreach (var meshPart in mesh.MeshParts)
            {
                // Vertex buffer parameters
                var vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                var vertexBufferSize = meshPart.NumVertices * vertexStride;

                // Get vertex data as float
                var vertexData = new float[vertexBufferSize / sizeof(float)];
                meshPart.VertexBuffer.GetData(vertexData);

                // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                for (var i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                {
                    var transformedPosition =
                        Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]),
                            worldTransform);

                    min = Vector3.Min(min, transformedPosition);
                    max = Vector3.Max(max, transformedPosition);
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }

        public static BoundingBox CalculateBoundingBox(Model model, Matrix worldTransform)
        {
            // Create variables to hold min and max xyz values for the model. Initialise them to extremes
            var modelMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var modelMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (var mesh in model.Meshes)
            {
                //Create variables to hold min and max xyz values for the mesh. Initialise them to extremes
                var meshMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                var meshMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                // There may be multiple parts in a mesh (different materials etc.) so loop through each
                foreach (var part in mesh.MeshParts)
                {
                    // The stride is how big, in bytes, one vertex is in the vertex buffer
                    // We have to use this as we do not know the make up of the vertex
                    var stride = part.VertexBuffer.VertexDeclaration.VertexStride;

                    var vertexData = new byte[stride * part.NumVertices];
                    part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices,
                        1); // fixed 13/4/11

                    // Find minimum and maximum xyz values for this mesh part
                    // We know the position will always be the first 3 float values of the vertex data
                    var vertPosition = new Vector3();
                    for (var ndx = 0; ndx < vertexData.Length; ndx += stride)
                    {
                        vertPosition.X = BitConverter.ToSingle(vertexData, ndx);
                        vertPosition.Y = BitConverter.ToSingle(vertexData, ndx + sizeof(float));
                        vertPosition.Z = BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2);

                        // update our running values from this vertex
                        meshMin = Vector3.Min(meshMin, vertPosition);
                        meshMax = Vector3.Max(meshMax, vertPosition);
                    }
                }

                // transform by mesh bone transforms
                meshMin = Vector3.Transform(meshMin, worldTransform);
                meshMax = Vector3.Transform(meshMax, worldTransform);

                // Expand model extents by the ones from this mesh
                modelMin = Vector3.Min(modelMin, meshMin);
                modelMax = Vector3.Max(modelMax, meshMax);
            }


            // Create and return the model bounding box
            return new BoundingBox(modelMin, modelMax);
        }

        #region Fields

        private static readonly VertexPositionColor[] Verts = new VertexPositionColor[8];

        private static readonly short[] Indices =
        {
            0, 1,
            1, 2,
            2, 3,
            3, 0,
            0, 4,
            1, 5,
            2, 6,
            3, 7,
            4, 5,
            5, 6,
            6, 7,
            7, 4
        };

        private static BasicEffect _effect;
        private static VertexDeclaration _vertDecl;

        #endregion
    }
}