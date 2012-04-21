﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Mechanect.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Mechanect.Cameras;
using Mechanect.Common;

namespace Mechanect.Screens
{
    class Experiment2 : Mechanect.Common.GameScreen
    {
        Environment2 env;

        /// <summary>
        /// Defining the Textures that will contain the images and will represent the objects in the experiment
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 21  </para>
        /// </remarks>

        Viewport viewPort;
        ContentManager content;
        SpriteBatch spriteBatch;

        SpriteFont spriteFont;
        SpriteFont velAngleFont;
        //GraphicsDevice graphicsDevice;

        private Texture2D backgroundTexture;
        private Texture2D xyAxisTexture;
        private Texture2D preyTexture;
        private Texture2D bowlTexture;
        private Texture2D grayTexture;
        private Texture2D velocityTexture;
        private Texture2D angleTexture;
        Texture2D lineConnector;

        //list of models to be drawn
        private List<CustomModel> models = new List<CustomModel>();
        private Camera camera;

        //Variables that will change how the Gui will look
        private Boolean preyEaten = false;
        private Boolean grayScreen = true;

        private int screenWidth;
        private int screenHeight;
        private Vector2 velGauge;
        private Vector2 angGauge;
        private Vector3 predetorPosition = new Vector3(50,50,0);
        private Vector2 preyPosition = new Vector2(500f, 200f);
        private Vector2 startAquariumPosition = new Vector2(40f, 430f);
        private Vector2 destinationAquariumPosition = new Vector2(750f, 400f);

        float backgroundTextureScaling;
        float xyAxisTextureScaling;
        float preyTextureScaling;
        float bowlTextureScaling;
        float grayTextureScaling;
        float velocityTextureScaling;
        float angleTextureScaling;
        float fishModelScaling;

        VoiceCommands voiceCommand;
        User2 user;
        Boolean aquariumReached;
        /// <summary>
        /// This is a constructor that will initialize the grphicsDeviceManager and define the content directory.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 20  </para>
        /// </remarks>
        public Experiment2(User2 user)
        {

            env = new Environment2();
            //graphicsDevice = ScreenManager.GraphicsDevice;
            this.user = user;
        }



        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of the content.
        /// Loaded the Fish Model
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 21  </para>
        /// </remarks>

        public override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            viewPort = ScreenManager.GraphicsDevice.Viewport;
            screenWidth = viewPort.Width;
            screenHeight = viewPort.Height;
            content = ScreenManager.Game.Content;
            spriteBatch = ScreenManager.SpriteBatch;
            LoadTextures();
            LoadModels();

            spriteFont = content.Load<SpriteFont>("Ariel");
            velAngleFont = content.Load<SpriteFont>("angleVelFont");

        }

        /// <summary>
        /// Allows the game to draw all the textures 
        /// Initializing the Background and x,y Axises
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 21  </para>
        /// </remarks>

