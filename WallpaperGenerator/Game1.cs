using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Windows.UI.ViewManagement;

namespace WallpaperGenerator
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        World world;
        VirtualResolutionRenderer vrr;
        KeyboardState previousKeyboardState;

        List<Sprite> stuff;

        int[] flat = {
            0xE74C3C, 0xE67E22, 0xF1C40F, 0x2ECC71,
            0x1ABC9C, 0x3498DB, 0x9B59B6
        };

        int[] ocean =
        {
            0xbf616a, 0xd08770, 0xebcb8b, 0xa3be8c,
            0x96b5b4, 0x8fa1b3, 0xb48ead
        };

        Color HexToRgb(int hex)
        {
            byte r = (byte)((hex & 0xFF0000) >> (2 * 8));
            byte g = (byte)((hex & 0x00FF00) >> (1 * 8));
            byte b = (byte)(hex & 0x0000FF);
            return new Color(r, g, b, (byte)255);
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 1920;
            Debug.WriteLine(Window.ClientBounds);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            stuff = new List<Sprite>();
            previousKeyboardState = Keyboard.GetState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            vrr = new VirtualResolutionRenderer(graphics, new Size(1080, 1920));
            world = new World(graphics, vrr, Content);

            World.TextureManager.Load("circle");
            World.TextureManager.Load("heart");
            World.TextureManager.Load("square");
            World.TextureManager.Load("star");
            World.TextureManager.Load("triangle");
            Regenerate();
        }

        void Regenerate()
        {
            stuff.Clear();
            for (int i = 0; i < 50; i++)
            {
                int random = World.random.Next(0, 5);
                string str = string.Empty;
                if (random == 0)
                {
                    str = "circle";
                }
                else if (random == 1)
                {
                    str = "heart";
                }
                else if (random == 2)
                {
                    str = "square";
                }
                else if (random == 3)
                {
                    str = "star";
                }
                else if (random == 4)
                {
                    str = "triangle";
                }

                Sprite sprite = new Sprite(World.TextureManager[str]);
                sprite.position = new Vector2(World.random.Next(0, 1080),
                    World.random.Next(0, 1920));
                int randomColor = World.random.Next(0, flat.Length);
                sprite.color = HexToRgb(flat[randomColor]);
                sprite.scale = (float)World.random.NextDouble(0.7, 1.3);
                sprite.rotation = World.random.Next(0, 360);
                sprite.UpdateBoundingRectangle();
                bool colliding = false;
                do
                {
                    colliding = false;
                    foreach (var other in stuff)
                    {
                        if (other.rectangle.Intersects(sprite.rectangle))
                        {
                            sprite.position = new Vector2(World.random.Next(0, 1080),
                                World.random.Next(0, 1920));
                            sprite.UpdateBoundingRectangle();
                            colliding = true;
                            break;
                        }
                    }
                }
                while (colliding == true);
                stuff.Add(sprite);
            }
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

            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDownAndUp(Keys.Space, previousKeyboardState))
            {
                Regenerate();
            }

            previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            world.BeginDraw();
            foreach (var sprite in stuff)
            {
                world.Draw(sprite.Draw);
            }
            world.EndDraw();

            base.Draw(gameTime);
        }
    }
}
