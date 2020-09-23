using CommandSystem.Commands;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Events.Handlers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ToucanPlugin.Commands;
using UnityEngine;

namespace ToucanPlugin.Handlers
{
    class Server
    {
        readonly Tcp Tcp = new Tcp();
        public void OnWaitingForPlayers()
        {
            if (AcGame.NextGamemode != 0)
            {
                AcGame.RoundGamemode = AcGame.NextGamemode;
                AcGame.NextGamemode = 0;
                new GamemodeSelector();
            }
            Tcp.Send("log Waiting for players...");
            //Log.Info("Waiting For players...");
            Tcp.Send("boosters");
        }

        public void OnRoundStarted()
        {
            Tcp.Send("log Round started");
            Exiled.API.Features.Map.Broadcast(5, ToucanPlugin.Instance.Config.RoundStartMessage);
        }

        public void OnRestartingRound()
        {
            Tcp.Send("log Round restarting...");
            AcGame.RoundGamemode = 0;
        }
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (ToucanPlugin.Instance.Config.DetonateAtRoundEnded) Exiled.API.Features.Warhead.Detonate();
            Tcp.Send($"log Round Ended\n```Winning Class: {ev.LeadingTeam}\nEscaped D-Class: {ev.ClassList.class_ds}\nRescued Scientists: {ev.ClassList.scientists}\nContained SCPs: {ev.ClassList.scps_except_zombies}\nWas warhead detonated: {Exiled.API.Features.Warhead.IsDetonated}\nKilled by warhead: {ev.ClassList.warhead_kills}```");
            Exiled.API.Features.Player.List.ToList().ForEach(u => Tcp.Send($"stats {u.UserId} gamesplayed 1"));
            Exiled.API.Features.Player.List.ToList().ForEach(u =>
            {
                if (ev.LeadingTeam == LeadingTeam.ChaosInsurgency && u.Team == Team.CDP || u.Team == Team.CHI) Tcp.Send($"stats {u.UserId} gameswonCD 1");
                else
if (ev.LeadingTeam == LeadingTeam.Anomalies && u.Team == Team.SCP || u.Role == RoleType.Tutorial || u.GroupName == "SCP-035") Tcp.Send($"stats {u.UserId} gameswonSCP 1");
                else
if (ev.LeadingTeam == LeadingTeam.FacilityForces && u.Team == Team.MTF || u.Team == Team.RSC)
                    Tcp.Send($"stats {u.UserId} gameswonMTF 1");
            });
            Player.Has008RandomSpawned = false;
            Player.SCPKills = 0;
            SpecMode.TUTSpecList = null;
        }
        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
            {
                SpecMode.TUTSpecList.ForEach(id => ev.Players.Add(Exiled.API.Features.Player.List.ToList().Find(x => x.Id.ToString() == id)));
                SpecMode.TUTSpecList = new List<string>();
                List<Exiled.API.Features.Player> playerList = new List<Exiled.API.Features.Player>((IEnumerable<Exiled.API.Features.Player>)ev.Players);
                System.Random rnd = new System.Random();
                if (Player.SCPKills <= 2)
                {
                    ev.Players.Clear();
                    bool CommanderFound = false;
                    playerList.ForEach(p =>
                        {
                            if (rnd.Next(1, 2) == 1 && !CommanderFound) { p.Inventory.AddNewItem(ItemType.KeycardNTFLieutenant); CommanderFound = true; }
                            p.SetRole(RoleType.FacilityGuard);
                            p.ClearInventory();
                            ToucanPlugin.Instance.Config.UIUSpawnItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                            p.Position = new Vector3(176.2091f, 984.6033f, 39.10069f);
                        });
                    Cassie.Message($"the u i u HasEntered", false, false);
                }
                else
                if (Player.SCPKills >= 5)
                {
                    //Just spawn normal mtf
                }
                else
                if (Player.SCPKills <= 15)
                {
                    ev.Players.Clear();
                    playerList.ForEach(p =>
                        {
                            p.SetRole(RoleType.NtfLieutenant);
                            p.ClearInventory();
                            ToucanPlugin.Instance.Config.HammerDownSpawnItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                            p.Position = new Vector3(0.1775058f, 1005.311f, -10.53564f);
                        });
                    Cassie.Message($"MTFUNIT n u 7 HasEntered ", false, false);
                }
                else
                if (Player.SCPKills <= 20)
                {
                    ev.Players.Clear();
                    playerList.ForEach(p =>
                        {
                            p.SetRole(RoleType.NtfCommander);
                            p.MaxHealth = 200;
                            p.MaxEnergy = 999;
                            p.MaxAdrenalineHealth = 300;
                            p.ClearInventory();
                            ToucanPlugin.Instance.Config.RedHandSpawnItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                            p.Position = new Vector3(86.69146f, 988.5291f, -68.22584f);
                        });
                    Cassie.Message($"the MTFUNIT red right hand HasEntered the o 5 have disignated this a x k event", false, false);
                }
            }
        }
    }
}
