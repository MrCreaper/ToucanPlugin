using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToucanPlugin.Commands;
using UnityEngine;

namespace ToucanPlugin.Handlers
{
    class Server
    {
        readonly System.Random rnd = new System.Random();
        readonly Tcp Tcp = new Tcp();
        readonly MessageResponder mr = new MessageResponder();
        public void OnWaitingForPlayers()
        {
            if (AcGame.NextGamemode != 0)
            {
                AcGame.RoundGamemode = AcGame.NextGamemode;
                AcGame.NextGamemode = 0;
                new GamemodeSelector();
            }
            Tcp.Send("log Waiting for players...");
            Tcp.Send("bestbois");
        }

        public void OnRoundStarted()
        {
            Tcp.Send("log Round started");
            Map.Broadcast(5, ToucanPlugin.Instance.Config.RoundStartMessage);
            //Janitor spawning
            List<Exiled.API.Features.Player> playerList = new List<Exiled.API.Features.Player>((IEnumerable<Exiled.API.Features.Player>)Exiled.API.Features.Player.List.ToList());
            if (playerList.Count >= 0 && rnd.Next(0, 5) == 1)
            {
                Exiled.API.Features.Player Janitor = playerList.Find(x => x.Role == RoleType.ClassD);
                Janitor.MaxHealth = 100;
                Janitor.MaxEnergy = 110;
                Janitor.MaxAdrenalineHealth = 100;
                Janitor.ClearInventory();
                ToucanPlugin.Instance.Config.JanitorItems.ForEach(item => Janitor.Inventory.AddNewItem((ItemType)item));
                Janitor.Position = Exiled.API.Features.Map.GetRandomSpawnPoint(RoleType.Scientist);
                Janitor.ShowHint("<i>You are a <color=yellow>Janior</color>... life sucz</i>");
            }
            //Containment Engineer spawning
            if (playerList.Count >= 0 && rnd.Next(0, 5) == 1)
            {
                if (playerList.Find(x => x.Role == RoleType.Scp106) == null) return;
                Exiled.API.Features.Player CE = playerList.Find(x => x.Role == RoleType.Scientist);
                CE.MaxHealth = 90;
                CE.MaxEnergy = 90;
                CE.MaxAdrenalineHealth = 120;
                CE.ClearInventory();
                ToucanPlugin.Instance.Config.JanitorItems.ForEach(item => CE.Inventory.AddNewItem((ItemType)item));
                CE.Position = Map.GetRandomSpawnPoint(RoleType.Scp106);
                CE.ShowHint("<i>You are a <color=yellow>Containment Engineer</color>. You had <color=yellow>one</color> job.</i>");
            }
            //Speedy boi spawning
            /*if (playerList.Count >= 5 && rnd.Next(0, 5) == 1)
            {
                if (playerList.Find(x => x.Role == RoleType.Scp106) == null) return;
                Exiled.API.Features.Player CE = playerList.Find(x => x.Role == RoleType.ClassD);
                CE.MaxEnergy = 9999;
                CE.Energy = CE.MaxEnergy;
                CE.ClearInventory();
                ToucanPlugin.Instance.Config.JanitorItems.ForEach(item => CE.Inventory.AddNewItem((ItemType)item));
                CE.Position = Exiled.API.Features.Map.GetRandomSpawnPoint(RoleType.ClassD);
                CE.ShowHint("<i>You are a <color=yellow>Speedy Fucker</color>. You are <color=yellow><b>FAST</b></color>.</i>");
            }*/
        }