        public void LoadTextures()
        {
            backgroundTexture = content.Load<Texture2D>("Textures/background");
            xyAxisTexture = content.Load<Texture2D>("Textures/xyAxis");
            preyTexture = content.Load<Texture2D>("Textures/worm");
            bowlTexture = content.Load<Texture2D>("Textures/bowl2");
            grayTexture = content.Load<Texture2D>("Textures/GrayScreen");
            velocityTexture = content.Load<Texture2D>("Textures/VelocityGauge");
            angleTexture = content.Load<Texture2D>("Textures/AngleGauge");
            lineConnector = new Texture2D(ScreenManager.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            lineConnector.SetData(new[] { Color.Gray });

            base.Initialize();
        }
        /// <summary>
        /// LoadModels will be called once per game and is the place to load
        /// all of the Models.
        /// Loaded the Fish Model
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 21  </para>
        /// </remarks>
        private void LoadModels()
        {
            //add model to array of models
            fishModelScaling = 0.007f;
            models.Add(new CustomModel(content.Load<Model>("Models/fish"), new Vector3(-500, -500, -1050), new Vector3(MathHelper.ToRadians(-35), MathHelper.ToRadians(0), 0), new Vector3(fishModelScaling), ScreenManager.GraphicsDevice));

            //create still camera
            camera = new TargetCamera(new Vector3(-3000, 0, 0), new Vector3(0, 0, 0), ScreenManager.GraphicsDevice);

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 20  </para>
        /// </remarks>
        public override void UnloadContent()
        {

        }



        private Boolean isPreyEaten()
        {
            Boolean isHit = false;
            Vector2 position = env.Predator.getLocation();
            Prey prey = env.Prey;
            if (position.X >= prey.Location.X - prey.Width / 2
                && position.X <= prey.Location.X + prey.Width / 2
                && position.Y >= prey.Location.Y - prey.Length / 2
                && position.Y <= prey.Location.Y + prey.Length / 2)
                isHit = true;
            return isHit;
        }

        private Boolean isAquariumReached()
        {
            Boolean isReached = false;
            Vector2 position = env.Predator.getLocation();
            Aquarium aquarium = env.Aquarium;
            if (position.X >= aquarium.Location.X - aquarium.Width / 2
                && position.X <= aquarium.Location.X + aquarium.Width / 2
                && position.Y >= aquarium.Location.Y - aquarium.Length / 2
                && position.Y <= aquarium.Location.Y + aquarium.Length / 2)
                isReached = true;
            return isReached;
        }


        /// <summary>
        /// Runs at every frame, Updates game parameters and checks for user's actions
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed AbdelAzim </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 21  </para>
        /// </remarks>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime, bool covered)
        {

            camera.Update();

            if (!grayScreen && user.MeasuredVelocity != 0 && !aquariumReached)
            {
                if (env.Predator.Velocity == null)
                    env.Predator.Velocity = new Vector2((float)(user.MeasuredVelocity * Math.Cos(user.MeasuredAngle)), (float)(user.MeasuredVelocity * Math.Sin(user.MeasuredAngle)));
                env.Predator.UpdatePosition(gameTime);
                if (!preyEaten) preyEaten = isPreyEaten();
                if (!aquariumReached) aquariumReached = isAquariumReached();
            }

            else
            {
                user.MeasureVelocityAndAngle();
            }

            base.Update(gameTime, covered);
        }

        /// <summary>
        /// This is to be called when the game should draw itself.
        /// Here all the GUI is drawn
        /// </summary>
        /// <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 20  </para>
        /// </remarks>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);
             backgroundTextureScaling =1;
             xyAxisTextureScaling =1 ;
             preyTextureScaling =0.1f;
             bowlTextureScaling =1 ;
             grayTextureScaling =1 ;
             
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(xyAxisTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(bowlTexture, startAquariumPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            string meters = "meters";
            spriteBatch.DrawString(velAngleFont, meters, new Vector2(0f, 0f), Color.Red);
            spriteBatch.DrawString(velAngleFont, meters, new Vector2(screenWidth - spriteFont.MeasureString(meters).X / 2, screenHeight - spriteFont.MeasureString(meters).Y / 2), Color.Red);

            if (!preyEaten)
                spriteBatch.Draw(preyTexture, preyPosition, null, Color.White, 0f, Vector2.Zero, preyTextureScaling, SpriteEffects.None, 0f);
            if (grayScreen)
            {
                spriteBatch.End();
                DrawGrayScreen();
                spriteBatch.Begin();
            }
            else
            {
                String velString = "Velocity = ";
                String angString = "Angle = ";
                spriteBatch.DrawString(velAngleFont, velString + env.Velocity, new Vector2(screenWidth - spriteFont.MeasureString(velString + angString).X / 2, 0), Color.Red);

                spriteBatch.DrawString(velAngleFont, angString + env.Angle, new Vector2(screenWidth - spriteFont.MeasureString(angString).X / 2, 0), Color.Red);

            }
            spriteBatch.Draw(bowlTexture, destinationAquariumPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            DrawLine(spriteBatch, lineConnector, 2f, Color.LightGray, new Vector2(startAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2, startAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2), new Vector2(30, startAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2));
            spriteBatch.DrawString(velAngleFont, startAquariumPosition.X + "", new Vector2(startAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2 - spriteFont.MeasureString(startAquariumPosition.X+"").X/4, screenHeight - 30), Color.Red);
            DrawLine(spriteBatch, lineConnector, 2f, Color.LightGray, new Vector2(startAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2, startAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2), new Vector2(startAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2, screenHeight - 30));
            spriteBatch.DrawString(velAngleFont, startAquariumPosition.Y + "", new Vector2(5,startAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2 - spriteFont.MeasureString(startAquariumPosition.Y + "").X / 4), Color.Red);
            
            DrawLine(spriteBatch, lineConnector, 2f, Color.LightGray, new Vector2(preyPosition.X + preyTexture.Width * preyTextureScaling / 2, preyPosition.Y + preyTexture.Height * preyTextureScaling / 2), new Vector2(30, preyPosition.Y + preyTexture.Height * preyTextureScaling / 2));
            spriteBatch.DrawString(velAngleFont, preyPosition.X + "", new Vector2(preyPosition.X + preyTexture.Width * preyTextureScaling / 2 - spriteFont.MeasureString(preyPosition.X + "").X / 4, screenHeight - 30), Color.Red);
            DrawLine(spriteBatch, lineConnector, 2f, Color.LightGray, new Vector2(preyPosition.X + preyTexture.Width * preyTextureScaling / 2, preyPosition.Y + preyTexture.Height * preyTextureScaling / 2), new Vector2(preyPosition.X + preyTexture.Width * preyTextureScaling / 2, screenHeight - 30));
            spriteBatch.DrawString(velAngleFont, preyPosition.Y + "", new Vector2(5,preyPosition.Y + preyTexture.Height * preyTextureScaling / 2 - spriteFont.MeasureString(preyPosition.Y + "").X / 4), Color.Red);
            
            DrawLine(spriteBatch, lineConnector, 2f, Color.LightGray, new Vector2(destinationAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2, destinationAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2), new Vector2(30, destinationAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2));
            spriteBatch.DrawString(velAngleFont, destinationAquariumPosition.X + "", new Vector2(destinationAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2 - spriteFont.MeasureString(destinationAquariumPosition.X + "").X / 4, screenHeight - 30), Color.Red);
            DrawLine(spriteBatch, lineConnector, 2f, Color.LightGray, new Vector2(destinationAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2, destinationAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2), new Vector2(destinationAquariumPosition.X + bowlTexture.Width * bowlTextureScaling / 2, screenHeight - 30));
            spriteBatch.DrawString(velAngleFont, destinationAquariumPosition.Y + "", new Vector2(5, destinationAquariumPosition.Y + bowlTexture.Height * bowlTextureScaling / 2 - spriteFont.MeasureString(destinationAquariumPosition.Y + "").X / 4), Color.Red);
            
            spriteBatch.End();



            foreach (CustomModel model in models)
            {
                model.Draw(camera);
                //takes the camera instance and draws the model 
            }

        }

        private void DrawGrayScreen()
        {
            velocityTextureScaling = 0.5f;
            angleTextureScaling = 0.85f;
            
            spriteBatch.Begin();
            spriteBatch.Draw(grayTexture, Vector2.Zero, Color.White);
            spriteBatch.Draw(velocityTexture, new Vector2(screenWidth / 16, screenHeight / 14), null, Color.White, 0f, Vector2.Zero, velocityTextureScaling, SpriteEffects.None, 0f);
            spriteBatch.Draw(angleTexture, new Vector2(screenWidth - screenWidth * 2 / 9, screenHeight / 14), null, Color.White, 0f, Vector2.Zero, angleTextureScaling, SpriteEffects.None, 0f);
            string testString = "Test angle and Velocity";
            spriteBatch.DrawString(spriteFont, testString, new Vector2((screenWidth / 4), 0), Color.Red);
            string sayString = "Say 'GO' or press OK";
            spriteBatch.DrawString(spriteFont, sayString, new Vector2((screenWidth / 4), screenHeight - 2*spriteFont.MeasureString(sayString).Y), Color.Red);
            String velString = "Velocity = " + env.Velocity;
            String angString = "Angle = " + env.Angle;
            spriteBatch.DrawString(velAngleFont, velString, new Vector2((screenWidth / 4)  +velocityTexture.Width*velocityTextureScaling/2  - spriteFont.MeasureString(velString).X, (screenHeight / 14) + velocityTexture.Height * velocityTextureScaling - spriteFont.MeasureString(velString).Y), Color.Red);
            spriteBatch.DrawString(velAngleFont, angString, new Vector2(screenWidth - ((screenWidth*2 / 9) +angleTexture.Width* angleTextureScaling  - spriteFont.MeasureString(angString).X), (screenHeight / 14) + velocityTexture.Height * velocityTextureScaling - spriteFont.MeasureString(velString).Y), Color.Red);
            spriteBatch.End();
        }


        /// <summary>
        /// This method will draw a gray line 
        /// implemented initially to connect the GUI objects with the x,y axises
        /// </summary>
        ///  <remarks>
        /// <para>AUTHOR: Mohamed Alzayat </para>   
        /// <para>DATE WRITTEN: April, 20 </para>
        /// <para>DATE MODIFIED: April, 20  </para>
        /// </remarks>
        /// <param name="batch">Takes the spriteBatch that will draw the line </param>
        /// <param name="lineTexture">Takes the texture of the line to be drawn</param>
        /// <param name="width">Determines the width of the line to be drawn</param>
        /// <param name="color">Determines the color to be drawn</param>
        /// <param name="point1">Determines the start point of the line to be drawn</param>
        /// <param name="point2">Determines the end point of the line to be drawn</param>
        void DrawLine(SpriteBatch spriteBatch, Texture2D lineTexture,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            spriteBatch.Draw(lineTexture, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }


        public override void Remove()
        {
            base.Remove();
        }
    }
}
