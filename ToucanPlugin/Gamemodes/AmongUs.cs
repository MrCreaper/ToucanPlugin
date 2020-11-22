using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using UnityEngine;

namespace ToucanPlugin.Gamemodes
{
    public class AmongUs
    {
        public static List<Player> Imposters { get; } = new List<Player>();
        public static List<Vector2> DeathCords { get; } = new List<Vector2>();
        public void AmongUs_()
        {
            int imposterCount = 1; // default
            List<Player> playerList = Player.List.ToList();
            System.Random rnd = new System.Random();
            playerList.ForEach(p =>
            {
                if (imposterCount != Imposters.Count)
                {
                    // The Imposter
                    p.SetRole(RoleType.ClassD);
                    p.Inventory.AddNewItem(ItemType.GunCOM15);
                    if (imposterCount == 1)
                        p.Broadcast(5, "<color=#db140d>You are a imposter...\nKill everyone to win.</color>");
                    Imposters.Add(p);
                }
                else
                {
                    // Crewmate
                    p.SetRole(RoleType.ClassD);
                    p.Broadcast(5, $"<color=#0d9ddb>You are a crewmate</color>\nThere is <color=#db140d>{imposterCount} imposter</color> among us");
                }
            });
            if (imposterCount != 1)
            {// Brodcast to multiple imposters
                string ImposterNameList = "";
                Imposters.ForEach(imp => ImposterNameList = $"{ImposterNameList} {imp.Nickname}");
                Imposters.ForEach(imp => imp.Broadcast(5, $"< color =#db140d>You are a imposter whit {ImposterNameList}...\nKill everyone (whit your one shot com15) to win.</color>"));
            }
        }
        static public void ReportBody(string reporterId)
        {
            Map.ClearBroadcasts();
            Map.Broadcast(6, $"!!!EMERGANY MEATING!!!\n(Called by: {Player.List.ToList().Find(x => x.UserId == reporterId).Nickname})");
            Player.List.ToList().ForEach(p => 
                p.Position = new Vector3(1f, 1f, 1f));
        }
    }
}
