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
using ShootCube.Dynamics.Player;
using ShootCube.Dynamics.Ambient.Gravitation;
using ShootCube.Items.Mountable.Weapons;

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
        public GravitationManager GravitationManager;

        // DEBUG

        DebugAxe _debugAxe;

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
            new Camera(0.007f, .15f);
            new ChunkManager(5, 5);
            ChunkManager.Start();

            //weatherManager = new WeatherManager();

            ChunkManager.Cubes[56, 32, 32] = 6;
            ChunkManager.Cubes[57, 32, 32] = 6;

            ChunkManager.Run();

            Globals.MainPlayer = new MainPlayer("");
            GravitationManager = new GravitationManager(Globals.MainPlayer, 0.25f, 0.055f);

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


            debugFont = Content.Load<SpriteFont>("debug");

            SkyEnvironment = new ShootCube.Sky.Sky(1);



            MouseControl.AddLeftAction(new Action(() =>
            {
                var p = ChunkManager.Pick(5);
                p?.Chunk.DestroyCube(p.Value, WeaponStatistics.Utilities.Pickaxe_diamond);
                _debugAxe.Activation = 20;
            }), new Action(() =>
            {
                _debugAxe.Activation = 0;
                foreach (var item in Informations.CorrespondingAudios)
                    item.Value.Stop();
            }), false, 55);


            // TEST

            _debugAxe = new DebugAxe();

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
            GravitationManager.Update(gameTime);
   
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            fpsCounter.Start(gameTime);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            
 
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
            _debugAxe.Render(gameTime);


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
                + "Rendering_Chunks: " + ChunkManager.AmountRenderingChunk + Environment.NewLine 
                + "Allocated_bytes: " + ((GC.GetTotalMemory(false) / 1024f) / 1024f) + " MB!";

            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, debug, new Vector2(0, 0), Color.Yellow);
            spriteBatch.End();

            ChunkManager.AmountRenderingChunk = 0;
        }
    }
}
