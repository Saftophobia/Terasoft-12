using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Mechanect.Common;

namespace Mechanect
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class levelSelect : Microsoft.Xna.Framework.DrawableGameComponent
    {
        //Position of the level select menu on the screen.
        public Vector2 position { get; set; }
        
        public Texture2D textureStrip, texture;
        public int frame, width, height, level; 
        SpriteBatch spriteBatch;
        ContentManager Content;
        Button rightArrow, leftArrow, firstButton, secondButton, thirdButton;
        MouseState mouse;
        List<Button> Buttons;
        public SpriteFont font;
        int counter;
        int[] values;
        bool[] isActive;
        //MKinect kinect;

        public levelSelect(Game game, Vector2 position, SpriteBatch spriteBatch)
            : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.position = position;
            this.frame = 0;
            this.Content = game.Content;
        }

        /// <remarks>
        ///<para>AUTHOR: Omar Abdulaal </para>
        ///</remarks>
        /// <summary>
        /// Initializes Buttons and Values.
        /// </summary>
        public new void Initialize()
        {
            //Initialize level values
            values = new int[3];
            values[0] = 1;
            values[1] = 2;
            values[2] = 3;

            //Initialize buttons isActive
            isActive = new bool[5];
            for (int i = 0; i < isActive.Length; i++)
                isActive[i] = true;

            //List that Contains All Buttons in the level select part.
            Buttons = new List<Button>();

            base.Initialize();
            //width and height of the textureStrip
            width = 425;
            height = 200;

            int screenW = GraphicsDevice.Viewport.Width;
            int screenH = GraphicsDevice.Viewport.Height;

            int ButtonWidth = Content.Load<GifAnimation.GifAnimation>("dummy").GetTexture().Width;
            //Create and Initialize all Buttons.
            rightArrow = new Button(Content.Load<GifAnimation.GifAnimation>("rightArrow"), Content.Load<GifAnimation.GifAnimation>("rightArrow"),
                new Vector2(position.X + ButtonWidth + 65 + width, position.Y + 15), screenW, screenH, Content.Load<Texture2D>("hand"));
            leftArrow = new Button(Content.Load<GifAnimation.GifAnimation>("leftArrow"), Content.Load<GifAnimation.GifAnimation>("leftArrow"),
                new Vector2(position.X, position.Y + 15), screenW, screenH, Content.Load<Texture2D>("hand"));
            firstButton = new Button(Content.Load<GifAnimation.GifAnimation>("dummy"), Content.Load<GifAnimation.GifAnimation>("dummySelected"),
                new Vector2(leftArrow.Position.X + Content.Load<GifAnimation.GifAnimation>("leftArrow").GetTexture().Width, position.Y), 
                screenW, screenH, Content.Load<Texture2D>("hand"));
            secondButton = new Button(Content.Load<GifAnimation.GifAnimation>("dummy"), Content.Load<GifAnimation.GifAnimation>("dummySelected"),
                new Vector2(firstButton.Position.X + ButtonWidth, position.Y),screenW, screenH, Content.Load<Texture2D>("hand"));
            thirdButton = new Button(Content.Load<GifAnimation.GifAnimation>("dummy"), Content.Load<GifAnimation.GifAnimation>("dummySelected"),
                new Vector2(secondButton.Position.X + ButtonWidth, position.Y), screenW, screenH, Content.Load<Texture2D>("hand"));

            //Add Buttons to the list.
            Buttons.Add(rightArrow);
            Buttons.Add(leftArrow);
            Buttons.Add(firstButton);
            Buttons.Add(secondButton);
            Buttons.Add(thirdButton);
            
        }
        /// <remarks>
        ///<para>AUTHOR: Omar Abdulaal </para>
        ///</remarks>
        /// <summary>
        /// Loads textures.
        /// </summary>
        public new void LoadContent()
        {
            texture = Content.Load<Texture2D>("texture");
            textureStrip = Content.Load<Texture2D>("textureStrip");
            base.LoadContent();
        }

        
        /// <remarks>
        ///<para>AUTHOR: Omar Abdulaal </para>
        ///</remarks>
        /// <summary>
        /// XNA Update Method.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            KeyboardState k = Keyboard.GetState();
            // If were at the leftmost frame.. Lock the left arrow Button.
            if (values[0] == 1)
                isActive[0] = false;
            else isActive[0] = true;

            //If right arrow Button is pressed.. Move the textureStrip one frame to the right 
            //and increase the values of the Buttons to match the levels
            if(rightArrow.isClicked() && isActive[4]) 
            {
                frame++;
                values[0]++; 
                values[1]++;
                values[2]++;
            }

            //Same as above but move the strip to the left by updating which frame to draw 
            //and decrease value of the Buttons.
            if (leftArrow.isClicked() && isActive[0])
            {
                frame--;
                values[0]--;
                values[1]--;
                values[2]--;
            }

            //If any of the level Buttons is pressed.. set the level to the value of that Button.
            if (firstButton.isClicked())
                level = values[1];
            if (secondButton.isClicked())
                level = values[2];
            if (thirdButton.isClicked())
                level = values[3];

            //Lock the right arrow Button if were at the right most frame.
            if (values[3] == 5)
                isActive[4] = false;
            else
                isActive[4] = true;

            
            base.Update(gameTime);
        }
        /// <remarks>
        ///<para>AUTHOR: Omar Abdulaal </para>
        ///</remarks>
        /// <summary>
        /// XNA Draw Method.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            //Draw the texture and textureStrip according to the frame
            spriteBatch.Draw(textureStrip, new Vector2(position.X + Content.Load<GifAnimation.GifAnimation>("leftArrow").GetTexture().Width + 70, position.Y + 30), new Rectangle(width*frame, 0, width, height),  Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, position, new Rectangle(0,0, texture.Width, texture.Height), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 1);

            //Draw each Button in the list
            foreach (Button b in Buttons)
                b.draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}