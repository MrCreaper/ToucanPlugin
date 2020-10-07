using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using ToucanPlugin.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToucanPlugin.Gamemodes;
using scp035;
using SerpentsHand;

namespace ToucanPlugin.Handlers
{
    class Player
    {
        readonly Tcp Tcp = new Tcp();
        public static bool Has008RandomSpawned { get; set; } = false;
        public static int SCPKills = 0;
        readonly MessageResponder mr = new MessageResponder();
        readonly Whitelist wl = new Whitelist();
        public void OnLeft(LeftEventArgs ev)
        {
            string message = ToucanPlugin.Instance.Config.LeftMessage.Replace("{player}", ev.Player.Nickname);
            Map.Broadcast(2, message);
            Tcp.Send($"log **{ev.Player.Nickname} ({ev.Player.UserId}) Left [{Exiled.API.Features.Player.List.Count() - 1}/20]**");
            if (Exiled.API.Features.Player.List.Count() - 1 == 0 && Round.IsStarted)
                Round.Restart();
        }

        public void OnJoin(JoinedEventArgs ev)
        {
            if (wl.Whitelisted)
                if (!wl.WhitelistUsers.Contains(ev.Player.UserId))
                    ev.Player.Kick("Sorry the server is right now whitelisted. Come back later!");
            if (ToucanPlugin.Instance.Config.ReplaceAdvertismentNames)
            {
                List<string> PlayerNameSplit = new List<string>(ev.Player.Nickname.Split(' '));
                PlayerNameSplit.ForEach(part =>
                {
                    if (part.ToLower().Contains(".com") || part.Contains(".tf") || part.ToLower().Contains("ttv/") || part.Contains("YT"))
                    {
                        ev.Player.DisplayNickname = ev.Player.Nickname.Replace(part, ToucanPlugin.Instance.Config.ReplaceAdvertismentNamesWhit);
                    }
                });
            }

            string message = ToucanPlugin.Instance.Config.JoinedMessage.Replace("{player}", ev.Player.Nickname);
            Map.Broadcast(2, message);
            if (ToucanPlugin.Instance.Config.MentionRoles)
            {
                if (Exiled.API.Features.Player.List.Count() == 5)
                    Tcp.Send($"log 5 PLAYERS <@&{ToucanPlugin.Instance.Config.FivePlayerRole}>");
                if (Exiled.API.Features.Player.List.Count() == 10)
                    Tcp.Send($"log 10 PLAYERS <@&{ToucanPlugin.Instance.Config.TenPlayerRole}>");
                if (Exiled.API.Features.Player.List.Count() == 15)
                    Tcp.Send($"log 15 PLAYERS <@&{ToucanPlugin.Instance.Config.FifteenPlayerRole}>");
            }
            Tcp.Send($"log **{ev.Player.Nickname} ({ev.Player.UserId}) Joined [{Exiled.API.Features.Player.List.Count()}/20]**");

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
            {
                Log.Info("Starting lonely round");
                Task.Factory.StartNew(() => LonelyRound());
            }
        }
        public void LonelyRound()
        {
            for (int i = 0; i < 60; i++)
            {
                if (Exiled.API.Features.Player.List.Count() == 1 && !Round.IsStarted && !Round.IsLobbyLocked)
                {
                    if (i == 59) Round.Start();
                    if (i == 30) Map.Broadcast(5, "Automatic round Starting in Tminus");
                    if (i >= 35) Map.Broadcast(1, $"{60 - i}");
                    if (i == 59) Map.Broadcast(5, $"Starting a really lonely round..!");
                    Thread.Sleep(1500);
                }
                else return;
            }
        }
        public void OnEscape(EscapingEventArgs ev)
        {
            bool classBool;
            if (ev.Player.Role == RoleType.ClassD)
            {
                classBool = false;
            }
            else //is scientist
            {
                classBool = true;
            }
            if (classBool == true) Tcp.Send($"stats {ev.Player.UserId} descapses 1");
            else
                Tcp.Send($"stats {ev.Player.UserId} sescapses 1");
            Map.Broadcast(4, $"{ev.Player.Nickname} escaped");
            string escapeMsg = $"escape {classBool} {ev.Player.UserId} {Exiled.API.Features.Player.List.ToList().Count}";
            if (ev.Player.IsCuffed)
            {
                escapeMsg = $"{escapeMsg} { Exiled.API.Features.Player.List.ToList().Find(x => x.Id.ToString().Contains(ev.Player.CufferId.ToString())).UserId}";
            }
            Tcp.Send(escapeMsg);
            Tcp.Send($"log **{ev.Player.Nickname} ({ev.Player.UserId}) Escaped**");
        }
        public void OnDead(DiedEventArgs ev)
        {
            if (ev.Killer.Team == Team.SCP) SCPKills++;
            //if (mr.ChaosHacker.Contains(ev.Target)) mr.ChaosHacker.Remove(ev.Target); // Remove the chaos hacker things
            if (ToucanPlugin.Instance.Config.Random008Spawn)
                if (!Has008RandomSpawned)
                {
                    if (ev.Target.Role != RoleType.ClassD || ev.Target.Role != RoleType.Scientist || ev.Target.Role != RoleType.FacilityGuard) return;
                    System.Random rnd = new System.Random();
                    if (rnd.Next(1, 100) == 1)
                    {
                        ev.Target.SetRole(RoleType.Scp0492);
                        Has008RandomSpawned = true;
                        ev.Target.Broadcast(10, $"<color=red>you have been infected by scp 008 convert all humans</color>");
                        Cassie.Message($"biological infection at {ev.Target.CurrentRoom.Zone}");
                    }
                }
            switch (ev.Target.Team)
            {
                case Team.SCP:
                    Tcp.Send($"stats {ev.Killer.UserId} scpkilled 1");
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
                /*case Team.RIP:
                    Tcp.Send($"stats {ev.Target.UserId} scikilled 1");
                    break;*/
                case Team.TUT:
                    Tcp.Send($"stats {ev.Killer.UserId} scikilled 1");
                    Tcp.Send($"stats {ev.Target.UserId} scideaths 1");
                    break;
            }
            if (SerpentsHand.API.SerpentsHand.GetSHPlayers().Contains(ev.Target) || scp035.API.Scp035Data.GetScp035() == ev.Target)
            {
                Tcp.Send($"stats {ev.Killer.UserId} scpkilled 1");
                Tcp.Send($"stats {ev.Target.UserId} scpdeaths 1");
            }
            if (ev.Killer.UserId == ev.Target.UserId) return;
            bool isff = false;
            if (ev.Killer.Team == ev.Target.Team)
            {
                isff = true;
            }
            bool isScp = false;
            if (ev.Target.Team == Team.SCP)
            {
                isScp = true;
            }
            if (scp035.API.Scp035Data.GetScp035() == ev.Killer) isff = false;
            Tcp.Send($"died {isff} {ev.Killer.UserId} {ev.Target.UserId} {ev.HitInformations.Tool} {isScp}");
            Tcp.Send($"log {ev.Target.Nickname} ({ev.Target.UserId}) killed by {ev.Killer.Nickname} ({ev.Killer.UserId}) whit {ev.HitInformations.Tool}");

            //Event bullshit
            switch (AcGame.RoundGamemode)
            {
                case GamemodeType.PeanutInfection:
                    ev.Target.SetRole(RoleType.Scp173);
                    break;
            }
        }
        /*public void OnSpawned(SpawningEventArgs ev)
        {
            Tcp.Send($"spawn {ev.Player.Role} {ev.Player.UserId}");
        }*/
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
        public void OnBanned(BannedEventArgs ev)
        {
            Tcp.Send($"log **{ev.Details.Issuer} ({ev.Details.OriginalName}) banned player {ev.Player.Nickname} ({ev.Player.UserId}). Ban duration: {ev.Details.Expires}. Reason: {ev.Details.Reason}.**");
        }
        public void OnKicked(KickedEventArgs ev)
        {
            Tcp.Send($"log **{ev.Player.Nickname} ({ev.Player.UserId}) has been kicked. Reason: {ev.Reason}**");
        }
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
                    dmgHealed = 100;
                    break;

