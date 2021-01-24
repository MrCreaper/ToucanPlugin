using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using ToucanPlugin.Commands;
using NPCS;
using VideoLibrary;
using Exiled.API.Enums;

namespace CustomExtensions
{
    // Extension methods must be defined in a static class.
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string Slice(this string input, int start, int end)
        {
            List<string> CList = input.Select(c => c.ToString()).ToList();
            string output = "";
            for (int i = start; i < end; i++)
            {
                CList[i] = "";
            }
            CList.ForEach(c => output += c);
            return output;
        }
    }
}

namespace ToucanPlugin
{
    using CustomExtensions;
    class MessageResponder
    {
        readonly Tcp Tcp = new Tcp();
        readonly Whitelist wl = new Whitelist();
        public List<string> BestBois;
        public enum SpectatorAbilityType
        {
            None = 0,
            Coke1 = 1,
            Coke2 = 2,
            Coke3 = 3,
            Coke4 = 4,
            CokeInf = 5,
            ForceStalk = 6,
            Spawn079 = 7,
            Bonk = 8,
            Spawn035 = 9,
        }
        public string SpectatorAbilityToNice(SpectatorAbilityType Ab)
        {
            switch (Ab)
            {
                default:
                case SpectatorAbilityType.None:
                    return $"[NULL {Ab}]";
                case SpectatorAbilityType.Coke1:
                    return $"Coke x1";
                case SpectatorAbilityType.Coke2:
                    return $"Coke x2";
                case SpectatorAbilityType.Coke3:
                    return $"Coke x3";
                case SpectatorAbilityType.Coke4:
                    return $"Coke x4";
                case SpectatorAbilityType.CokeInf:
                    return $"Coke x∞";
                case SpectatorAbilityType.ForceStalk:
                    return $"Force Stalk";
                case SpectatorAbilityType.Bonk:
                    return $"BONK";
                case SpectatorAbilityType.Spawn035:
                    return $"You are 035!";
            }
        }
        public string RemoveThatShit(string DisgustingSpaces)
        {
            List<string> DisgustingSpacesArray = DisgustingSpaces.Select(c => c.ToString()).ToList();
            string CringeSpace = DisgustingSpacesArray[DisgustingSpacesArray.Count - 1];
            string NewString = "";
            bool ShitFound = false;
            for (int i = DisgustingSpacesArray.Count - 1; 0 < i; i--)
            {
                if (DisgustingSpacesArray[i] == CringeSpace && !ShitFound)
                    DisgustingSpacesArray[i] = "";
                else
                    ShitFound = true;
            }
            DisgustingSpacesArray.ForEach(c => NewString += c);
            if (NewString == "") Tcp.Disconnect();
            return NewString;
        }
        public void Connected()
        {
            if (Server.Name != null)
                Exiled.API.Features.Server.Name = Exiled.API.Features.Server.Name.Replace($"<color=#00000000><size=1>Exiled {Exiled.Loader.Loader.Version.ToString().Replace(".0", "")}</size></color>", "");
            UpdateMap();
            SendStaticInfo();
            SendInfo();
            UpdatePlayerList();
        }
        public void Respond(string Cmd0)
        {
            string Cmd = RemoveThatShit(Cmd0);
            List<string> Cmds = new List<string>(Cmd.Split(' '));
            if (Cmd0.Split(' ')[0].Length == Tcp.MaxMessageLenght || Cmd == "" || Cmd.Length == 1)
            {
                //Log.Debug($"Empty Data Recived >{Cmd0}<");
                if (!Tcp.auth)
                    Tcp.Disconnect();
                return;
            }
            else
                Log.Debug($"Recived >{Cmd}<", ToucanPlugin.Instance.Config.Debug);
            switch (Cmds[0])
            {
                default:
                    return;
                case "itemBought":
                    //itemBought {buyer} {item} {coins left}
                    Player p = Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1]));
                    p.AddItem((ItemType)int.Parse(Cmds[2].ToString()));
                    p.SendConsoleMessage($"Thanks for buying a {(ItemType)int.Parse(Cmds[2])}", "#fffff");
                    p.ShowHint($"<i>Thanks for Buying a <color=yellow>{ (ItemType)int.Parse(Cmds[2])}</color>, {Cmds[3]} Coins left</i>", 6);
                    Tcp.SendLog($"{p.Nickname} ({Cmds[1]}) Bought an {(ItemType)int.Parse(Cmds[2].ToString())}");
                    Tcp.Send($"stats {p.UserId} itemsbought 1");
                    break;

