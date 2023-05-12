﻿using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs;
using Hints;
using MEC;
using Respawning;
using UnityEngine;

namespace GRU_P
{
    using PlayerRoles;

    public class EventHandlers
    {
        private Config cfg;
        private CoroutineHandle timerCoroutine = new CoroutineHandle();
        private Vector3 EscapeZone => Escape.WorldPos;
        private bool SHSpawned = false;

        
        public void OnRoundStarted()
        {
            SHSpawned = false;
            if (timerCoroutine.IsRunning)
            {
                Timing.KillCoroutines(timerCoroutine);
            }

            API.Escaped = 0;
            timerCoroutine = Timing.RunCoroutine(CheckEscape());
        }
        
        private IEnumerator<float> CheckEscape()
        {
            while (Round.IsStarted)
            {
                for (;;)
                {
                    yield return Timing.WaitForSeconds(1.5f);

                    foreach (Player player in Player.List)
                    {
                        if (!player.IsCuffed || (player.Role.Team == Team.SCPs && player.Role.Team == Team.Dead && player.Role.Team == Team.OtherAlive) ||
                            (EscapeZone - player.Position).sqrMagnitude > 156.5f)
                            continue;

                        if (!API.IsGRUP(player.Cuffer))
                        {
                            continue;
                        }

                        API.Escaped += 1;
                        List < Item > items = player.Items.ToList();
                        API.SpawnPlayer(player, "agent");
                        Timing.WaitForSeconds(1);
                        foreach (Item item in items)
                        {
                            item.Spawn(player.Position, default);
                        }
                    }
                }
            }
        }

        public void OnEndingRound(EndingRoundEventArgs ev)
        {
            bool scpAlive = API.CountRoles(Team.SCPs) > 0;
            bool grupAlive = API.GetGRUPPlayers().Count > 0;

            if (grupAlive && scpAlive)
            {
                ev.IsAllowed = false;
            }else if (grupAlive && API.Escaped > 0)
            {
                Map.ShowHint("GRU-P also won!", 7);
            }
        }
        
        public void OnDied(DiedEventArgs ev)
        {
            if (API.IsGRUP(ev.Target))
            {
                API.DestroyGRUP(ev.Target);
            }
        }
        
        public void OnAnnouncingScpTerminationEvent(AnnouncingScpTerminationEventArgs ev)
        {
            if (API.IsGRUP(ev.Killer) && ev.Player.Role.Team == Team.SCPs)
            {
                ev.IsAllowed = false;
                string scpType = null;
                switch (ev.Player.Role.Type)
                {
                    case RoleTypeId.Scp096:
                        scpType = "SCP 0 9 6";
                        break;
                    case RoleTypeId.Scp049:
                        scpType = "SCP 0 4 9";
                        break;
                    case RoleTypeId.Scp079:
                        scpType = "SCP 0 7 9";
                        break;
                    case RoleTypeId.Scp173:
                        scpType = "SCP 1 7 3";
                        break;
                    case RoleTypeId.Scp939:
                        scpType = "SCP 9 3 9";
                        break;
                    case RoleTypeId.Scp106:
                        scpType = "SCP 1 0 6";
                        break;
                    case RoleTypeId.Scp0492:
                        scpType = null;
                        break;
                }
                if(scpType != null)
                    Cassie.Message($"{scpType} has been terminated by G R U division P");
            }
        }

        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (API.IsGRUP(ev.Player) && ev.NewRole != RoleTypeId.Tutorial)
                API.DestroyGRUP(ev.Player);
        }

        public void OnSpawn(RespawningTeamEventArgs ev)
        {
            if (!SHSpawned)
            {
                foreach (var Player in Player.List)
                {
                    if (Player.SessionVariables.ContainsKey("IsSH"))
                        SHSpawned = false;
                }
            }
            
            if(SHSpawned)
                return;
            
            if ((Respawn.NtfTickets - Respawn.ChaosTickets) < cfg.differenceTickets || (Respawn.ChaosTickets - Respawn.NtfTickets) < cfg.differenceTickets)
            {
                if(UnityEngine.Random.Range(1, 101) > Plugin.Singleton.Config.Chance)
                {
                    ev.IsAllowed = false;
                    Respawn.NtfTickets -= 2;
                    Respawn.ChaosTickets += 2;
                    API.SpawnSquad(ev.Players.Count);
                }
            }
        }
        
        public EventHandlers(Plugin plugin)
        {
            cfg = plugin.Config;
        }
    }
}