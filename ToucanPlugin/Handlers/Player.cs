using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToucanPlugin.Commands;
using ToucanPlugin.Gamemodes;
using UnityEngine;
using scp035;
using SerpentsHand;

namespace ToucanPlugin.Handlers
{
    class Player
    {
        readonly Tcp Tcp = new Tcp();
        public static bool Has008RandomSpawned { get; set; } = false;
        public static int SCPKills = 0;
        public static Dictionary<string, int> petConnections = new Dictionary<string, int>();
        readonly MessageResponder mr = new MessageResponder();
        public static List<Exiled.API.Features.Player> PlayersCrouchingList = new List<Exiled.API.Features.Player>();

        public void OnPreAuthenticating(PreAuthenticatingEventArgs ev)
        {
            if (!Whitelist.WhitelistUsers.Contains(ev.UserId) && Whitelist.Whitelisted)
            {
                //ev.Disallow();// ev.Player.Kick("Sorry the server is right now whitelisted. Come back later!");
                ev.RejectBanned("Sorry the server is right now closed. Come back later!", 0, false);
                Log.Info($"Denied Entry to the server to {ev.UserId}");
                return;
            }
        }
        public void OnJoin(JoinedEventArgs ev)
        {
            if (ToucanPlugin.Instance.Config.ReplaceAdvertismentNames)
            {
                List<string> PlayerNameSplit = new List<string>(ev.Player.Nickname.Split(' '));
                List<string> illigalNameParts = new List<string>(ToucanPlugin.Instance.Config.ADBlacklist);
                PlayerNameSplit.ForEach(part =>
                {
                    illigalNameParts.ForEach(NO =>
                    {
                        if (part.ToLower().Contains(NO))
                            ev.Player.DisplayNickname = ev.Player.Nickname.Replace(part, ToucanPlugin.Instance.Config.ReplaceAdvertismentNamesWhit);
                    });
                });
            }

            string message = ToucanPlugin.Instance.Config.JoinedMessage.Replace("{player}", ev.Player.Nickname);
            Map.Broadcast(2, message);
            Tcp.SendLog($"**{ev.Player.Nickname} ({ev.Player.UserId}) Joined [{Exiled.API.Features.Player.List.Count()}/20]**");

            //Top ranker Role
            if (mr.BestBois != null && mr.BestBois.Contains(ev.Player.UserId))
            {
                UserGroup topGroup = new UserGroup
                {
                    BadgeText = "Highest Ranker",
                    BadgeColor = "gold"
                };
                if (ev.Player.RankName != null)
                    ev.Player.SetRank("top", topGroup);
            }
            if (Exiled.API.Features.Player.List.Count() == 1 && !Round.IsStarted && ToucanPlugin.Instance.Config.LonelyRound)
                Task.Factory.StartNew(() => LonelyRound());
            UpdatePlayerList();
        }
        public void OnLeft(LeftEventArgs ev)
        {
            string message = ToucanPlugin.Instance.Config.LeftMessage.Replace("{player}", ev.Player.Nickname);
            Map.Broadcast(2, message);
            Tcp.SendLog($"**{ev.Player.Nickname} ({ev.Player.UserId}) Left [{Exiled.API.Features.Player.List.Count() - 1}/20]**");
            UpdatePlayerList(ev.Player.UserId);
            if (Exiled.API.Features.Player.List.Count() - 1 <= 0 && Round.IsStarted)
                Round.Restart();
            UpdateVoiceChannel(ev.Player, RoleType.None);
        }
        private void LonelyRound()
        {
            for (int i = 0; i < 60; i++)
            {
                if (Exiled.API.Features.Player.List.Count() == 1 && !Round.IsStarted && !Round.IsLobbyLocked)
                {
                    if (i == 59) Round.Start();
                    if (i == 30) Map.ShowHint("Automatic round Starting in...", 5);
                    if (i >= 35) Map.ShowHint($"{60 - i}", 1);
                    if (i == 59) Map.ShowHint($"Starting a really lonely round..!", 5);
                    Thread.Sleep(1200);
                }
                else return;
            }
        }
        public void UpdatePlayerList(string ExcludedId = "")
        {
            string playerList = "[";
            for (int i = 0; i <= Exiled.API.Features.Player.List.ToList().Count - 1; i++)
            {
                Exiled.API.Features.Player p = Exiled.API.Features.Player.List.ToList()[i];
                if (ExcludedId != p.UserId)
                {
                    string Coma = ",";
                    if (Exiled.API.Features.Player.List.ToList().Count - 1 == i || Exiled.API.Features.Player.List.ToList().Count - 1 >= i + 1 && Exiled.API.Features.Player.List.ToList()[i + 1].UserId == ExcludedId)
                        Coma = "";
                    playerList += $"{{\"id\":{p.Id},\"name\":\"{p.Nickname.Replace("\"", "")}\",\"userid\":\"{p.UserId}\"}}{Coma}";
                }
            }
            playerList += "]";
            Tcp.Send($"list {playerList}");
        }
        public void OnEscape(EscapingEventArgs ev)
        {
            bool classBool;
            if (ev.Player.Role == RoleType.ClassD)
                classBool = false;
            else //is scientist
                classBool = true;
            if (classBool == true) Tcp.Send($"stats {ev.Player.UserId} descapses 1");
            else
                Tcp.Send($"stats {ev.Player.UserId} sescapses 1");
            Map.Broadcast(4, $"{ev.Player.Nickname} escaped");
            string escapeMsg = $"escape {classBool} {ev.Player.UserId}";
            if (ev.Player.IsCuffed)
            {
                Exiled.API.Features.Player cuffer = Exiled.API.Features.Player.List.ToList().Find(x => x.Id.ToString().Contains(ev.Player.CufferId.ToString()));
                escapeMsg = $"{escapeMsg} {cuffer.UserId}";
                if (ev.Player.Role == RoleType.ClassD)
                    Tcp.Send($"stats {cuffer.UserId} escortedDclass 1");
                else
                    //Was a scientist
                    Tcp.Send($"stats {cuffer.UserId} escortScientist 1");
            }
            Tcp.Send(escapeMsg);
            Tcp.SendLog($"**{ev.Player.Nickname} ({ev.Player.UserId}) Escaped**");
        }
        public void OnDied(DiedEventArgs ev)
        {
            SpoopyGhosts.InvisScpDead(ev.Target);
            if (ev.Killer.Team == Team.SCP) SCPKills++;
            //if (mr.ChaosHacker.Contains(ev.Target)) mr.ChaosHacker.Remove(ev.Target); // Remove the chaos hacker things
            if (!Has008RandomSpawned && ToucanPlugin.Instance.Config.Random008Spawn)
            {
                if (ev.Target.Role != RoleType.ClassD || ev.Target.Role != RoleType.Scientist || ev.Target.Role != RoleType.FacilityGuard) return;
                System.Random rnd = new System.Random();
                if (rnd.Next(0, 50) == 1)
                {
                    ev.Target.SetRole(RoleType.Scp0492);
                    Has008RandomSpawned = true;
                    ev.Target.Broadcast(10, $"<color=red>You have been infected by scp 008 convert all humans</color>");
                    Cassie.Message($"biological infection at {ev.Target.CurrentRoom.Zone}");
                }
            }
            UpdateVoiceChannel(ev.Target, RoleType.Spectator);
            if (ev.Killer.UserId == ev.Target.UserId) return;
            bool isff = false;
            if (ev.Killer.Team == ev.Target.Team)
                isff = true;
            bool isTargetScp = false;
            if (ev.Target.Team == Team.SCP || scp035.API.Scp035Data.GetScp035() == ev.Killer)
                isTargetScp = true;
            Tcp.Send($"died {isff} {ev.Target.UserId} {ev.Killer.UserId} {ev.HitInformations.Tool} {isTargetScp}");
            Tcp.SendLog($"{ev.Target.Nickname} ({ev.Target.UserId}) killed by {ev.Killer.Nickname} ({ev.Killer.UserId}) whit {ev.HitInformations.GetDamageName()}");
            switch (ev.Killer.Team)
            {
                case Team.SCP:
                    Tcp.Send($"stats {ev.Killer.UserId} scpkills 1");
                    break;
                case Team.MTF:
                    Tcp.Send($"stats {ev.Killer.UserId} mtfkills 1");
                    break;
                case Team.CHI:
                    Tcp.Send($"stats {ev.Killer.UserId} chaoskills 1");
                    break;
                case Team.RSC:
                    Tcp.Send($"stats {ev.Killer.UserId} scikills 1");
                    break;
                case Team.CDP:
                    Tcp.Send($"stats {ev.Killer.UserId} dclasskills 1");
                    break;
                case Team.TUT:
                    Tcp.Send($"stats {ev.Killer.UserId} scikills 1");
                    break;
            }
            switch (ev.Target.Team)
            {
                case Team.SCP:
                    Tcp.Send($"stats {ev.Killer.UserId} scpkilled 1");
                    switch (ev.Target.Role)
                    {
                        case RoleType.Scp173:
                            Tcp.Send($"stats {ev.Killer.UserId} killed173 1");
                            break;
                        case RoleType.Scp106:
                            Tcp.Send($"stats {ev.Killer.UserId} killed106 1");
                            break;
                        case RoleType.Scp049:
                            Tcp.Send($"stats {ev.Killer.UserId} killed049 1");
                            break;
                        case RoleType.Scp0492:
                            Tcp.Send($"stats {ev.Killer.UserId} killed0492 1");
                            break;
                        case RoleType.Scp096:
                            Tcp.Send($"stats {ev.Killer.UserId} killed096 1");
                            break;
                        case RoleType.Scp93953:
                            Tcp.Send($"stats {ev.Killer.UserId} killed939 1");
                            break;
                        case RoleType.Scp93989:
                            Tcp.Send($"stats {ev.Killer.UserId} killed939 1");
                            break;
                    }
                    Tcp.Send($"stats {ev.Target.UserId} scpdeaths 1");
                    break;
                case Team.MTF:
                    Tcp.Send($"stats {ev.Killer.UserId} mtfkilled 1");
                    Tcp.Send($"stats {ev.Target.UserId} mtfdeaths 1");
                    break;
                case Team.CHI:
                    Tcp.Send($"stats {ev.Killer.UserId} chaoskilled 1");
                    Tcp.Send($"stats {ev.Target.UserId} chaosdeaths 1");
                    break;
                case Team.RSC:
                    Tcp.Send($"stats {ev.Killer.UserId} scikilled 1");
                    Tcp.Send($"stats {ev.Target.UserId} scideaths 1");
                    break;
                case Team.CDP:
                    Tcp.Send($"stats {ev.Killer.UserId} dclasskilled 1");
                    Tcp.Send($"stats {ev.Target.UserId} dclassdeaths 1");
                    break;
            }
            if (SerpentsHand.API.SerpentsHand.GetSHPlayers().Contains(ev.Target) || scp035.API.Scp035Data.GetScp035() == ev.Target)
            {
                Tcp.Send($"stats {ev.Killer.UserId} scpkilled 1");
                Tcp.Send($"stats {ev.Target.UserId} scpdeaths 1");
            }

            //Event bullshit
            switch (GamemodeLogic.RoundGamemode)
            {
                case GamemodeType.PeanutInfection:
                    ev.Target.SetRole(RoleType.Scp173);
                    break;
            }
            if (ev.Target.Role == RoleType.Scp173 && ev.Killer.CurrentItem.id == ItemType.MicroHID)
                Tcp.Send($"stats {ev.Killer.UserId} microedNuts 1");
        }
        public void OnSpawned(SpawningEventArgs ev)
        {
            System.Random rnd = new System.Random();
            Exiled.API.Features.Player p = ev.Player;
            List<Exiled.API.Features.Player> playerList = new List<Exiled.API.Features.Player>((IEnumerable<Exiled.API.Features.Player>)Exiled.API.Features.Player.List.ToList());
            ToucanPlugin.Instance.Config.CustomPersonel.ForEach(per =>
            {
                if (!per.Enabled || per.PlayerCount < Exiled.API.Features.Player.List.Count()) return;
                if (per.Role.GetTeam() == Team.MTF || per.Role.GetTeam() == Team.CHI && SCPKills >= per.MaxSCPKills || SCPKills < per.MinSCPKills) return;
                if (per.Role == ev.RoleType && rnd.Next(per.ReplaceChance, 100) <= per.ReplaceChance)
                    p.SetRole(per.Role);
                else return;
                if (per.MaxHealth != -1) p.MaxHealth = per.MaxHealth;
                if (per.MaxAdrenalin != -1) p.MaxAdrenalineHealth = per.MaxAdrenalin;
                if (per.MaxEnergy != -1) p.MaxEnergy = per.MaxEnergy;
                per.Items.ForEach(item => p.Inventory.AddNewItem((ItemType)item));
                if (per.PreSetSpawnPosRole != RoleType.None)
                    p.Position = per.PreSetSpawnPosRole.GetRandomSpawnPoint();
                else if (per.PreSetSpawnPosRoom != RoomType.Unknown)
                    p.Position = Map.Rooms.ToList().Find(x => x.Type == per.PreSetSpawnPosRoom).Position;
                else
                    p.Position = new Vector3(per.SpawnPos.X, per.SpawnPos.Y, per.SpawnPos.Z);
                p.ShowHint(per.Hint, 6);
            });
        }
        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!mr.ChaosHacker.Contains(ev.Player)) return;
            if (ev.IsAllowed) return;
            float ap = ev.Player.AdrenalineHealth;
            float apCost = 0;
            if (ev.Door.destroyed) return;
            switch (ev.Door.Type())
            {
                case DoorType.NukeSurface:
                    apCost = 110;
                    break;
                case DoorType.GateA:
                    apCost = 75;
                    break;
                case DoorType.GateB:
                    apCost = 75;
                    break;
                case DoorType.Scp106Primary:
                    apCost = 85;
                    break;
                case DoorType.Scp106Secondary:
                    apCost = 85;
                    break;
                case DoorType.Scp106Bottom:
                    apCost = 85;
                    break;
                case DoorType.Scp914:
                    apCost = 25;
                    break;
                case DoorType.Scp012:
                    apCost = 25;
                    break;
                case DoorType.LczArmory:
                    apCost = 30;
                    break;
                case DoorType.HczArmory:
                    apCost = 30;
                    break;
                case DoorType.NukeArmory:
                    apCost = 55;
                    break;
                case DoorType.Scp049Armory:
                    apCost = 55;
                    break;
                case DoorType.HID:
                    apCost = 90;
                    break;
                case DoorType.CheckpointEntrance:
                    apCost = 35;
                    break;
                case DoorType.CheckpointLczA:
                    apCost = 35;
                    break;
                case DoorType.CheckpointLczB:
                    apCost = 35;
                    break;
                case DoorType.Intercom:
                    apCost = 40;
                    break;
                case DoorType.Scp079First:
                    apCost = 45;
                    break;
                case DoorType.Scp079Second:
                    apCost = 45;
                    break;
                case DoorType.Scp096:
                    apCost = 40;
                    break;
            }
            if (apCost == 0) return;
            if (ap < apCost) ev.Player.Broadcast(2, $"Need {ap - apCost} more ap to open that door!");
            else
            {
                _ = ap - apCost;
                if (ev.Door.isOpen) ev.Door.isOpen = false;
                else
                    ev.Door.isOpen = true;
            }
        }
        public void OnBanned(BannedEventArgs ev) =>
            Tcp.SendLog($"```{ev.Details.Issuer} ({ev.Details.OriginalName}) banned player {ev.Player.Nickname} ({ev.Player.UserId}). Ban duration: {ev.Details.Expires}. Reason: {ev.Details.Reason}.```");
        public void OnKicked(KickedEventArgs ev) =>
            Tcp.SendLog($"```{ev.Player.Nickname} ({ev.Player.UserId}) has been kicked. Reason: {ev.Reason}```");
        public void OnMedicalItemUsed(UsedMedicalItemEventArgs ev)
        {
            if (ev.Item == ItemType.SCP207) Tcp.Send($"stats {ev.Player.UserId} cokedrunk 1");
            int dmgHealed = 0;
            switch (ev.Item)
            {
                case ItemType.Painkillers:
                    dmgHealed = 45;
                    break;

                case ItemType.Medkit:
                    dmgHealed = 65;
                    break;

                case ItemType.Adrenaline:
                    dmgHealed = 50;
                    break;

                case ItemType.SCP500:
                    dmgHealed = ev.Player.MaxHealth;
                    break;

                case ItemType.SCP207:
                    dmgHealed = 30;
                    break;
            }
            if (dmgHealed != 0)
                Tcp.Send($"stats {ev.Player.UserId} dmghealed {dmgHealed}");
            System.Random rnd = new System.Random();
            if (ev.Item == ItemType.Adrenaline && rnd.Next(0, 100) == 1)
                if (ev.Player.Health - dmgHealed < 0)
                {
                    ev.Player.Kill();
                    ev.Player.ShowHint("<b>F</b>", 5);
                }
                else
                    ev.Player.Health = ev.Player.Health - dmgHealed;
        }
        public void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            if (ev.Type == GrenadeType.Flashbang)
            {
                Tcp.SendLog($"{ev.Player.Nickname} ({ev.Player.UserId}) threw a Flashbang!");
                Tcp.Send($"stats {ev.Player.UserId} flashThrown 1");
            }
            if (ev.Type == GrenadeType.FragGrenade)
            {
                Tcp.SendLog($"{ev.Player.Nickname} ({ev.Player.UserId}) threw a Grenade!");
                Tcp.Send($"stats {ev.Player.UserId} grenadeThrown 1");
            }
            if (ev.Type == GrenadeType.Scp018)
            {
                Tcp.SendLog($"{ev.Player.Nickname} ({ev.Player.UserId}) threw a SCP-018!");
                Tcp.Send($"stats {ev.Player.UserId} scp018Thrown 1");
            }
        }
        public void OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev)
        {
            UpdateVoiceChannel(ev.Player, RoleType.Spectator);
            List<Exiled.API.Features.Player> playerList = new List<Exiled.API.Features.Player>((IEnumerable<Exiled.API.Features.Player>)Exiled.API.Features.Player.List.ToList());
            if (playerList.Find(x => x.Role == RoleType.Scp106) != null)
                Tcp.Send($"stats {ev.Player.UserId} femur 1");
            else
                ev.Player.ShowHint("<i>Well that was a waste...</i>");
        }
        public void OnHurting(HurtingEventArgs ev)
        {
            if (ev.Target.Role == RoleType.Scp106 && ToucanPlugin.Instance.Config.Force106Femur && !Warhead.IsDetonated && ev.Attacker.CurrentItem.id == ItemType.MicroHID)
                if (ev.Target.Health < ev.Target.MaxHealth)
                    ev.Target.Health = ev.Target.Health + ev.Amount;
                else
                    ev.Target.Health = ev.Target.MaxHealth;
            if (ev.Target.Side == ev.Attacker.Side && ev.Target != ev.Attacker && scp035.API.Scp035Data.GetScp035() != ev.Attacker && scp035.API.Scp035Data.GetScp035() != ev.Target && !ev.Attacker.Sender.CheckPermission(PlayerPermissions.FriendlyFireDetectorImmunity) && !Exiled.API.Features.Server.FriendlyFire && ReflectControl.Reflect)
            {
                if (ev.Target.Health < ev.Target.MaxHealth)
                    ev.Target.Health = ev.Target.Health + ev.Amount;
                else
                    ev.Target.Health = ev.Target.MaxHealth;
                if (!ev.Attacker.IsGodModeEnabled)
                {
                    ev.Attacker.Health = ev.Attacker.Health - ev.Amount;
                    if (ev.Attacker.Health <= 0) ev.Attacker.Kill();
                }
                ev.Attacker.Broadcast(1, $"Dmg reflected! (Reflector: {ev.Target.Nickname})");
            }
        }
        public void OnChangingRole(ChangingRoleEventArgs ev) =>
            UpdateVoiceChannel(ev.Player, ev.NewRole);
        public Team GetTeam(Exiled.API.Features.Player p, RoleType Role = RoleType.None)
        {
            if (Role == RoleType.None) Role = p.Role;
            Team PlayerTeam = Team.RIP;
            if (Role.GetSide() == Side.Tutorial) PlayerTeam = Team.TUT;
            if (Role.GetSide() == Side.Scp) PlayerTeam = Team.SCP;
            if (Role.GetTeam() == Team.RSC) PlayerTeam = Team.RSC;
            if (Role.GetTeam() == Team.MTF) PlayerTeam = Team.MTF;
            if (Role.GetTeam() == Team.CDP) PlayerTeam = Team.CDP;
            if (Role.GetTeam() == Team.CHI) PlayerTeam = Team.CHI;
            if (SerpentsHand.API.SerpentsHand.GetSHPlayers().Contains(p)) PlayerTeam = Team.SCP;
            if (scp035.API.Scp035Data.GetScp035() == p) PlayerTeam = Team.SCP;
            return PlayerTeam;
        }
        public void UpdateVoiceChannel(Exiled.API.Features.Player p, RoleType NewRole = RoleType.Spectator)
        {
            if (NewRole != RoleType.None)
                Tcp.Send($"vc {p.UserId} {GetTeam(p, NewRole)}");
            else Tcp.Send($"vc {p.UserId} WAITING");
        }
        public void UpdateVoiceChannels()
        {
            Exiled.API.Features.Player.List.ToList().ForEach(p =>
            Tcp.Send($"vc {p.UserId} {GetTeam(p)}"));
        }
        public void OnTriggeringTesla(TriggeringTeslaEventArgs ev)
        {
            if (ev.Player.IsNTF)
                ev.IsTriggerable = false;
            if (ev.Player.Role == RoleType.Scp0492 && ev.IsInHurtingRange)
                ev.Player.Position = new Vector3(ev.Player.Position.x + 5, ev.Player.Position.y, ev.Player.Position.z);
        }
        public void StartDetectingCrouching()
        {
            Task.Factory.StartNew(() =>
            {
                if (!ToucanPlugin.Instance.Config.CrouchingEnabled) return;
                while (true)
                    Exiled.API.Features.Player.List.ToList().ForEach(p =>
                    {
                        if (p.MoveState == PlayerMovementState.Sneaking && !PlayersCrouchingList.Contains(p))
                        {
                            p.Scale = new Vector3(ToucanPlugin.Instance.Config.CrouchingSize.X, ToucanPlugin.Instance.Config.CrouchingSize.Y, ToucanPlugin.Instance.Config.CrouchingSize.Z);
                            PlayersCrouchingList.Add(p);
                        }
                        else if (p.MoveState != PlayerMovementState.Sneaking && PlayersCrouchingList.Contains(p))
                        {
                            p.Scale = new Vector3(1, 1, 1);
                            PlayersCrouchingList.Remove(p);
                        }
                    });
            });
        }
    }
}
