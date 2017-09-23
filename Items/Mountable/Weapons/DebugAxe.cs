using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShootCube.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShootCube.Items.Mountable.Weapons
{
    public class DebugAxe
    {
        public Model AxeModel;
        public Matrix[] _transformations;

        public int Activation = 0;

        public DebugAxe()
        {
            AxeModel = Global.Globals.Content.Load<Model>(@"Cube Axe");

            _transformations = new Matrix[AxeModel.Bones.Count];
            AxeModel.CopyAbsoluteBoneTransformsTo(_transformations);

        }

        public void Render(GameTime gTime)
        {
            foreach (var mesh in AxeModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = Camera.View;
                    effect.Projection = Camera.Projection;
                    effect.World = _transformations[mesh.ParentBone.Index]*  Matrix.CreateScale(0.0005f)
                        * Matrix.CreateRotationY(2.15f) * Matrix.CreateRotationX((float)Math.Sin(gTime.TotalGameTime.TotalSeconds * Activation)) *
                        Matrix.CreateTranslation(.1f,-.05f,-.45f) * Matrix.Invert(Camera.View);
 
                   
                    effect.CurrentTechnique.Passes[0].Apply();
                }
                mesh.Draw();
            }
        }
    }
}
