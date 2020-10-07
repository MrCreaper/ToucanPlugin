using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using ToucanPlugin.Commands;
using MediaToolkit;
using VideoLibrary;
using MediaToolkit.Model;
using Assets._Scripts.Dissonance;
using CommandSystem.Commands;
using System.Drawing;
using System.IO;
using System.Net;
using System.Drawing.Imaging;
using MEC;
using UnityEngine;

namespace ToucanPlugin
{
    class MessageResponder
    {
        readonly Tcp Tcp = new Tcp();
        readonly Whitelist wl = new Whitelist();
        public List<Player> ChaosHacker { get; set; } = new List<Player>();
        public List<string> BestBois;
        public void Respond(string Cmd)
        {
            Log.Debug($"Recived {Cmd}");
            List<string> Cmds = new List<string>(Cmd.Split(' '));
            switch (Cmds[0])
            {
                case "itemBought":
                    //itemBought {buyer} {item} {coins left}
                    Player p = Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1]));
                    p.AddItem((ItemType)int.Parse(Cmds[2].ToString()));
                    p.SendConsoleMessage($"Thanks for buying a {(ItemType)int.Parse(Cmds[2])}", "#fffff");
                    p.ShowHint($"<i>Item Bought a <color=yellow>{ (ItemType)int.Parse(Cmds[2])}</color></i>");
                    Tcp.Send($"log {p.Nickname} ({Cmds[1]}) Bought an {(ItemType)int.Parse(Cmds[2].ToString())}");
                    Tcp.Send($"stats {p.UserId} itemsbought 1");
                    break;

                case "consoleMsg":
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).SendConsoleMessage(Cmd.Replace($"consoleMsg {Cmds[1]} ", ""), "#fffff");
                    break;

                case "rcoins": //recived coins
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Broadcast(2, $"<color=#fcd303>Recived {Cmds[2]}Coins</color>");
                    break;

                case "kill":
                    //kill {killee} {killer}
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Kill();
                    break;

                case "kick":
                    //kick {kickee} {kicker} {reason}
                    String kickreason = null;
                    if (Cmds[3] != null)
                    {
                        kickreason = Cmd.Replace($"kick {Cmds[1]} {Cmds[2]} ", "");
                    }
                    else
                    {
                        kickreason = ToucanPlugin.Instance.Config.DefaultKickReason.Replace("{kicker}", Cmds[2]);
                    }
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Kick(kickreason, Cmds[1]);
                    Tcp.Send(Cmd);
                    break;

                case "kickall":
                    //kick {kicker} {reason}
                    string kickallreason = null;
                    if (Cmds[2] != null)
                    {
                        kickallreason = Cmd.Replace($"kickall {Cmds[1]} ", "");
                    }
                    else
                    {
                        kickallreason = ToucanPlugin.Instance.Config.DefaultKickAllReason.Replace("{kicker}", Cmds[1]);
                    }
                    Player.List.ToList().ForEach(user => user.Kick(kickallreason));
                    Tcp.Send(Cmd);
                    Tcp.Send($"log Everyone was kicked remotly by {Cmds[1]}");
                    break;

                case "ban":
                    //ban {duration} {banee} {banner} {reason}
                    string banreason = null;
                    if (Cmds[4] != null)
                    {
                        banreason = Cmd.Replace($"ban {Cmds[1]} {Cmds[2]} {Cmds[3]} ", "");
                    }
                    else
                    {
                        banreason = ToucanPlugin.Instance.Config.DefaultBanReason.Replace("{banner}", Cmds[3]);
                    }
                    Player.List.ToList().Find(x => x.UserId.Contains(Cmds[1])).Ban(int.Parse(Cmds[1]), banreason, Cmds[2]);
                    Tcp.Send(Cmd);
                    Tcp.Send($"log {Cmds[2]} was banned for {Cmds[1]} by {Cmds[3]} for the reason of; {Cmds[4]}");
                    break;

                case "restartRound":
                    Log.Info("Remote restart round recived.");
                    Round.Restart();
                    break;

                case "store":
                    if (Store.StoreStock != null) return;
                    Store.StoreStock = Cmd.ToString().Remove(0, 6);
                    Log.Info("Store Retrived");
                    break;

                case "msg":
                    string msg = Cmd.Remove(0, 3);
                    Player.List.ToList().ForEach(player =>
                        player.SendConsoleMessage(msg, "#fffff"));
                    break;

                case "list":
                    string playerList = $"List of players ({Player.List.ToList().Count}):";
                    Player.List.ToList().ForEach(player =>
                    {
                        playerList = $"{playerList}\n - [{player.Id}]{player.DisplayNickname} ({player.UserId})";
                    });
                    /*for(int i = 0; i < Player.List.ToList().Count; i++) {
                        playerList = $"{playerList}\n - [{Player.List.ToList()[i].Id}]{Player.List.ToList()[i].DisplayNickname} ({Player.List.ToList()[i].UserId})";
                    };*/
                    Tcp.Send($"list {Cmds[1]} {playerList}".Trim());
                    break;

                case "bestbois":
                    BestBois = new List<string>(Cmd.Replace("bestbois ", "").Split(' '));
                    Log.Info("Best bois recived!");
                    break;

                case "whitelist":
                    // whitelist {message channel} {comment} {whitelist user}
                    if (Cmds[3] == null)
                    {
                        if (wl.Whitelisted)
                        {
                            wl.Whitelisted = false;
                            Tcp.Send($"msg {Cmd[1]} Server is now open!");
                        }
                        else
                        {
                            wl.Whitelisted = true;
                            Tcp.Send($"msg {Cmd[1]} Server is now closed!");
                        }
                    }
                    else
                    {
                        if (wl.WhitelistUsers.Contains(Cmds[3]))
                        {
                            wl.Add(Cmds[3], Cmds[2]);
                            Tcp.Send($"msg {Cmd[1]} User ({Cmd[3]}) now whitelisted!");
                        }
                        else
                        {
                            wl.Remove(Cmds[3]);
                            Tcp.Send($"msg {Cmd[1]} User ({Cmd[3]}) taken off the whitelist.");
                        }
                    }
                    break;

                case "play":
                    //var source = @"<your destination folder>";
                    var youtube = YouTube.Default;
                    var vid = youtube.GetVideo(Cmds[1]);
                    /*System.IO.File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

                    var inputFile = new MediaFile { Filename = source + vid.FullName };
                    var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };

                    using (var engine = new Engine())
                    {
                        engine.GetMetadata(inputFile);

                        engine.Convert(inputFile, outputFile);
                    }*/
                    GameObject sp = new GameObject
                    {
                        name = "Toucan"
                    };
                    sp.SetActive(true);
                    sp.GetComponentInChildren<DissonanceUserSetup>();
                    Intercom.host._StartTransmitting(sp);
                    break;
            }
        }
    }
}
