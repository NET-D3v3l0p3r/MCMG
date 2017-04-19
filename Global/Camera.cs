using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ShootCube.World.Chunk.Model;
using Microsoft.Xna.Framework.Input;

namespace ShootCube.Global
{
    public class Camera
    {
        public static Vector3 REFERENCEVECTOR = new Vector3(0, 0, -1);

        public static float Yaw { get; private set; }
        public static float Pitch { get; private set; }

        public static float MouseSensity { get; set; }
        public static float Velocity { get; set; }

        public static Matrix View, Projection;

        public static Vector3 CameraPosition { get; set; }
        public static Vector3 Direction { get; private set; }

        public static BoundingFrustum ViewFrustum { get; private set; }
        public static Ray MouseRay { get; private set; }

        private static int oldX, oldY;
        public Camera(float dpi, float velocity)
        {
            MouseSensity = dpi;
            Velocity = velocity;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Globals.GraphicsDevice.Viewport.AspectRatio, 0.1f, 1000.0f); ;

            CameraPosition = new Vector3(16 * ChunkManager.Width / 2, 256, 16 * ChunkManager.Depth / 2);

            oldX = Globals.GraphicsDevice.Viewport.Width / 2;
            oldY = Globals.GraphicsDevice.Viewport.Height / 2;
        }

        public static void Update()
        {
            float dX = Mouse.GetState().X - oldX;
            float dY = Mouse.GetState().Y - oldY;

            Pitch += -MouseSensity * dY;
            Yaw += -MouseSensity * dX;

            Pitch = MathHelper.Clamp(Pitch, -1.5f, 1.5f);

            calculateViewMatrix();

            try
            {
                Mouse.SetPosition(Globals.GraphicsDevice.Viewport.Width / 2, Globals.GraphicsDevice.Viewport.Height / 2);
            }
            catch { }

            ViewFrustum = new BoundingFrustum(View * Projection);

            // RAY CALCULATION
            Vector3 nearPlane = Globals.GraphicsDevice.Viewport.Unproject(new Vector3(Globals.GraphicsDevice.Viewport.Width / 2, Globals.GraphicsDevice.Viewport.Height / 2, 0), Projection, View, Matrix.Identity);
            Vector3 farPlane = Globals.GraphicsDevice.Viewport.Unproject(new Vector3(Globals.GraphicsDevice.Viewport.Width / 2, Globals.GraphicsDevice.Viewport.Height / 2, 1), Projection, View, Matrix.Identity);

            Vector3 rayDirection = farPlane - nearPlane;
            MouseRay = new Ray(nearPlane, rayDirection);
        }

        public static void Move(Vector3 direction, BoundingBox[] boundingBoxes = null)
        {
            Matrix rotation = Matrix.CreateRotationY(Yaw);
            Vector3 transformed = Vector3.Transform(direction, rotation);

            if (boundingBoxes != null)
            {
                Vector3 temp = CameraPosition + (transformed * Velocity);
                for (int i = 0; i < boundingBoxes.Length; i++)
                {
                    if (boundingBoxes[i].Contains(temp) == ContainmentType.Intersects)
                        return;
                }

            }
            CameraPosition += transformed * Velocity;
        }

        private static void calculateViewMatrix()
        {

            Matrix rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
            Vector3 transformed = Vector3.Transform(REFERENCEVECTOR, rotation);
            Direction = CameraPosition + transformed;

            View = Matrix.CreateLookAt(CameraPosition, Direction, Vector3.Up);

        }
    }
}
