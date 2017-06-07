using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ShootCube.World.Chunk.Model;
using static ShootCube.Global.Globals;

namespace ShootCube.Global
{
    public class Camera
    {
        public static Vector3 Referencevector = new Vector3(0, 0, -1);

        public static Matrix View, Projection;

        public static Vector3 CameraPosition;
        public static Vector3 Direction;

        public static Orientation CameraOrientation;
        public static Vector3 DirectionStationary;

        private static int _oldX, _oldY;

        public Camera(float dpi, float velocity)
        {
            MouseSensity = dpi;
            Velocity = velocity;

            Projection =
                Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f,
                    1024);
            ;

            CameraPosition = new Vector3(16 * ChunkManager.Width / 2, 65, 16 * ChunkManager.Depth / 2);

            _oldX = GraphicsDevice.Viewport.Width / 2;
            _oldY = GraphicsDevice.Viewport.Height / 2;
        }

        public static float Yaw { get; private set; }
        public static float Pitch { get; private set; }

        public static float MouseSensity { get; set; }
        public static float Velocity { get; set; }

        public static BoundingFrustum ViewFrustum { get; private set; }
        public static Ray MouseRay { get; private set; }

        public static void Update()
        {
            float dX = Mouse.GetState().X - _oldX;
            float dY = Mouse.GetState().Y - _oldY;

            Pitch += -MouseSensity * dY;
            Yaw += -MouseSensity * dX;

            Pitch = MathHelper.Clamp(Pitch, -1.5f, 1.5f);

            CalculateViewMatrix();

            try
            {
                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            }
            catch
            {
            }

            ViewFrustum = new BoundingFrustum(View * Projection);

            // RAY CALCULATION
            var nearPlane =
                GraphicsDevice.Viewport.Unproject(
                    new Vector3(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2, 0), Projection,
                    View, Matrix.Identity);
            var farPlane =
                GraphicsDevice.Viewport.Unproject(
                    new Vector3(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2, 1), Projection,
                    View, Matrix.Identity);

            var rayDirection = farPlane - nearPlane;
            rayDirection /= rayDirection.Length();

            MouseRay = new Ray(nearPlane, rayDirection);

            // ORIENTATION

            if (DirectionStationary.X == 0 && DirectionStationary.Y == 0 && DirectionStationary.Z == 1)
                CameraOrientation = Orientation.North;

            if (DirectionStationary.X == -1 && DirectionStationary.Y == 0 && DirectionStationary.Z == 0)
                CameraOrientation = Orientation.East;

            if (DirectionStationary.X == 0 && DirectionStationary.Y == 0 && DirectionStationary.Z == -1)
                CameraOrientation = Orientation.South;

            if (DirectionStationary.X == 1 && DirectionStationary.Y == 0 && DirectionStationary.Z == 0)
                CameraOrientation = Orientation.West;


            if (DirectionStationary.X == -1 && DirectionStationary.Y == 0 && DirectionStationary.Z == 1)
                CameraOrientation = Orientation.NorthEast;

            if (DirectionStationary.X == 1 && DirectionStationary.Y == 0 && DirectionStationary.Z == 1)
                CameraOrientation = Orientation.NorthWest;

            if (DirectionStationary.X == -1 && DirectionStationary.Y == 0 && DirectionStationary.Z == -1)
                CameraOrientation = Orientation.SouthEast;

            if (DirectionStationary.X == 1 && DirectionStationary.Y == 0 && DirectionStationary.Z == -1)
                CameraOrientation = Orientation.SouthWest;

            if (DirectionStationary.X == 0 && DirectionStationary.Y == -1 && DirectionStationary.Z == 0)
                CameraOrientation = Orientation.Down;
            if (DirectionStationary.X == 0 && DirectionStationary.Y == 1 && DirectionStationary.Z == 0)
                CameraOrientation = Orientation.Up;
        }

        public static void Move(Vector3 direction)
        {
            var rotation = Matrix.CreateRotationY(Yaw);
            var transformed = Vector3.Transform(direction, rotation);

            transformed *= Velocity;

            if (!IsColliding(new Vector3(CameraPosition.X + transformed.X + (transformed.X < 0 ? -.5f : .5f),
                CameraPosition.Y, CameraPosition.Z)))
                CameraPosition.X += transformed.X;
            if (!IsColliding(new Vector3(CameraPosition.X, CameraPosition.Y,
                CameraPosition.Z + transformed.Z + (transformed.Z < 0 ? -.5f : .5f))))
                CameraPosition.Z += transformed.Z;
            if (!IsColliding(new Vector3(CameraPosition.X,
                CameraPosition.Y + transformed.Y + (transformed.Y < 0 ? -.5f : .5f), CameraPosition.Z)))
                CameraPosition.Y += transformed.Y;
        }

        public static bool IsColliding(Vector3 to)
        {
            foreach (var chunk in ChunkManager.CurrentChunk.Neighbours)
            {
                if (chunk == null)
                    continue;
                for (var i = 0; i < chunk.BoundingBoxes.Count; i++)
                    if (chunk.BoundingBoxes[i].Contains(to) == ContainmentType.Contains)
                        return true;
            }

            return false;
        }

        private static void CalculateViewMatrix()
        {
            var rotation = Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
            var transformed = Vector3.Transform(Referencevector, rotation);

            DirectionStationary = transformed;
            DirectionStationary = new Vector3((float) Math.Round(DirectionStationary.X),
                (float) Math.Round(DirectionStationary.Y), (float) Math.Round(DirectionStationary.Z));

            Direction = CameraPosition + transformed;

            View = Matrix.CreateLookAt(CameraPosition, Direction, Vector3.Up);
        }
    }
}