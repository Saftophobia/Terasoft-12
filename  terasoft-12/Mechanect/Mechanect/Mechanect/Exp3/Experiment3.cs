﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using UI.Cameras;
using UI.Animation;
using UI.Components;
using Mechanect.Classes;
using Physics;
using Mechanect.Screens;
using ButtonsAndSliders;

namespace Mechanect.Exp3
{
    public class Experiment3 : Mechanect.Common.GameScreen
    {
        private Ball ball;
        private Bar bar;
        private Environment3 environment;
        private User3 user;

        private TargetCamera targetCamera;
        private BallAnimation animation;
        private Simulation simulation;

        private bool pauseScreenShowed;
        private bool firstAnimation;

        private float arriveVelocity;
        private Vector3 shootVelocity;

        private Button mainMenu;
        private Button newGame;

        /// <summary>
        /// constructs a new Experiment3 screen
        /// </summary>
        /// <param name="user">user</param>
        /// <remarks>
        /// Author : Bishoy Bassem
        /// </remarks>
        public Experiment3(User3 user)
        {
            arriveVelocity = 10;
            firstAnimation = true;
            user.shootingPosition = new Vector3(0, 3, 62);
            this.user = user;
        }

        /// <summary>
        /// loads the experiment's content
        /// </summary>
        /// <remarks>
        /// Author : Bishoy Bassem
        /// </remarks>
        public override void LoadContent()
        {

            targetCamera = new TargetCamera(new Vector3(0, 70, 195), new Vector3(0,45,0), ScreenManager.GraphicsDevice);

            environment = new Environment3(ScreenManager.Game.Content, ScreenManager.GraphicsDevice, user);
            environment.LoadContent();

            ball = new Ball(4, ScreenManager.GraphicsDevice, ScreenManager.Game.Content);
            ball.GenerateIntialPosition(environment.terrainWidth, environment.terrainHeight);
            ball.GenerateBallMass(0.004f, 0.006f);

            environment.ball = ball;

            Vector3 intialVelocity = LinearMotion.CalculateIntialVelocity(user.shootingPosition - ball.Position, arriveVelocity, environment.Friction);

            animation = new BallAnimation(ball, environment.HoleProperty, intialVelocity, environment.Friction);

            bar = new Bar(new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - 10, ScreenManager.GraphicsDevice.Viewport.Height - 225), ScreenManager.SpriteBatch, new Vector2(ball.Position.X, ball.Position.Z), new Vector2(ball.Position.X, ball.Position.Z), new Vector2(user.shootingPosition.X, user.shootingPosition.Z), ScreenManager.Game.Content);

            int screenWidth = this.ScreenManager.GraphicsDevice.Viewport.Width;
            int screenHeight = this.ScreenManager.GraphicsDevice.Viewport.Height;

            mainMenu = Tools3.MainMenuButton(ScreenManager.Game.Content, new Vector2(screenWidth - 245,
                screenHeight - 125), screenWidth, screenHeight, user);

            newGame = Tools3.NewGameButton(ScreenManager.Game.Content, new Vector2(screenWidth - 125,
                screenHeight - 125), screenWidth, screenHeight, user);

            base.LoadContent();
        }

        /// <summary>
        /// updates the experiment's screen
        /// </summary>
        /// <param name="gameTime">GameTime instance</param>
        /// <remarks>
        /// Author : Bishoy Bassem
        /// </remarks>
        public override void Update(GameTime gameTime)
        {
            environment.PlayerModel.Update();
            environment.PlayerAnimation.Update();
            ball.SetHeight(environment.GetHeight(ball.Position));
            if (firstAnimation)
            {
                float distance = animation.Displacement.Length();
                float totalDistance = (user.shootingPosition - animation.StartPosition).Length();
                if (distance / totalDistance > 0.5 && !pauseScreenShowed)
                {
                    pauseScreenShowed = true;
                    FreezeScreen();
                    ScreenManager.AddScreen(new PauseScreen(user,7,ball.Mass,user.assumedLegMass,environment.HoleProperty.Position));
                    //add pause screen
                }
                bar.Update(new Vector2(ball.Position.X,ball.Position.Z));
                /*if (distance / totalDistance > 1)
                {
                    firstAnimation = false;
                    this.shootVelocity = new Vector3(10, 0, -10);
                    animation = new BallAnimation(ball, environment.HoleProperty, this.shootVelocity, environment.Friction);
                }*/
                if (ball.hasBallEnteredShootRegion())
                {
                    user.UpdateMeasuringVelocityAndAngle(gameTime);
                    Vector3 shootVelocity = user.velocity;
                    if (user.hasShot && shootVelocity.Length() != 0)
                    {
                        firstAnimation = false;
                        this.shootVelocity = environment.GetVelocityAfterCollision(shootVelocity);
                       
                    }
                    animation = new BallAnimation(ball, environment.HoleProperty, this.shootVelocity, environment.Friction);
                }
                if (animation.Finished())
                {
                    //add final screen
                }
            }
            else if (animation.Finished() && simulation == null)
            {
                simulation = new Simulation(ball, environment.HoleProperty, user.shootingPosition, shootVelocity, environment.Friction, ScreenManager.Game.Content, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch);
            }
            
            if (simulation != null)
            {
                mainMenu.Update(gameTime);
                newGame.Update(gameTime);
                if (mainMenu.IsClicked())
                {
                    //add main menu screen
                }
                if (newGame.IsClicked())
                {
                    //add Experiment3 screen
                }
                simulation.Update(gameTime);
                if (simulation.Finished())
                {
                    //add final screen
                }
            }
            else
            {
                animation.Update(gameTime.ElapsedGameTime);
            }
            
            targetCamera.Update();
            base.Update(gameTime);
        }

        /// <summary>
        /// draws the experiment's screen
        /// </summary>
        /// <param name="gameTime">GameTime instance</param>
        /// <remarks>
        /// Author : Bishoy Bassem
        /// </remarks>
        public override void Draw(GameTime gameTime)
        {
            Camera camera = targetCamera;
            if (simulation != null)
            {
                camera = simulation.Camera;
            }
            environment.Draw(camera, gameTime);
            ball.Draw(camera);
            if (firstAnimation)
            {
                bar.Draw();
            }
            if (simulation != null)
            {
                simulation.Draw();
                ScreenManager.SpriteBatch.Begin();
                newGame.Draw(ScreenManager.SpriteBatch, 0.5f);
                mainMenu.Draw(ScreenManager.SpriteBatch, 0.5f);
                mainMenu.DrawHand(ScreenManager.SpriteBatch);
                ScreenManager.SpriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public override void UnloadContent()
        {

        }

    }

}
