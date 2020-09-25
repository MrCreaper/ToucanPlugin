using Exiled.API.Features;
using Exiled.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using UnityEngine;

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
            Tcp.Send($"log {ev.Player.Nickname} ({ev.Player.UserId}) Left [{Exiled.API.Features.Player.List.Count() - 1}/20]");
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
            Tcp.Send($"log {ev.Player.Nickname} ({ev.Player.UserId}) Joined [{Exiled.API.Features.Player.List.Count()}/20]");

            //Booster Role
            if (mr.Boosters != null && mr.Boosters.Contains(ev.Player.UserId))
            {
                UserGroup boosterGroup = new UserGroup
                {
                    BadgeText = "Server Nitro Booster",
                    BadgeColor = "pink"
                };
                ev.Player.SetRank("boost", boosterGroup);
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
                escapeMsg = escapeMsg + " " + Exiled.API.Features.Player.List.ToList().Find(x => x.Id.ToString().Contains(ev.Player.CufferId.ToString())).UserId;
            }
            Tcp.Send(escapeMsg);
            Tcp.Send($"log {ev.Player.Nickname} ({ev.Player.UserId}) Escaped");
        }
        public void OnDead(DiedEventArgs ev)
        {
            if (ev.Killer.Team == Team.SCP) SCPKills++;
            if (ToucanPlugin.Instance.Config.Random008Spawn)
            {
                if (!Has008RandomSpawned)
                {
                    if (ev.Target.Role != RoleType.ClassD || ev.Target.Role != RoleType.Scientist || ev.Target.Role != RoleType.FacilityGuard) return;
                    System.Random rnd = new System.Random();
                    int spawnChance = rnd.Next(1, 100);
                    if (spawnChance == 1)
                    {
                        ev.Target.SetRole(RoleType.Scp0492);
                        Has008RandomSpawned = true;
                        ev.Target.Broadcast(10, $"<color=red>you have been infected by scp 008 convert all humans</color>");
                        Cassie.Message($"biological infection at {ev.Target.CurrentRoom.Zone}");
                    }
                }
            }
            switch (ev.Target.Team) {
                case Team.SCP:
                    Tcp.Send($"stats {ev.Killer.UserId} scpkilled 1");
                    break;
                case Team.MTF:
                    Tcp.Send($"stats {ev.Killer.UserId} mtfkilled 1");
                    break;
                case Team.CHI:
                    Tcp.Send($"stats {ev.Killer.UserId} chaoskilled 1");
                    break;
                case Team.RSC:
                    Tcp.Send($"stats {ev.Killer.UserId} scikilled 1");
                    break;
                case Team.CDP:
                    Tcp.Send($"stats {ev.Killer.UserId} dclasskilled 1");
                    break;
                /*case Team.RIP:
                    Tcp.Send($"stats {ev.Killer.UserId} scikilled 1");
                    break;*/
                case Team.TUT:
                    Tcp.Send($"stats {ev.Killer.UserId} scikilled 1");
                    break;
            }
            switch (ev.Killer.Team)
            {
                case Team.SCP:
                    Tcp.Send($"stats {ev.Target.UserId} scpkills 1");
                    break;
                case Team.MTF:
                    Tcp.Send($"stats {ev.Target.UserId} mtfkills 1");
                    break;
                case Team.CHI:
                    Tcp.Send($"stats {ev.Target.UserId} chaoskills 1");
                    break;
                case Team.RSC:
                    Tcp.Send($"stats {ev.Target.UserId} scikills 1");
                    break;
                case Team.CDP:
                    Tcp.Send($"stats {ev.Target.UserId} dclasskills 1");
                    break;
                /*case Team.RIP:
                    Tcp.Send($"stats {ev.Target.UserId} scikilled 1");
                    break;*/
                case Team.TUT:
                    Tcp.Send($"stats {ev.Target.UserId} scikills 1");
                    break;
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
            if (ev.Killer.RankName != "SCP-035") isff = false;
            Tcp.Send($"died {isff} {ev.Killer.UserId} {ev.Target.UserId} {ev.HitInformations.Tool} {isScp}");
            Tcp.Send($"log {ev.Target.Nickname} ({ev.Target.UserId}) killed by {ev.Killer.Nickname} ({ev.Killer.UserId}) whit {ev.HitInformations.Tool}");
        }
        /*public void OnSpawned(SpawningEventArgs ev)
        {
            Tcp.Send($"spawn {ev.Player.Role} {ev.Player.UserId}");
        }*/
        public void OnBanned(BannedEventArgs ev)
        {
            Tcp.Send($"log {ev.Details.Issuer} ({ev.Details.OriginalName}) banned player {ev.Player.Nickname} ({ev.Player.UserId}). Ban duration: {ev.Details.Expires}. Reason: {ev.Details.Reason}.");
        }
        public void OnKicked(KickedEventArgs ev)
        {
            Tcp.Send($"log {ev.Player.Nickname} ({ev.Player.UserId}) has been kicked. Reason: {ev.Reason}");
        }
        public void OnMedicalItemUsed(UsedMedicalItemEventArgs ev)
        {
            if (ev.Item == ItemType.SCP207) Tcp.Send($"stats {ev.Player.UserId} 207drunk 1");
            int dmgHealed = 0;
            switch (ev.Item) {
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
            
            Tcp.Send($"stats {ev.Player.UserId} dmghealed {dmgHealed}");
        }
        public void OnThrowingGrenade(ThrowingGrenadeEventArgs ev)
        {
            Tcp.Send($"log {ev.Player.Nickname} threw a grenade");
        }
    }
}
