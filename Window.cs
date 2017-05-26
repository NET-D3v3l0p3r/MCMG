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

        public Sky.Sky SkyEnvironment;



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


        LightSource source;

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

            new Camera(0.007f, .35f);
            new ChunkManager(15, 15);
            ChunkManager.Start();

            ChunkManager.Cubes[56, 32, 32] = 6;

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


            KeyboardControl.AddKey(new Key(Keys.X, new Action(() =>
            {

                source = new LightSource(Camera.CameraPosition, 13);
                source.Emit();


            }), true));

            debugFont = Content.Load<SpriteFont>("debug");

            SkyEnvironment = new ShootCube.Sky.Sky(1);


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


        Profile? _new;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!this.IsActive)
                return;

            KeyboardControl.Update();
            Camera.Update();
            ChunkManager.Update(gameTime);
             
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {

                _new = ChunkManager.Pick(5);
                _new?.Chunk.RemoveCube(_new.Value.BoundingBox);
            }




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
            GraphicsDevice.Clear(Color.CornflowerBlue );

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
            Globals.Effect.Parameters["GlobalValue"].SetValue(MathHelper.Clamp((float)Math.Sin(Sky.Sky.Time), 1.0f, 2.0f));

            Globals.Effect.CurrentTechnique.Passes[0].Apply();
            ChunkManager.Render();


            // TODO: Add your drawing code here

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
                + "Look_at: " + Camera.CameraOrientation;

            spriteBatch.Begin();
            spriteBatch.DrawString(debugFont, debug, new Vector2(0, 0), Color.Yellow);
            spriteBatch.End();
        }
    }
}
