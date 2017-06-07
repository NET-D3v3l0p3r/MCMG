using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShootCube.Declaration
{
    public struct VptvDeclaration : IVertexType
    {
        public static VertexDeclaration vertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
        );

        public VertexDeclaration VertexDeclaration => vertexDeclaration;

        public Vector3 Position { get; set; }
        public Vector2 TextureCoordinate { get; set; }
        public Vector2 Value { get; set; }

        public VptvDeclaration(Vector3 pos, Vector2 tex, float v)
            : this()
        {
            Position = pos;
            TextureCoordinate = tex;
            Value = new Vector2(v);
        }
    }
}