                case ItemType.SCP207:
                    dmgHealed = 30;
                    break;
            }
            if (dmgHealed != 0)
                Tcp.Send($"stats {ev.Player.UserId} dmghealed {dmgHealed}");
        }
        public enum GrenadeType
        {
            Grenade = 0,
            SCP018 = 1,
            FlashGrenade = 2,
        }
        public void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            Tcp.Send($"log {ev.Player.Nickname} threw a {(GrenadeType)ev.Id}");
        }
        public void OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs ev)
        {
            List<Exiled.API.Features.Player> playerList = new List<Exiled.API.Features.Player>((IEnumerable<Exiled.API.Features.Player>)Exiled.API.Features.Player.List.ToList());
            if (playerList.Find(x => x.Role == RoleType.Scp106) != null)
                Tcp.Send($"femur {ev.Player.UserId}");
        }
        public void OnHurting(HurtingEventArgs ev)
        {
            // Among us game
            if (AmongUs.ImpostersSet.Contains(ev.Attacker) && AcGame.RoundGamemode == GamemodeType.AmongUs)
            {
                ev.Target.Kill();
                AmongUs.DeathCords.Add(ev.Target.Position);
            }
            else
            {
                if (ToucanPlugin.Instance.Config.ReflectTeamDMG && ev.Target.Team == ev.Attacker.Team && ev.Target != ev.Attacker && scp035.API.Scp035Data.GetScp035() != ev.Attacker)
                {
                    ev.Target.Health = ev.Target.Health + ev.Amount;
                    ev.Attacker.Health = ev.Attacker.Health - ev.Amount;
                    ev.Attacker.Broadcast(1, $"Dmg reflected! (Reflector: {ev.Target.Nickname})");
                }
            }
        }
        public void OnChangingRole(ChangingRoleEventArgs ev)
        {
            Team PlayerTeam = Team.RIP;
            if (ev.NewRole.GetSide() == Side.Tutorial) PlayerTeam = Team.TUT;
            if (ev.NewRole.GetSide() == Side.Scp) PlayerTeam = Team.SCP;
            if (ev.NewRole.GetTeam() == Team.RSC) PlayerTeam = Team.RSC;
            if (ev.NewRole.GetTeam() == Team.MTF) PlayerTeam = Team.MTF;
            if (ev.NewRole.GetTeam() == Team.CDP) PlayerTeam = Team.CDP;
            if (ev.NewRole.GetTeam() == Team.CHI) PlayerTeam = Team.CHI;
            if (SerpentsHand.API.SerpentsHand.GetSHPlayers().Contains(ev.Player)) PlayerTeam = Team.SCP;
            if (scp035.API.Scp035Data.GetScp035() == ev.Player) PlayerTeam = Team.SCP;
            Tcp.Send($"vc {ev.Player.UserId} {PlayerTeam}");
        }
    }
}
