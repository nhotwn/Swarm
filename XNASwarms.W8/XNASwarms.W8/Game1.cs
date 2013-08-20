﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ScreenSystem.Debug;
using ScreenSystem.ScreenSystem;
using SwarmEngine;
using System;
using XNASwarms;
using XNASwarms.Emitters;
using XNASwarms.Screens.SwarmScreen;
using XNASwarms.W8.Analysis.Components;

namespace XNASwarms.W8
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
#if NETFX_ARM
            this.TargetElapsedTime = TimeSpan.FromSeconds(1.0f / 30.0f);
#endif
        }

        protected override void Initialize()
        {
            ScreenManager screenManager = new ScreenManager(this);
            this.Components.Add(screenManager);

            var backgroundscreen = new BackgroundScreen();
            screenManager.AddScreen(backgroundscreen);

            var debugScreen = new DebugScreen(screenManager, false);
            screenManager.Game.Components.Add(debugScreen);
            this.Services.AddService(typeof(IDebugScreen), debugScreen);

            Recipe[] recipes = new Recipe[1];
            recipes[0] = new Recipe(StockRecipies.Stable_A);
            PopulationSimulator populationSimulator = new PopulationSimulator(0, 0, recipes);

            var swarmEmitterComponent = new SwarmEmitterComponent(populationSimulator);

            var swarmAnalysisComponent = new SwarmAnalysisComponent(this.Services.GetService(typeof(IDebugScreen)) as IDebugScreen);

            SwarmScreen1 swarmScreen = new SwarmScreen1(swarmEmitterComponent, swarmAnalysisComponent, populationSimulator);
            screenManager.AddScreen(swarmScreen);
            base.Initialize();
        }

        protected override void LoadContent()
        {
          
        }

        protected override void UnloadContent()
        {
           
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
