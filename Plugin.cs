﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API;
using Exiled.API.Features;
using Exiled.Events.Handlers;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace GRU_P
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Singleton;
        
        public override string Author { get; } = "moddedmcplayer";
        public override string Name { get; } = "GRU-P";
        public override Version Version { get; } = new Version(0, 0, 1);
        public override Version RequiredExiledVersion { get; } = new Version(5, 0, 0);

        public EventHandlers EventHandler;

        public override void OnEnabled()
        {
            RegisterEvents(); 
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnRegisterEvents();
            base.OnDisabled();
        }

        private void RegisterEvents()
        {
            EventHandler = new EventHandlers(this);
            Server.EndingRound += EventHandler.OnEndingRound;
        }

        private void UnRegisterEvents()
        {
            Server.EndingRound -= EventHandler.OnEndingRound;
            EventHandler = null;
        }
    }
}