/// João Soares (17431)

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP_IP3D
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont arialBlack30;

        ICamera camera;
        ClsCameraGhostMode cameraGhostMode;
        ClsCameraSurfaceFollow cameraSurfaceFollow;

        Cls3DAxis eixos;

        Texture2D heightsMap;
        Texture2D terrainTexture;
        ClsTerrain terrain;

        ClsNormalsLines normalsLines;

        Model tankModel;
        Model cannonBallModel;
        ClsTanksManager tanksManager;

        List<ICollider> colliders;
        bool drawColliders = true;
        bool isCollidersKeyPressed = false;

        bool seeHealth = false;
        bool isSeeHealthKeyPressed = false;

        public Game1()
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

            // TODO: use this.Content to load your game content here
            GameSounds.LoadAudio(this);

            arialBlack30 = Content.Load<SpriteFont>("ArialBlack30");

            eixos = new Cls3DAxis(GraphicsDevice);

            heightsMap = Content.Load<Texture2D>("lh3d1");
            terrainTexture = Content.Load<Texture2D>("grass");
            terrain = new ClsTerrain(GraphicsDevice, heightsMap, terrainTexture);

            normalsLines = new ClsNormalsLines(GraphicsDevice, this);

            colliders = new List<ICollider>();

            tankModel = Content.Load<Model>("tank/tank");
            cannonBallModel = Content.Load<Model>("CannonBall");
            tanksManager = new ClsTanksManager(this, GraphicsDevice, tankModel, cannonBallModel);

            cameraGhostMode = new ClsCameraGhostMode(GraphicsDevice);
            cameraSurfaceFollow = new ClsCameraSurfaceFollow(this, GraphicsDevice);
            camera = cameraGhostMode;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(GameSettings.CameraGhostMode) && !ks.IsKeyDown(GameSettings.CameraSurfaceFollow) && !ks.IsKeyDown(GameSettings.CameraTankFollow(true)) && !ks.IsKeyDown(GameSettings.CameraTankFollow(false)))
                camera = cameraGhostMode;
            if (!ks.IsKeyDown(GameSettings.CameraGhostMode) && ks.IsKeyDown(GameSettings.CameraSurfaceFollow) && !ks.IsKeyDown(GameSettings.CameraTankFollow(true)) && !ks.IsKeyDown(GameSettings.CameraTankFollow(false)))
                camera = cameraSurfaceFollow;
            if (!ks.IsKeyDown(GameSettings.CameraGhostMode) && !ks.IsKeyDown(GameSettings.CameraSurfaceFollow) && ks.IsKeyDown(GameSettings.CameraTankFollow(true)) && !ks.IsKeyDown(GameSettings.CameraTankFollow(false)))
                camera = tanksManager.Tank1.CameraTankFollow;
            if (!ks.IsKeyDown(GameSettings.CameraGhostMode) && !ks.IsKeyDown(GameSettings.CameraSurfaceFollow) && !ks.IsKeyDown(GameSettings.CameraTankFollow(true)) && ks.IsKeyDown(GameSettings.CameraTankFollow(false)))
                camera = tanksManager.Tank2.CameraTankFollow;

            camera.Update(Keyboard.GetState(), Mouse.GetState(), gameTime);

            eixos.Update(gameTime);

            normalsLines.Update(gameTime);

            tanksManager.Update(gameTime);

            // test collisions
            for (int i = 0; i < colliders.Count - 1; i++)
            {
                for (int j = i + 1; j < colliders.Count; j++)
                {
                    if (colliders[i].CheckIfCollidesWith(colliders[j]))
                    {
                        colliders[i].CollidedWith(colliders[j]);
                        colliders[j].CollidedWith(colliders[i]);
                    }
                }
            }

            // drawColliders ?
            if (ks.IsKeyDown(GameSettings.Colliders))
                isCollidersKeyPressed = true;
            if (ks.IsKeyUp(GameSettings.Colliders) && isCollidersKeyPressed)
            {
                drawColliders = !drawColliders;
                isCollidersKeyPressed = false;
            }

            // show both thanks' health on the screen ?
            if (ks.IsKeyDown(GameSettings.SeeHealth))
                isSeeHealthKeyPressed = true;
            if (ks.IsKeyUp(GameSettings.SeeHealth) && isSeeHealthKeyPressed)
            {
                seeHealth = !seeHealth;
                isSeeHealthKeyPressed = false;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            eixos.Draw(GraphicsDevice, camera);

            terrain.Draw(GraphicsDevice, camera);

            normalsLines.Draw(GraphicsDevice, camera);

            tanksManager.Draw(GraphicsDevice, camera);

            // draw colliders, if allowed
            if(drawColliders)
                for (int i = 0; i < colliders.Count; i++)
                    colliders[i].DrawCollider(camera);

            // show both thanks' health on the screen, if allowed
            if(seeHealth)
                ClsGUI.Draw(GraphicsDevice, arialBlack30, tanksManager.Tank1.Health, tanksManager.Tank2.Health);

            base.Draw(gameTime);
        }

        public ClsTerrain Terrain { get { return terrain; } }
        public ICamera Camera { get { return camera; } }
        public List<ICollider> Colliders { get { return colliders; } }
    }
}
