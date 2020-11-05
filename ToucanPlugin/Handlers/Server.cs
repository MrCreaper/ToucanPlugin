using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToucanPlugin.Commands;
using ToucanPlugin.Gamemodes;
using UnityEngine;

namespace ToucanPlugin.Handlers
{
    public class XYZ : IEquatable<XYZ>
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public bool Equals(XYZ other) // Why is this here?!
        {
            if (other == null) return false;
            return (this.X.Equals(other.X));
        }
    }
    public class CustomSquadSpawns : IEquatable<CustomSquadSpawns>
    {
        public string Name { get; set; } // Just for the user
        public Respawning.SpawnableTeamType Team { get; set; }
        public int ReplaceChance { get; set; } // Chance 1-100 for [name] to spawn insteed of [team] (0 for scpKill)
        public RoleType Role { get; set; }
        public int MaxHealth { get; set; }
        public int MaxAdrenalin { get; set; }
        public int MaxEnergy { get; set; }
        public List<int> Items { get; set; }
        public List<int> CommanderItems { get; set; }
        public RoleType PreSetSpawnPos { get; set; }
        public XYZ SpawnPos { get; set; }
        public int SquadMaxSize { get; set; }
        public int MaxSCPKills { get; set; }
        public int MinSCPKills { get; set; }
        public string CassieAnnc { get; set; }

        public bool Equals(CustomSquadSpawns other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        // Should also override == and != operators.
    }
    public class CustomPersonelSpawns : IEquatable<CustomPersonelSpawns>
    {
        public string Name { get; set; } // Just for the user
        public RoleType Role { get; set; }
        public int PlayerCountNeeded { get; set; }
        public int ReplaceChance { get; set; } // Chance 1-100 for [name] to spawn insteed of [team] (0 for scpKill)
        public int MaxHealth { get; set; }
        public int MaxEnergy { get; set; }
        public int MaxAdrenalin { get; set; }
        public List<int> Items { get; set; }
        public RoleType PreSetSpawnPos { get; set; }
        public RoomType PreSetSpawnPosRoom { get; set; }
        public XYZ SpawnPos { get; set; }
        public string Hint { get; set; }

        public bool Equals(CustomPersonelSpawns other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }
        // Should also override == and != operators.
    }
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
            Tcp.SendLog("Waiting for players...");
        }

        public void OnRoundStarted()
        {
            Tcp.SendLog("Round started");
            if (AcGame.NextGamemode == GamemodeType.None)
                Map.Broadcast(5, ToucanPlugin.Instance.Config.RoundStartMessage);
            else
                Map.Broadcast(5, $"Next Round Gamemode: <i><b>{AcGame.RoundGamemode}</b></i>");
            if (AcGame.RoundGamemode == GamemodeType.None)
                Map.Broadcast(5, ToucanPlugin.Instance.Config.RoundStartMessage);
            else
                Map.Broadcast(5, $"Gamemode: <i><b>{AcGame.RoundGamemode}</b></i>");
            if (rnd.Next(0, 3) == 0 && Exiled.API.Features.Player.List.ToList().Find(x => x.Role == RoleType.Scp173) != null)
                Map.Doors.ToList().Find(x => x.DoorName == "173").locked = true; // Lock 173 (1162)
            if (AcGame.GamemodesPaused) return;
            int CountChances = 0;
            CountChances += ToucanPlugin.Instance.Config.GamemodeChances[0].AmongUs;
            CountChances += ToucanPlugin.Instance.Config.GamemodeChances[0].CandyRush;
            CountChances += ToucanPlugin.Instance.Config.GamemodeChances[0].PeanutInfection;
            CountChances += ToucanPlugin.Instance.Config.GamemodeChances[0].QuietPlace;
            if (CountChances != 100)
                Log.Warn($"Gamemode chances do NOT add up to 100.");
            else
            {
                if (rnd.Next(0, 101) > ToucanPlugin.Instance.Config.RandomGamemodeChance) return;
                int GAMEMODE = rnd.Next(0, 101);
                int RandomCounter = 0;
                if (GAMEMODE <= ToucanPlugin.Instance.Config.GamemodeChances[0].AmongUs)
                    AcGame.NextGamemode = GamemodeType.AmongUs;
                else
                {
                    RandomCounter = +ToucanPlugin.Instance.Config.GamemodeChances[0].AmongUs;
                    if (GAMEMODE <= ToucanPlugin.Instance.Config.GamemodeChances[0].CandyRush + RandomCounter)
                        AcGame.NextGamemode = GamemodeType.CandyRush;
                    else
                    {
                        RandomCounter = +ToucanPlugin.Instance.Config.GamemodeChances[0].CandyRush;
                        if (GAMEMODE <= ToucanPlugin.Instance.Config.GamemodeChances[0].PeanutInfection + RandomCounter)
                            AcGame.NextGamemode = GamemodeType.PeanutInfection;
                        else
                        {
                            RandomCounter = +ToucanPlugin.Instance.Config.GamemodeChances[0].CandyRush;
                            if (GAMEMODE <= ToucanPlugin.Instance.Config.GamemodeChances[0].QuietPlace + RandomCounter)
                                AcGame.NextGamemode = GamemodeType.QuitePlace;
                            else
                                AcGame.NextGamemode = GamemodeType.None;
                        }
                    }
                }
            }
        }

        public void OnRestartingRound() =>
            Tcp.SendLog($"Round restarting...");
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (ToucanPlugin.Instance.Config.DetonateAtRoundEnded && Warhead.IsDetonated) Warhead.Detonate();
            Tcp.SendLog($"Round Ended\n```Winning Class: {ev.LeadingTeam}\nEscaped D-Class: {ev.ClassList.class_ds}\nRescued Scientists: {ev.ClassList.scientists}\nContained SCPs: {ev.ClassList.scps_except_zombies}\nWas warhead detonated: {Exiled.API.Features.Warhead.IsDetonated}\nKilled by warhead: {ev.ClassList.warhead_kills}```");
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
            });

            int Count0492 = 0;
            int Count939 = 0;
            int CountChaos = 0;
            Exiled.API.Features.Player DocPetReciver = null;
            Exiled.API.Features.Player DogPetReciver = null;
            Exiled.API.Features.Player.List.ToList().ForEach(p =>
            {
                if (p.Role == RoleType.Scp0492)
                {
                    Count0492++;
                    DocPetReciver = p;
                }
                if (p.Role == RoleType.Scp93953 || p.Role == RoleType.Scp93989)
                {
                    Count939++;
                    DogPetReciver = p;
                }
                if (p.Role == RoleType.ChaosInsurgency)
                    CountChaos++;
            });
            if (Count0492 == 1 && DocPetReciver != null)
                Tcp.Send($"stats {DocPetReciver.UserId} ZOMBPET");
            if (CountChaos > 3 && Count939 == 1 && DogPetReciver != null)
                Tcp.Send($"stats {DogPetReciver.UserId} DOGPET");
            switch (AcGame.RoundGamemode)
            {
                case GamemodeType.PeanutInfection:
                    int SurvivorsAlive = 0;
                    int PeanutsAlive = 0;
                    Exiled.API.Features.Player.List.ToList().ForEach(p =>
                    {
                        if (RealPeanutInfection.DClass.Contains(p) && p.IsAlive)
                            SurvivorsAlive++;
                        else
                        if (RealPeanutInfection.Nuts.Contains(p) && p.IsAlive)
                            PeanutsAlive++;
                    });
                    if (SurvivorsAlive == PeanutsAlive)
                        Map.Broadcast(6, $"ITS A TIE?!");
                    else
                    if (SurvivorsAlive > PeanutsAlive)
                    {
                        RealPeanutInfection.DClass.ForEach(p => Tcp.Send($"eventWin {p.UserId} 0 PeanutInfection"));
                        Map.Broadcast(6, $"<color=orange>SURVIVORS WIN!</color>");
                    }
                    else
                    if (SurvivorsAlive < PeanutsAlive)
                    {
                        RealPeanutInfection.Nuts.ForEach(p => Tcp.Send($"eventWin {p.UserId} 1 PeanutInfection"));
                        Map.Broadcast(6, $"<color=red>PEANUTS WIN!</color>");
                    }
                    break;

                    /*case GamemodeType.CandyRush:
                        int DefusersAlive = 0;
                        int BommbersAlive = 0;
                        Exiled.API.Features.Player.List.ToList().ForEach(p =>
                        {
                            if (CandyRush.DefuserList.Contains(p.UserId) && p.IsAlive)
                                DefusersAlive++;
                            else
                            if (CandyRush.BommerList.Contains(p.UserId) && p.IsAlive)
                                BommbersAlive++;
                        });
                        if (DefusersAlive == BommbersAlive)
                            Map.Broadcast(6, $"ITS A TIE?!");
                        else
                        if (DefusersAlive > BommbersAlive)
                        {
                            CandyRush.DefuserList.ForEach(id => Tcp.Send($"eventWin {id} 0 CandyRush"));
                            Map.Broadcast(6, $"<color=cyan>DEFUSERS WIN!</color>");
                        }
                        else
                        if (DefusersAlive < BommbersAlive)
                        {
                            CandyRush.BommerList.ForEach(id => Tcp.Send($"eventWin {id} 1 CandyRush"));
                            Map.Broadcast(6, $"<color=red>BOMMERS WIN!</color>");
                        }
                        break;*/
            }
            Player.Has008RandomSpawned = false;
            Player.SCPKills = 0;
            SpecMode.TUTSpecList = null;
            mr.ChaosHacker = null;
            RealPeanutInfection.DClass.Clear();
            RealPeanutInfection.Nuts.Clear();
        }
        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            SpecMode.TUTSpecList.ForEach(id => ev.Players.Add(Exiled.API.Features.Player.List.ToList().Find(x => x.Id.ToString() == id)));
            SpecMode.TUTSpecList.Clear();
            List<Exiled.API.Features.Player> playerList = new List<Exiled.API.Features.Player>(ev.Players);
            Vector3 MTFSpawnLocaltion = new Vector3(0, 0, 0);
            ToucanPlugin.Instance.Config.CustomSquads.ForEach(s =>
            {
                if (s.Team != ev.NextKnownTeam && rnd.Next(s.ReplaceChance, 100) > s.ReplaceChance && s.MaxSCPKills < Player.SCPKills && s.MinSCPKills > Player.SCPKills) return;
                ev.Players.Clear();
                playerList.ForEach(p =>
                {
                    p.SetRole(s.Role);
                    if (s.MaxHealth != -1) p.MaxHealth = s.MaxHealth;
                    if (s.MaxAdrenalin != -1) p.MaxAdrenalineHealth = s.MaxAdrenalin;
                    if (s.MaxEnergy != -1) p.MaxEnergy = s.MaxEnergy;
                    s.Items.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                    if (s.PreSetSpawnPos != RoleType.None)
                        p.Position = s.PreSetSpawnPos.GetRandomSpawnPoint();
                    else
                        p.Position = new Vector3(s.SpawnPos.X, s.SpawnPos.Y, s.SpawnPos.Z);
                });
                int leaderIndex = rnd.Next(0, playerList.Count);
                if (s.CommanderItems.Count != 0)
                {
                    Exiled.API.Features.Player Commander = playerList[leaderIndex];
                    Commander.ClearInventory();
                    s.CommanderItems.ForEach(item => Commander.Inventory.AddNewItem((ItemType)item));
                    Commander.ShowHint($"<i>You are the Commander of {s.Name}, you have something special in your inventory</i>", 6);
                }
            });
            if (ev.NextKnownTeam == Respawning.SpawnableTeamType.NineTailedFox)
            {
                if (Player.SCPKills <= 15 && playerList.Count >= 2 && ToucanPlugin.Instance.Config.CanMedicMTFSpawn)
                { // MTF Medic
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
                if (Player.SCPKills <= 15 && playerList.Count >= 2 && ToucanPlugin.Instance.Config.CanChaosHackerSpawn)
                {
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
        public void ChaosHackerCharge(Exiled.API.Features.Player p)
        {
            while (mr.ChaosHacker.Contains(p))
            {
                if (p.AdrenalineHealth + 1 > p.MaxAdrenalineHealth)
                    p.AdrenalineHealth = p.MaxAdrenalineHealth;
                else
                    p.AdrenalineHealth = +1;
                Thread.Sleep(100);
            }
        }
        public void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            string cmd = ev.Name;
            ev.Arguments.ForEach(arg => cmd += $" {arg}");
            if (!ev.Sender.IsHost) Tcp.Send($"slog [{DateTime.Now}] **{ev.Sender.Nickname}** Sent:\n```{cmd}```");
        }
    }
}
