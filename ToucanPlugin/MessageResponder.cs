using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using ToucanPlugin.Commands;
using NPCS;
using VideoLibrary;

namespace ToucanPlugin
{
    class MessageResponder
    {
        readonly Tcp Tcp = new Tcp();
        readonly Whitelist wl = new Whitelist();
        public List<Player> ChaosHacker { get; set; } = new List<Player>();
        public List<string> BestBois;
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
            return NewString;
        }
        public void Respond(string Cmd0)
        {
            string Cmd = RemoveThatShit(Cmd0);
            List<string> Cmds = new List<string>(Cmd.Split(' '));
            if (Cmd0.Split(' ')[0].Length == Tcp.MaxMessageLenght)
                Tcp.S.Close();
            else
                Log.Debug($"Recived >{Cmd}<");
            switch (Cmds[0])
            {
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
                            Log.Info(Cmd.Replace($"log {Cmds[1]} ", ""));
                            break;
                        case "warn":
                            Log.Warn(Cmd.Replace($"log {Cmds[1]} ", ""));
                            break;
                        case "error":
                            Log.Error(Cmd.Replace($"log {Cmds[1]} ", ""));
                            break;
                    }
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
                    string NewStore = Cmd.ToString().Remove(0, 6);
                    if (Store.StoreStock == NewStore) return;
                    List<string> StoreStockList = NewStore.Select(c => c.ToString()).ToList();
                    if (StoreStockList[1] == "#" && StoreStockList[StoreStockList.Count - 1] == "#")
                    {
                        Store.StoreStock =NewStore;
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
                    BestBois = new List<string>(Cmd.Replace("bestbois ", "").Split(' '));
                    if (BestBois.Count >= 2)
                        Log.Info("Best bois recived!");
                    else
                    {
                        Tcp.Send($"updateDataReq 1");
                        Log.Warn($"Yeah soo, best bois list is a bit short whit only {BestBois.Count - 1} users... [This will fix itself soon]");
                    }
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
                        wl.Add(Cmds[1], Cmds[2]);
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
            }
        }
        private void FrameDataToIcom(Images.FrameData fd) { Log.Info($">{fd.Data}<"); Intercom.host.UpdateIntercomText(fd.Data); }
    }
}
