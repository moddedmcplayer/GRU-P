using System;
using CommandSystem;
using Exiled.Permissions.Extensions;
using Exiled.API.Features;

namespace GRU_P.Commands.Subcmds
{
    public class Spawn : ICommand
    {
        public string Command { get; } = "spawn";
        public string[] Aliases { get; } = {"s"};
        public string Description { get; } = "Makes player a GRU-P Agent";

        public string[] types = {"commissar", "agent", "trooper"};

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("grup.spawn"))
            {
                response = "You don't have permission to execute this command. Required permission: grup.spawn";
                return false;
            }
            
            if (arguments.Count == 0)
            {
                Player ply = Player.Get(sender);

                if (API.IsGRUP(ply))
                {
                    if (API.GetGRUPType(ply) == "commissar")
                    {
                        response = $"You are already a GRU-P commissar!";
                        return false;
                    }
                    ply.SessionVariables.Remove("IsGRUP");
                }

                API.SpawnPlayer(ply, "commissar");
                response = "You are now a GRU-P commissar.";
                return true;
            }
            
            string type = arguments.At(0).ToLower();

            if (arguments.Count == 1)
            {
                Player ply = Player.Get(sender);
                
                if (!types.Contains(type))
                {
                    response = "Invalid argument. Please enter a valid type!";
                    return false;
                }

                if (API.IsGRUP(ply))
                {
                    if (API.GetGRUPType(ply) == type)
                    {
                        response = $"You are already a GRU-P {type}!";
                        return false;
                    }
                    ply.SessionVariables.Remove("IsGRUP");
                }
                API.SpawnPlayer(ply, type);
                response = $"You are now a GRU-P {type}.";
                return true;
            }
            
            Player player = Player.Get(arguments.At(1));
            
            if (player == null)
            {
                response = "Invalid argument. Please give player's id or nickname!";
                return false;
            }
            
            if (!types.Contains(type))
            {
                response = "Invalid argument. Please enter a valid type!";
                return false;
            }

            if (API.IsGRUP(player))
            {
                if (API.GetGRUPType(player) == type)
                {
                    response = $"({player.Id}) {player.Nickname} is already a GRU-P {type}!";
                    return false;
                }
                player.SessionVariables.Remove("IsGRUP");
            }

            API.SpawnPlayer(player, type);
            response = $"({player.Id}) {player.Nickname} is now a GRU-P {type}.";
            return true;
            
        }
    }
}