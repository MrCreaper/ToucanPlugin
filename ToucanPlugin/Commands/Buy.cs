using CommandSystem;
using GameCore;
using System;
using Exiled.API.Features;
using System.Linq;
using RemoteAdmin;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class Buy : ICommand
    {
        readonly Tcp tcp = new Tcp();
        public string Command { get; } = "buy";

        public string[] Aliases { get; } = { "b" };

        public string Description { get; } = "Buy stuff to ruin other peoples lives";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            if (Tcp.IsConnected())
            {
                if (Store.StoreStock != null)
                {
                    if (ToucanPlugin.Instance.Config.CanBuy)
                    {
                        if (Sender is PlayerCommandSender player) //CommandSender
                        {
                            if (Round.IsStarted)
                            {
                                if (Player.List.ToList().Find(x => x.UserId.Contains(player.SenderId)).IsAlive)
                                {
                                    if (Player.List.ToList().Find(x => x.UserId.Contains(player.SenderId)).Team != Team.SCP)
                                    {
                                        string[] args = arguments.Array;
                                        if (args[1] != null)
                                        {
                                            var isNumeric = int.TryParse(args[1], out int itemNum);
                                            if (isNumeric == true)
                                            {
                                                if (itemNum >= 1)
                                                {
                                                    if (itemNum < Store.StoreStock.Split('\n').Length)
                                                    {
                                                        tcp.Send($"buy {player.SenderId} {args[1]}");
                                                        response = "Please standby...";
                                                        return true;
                                                    }
                                                    else
                                                    {
                                                        response = "[That number] Its too big! Thats what she said.";
                                                        return false;
                                                    }
                                                }
                                                else
                                                {
                                                    response = "Hes power is over -9000! It isnt that good now right?";
                                                    return false;
                                                }
                                            }
                                            else
                                            {
                                                response = $"Yeah can i have a... {args[1]}?";
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            response = $"Didnt add wished item.";
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        response = $"How will you use the item whit your stinky fingies?";
                                        return false;
                                    }
                                }
                                else
                                {
                                    response = $"You can only buy stuff when your alive.";
                                    return false;
                                }
                            }
                            else
                            {
                                response = $"Wait for the round to start!";
                                return false;
                            }
                        }
                        else
                        {
                            response = $"Fuck off";
                            return false;
                        }
                    }
                    else
                    {
                        response = $"Sorry but buying stuff is currently disable.";
                        return false;
                    }
                }
                else
                {
                    response = $"Sorry the store hasnt been deliverd yet, try again later.";
                    return false;
                }
            }
            else
            {
                response = $"Sorry, we have lost connection to Toucan Servers. Try again in a few minutes.";
                return false;
            }
        }
    }
}