                case "log":
                    switch (Cmds[1])
                    {
                        case "info":
                            Log.Info($"[Toucan Server Message] {Cmd.Replace($"{Cmds[0]} {Cmds[1]} ", "")}");
                            break;
                        case "warn":
                            Log.Warn($"[Toucan Server Message] {Cmd.Replace($"{Cmds[0]} {Cmds[1]} ", "")}");
                            break;
                        case "error":
                            Log.Error($"[Toucan Server Message] {Cmd.Replace($"{Cmds[0]} {Cmds[1]} ", "")}");
                            break;
                    }
                    break;

                case "cmd":
                    break;

                case "console":
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).SendConsoleMessage(Cmd.Replace($"console {Cmds[1]} ", ""), "#fffff");
                    break;

                case "bc":
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Broadcast(ushort.Parse(Cmds[2]), Cmd.Replace($"bc {Cmds[1]} {Cmds[2]} ", ""));
                    break;

                case "hint":
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).ShowHint(Cmd.Replace($"hint {Cmds[1]} {Cmds[2]} ", ""), int.Parse(Cmds[2]));
                    break;

                case "rcoins": //recived coins
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).ShowHint($"<i>Recived <color=yellow>{Cmds[2]} Coins</color></i>", 5);
                    break;

                case "msg":
                    string fullMsg = Cmd.Replace($"{Cmds[0]} ", "");
                    List<string> fullMsgC = fullMsg.Select(c => c.ToString()).ToList();
                    int msgStart = 0;
                    for (int i = 0; i < fullMsg.Length - 1; i++)
                    {
                        if (fullMsgC[i] == ":" && msgStart == 0)
                            msgStart = i;
                    }
                    string Sender = fullMsg.Slice(msgStart - 1, fullMsg.Length - 1);
                    string Message = fullMsg.Slice(1, msgStart + 1);
                    new Chat().SendMsgInGame(Sender, Message);
                    break;

                case "kill":
                    //kill {killee} {killer}
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Kill();
                    break;

                case "kick":
                    //kick {kickee} {kicker} {reason}
                    String kickreason = null;
                    if (Cmds[3] != null)
                        kickreason = Cmd.Replace($"kick {Cmds[1]} {Cmds[2]} ", "");
                    else
                        kickreason = ToucanPlugin.Instance.Config.DefaultKickReason.Replace("{kicker}", Cmds[2]);
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Kick(kickreason, Cmds[1]);
                    Tcp.Send(Cmd);
                    break;

                case "kickall":
                    //kick {kicker} {reason}
                    string kickallreason = null;
                    if (Cmds[2] != null)
                        kickallreason = Cmd.Replace($"kickall {Cmds[1]} ", "");
                    else
                        kickallreason = ToucanPlugin.Instance.Config.DefaultKickAllReason.Replace("{kicker}", Cmds[1]);
                    Player.List.ToList().ForEach(user => user.Kick(kickallreason));
                    Tcp.Send(Cmd);
                    Tcp.SendLog($"Everyone was kicked remotly by {Cmds[1]}");
                    break;

                case "ban":
                    //ban {duration} {banee} {banner} {reason}
                    string banreason = null;
                    if (Cmds[4] != null)
                        banreason = Cmd.Replace($"ban {Cmds[1]} {Cmds[2]} {Cmds[3]} ", "");
                    else
                        banreason = ToucanPlugin.Instance.Config.DefaultBanReason.Replace("{banner}", Cmds[3]);
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Ban(int.Parse(Cmds[1]), banreason, Cmds[2]);
                    Tcp.Send(Cmd);
                    Tcp.SendLog($"'''{Cmds[2]} was banned for {Cmds[1]} by {Cmds[3]} for the reason of;\n{Cmds[4]}'''");
                    break;

                case "restartRound":
                    Log.Info("Remote restart round recived.");
                    Round.Restart();
                    break;

                case "store":
                    string NewStore = Cmd.ToString().Replace($"{Cmds[0]} ", "");
                    if (Store.StoreStock == NewStore) return;
                    List<string> StoreStockList = NewStore.Select(c => c.ToString()).ToList();
                    if (StoreStockList[1] == "#" && StoreStockList[StoreStockList.Count - 1] == "#")
                    {
                        Store.StoreStock = NewStore;
                        Log.Info($"Store Retrived{Store.StoreStock}");
                    }
                    else
                    {
                        Store.StoreStock = null;
                        Tcp.Send($"updateDataReq 0");
                        Log.Warn($"Store is fucked up... [This will fix itself soon]");
                    }
                    break;

                case "bestbois":
                    List<string> BestBoisUp = new List<string>(Cmd.Replace("bestbois ", "").Split(' '));
                    bool confirmed = true;
                    BestBoisUp.ForEach(id =>
                    {
                        if (!id.Contains("@steam") && !id.Contains("@discord"))
                            confirmed = false;
                    });
                    if (BestBoisUp.Count >= 2 && confirmed)
                    {
                        BestBois = BestBoisUp;
                        Log.Info("Best bois recived!");
                    }
                    else
                    {
                        Tcp.Send($"updateDataReq 1");
                        Log.Warn($"Yeah soo, best bois list is a bit short whit only {BestBois.Count - 1} users... [This will fix itself soon]");
                    }
                    break;

                case "specAbilList":
                    string NewSpectatorAbilityList = Cmd.ToString().Replace($"{Cmds[0]} ", "");
                    if (SpectatorAbilityList.SpectatorAbilityStock == NewSpectatorAbilityList) return;
                    List<string> NewSpectatorAbilityListList = NewSpectatorAbilityList.Select(c => c.ToString()).ToList();
                    if (NewSpectatorAbilityListList[1] == "#" && NewSpectatorAbilityListList[NewSpectatorAbilityListList.Count - 1] == "#")
                    {
                        SpectatorAbilityList.SpectatorAbilityStock = NewSpectatorAbilityList;
                        Log.Info($"Spectator Ability List Retrived{SpectatorAbilityList.SpectatorAbilityStock}");
                    }
                    else
                    {
                        SpectatorAbilityList.SpectatorAbilityStock = null;
                        Tcp.Send($"updateDataReq 2");
                        Log.Warn($"Spectator Ability List is fucked up...");
                    }
                    break;

                case "specAbility":
                    Player Spactator = Player.List.ToList().Find(x => x.UserId == Cmds[1]);
                    Player Spactatee = Player.List.ToList().Find(x => x.UserId == Cmds[2]);
                    if (Spactator == null || Spactatee == null) return;
                    SpectatorAbilityType SpecAbil = (SpectatorAbilityType)int.Parse(Cmds[3]);
                    switch (SpecAbil)
                    {
                        /*case SpectatorAbilityType.Coke1:
                            //Spactatee.EnableEffect<CustomPlayerEffects.Scp207>();
                            Spactatee.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(1);
                            break;
                        case SpectatorAbilityType.Coke2:
                            Spactatee.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(2);
                            break;
                        case SpectatorAbilityType.Coke3:
                            Spactatee.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(3);
                            break;
                        case SpectatorAbilityType.Coke4:
                            Spactatee.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(4);
                            break;
                        case SpectatorAbilityType.CokeInf:
                            Spactatee.ChangeEffectIntensity<CustomPlayerEffects.Scp207>(100);
                            break;*/
                        case SpectatorAbilityType.ForceStalk:
                            if (Player.List.ToList().Find(x => x.Role == RoleType.Scp106) == null)
                                Log.Debug($"No scp 106 found", ToucanPlugin.Instance.Config.Debug);
                            else
                            {
                                Player Scp106 = Player.List.ToList().Find(x => x.Role == RoleType.Scp106);
                                //float _ = new Stalky106.StalkyMethods().PortalProcedure(Spactatee.ReferenceHub.characterClassManager.Scp106, Spactatee.Position);
                                Scp106.ReferenceHub.characterClassManager.Scp106.portalPosition = Spactatee.Position;
                                Scp106.ReferenceHub.characterClassManager.Scp106.CmdUsePortal();
                            }
                            break;
                        case SpectatorAbilityType.Bonk:
                            Spactatee.Scale = new UnityEngine.Vector3(1, 0.4f, 1);
                            break;
                        case SpectatorAbilityType.Spawn035:
                            if (scp035.API.Scp035Data.GetScp035() == null)
                                scp035.API.Scp035Data.Spawn035(Spactatee);
                            break;
                    }
                    if (Spactator.DisplayNickname == null)
                        Spactatee.Broadcast(4, $"<i>Spectator Ability!\nAbility: <color=yellow>{SpectatorAbilityToNice(SpecAbil)}</color>\n<size=10>Spectator: {Spactator.Nickname}</size></i>");
                    else
                        Spactatee.Broadcast(4, $"<i>Spectator Ability!\nAbility: <color=yellow>{SpectatorAbilityToNice(SpecAbil)}</color>\n<size=10>Spectator: {Spactator.DisplayNickname}</size></i>");
                    break;

                case "pet":
                    Player petOwner = Player.List.ToList().Find(x => x.UserId == Cmds[1]);
                    if (petOwner == null) return;
                    Npc pet = NPCS.Methods.CreateNPC(
                    new UnityEngine.Vector3(float.Parse(Cmds[2]), float.Parse(Cmds[3]), float.Parse(Cmds[4])),
                    new UnityEngine.Vector2(float.Parse(Cmds[5]), float.Parse(Cmds[6])),
                    new UnityEngine.Vector3(float.Parse(Cmds[7]), float.Parse(Cmds[8]), float.Parse(Cmds[9])),
                        (RoleType)int.Parse(Cmds[10]),
                        (ItemType)int.Parse(Cmds[11]),
                        Cmd.Replace($"pet {Cmds[1]} {Cmds[2]} {Cmds[3]} {Cmds[4]} {Cmds[5]} {Cmds[6]} {Cmds[7]} {Cmds[8]} {Cmds[9]} {Cmds[10]} {Cmds[11]} {Cmds[12]}", ""), Cmds[12]);
                    pet.Follow(petOwner);
                    Handlers.Player.petConnections.Add(petOwner.UserId, pet.GetInstanceID());
                    petOwner.SendConsoleMessage($"Equiped {Cmd.Replace($"pet {Cmds[1]} {Cmds[2]} {Cmds[3]} {Cmds[4]} {Cmds[5]} {Cmds[6]} {Cmds[7]} {Cmds[8]} {Cmds[9]} {Cmds[10]} {Cmds[11]} {Cmds[12]} ", "")}", "#fffff");
                    petOwner.ShowHint($"Equiped {Cmd.Replace($"pet {Cmds[1]} {Cmds[2]} {Cmds[3]} {Cmds[4]} {Cmds[5]} {Cmds[6]} {Cmds[7]} {Cmds[8]} {Cmds[9]} {Cmds[10]} {Cmds[11]} {Cmds[12]} ", "")}", 10);
                    break;

                case "whitelist":
                    // whitelist {whitelist user} {comment}
                    if (Cmds[1] == null) // Open/Close Server
                        if (Whitelist.Whitelisted)
                        {
                            Whitelist.Whitelisted = false;
                            Tcp.SendLog($"**{Server.Name} is now open!**");
                        }
                        else
                        {
                            Whitelist.Whitelisted = true;
                            Tcp.SendLog($"**{Server.Name} is now closed!**");
                        }
                    else
                        if (!Whitelist.WhitelistUsers.Contains(Cmds[1])) // Whitelist user
                    {
                        wl.Add(Cmds[1], Cmd.Replace($"{Cmds[0]} {Cmds[1]} ", ""));
                        Tcp.SendLog($"**User ({Cmd[1]}) now whitelisted!**");
                    }
                    else
                    {
                        wl.Remove(Cmds[1]);
                        Tcp.SendLog($"**User ({Cmd[1]}) taken off the whitelist.**");
                    }
                    break;

                case "icomimg":
                    Action<Images.FrameData> Handler = FrameDataToIcom;
                    Intercom.host.UpdateIntercomText($"Waiting on image...");
                    Images.API.LocationToText(Cmds[1], Handler, true);
                    break;

                case "play":
                    var youtube = YouTube.Default;
                    var vid = youtube.GetVideo(Cmds[1]);
                    //CommsHack.AudioAPI.API.PlayStream(vid.Stream(), float.Parse(Cmds[2]));
                    break;

                case "pause":
                    bool IsAliveAndPaused = CommsHack.HackMain.handle.IsAliveAndPaused;
                    if (IsAliveAndPaused)
                        IsAliveAndPaused = false;
                    else
                        IsAliveAndPaused = true;
                    break;

                case "infoS":
                    SendStaticInfo();
                    break;

                case "infoR":
                    SendInfo();
                    break;

                case "map":
                    UpdateMap();
                    break;
            }
        }
        public string UpdatePlayerList(string ExcludedId = "", bool autoSend = true)
        {
            string playerList = "[";
            for (int i = 0; i <= Exiled.API.Features.Player.List.ToList().Count - 1; i++)
            {
                Exiled.API.Features.Player p = Exiled.API.Features.Player.List.ToList()[i];
                if (ExcludedId != p.UserId)
                {
                    string Coma = ",";
                    if (Player.List.ToList().Count - 1 >= i || Player.List.ToList()[i + 1].UserId == ExcludedId && Player.List.ToList().Count >= i)
                        Coma = "";
                    playerList += $"{{\"id\":{p.Id},\"name\":\"{p.Nickname.Replace("\"", "")}\",\"userid\":\"{p.UserId}\", \"role\": \"{p.Role}\",\"room\":\"{p.CurrentRoom.Type}\",\"x\":{p.Position.x},\"y\":{p.Position.y}}}{Coma}";
                }
            }
            playerList += "]";
            if (autoSend)
                Tcp.Send($"list {playerList}");
            return playerList;
        }
        public string UpdateMap()
        {
            if (Map.Rooms == null) return null;
            string MapMsg = "";
            ZoneType lastZone = ZoneType.Unspecified;
            Map.Rooms.ToList().ForEach(r =>
            {
                if (lastZone != r.Zone)
                {
                    lastZone = r.Zone;
                    MapMsg += $"{(int)r.Zone} ";
                }
                MapMsg += $"{(int)r.Type}|{r.Transform.position.x}|{r.Transform.position.y}|{r.Transform.rotation.y} ";
            });
            Tcp.Send($"map {MapMsg}");
            return MapMsg;
        }
        private string CleanServerName(string name)
        {
            string result = name;
            List<string> nameArray = result.Select(c => c.ToString()).ToList();
            int end = 0;
            bool found = false;
            for (int i = name.Length - 1; 0 <= i; i--)
            {
                if (nameArray[i] == ">")
                {
                    found = true;
                    end = i;
                }
                if (nameArray[i] == "<" && found)
                {
                    for (int I = 0; I <= end - i; I++)
                        nameArray[end - I] = "";
                    found = false;
                }
            }
            string resultFUCK = "";
            nameArray.ForEach(x => resultFUCK += x);
            return resultFUCK;
        }
        public void SendStaticInfo() =>
            Tcp.Send($"infoS [\"{Server.Name.Replace("\"", "")}\", \"{CleanServerName(Server.Name.Replace("\"", ""))}\", \"{Server.IpAddress}:{Server.Port}\", \"{Server.FriendlyFire}\"]");
        public void SendInfo() =>
            Tcp.Send($"infoR [\"{Round.IsStarted}\", \"{Round.IsLocked}\", \"{Round.IsLobbyLocked}\", \"{Round.ElapsedTime.Days}d:{Round.ElapsedTime.Hours}h:{Round.ElapsedTime.Minutes}m:{Round.ElapsedTime.Seconds}s.{Round.ElapsedTime.Milliseconds}ms\", \"{Map.IsLCZDecontaminated}\", {Map.ActivatedGenerators}, \"{GamemodeLogic.RoundGamemode}\", \"{GamemodeLogic.NextGamemode}\"]");
        private void FrameDataToIcom(Images.FrameData fd) { Log.Info($">{fd.Data}<"); Intercom.host.UpdateIntercomText(fd.Data); }
    }
}
