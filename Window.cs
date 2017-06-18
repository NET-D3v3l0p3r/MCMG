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
using ShootCube.World.Chunk;
using ShootCube.Global.Picking;
using ShootCube.Dynamics;
using ShootCube.Global.Utilities;
using ShootCube.Dynamics.Ambient;

namespace ShootCube
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Window : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        FpsCounter fpsCounter = new FpsCounter();
        SpriteFont debugFont;

        //WeatherManager weatherManager;

        public Sky.Sky SkyEnvironment;
        public LightManager LightManager;



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

            LightManager = new LightManager();
            new Camera(0.007f, .35f);
            new ChunkManager(15, 15);
            ChunkManager.Start();

            //weatherManager = new WeatherManager();

            ChunkManager.Cubes[56, 32, 32] = 6;
            ChunkManager.Cubes[57, 32, 32] = 6;

            ChunkManager.Run();

            KeyboardControl.AddKey(new Key(Keys.W, new Action(() =>
            {
                Camera.Move(new Vector3(0, 0, -1));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.A, new Action(() =>
            {
                Camera.Move(new Vector3(-1, 0, 0));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.S, new Action(() =>
            {
                Camera.Move(new Vector3(0, 0, 1));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.D, new Action(() =>
            {
                Camera.Move(new Vector3(1, 0, 0));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.Space, new Action(() =>
            {
                Camera.Move(new Vector3(0, 1, 0));
            }), false));
            KeyboardControl.AddKey(new Key(Keys.LeftShift, new Action(() =>
            {
                Camera.Move(new Vector3(0, -1, 0));
            }), false));

            KeyboardControl.AddKey(new Key(Keys.Escape, new Action(() =>
            {
               Exit();
            }), true));

            WeaponStatistics.Initialize();

            KeyboardControl.AddKey(new Key(Keys.X, new Action(() =>
            {
                LightSource source = new LightSource(Camera.CameraPosition, 13);
                source.Emit();
            }), true));
            KeyboardControl.AddKey(new Key(Keys.Y, new Action(() =>
            {
                
            }), true));


            debugFont = Content.Load<SpriteFont>("debug");

            SkyEnvironment = new ShootCube.Sky.Sky(1);



            MouseControl.AddLeftAction(new Action(() =>
            {
                var p = ChunkManager.Pick(5);
                p?.Chunk.DestroyCube(p.Value, WeaponStatistics.Utilities.Pickaxe_diamond);
            }), false, 55);


            MouseControl.AddRightAction(new Action(() =>
            {
                var p = ChunkManager.Pick(5);
                if (!p.HasValue)
                    return;

                var min = p.Value.BoundingBox.Min;
                var pos = new Vector3(min.X, p.Value.BoundingBox.Max.Y + 1, min.Z);

            }), false, 250);



            // TEST



        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        Profile? general;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!this.IsActive)
                return;

            MouseControl.Update(gameTime);
            KeyboardControl.Update();
            Camera.Update();
            ChunkManager.Update(gameTime);

            general = ChunkManager.Pick(5.0f);


            //weatherManager.Update();
            SkyEnvironment.Update(gameTime);
            


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            fpsCounter.Start(gameTime);
            GraphicsDevice.Clear(Color.Black );

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GraphicsDevice.Indices = Globals.IndexBuffer;


            // CHUNKS

            //Globals.Effect.View = Camera.View;
            //Globals.Effect.Projection = Camera.Projection;
            //Globals.Effect.World = Matrix.Identity;
            //Globals.Effect.TextureEnabled = true;
            //Globals.Effect.Texture = ChunkManager.TextureAtlas;

            Globals.Effect.Parameters["View"].SetValue(Camera.View);
            Globals.Effect.Parameters["World"].SetValue(Matrix.Identity);
            Globals.Effect.Parameters["Projection"].SetValue(Camera.Projection);
            Globals.Effect.Parameters["TextureAtlas"].SetValue(ChunkManager.TextureAtlas);


            Globals.Effect.CurrentTechnique.Passes[0].Apply();
            ChunkManager.Render();


            // TODO: Add your drawing code here

            //weatherManager.Render();
            SkyEnvironment.Render();
            //source?.DebugDrawLight();

            printDebug();


            base.Draw(gameTime);

        }

        private void printDebug()
        {
            var fps = fpsCounter.End();
            string debug =
                @"FPS: " + fps + Environment.NewLine
                + "Position: " + Camera.CameraPosition + Environment.NewLine
                + "Chunk: " + ChunkManager.CurrentChunk.ChunkId + Environment.NewLine
                + "Look_at: " + Camera.CameraOrientation + Environment.NewLine
                + "Face_of_cube: " + (general.HasValue ? general.Value.Face.ToString() : "NULL") + Environment.NewLine
                + "Allocated_bytes: " + ((GC.GetTotalMemory(false) / 1024f) / 1024f) + " MB!";

            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, debug, new Vector2(0, 0), Color.Yellow);
            spriteBatch.End();
        }
    }
}
