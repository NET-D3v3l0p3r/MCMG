using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;
using System.Linq;
using ShootCube.World.Chunk.Model;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using ShootCube.Sky;
using ShootCube.Global.Input;


namespace ShootCube.Declaration
{
    public struct VPTVDeclaration : IVertexType
    {
        public static VertexDeclaration vertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 2)
        );

        public VertexDeclaration VertexDeclaration
        {
            get { return vertexDeclaration; }
        }

        public Vector3 Position { get; set; }
        public Vector2 TextureCoordinate { get; set; }
        public Vector2 AnimationCoordinate { get; set; }
        public Vector2 Value { get; set; }

        public VPTVDeclaration(Vector3 pos, Vector2 tex, Vector2 anim, float v)
            :this()
        {

            Position = pos;
            TextureCoordinate = tex;
            AnimationCoordinate = anim;
            Value = new Vector2(v);
        }

    }
}