        public void OnRestartingRound()
        {
            Tcp.Send("log Round restarting...");
        }
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (ToucanPlugin.Instance.Config.DetonateAtRoundEnded && Exiled.API.Features.Warhead.IsDetonated) Exiled.API.Features.Warhead.Detonate();
            Tcp.Send($"log Round Ended\n```Winning Class: {ev.LeadingTeam}\nEscaped D-Class: {ev.ClassList.class_ds}\nRescued Scientists: {ev.ClassList.scientists}\nContained SCPs: {ev.ClassList.scps_except_zombies}\nWas warhead detonated: {Exiled.API.Features.Warhead.IsDetonated}\nKilled by warhead: {ev.ClassList.warhead_kills}```");
            Exiled.API.Features.Player.List.ToList().ForEach(u => Tcp.Send($"stats {u.UserId} gamesplayed 1"));
            Exiled.API.Features.Player.List.ToList().ForEach(u =>
            {
                if (ev.LeadingTeam == LeadingTeam.ChaosInsurgency && u.Team == Team.CDP || u.Team == Team.CHI)
                    Tcp.Send($"stats {u.UserId} gameswonCD 1");
                else
if (ev.LeadingTeam == LeadingTeam.Anomalies && u.Team == Team.SCP || SerpentsHand.API.SerpentsHand.GetSHPlayers().Contains(u) || scp035.API.Scp035Data.GetScp035() == u)
                    Tcp.Send($"stats {u.UserId} gameswonSCP 1");
                else
if (ev.LeadingTeam == LeadingTeam.FacilityForces && u.Team == Team.MTF || u.Team == Team.RSC)
                    Tcp.Send($"stats {u.UserId} gameswonMTF 1");
                //else Tcp.Send($"stats {u.UserId} gameswonSTALEMATE 1");
            });
            AcGame.RoundGamemode = 0;
            Player.Has008RandomSpawned = false;
            Player.SCPKills = 0;
            SpecMode.TUTSpecList = null;
            mr.ChaosHacker = null;
        }
        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            Vector3 MTFSpawnLocaltion = new Vector3(0,0,0);
            SpecMode.TUTSpecList.ForEach(id => ev.Players.Add(Exiled.API.Features.Player.List.ToList().Find(x => x.Id.ToString() == id)));
            SpecMode.TUTSpecList = new List<string>();
            List<Exiled.API.Features.Player> playerList = new List<Exiled.API.Features.Player>((IEnumerable<Exiled.API.Features.Player>)ev.Players);
            if (rnd.Next(0, 50) == 1)
            { //Zipper gang
                ev.Players.Clear();
                playerList.ForEach(p =>
                {
                    p.SetRole(RoleType.Tutorial);
                    p.MaxHealth = 69;
                    p.MaxEnergy = 420;
                    p.MaxAdrenalineHealth = 360;
                    p.ClearInventory();
                    ToucanPlugin.Instance.Config.RedHandSpawnItems.ForEach(item => p.Inventory.AddNewItem((ItemType)14));
                    p.Position = new Vector3(176f, 984f, 39f);
                });
                Cassie.Message($"the z i p HasEntered", false, false);
            }
            else
            {
                if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
                {
                    if (Player.SCPKills <= 2)
                    { // UIU
                        ev.Players.Clear();
                        List<int> spawnerNumList = new List<int>();
                        playerList.ForEach(p =>
                            {
                                spawnerNumList.Add(p.Id);
                                p.SetRole(RoleType.FacilityGuard);
                                p.ClearInventory();
                                ToucanPlugin.Instance.Config.UIUSpawnItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                                MTFSpawnLocaltion = new Vector3(176f, 984f, 39f);
                                p.Position = MTFSpawnLocaltion;
                            });
                        int leaderIndex = rnd.Next(0, spawnerNumList.Count);
                        playerList[leaderIndex].Inventory.AddNewItem(ItemType.KeycardNTFLieutenant);
                        Cassie.Message($"the u i u HasEntered", false, false);
                    }
                    else
                    if (Player.SCPKills <= 15 && Player.SCPKills >= 10)
                    { // NU7
                        ev.Players.Clear();
                        playerList.ForEach(p =>
                            {
                                p.SetRole(RoleType.NtfLieutenant);
                                p.ClearInventory();
                                ToucanPlugin.Instance.Config.HammerDownSpawnItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                                MTFSpawnLocaltion = new Vector3(0f, 1005f, -10f);
                                p.Position = MTFSpawnLocaltion;
                            });
                        Cassie.Message($"MTFUNIT n u 7 HasEntered ", false, false);
                    }
                    else
                    if (Player.SCPKills <= 20 && Player.SCPKills >= 16)
                    { //RED RIGHT HAND
                        ev.Players.Clear();
                        playerList.ForEach(p =>
                            {
                                p.SetRole(RoleType.NtfCommander);
                                p.MaxHealth = 200;
                                p.MaxEnergy = 999;
                                p.MaxAdrenalineHealth = 300;
                                p.ClearInventory();
                                ToucanPlugin.Instance.Config.RedHandSpawnItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                                MTFSpawnLocaltion = new Vector3(86f, 988f, -68f);
                                p.Position = MTFSpawnLocaltion;
                            });
                        Cassie.Message($"the MTFUNIT red right hand HasEntered the o 5 have disignated this a x k event", false, false);
                    }
                    if (Player.SCPKills <= 15 && playerList.Count >= 2)
                    {
                        if (!ToucanPlugin.Instance.Config.CanMedicMTFSpawn) return;
                        Exiled.API.Features.Player p = playerList[rnd.Next(0, playerList.Count)];
                        p.SetRole(RoleType.NtfLieutenant);
                        p.MaxHealth = 75;
                        p.MaxEnergy = 75;
                        p.MaxAdrenalineHealth = 300;
                        p.ClearInventory();
                        ToucanPlugin.Instance.Config.MTFMedicItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                        p.Position = MTFSpawnLocaltion;
                        p.Broadcast(5, $"< size = 60 > You are < color = #185ede><b>A MTF Medic</b></color></size>\n\n < i > Help the < color = \"cyan\" > MTF </ color > by healing them and giving them aid! </ i > ");
                    }
                }
                else if (ev.NextKnownTeam == Respawning.SpawnableTeamType.ChaosInsurgency)
                { // Is chaos spawn.
                    if (Player.SCPKills <= 15 && playerList.Count >= 2)
                    {
                        if (!ToucanPlugin.Instance.Config.CanChaosHackerSpawn) return;
                        Exiled.API.Features.Player p = playerList[rnd.Next(0, playerList.Count)];
                        p.MaxHealth = 75;
                        p.MaxEnergy = 75;
                        p.MaxAdrenalineHealth = 50; //AP
                        p.AdrenalineHealth = p.MaxAdrenalineHealth;
                        p.ClearInventory();
                        ToucanPlugin.Instance.Config.ChaosHackerItems.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                        p.Position = Map.GetRandomSpawnPoint(RoleType.ChaosInsurgency);
                        p.Broadcast(5, $"< size = 60 > You are < color = #2a6e02><b>A Chaos Hacker</b></color></size>\n\n < i > Help the < color = \"green\" > Chaos </ color > by hacking doors, your ahp is you ap! </ i > ");
                        mr.ChaosHacker.Add(p);
                        Task.Factory.StartNew(() => ChaosHackerCharge(p));
                    }
                }
            }
        }
        public void ChaosHackerCharge(Exiled.API.Features.Player p)
        {
            while (mr.ChaosHacker.Contains(p))
            {
                if (p.AdrenalineHealth + 1 > p.MaxAdrenalineHealth)
                {
                    p.AdrenalineHealth = p.MaxAdrenalineHealth;
                }
                else
                {
                    p.AdrenalineHealth = +1;
                }
                Thread.Sleep(100);
            }
        }
        public void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            string cmd = ev.Name;
            ev.Arguments.ForEach(arg => cmd += $" {arg}");
            if (!ev.Sender.IsHost) Tcp.Send($"slog **{ev.Sender.Nickname}** Sent:\n```{cmd}```");
        }
    }
}
