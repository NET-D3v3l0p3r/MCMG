using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShootCube.Global;
using System.Linq;
using ShootCube.World.Chunk.Model;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ShootCube
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Window : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Window()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();

            IsFixedTimeStep = false;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Globals.GraphicsDevice = GraphicsDevice;
            Globals.GraphicsDeviceManager = graphics;
            Globals.Content = Content;

            Globals.Initialize();


            new ChunkManager(25, 25);
            new Camera(0.007f, .5f);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            ChunkManager.Start();
            sw.Stop();

            Console.WriteLine(sw.Elapsed.TotalSeconds + " s!");
            activeChunk = ChunkManager.Chunks[0];


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        bool pressed;
        Chunk activeChunk;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!this.IsActive)
                return;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            for (int i = 0; i < ChunkManager.Chunks.Length; i++)
            {
                if (ChunkManager.Chunks[i].ChunkBox.Contains(Camera.CameraPosition) == ContainmentType.Contains)
                {
                    activeChunk = ChunkManager.Chunks[i];
                    break;
                }
            }


            if (Keyboard.GetState().IsKeyDown(Keys.W))
                Camera.Move(new Vector3(0, 0, -1), activeChunk.BoundingBoxes.ToArray());
            if (Keyboard.GetState().IsKeyDown(Keys.S))
                Camera.Move(new Vector3(0, 0, 1), activeChunk.BoundingBoxes.ToArray());
            if (Keyboard.GetState().IsKeyDown(Keys.D))
                Camera.Move(new Vector3(1, 0, 0), activeChunk.BoundingBoxes.ToArray());
            if (Keyboard.GetState().IsKeyDown(Keys.A))
                Camera.Move(new Vector3(-1, 0, 0), activeChunk.BoundingBoxes.ToArray());

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                Camera.Move(new Vector3(0, -1, 0), activeChunk.BoundingBoxes.ToArray());
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
                Camera.Move(new Vector3(0, 1, 0), activeChunk.BoundingBoxes.ToArray());



            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !pressed)
            {
                pressed = true;
                for (int j = 0; j < activeChunk.BoundingBoxes.Count; j++)
                {
                    var result = Camera.MouseRay.Intersects(activeChunk.BoundingBoxes[j]);
                    if (result.HasValue && result.Value < 3)
                    {
                        activeChunk.RemoveCube(activeChunk.BoundingBoxes[j]);
                        break;
                    }
                }


            }
            if (Mouse.GetState().LeftButton == ButtonState.Released)
                pressed = false;

            Camera.Update();
            ChunkManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            Globals.Effect.View = Camera.View;
            Globals.Effect.Projection = Camera.Projection;
            Globals.Effect.World = Matrix.Identity;
            Globals.Effect.TextureEnabled = true;
            Globals.Effect.Texture = ChunkManager.TextureAtlas;
            //Globals.Effect.PreferPerPixelLighting = true;
            //Globals.Effect.EnableDefaultLighting();
            Globals.Effect.CurrentTechnique.Passes[0].Apply();

            Globals.GraphicsDevice.Indices = Globals.IndexBuffer;
            ChunkManager.Render();


            //for (int i = 0; i < ChunkManager.Chunks.Length; i++)
            //{
            //    if (ChunkManager.Chunks[i].ChunkBox.Contains(Camera.CameraPosition) == ContainmentType.Contains)
            //    {
            //        for (int j = 0; j < ChunkManager.Chunks[i].BoundingBoxes.Count; j++)
            //        {
            //            BoundingBoxRenderer.Render(ChunkManager.Chunks[i].BoundingBoxes[j], GraphicsDevice, Camera.View, Camera.Projection, Color.Red);
            //        }
            //    }
            //}


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
