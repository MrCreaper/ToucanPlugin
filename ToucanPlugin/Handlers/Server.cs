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
    public class XYZ
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3 ToVector3(XYZ xyz)
        {
            return new Vector3(xyz.X, xyz.Y, xyz.Z);
        }
    }
    public class PlayerCountMentionsClass
    {
        public int PlayerCount { get; set; }
        public string RoleID { get; set; }
    }
    public enum AbilityType
    {
        None = 0,
        HealingGrenade = 1,
        DoorHacking = 2,
        IcomDisabling = 3,
        Blackout = 4,
        RadioDisabling = 4,
    }
    public class CustomSquadSpawns
    {
        public bool Enabled { get; set; }
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
    }
    public class CustomPersonelSpawns
    {
        public bool Enabled { get; set; }
        public string Name { get; set; } // Just for the user
        public RoleType Role { get; set; }
        public int PlayerCount { get; set; }
        public int MaxSCPKills { get; set; }
        public int MinSCPKills { get; set; }
        public int ReplaceChance { get; set; } // Chance 1-100 for [name] to spawn insteed of [team] (0 for scpKill)
        public int MaxHealth { get; set; }
        public int MaxEnergy { get; set; }
        public int MaxAdrenalin { get; set; }
        public List<int> Items { get; set; }
        public RoleType PreSetSpawnPosRole { get; set; }
        public RoomType PreSetSpawnPosRoom { get; set; }
        public XYZ SpawnPos { get; set; }
        public string Hint { get; set; }
        public List<AbilityType> Abilities { get; set; }
    }
    class GamemodeChanceArrayThing
    {
        public GamemodeType Gamemode { get; set; }
        public int Num { get; set; }
    }
    class Server
    {
        readonly System.Random rnd = new System.Random();
        readonly Tcp Tcp = new Tcp();
        public Dictionary<int, bool> LastPlayerCountMentions { get; set; } = new Dictionary<int, bool>();
        public Dictionary<string, bool> LastPlayerRoleMentions { get; set; } = new Dictionary<string, bool>();
        public void OnWaitingForPlayers()
        {
            if (GamemodeLogic.NextGamemode != GamemodeType.None)
            {
                GamemodeLogic.RoundGamemode = GamemodeLogic.NextGamemode;
                GamemodeLogic.NextGamemode = 0;
            }
            Tcp.SendLog("Waiting for players...");
            Player.PlayersCrouchingList.Clear();
        }

        public void OnRoundStarted()
        {
            GamemodeLogic gl = new GamemodeLogic();
            Tcp.SendLog("Round started");
            if (rnd.Next(0, 3) == 0 && Exiled.API.Features.Player.List.ToList().Find(x => x.Role == RoleType.Scp173) != null)
                Map.Rooms.ToList().Find(x => x.Type == RoomType.Lcz173).Doors.ToList()[0].locked = true; // Lock 173 (1162)
            new MessageResponder().UpdateMap();
            if (!GamemodeLogic.GamemodesPaused)
            {
                if (GamemodeLogic.RoundGamemode == GamemodeType.None)
                    Map.Broadcast(5, ToucanPlugin.Instance.Config.RoundStartMessage);
                else
                    Map.Broadcast(5, $"Gamemode: <i><b>{gl.ConvertToNice(GamemodeLogic.RoundGamemode)}</b></i>");
                if (GamemodeLogic.NextGamemode == GamemodeType.None)
                    Map.Broadcast(5, ToucanPlugin.Instance.Config.RoundStartMessage);
                else
                    Map.Broadcast(5, $"Next Round Gamemode: <i><b>{gl.ConvertToNice(GamemodeLogic.RoundGamemode)}</b></i>");

                int TotalGamemodeChance = 0;
                int NoGamemodeChance = 0;
                ToucanPlugin.Instance.Config.GamemodeChances.ToList().ForEach(g => {
                    TotalGamemodeChance += g.Value;
                    NoGamemodeChance += 100 - g.Value;
                    });
                int RandomGamemodeNumber = rnd.Next(0, TotalGamemodeChance + NoGamemodeChance);
                int i = 0;
                if (RandomGamemodeNumber > TotalGamemodeChance)
                    ToucanPlugin.Instance.Config.GamemodeChances.ToList().ForEach(g =>
                {
                    if (i >= RandomGamemodeNumber)
                        GamemodeLogic.NextGamemode = g.Key;
                    else
                        i += g.Value;
                });
                ToucanPlugin.Instance.Config.PlayerCountMentions.ToList().ForEach(r =>
                {
                    if (Whitelist.Whitelisted) return;
                    if (!LastPlayerCountMentions[r.PlayerCount] && Exiled.API.Features.Player.List.ToList().Count == r.PlayerCount)
                    {
                        Tcp.SendLog($"{r.PlayerCount} PLAYERS <@&{r.RoleID}>");
                        LastPlayerCountMentions[r.PlayerCount] = true;
                    }
                    if (LastPlayerCountMentions[r.PlayerCount] && Exiled.API.Features.Player.List.ToList().Count < r.PlayerCount)
                        LastPlayerCountMentions[r.PlayerCount] = false;
                });
            }
        }

        public void OnRestartingRound() =>
            Tcp.SendLog($"Round restarting...");
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            if (ToucanPlugin.Instance.Config.DetonateAtRoundEnded && Warhead.IsDetonated)
            {
                Warhead.Detonate();
                Warhead.DetonationTimer = ev.TimeToRestart;
            }
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
            switch (GamemodeLogic.RoundGamemode)
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
            Handlers.Player.perConnections.Clear();
            RealPeanutInfection.DClass.Clear();
            RealPeanutInfection.Nuts.Clear();
        }
        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.Players.Count == 0) return;
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
                if (s.CommanderItems.Count != 0)
                {
                    Exiled.API.Features.Player Commander = playerList[rnd.Next(0, playerList.Count)];
                    Commander.ClearInventory();
                    s.CommanderItems.ForEach(item => Commander.Inventory.AddNewItem((ItemType)item));
                    Commander.ShowHint($"<i>You are the <color=yellow>Commander</color> of <color=yellow>{s.Name}</color></i>", 6);
                }
            });
        }
        public void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            string cmd = ev.Name;
            ev.Arguments.ForEach(arg => cmd += $" {arg}");
            if (!ev.Sender.IsHost) Tcp.Send($"slog [{DateTime.Now}] **{ev.Sender.Nickname}** ({ev.Sender.UserId}) Sent:\n```{cmd}```");
        }
        public static bool LastLights = false;
        public void StartDetectBlackout()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        if (SCP_575.Plugin.TimerOn != LastLights)
                        {
                            Log.Warn($"LIGHTS {SCP_575.Plugin.TimerOn}");
                            Tcp.Send($"blackout {SCP_575.Plugin.TimerOn}");
                            LastLights = SCP_575.Plugin.TimerOn;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"ERROR WHILE DETECTING BLACKOUT: {e}");
                        return;
                    }
                }
            });
        }
    }
}
