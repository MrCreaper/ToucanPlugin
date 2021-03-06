﻿using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using Exiled.Events.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToucanPlugin.Commands;
using ToucanPlugin.Gamemodes;
using UnityEngine;

namespace ToucanPlugin.Handlers
{
    public class Player
    {
        readonly Tcp Tcp = new Tcp();
        public static bool Has008RandomSpawned { get; set; } = false;
        public static int SCPKills = 0;
        public static Dictionary<string, int> petConnections = new Dictionary<string, int>();
        public static Dictionary<Exiled.API.Features.Player, CustomPersonelSpawns> perConnections = new Dictionary<Exiled.API.Features.Player, CustomPersonelSpawns>();
        readonly MessageResponder mr = new MessageResponder();
        public static List<Exiled.API.Features.Player> PlayersCrouchingList = new List<Exiled.API.Features.Player>();

        public void OnPreAuthenticating(PreAuthenticatingEventArgs ev)
        {
            if (!Whitelist.WhitelistUsers.Contains(ev.UserId) && Whitelist.Whitelisted)
            {
                //ev.Disallow();// ev.Player.Kick("Sorry the server is right now whitelisted. Come back later!");
                ev.RejectBanned("Sorry the server is right now closed. Come back later!", 0, false);
                Log.Info($"Denied Entry to the server for {ev.UserId}");
                return;
            }
        }
        public void OnJoin(JoinedEventArgs ev)
        {
            if (ev.Player == null) Log.Debug("oh shit", ToucanPlugin.Singleton.Config.Debug);
            if (ToucanPlugin.Singleton.Config.ReplaceAdvertismentNames)
            {
                List<string> PlayerNameSplit = new List<string>(ev.Player.Nickname.Split(' '));
                List<string> illigalNameParts = new List<string>(ToucanPlugin.Singleton.Config.ADBlacklist);
                PlayerNameSplit.ForEach(part =>
                {
                    illigalNameParts.ForEach(NO =>
                    {
                        if (part.ToLower().Contains(NO))
                            ev.Player.DisplayNickname = ev.Player.Nickname.Replace(part, ToucanPlugin.Singleton.Config.ReplaceAdvertismentNamesWhit);
                    });
                });
            }

            string message = ToucanPlugin.Singleton.Config.JoinedMessage.Replace("{player}", ev.Player.Nickname);
            Map.Broadcast(2, message);
            Tcp.SendLog($"**{ev.Player.Nickname} ({ev.Player.UserId}) Joined [{Exiled.API.Features.Player.List.Count()}/20]**");

            //Top ranker Role
            if (mr.BestBois != null && mr.BestBois.Contains(ev.Player.UserId))
            {
                //p.SendCustomSyncVar(p.ReferenceHub.networkIdentity, typeof(ServerRoles), nameof(ServerRoles.NetworkMyText), "FUCK");
                UserGroup topGroup = new UserGroup
                {
                    BadgeText = "Highest Ranker",
                    BadgeColor = "gold"
                };
                if (ev.Player.RankName != null)
                    ev.Player.SetRank("top", topGroup);
            }
            if (Exiled.API.Features.Player.List.Count() == 1 && !Round.IsStarted && ToucanPlugin.Singleton.Config.LonelyRound)
                Task.Factory.StartNew(() => LonelyRound());
            mr.UpdatePlayerList();
        }
        public void OnLeft(LeftEventArgs ev)
        {
            string message = ToucanPlugin.Singleton.Config.LeftMessage.Replace("{player}", ev.Player.Nickname);
            Map.Broadcast(2, message);
            Tcp.SendLog($"**{ev.Player.Nickname} ({ev.Player.UserId}) Left [{Exiled.API.Features.Player.List.Count() - 1}/20]**");
            mr.UpdatePlayerList(ev.Player.UserId);
            if (Exiled.API.Features.Player.List.Count() - 1 <= 0 && Round.IsStarted)
            {
                Log.Warn("Empty server, restarting...");
                GamemodeLogic.NextGamemode = GamemodeType.None;
                if (ToucanPlugin.Singleton.Config.RestartEmptyServer)
                    Round.ForceEnd();
            }
            UpdateVoiceChannel(ev.Player, RoleType.None);
        }
        private void LonelyRound()
        {
            for (int i = 60; i >= 0; i--)
            {
                if (Exiled.API.Features.Player.List.Count() == 1 && !Round.IsStarted && !Round.IsLobbyLocked)
                {
                    if (i == 0) Round.Start();
                    if (i == 30) Map.ShowHint($"Starting a really lonely round..!", 5);
                    if (i >= 35) Map.ShowHint($"{i}", 1);
                    if (i == 59) Map.ShowHint("Automatic round Starting in...", 5);
                    Thread.Sleep(1200);
                }
                else return;
            }
        }
        /*private void UpdateHud()
        {
            string template = "<align=left><voffset=38em><size=50%><alpha=#44>SanyaPlugin Ex-HUD [VERSION] ([STATS])\n<alpha=#ff></size></align><align=right>[LIST]</align><align=center>[CENTER_UP][CENTER][CENTER_DOWN][BOTTOM]</align></voffset>";
        }
        private void UpdateExHud()
        {
            if (DisableHud || !_plugin.Config.ExHudEnabled) return;
            if (!(_timer > 1f)) return;

            string curText = _hudTemplate.Replace("[STATS]",
                $"St:{DateTime.Now:HH:mm:ss} " +
                $"Rtt:{LiteNetLib4MirrorServer.Peers[player.Connection.connectionId].Ping}ms " +
                $"Ps:{ServerConsole.PlayersAmount}/{CustomNetworkManager.slots} " +
                $"Em:{(int)EventHandlers.eventmode} " +
                $"Ti:{RespawnTickets.Singleton.GetAvailableTickets(SpawnableTeamType.NineTailedFox)}/{RespawnTickets.Singleton.GetAvailableTickets(SpawnableTeamType.ChaosInsurgency)} " +
                $"Vc:{(player.IsMuted ? "D" : "E")}");

            //[SCPLIST]
            if (RoundSummary.singleton._roundEnded && EventHandlers.sortedDamages != null)
            {
                int rankcounter = 1;
                string damageList = string.Empty;
                damageList += "Round Damage Ranking:\n";
                foreach (var stats in EventHandlers.sortedDamages)
                {
                    if (stats.Value == 0) continue;
                    damageList += $"[{rankcounter}]{stats.Key}({stats.Value}Damage)\n";
                    rankcounter++;
                    if (rankcounter > 5) break;
                }
                damageList.TrimEnd('\n');

                curText = curText.Replace("[LIST]", FormatStringForHud(damageList, 6));
            }
            else if (player.Team == Team.SCP)
            {
                string scpList = string.Empty;
                foreach (var scp in scplists)
                    if (scp.Role == RoleType.Scp079)
                        scpList += $"{scp.ReferenceHub.characterClassManager.CurRole.fullName}:Tier{scp.ReferenceHub.scp079PlayerScript.curLvl + 1}\n";
                    else
                        scpList += $"{scp.ReferenceHub.characterClassManager.CurRole.fullName}:{scp.GetHealthAmountPercent()}%\n";
                scpList.TrimEnd('\n');

                curText = curText.Replace("[LIST]", FormatStringForHud(scpList, 6));
            }
            else if (player.Team == Team.MTF)
            {
                string MtfList = string.Empty;
                MtfList += $"<color=#5b6370>FacilityGuard:{RoundSummary.singleton.CountRole(RoleType.FacilityGuard)}</color>\n";
                MtfList += $"<color=#003eca>Commander:{RoundSummary.singleton.CountRole(RoleType.NtfCommander)}</color>\n";
                MtfList += $"<color=#0096ff>Lieutenant:{RoundSummary.singleton.CountRole(RoleType.NtfLieutenant)}</color>\n";
                MtfList += $"<color=#6fc3ff>Cadet:{RoundSummary.singleton.CountRole(RoleType.NtfCadet)}</color>\n";
                MtfList += $"<color=#0096ff>NTFScientist:{RoundSummary.singleton.CountRole(RoleType.NtfScientist)}</color>\n";
                MtfList += $"<color=#ffff7c>Scientist:{RoundSummary.singleton.CountRole(RoleType.Scientist)}</color>\n";
                MtfList.TrimEnd('\n');

                curText = curText.Replace("[LIST]", FormatStringForHud(MtfList, 6));
            }
            else if (player.Team == Team.CHI)
            {
                string CiList = string.Empty;
                CiList += $"<color=#008f1e>ChaosInsurgency:{RoundSummary.singleton.CountRole(RoleType.ChaosInsurgency)}</color>\n";
                CiList += $"<color=#ff8e00>ClassD:{RoundSummary.singleton.CountRole(RoleType.ClassD)}</color>\n";
                CiList.TrimEnd('\n');

                curText = curText.Replace("[LIST]", FormatStringForHud(CiList, 6));
            }
            else
                curText = curText.Replace("[LIST]", FormatStringForHud(string.Empty, 6));

            //[CENTER_UP]
            if (player.Role == RoleType.Scp079)
                curText = curText.Replace("[CENTER_UP]", FormatStringForHud(player.ReferenceHub.animationController.curAnim == 1 ? "Extend:Enabled" : "Extend:Disabled", 6));
            else if (player.Role == RoleType.Scp049)
                if (!player.ReferenceHub.fpc.NetworkforceStopInputs)
                    curText = curText.Replace("[CENTER_UP]", FormatStringForHud($"Corpse in stack:{SanyaPlugin.Singleton.Handlers.scp049stackAmount}", 6));
                else
                    curText = curText.Replace("[CENTER_UP]", FormatStringForHud($"Trying to cure...", 6));
            else
                curText = curText.Replace("[CENTER_UP]", FormatStringForHud(string.Empty, 6));

            //[CENTER]
            if (AlphaWarheadController.Host.inProgress && !AlphaWarheadController.Host.detonated)
                if (!AlphaWarheadController.Host.doorsOpen)
                    curText = curText.Replace("[CENTER]", FormatStringForHud(
                        (AlphaWarheadController._resumeScenario < 0
                        ? AlphaWarheadController.Host.scenarios_resume[AlphaWarheadController._startScenario].tMinusTime.ToString("\n00 : 00")
                        : AlphaWarheadController.Host.scenarios_resume[AlphaWarheadController._resumeScenario].tMinusTime.ToString("\n00 : 00")
                    ), 6));
                else
                    curText = curText.Replace("[CENTER]", FormatStringForHud($"<color=#ff0000>{AlphaWarheadController.Host.timeToDetonation.ToString("\n00 : 00")}</color>", 6));
            else
                curText = curText.Replace("[CENTER]", FormatStringForHud(string.Empty, 6));

            //[CENTER_DOWN]
            if (player.Team == Team.RIP && _respawnCounter != -1 && !Warhead.IsDetonated)
                if (_respawnCounter == 0)
                    curText = curText.Replace("[CENTER_DOWN]", FormatStringForHud($"間もなくリスポーンします", 6));
                else
                    curText = curText.Replace("[CENTER_DOWN]", FormatStringForHud($"リスポーンまで{_respawnCounter}秒", 6));
            else if (!string.IsNullOrEmpty(_hudCenterDownString))
                curText = curText.Replace("[CENTER_DOWN]", FormatStringForHud(_hudCenterDownString, 6));
            else
                curText = curText.Replace("[CENTER_DOWN]", FormatStringForHud(string.Empty, 6));

            //[BOTTOM]
            if (Intercom.host.speaking && Intercom.host.speaker != null)
                curText = curText.Replace("[BOTTOM]", $"{Player.Get(Intercom.host.speaker)?.Nickname}が放送中...");
            else
                curText = curText.Replace("[BOTTOM]", string.Empty);

            _hudText = curText;
            player.SendTextHintNotEffect(_hudText, 2);
        }
        private string FormatStringForHud(string text, int needNewLine)
        {
            int curNewLine = text.Count(x => x == '\n');
            for (int i = 0; i < needNewLine - curNewLine; i++)
                text += '\n';
            return text;
        }*/
        public void OnEscape(EscapingEventArgs ev)
        {
            bool classBool = (ev.Player.Role == RoleType.ClassD ? true : false);
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
            if (!Has008RandomSpawned && ToucanPlugin.Singleton.Config.Random008Spawn)
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
            Tcp.SendLog($"{ev.Target.Nickname} ({ev.Target.UserId}) killed by {ev.Killer.Nickname} ({ev.Killer.UserId}) with {ev.HitInformations.GetDamageName()}");
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
            ToucanPlugin.Singleton.Config.CustomPersonel.ForEach(per =>
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
                perConnections.Add(p, per);
                p.ShowHint(per.Hint, 6);
            });
        }
        public void ChaosHackerCharge(Exiled.API.Features.Player p)
        {
            Task.Factory.StartNew(() =>
            {
                while (perConnections[p].Abilities.Contains(AbilityType.DoorHacking) || perConnections[p].Abilities.Contains(AbilityType.IcomDisabling) || perConnections[p].Abilities.Contains(AbilityType.Blackout))
                {
                    if (p.AdrenalineHealth + 1 > p.MaxAdrenalineHealth)
                        p.AdrenalineHealth = p.MaxAdrenalineHealth;
                    else
                        p.AdrenalineHealth += 1;
                    Thread.Sleep(100);
                }
            });
        }
        public void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (!perConnections.ContainsKey(ev.Player)) return;
            if (!perConnections[ev.Player].Abilities.Contains(AbilityType.DoorHacking) || !ev.IsAllowed || ev.Door.GetComponent<Interactables.Interobjects.BreakableDoor>().IsDestroyed) return;
            float ap = ev.Player.AdrenalineHealth;
            float apCost = 0;
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
                ap -= apCost;
                if (ev.Door.NetworkTargetState)
                    ev.Door.NetworkTargetState = false;
                else
                    ev.Door.NetworkTargetState = true;
            }
        }

        public void OnBanned(BannedEventArgs ev)
        {
            if (ev.Player != null)
                Tcp.SendLog($"```{ev.Details.Issuer} ({ev.Details.OriginalName}) banned player {ev.Player.Nickname} ({ev.Player.UserId}). Ban duration: {ev.Details.Expires}. Reason: {ev.Details.Reason}.```");
        }
        public void OnKicked(KickedEventArgs ev)
        {
            if (ev.Player != null)
                Tcp.SendLog($"```{ev.Player.Nickname} ({ev.Player.UserId}) has been kicked. Reason: {ev.Reason}```");
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
            switch (ev.Type)
            {
                case GrenadeType.Flashbang:
                    Tcp.SendLog($"{ev.Player.Nickname} ({ev.Player.UserId}) threw a Flashbang!");
                    Tcp.Send($"stats {ev.Player.UserId} flashThrown 1");
                    break;
                case GrenadeType.FragGrenade:
                    Tcp.SendLog($"{ev.Player.Nickname} ({ev.Player.UserId}) threw a Grenade!");
                    Tcp.Send($"stats {ev.Player.UserId} grenadeThrown 1");
                    break;
                case GrenadeType.Scp018:
                    Tcp.SendLog($"{ev.Player.Nickname} ({ev.Player.UserId}) threw a SCP-018!");
                    Tcp.Send($"stats {ev.Player.UserId} scp018Thrown 1");
                    break;
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
            if (ev.Target.Role == RoleType.Scp106 && ToucanPlugin.Singleton.Config.Force106Femur && !Warhead.IsDetonated && ev.Attacker.CurrentItem.id != ItemType.MicroHID)
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
            Team PlayerTeam = p.Team;
            if (Role == RoleType.None) Role = p.Role;
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
        private static bool CrouchingRunning = false;
        public static void StartDetectingCrouching(bool Enable = true)
        {
            CrouchingRunning = Enable;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!ToucanPlugin.Singleton.Config.CrouchingEnabled || !CrouchingRunning) return;
                    Exiled.API.Features.Player.List.ToList().ForEach(p =>
                        {
                            if (p.MoveState == PlayerMovementState.Sneaking && !PlayersCrouchingList.Contains(p))
                            {
                                p.Scale = new Vector3(ToucanPlugin.Singleton.Config.CrouchingSize.X, ToucanPlugin.Singleton.Config.CrouchingSize.Y, ToucanPlugin.Singleton.Config.CrouchingSize.Z);
                                PlayersCrouchingList.Add(p);
                            }
                            else if (p.MoveState != PlayerMovementState.Sneaking && PlayersCrouchingList.Contains(p))
                            {
                                p.Scale = new Vector3(1, 1, 1);
                                PlayersCrouchingList.Remove(p);
                            }
                        });
                    Thread.Sleep(500);
                }
            });
        }
    }
}
