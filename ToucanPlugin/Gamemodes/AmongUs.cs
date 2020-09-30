using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class AmongUs
    {
        public static List<Player> ImpostersSet { get; } = new List<Player>();
        public void Amongus()
        {
            int imposterCount = 1; // default
            List<Player> playerList = Player.List.ToList();
            Random rnd = new Random();
            playerList.ForEach(p =>
            {
                if (imposterCount != ImpostersSet.Count)
                {
                    // The Imposter
                    p.SetRole(RoleType.ClassD);
                    p.Inventory.AddNewItem(ItemType.GunCOM15);
                    if (imposterCount == 1)
                        p.Broadcast(5, "< color =#db140d>You are a imposter...\nKill everyone to win.</color>");
                    ImpostersSet.Add(p);
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
                ImpostersSet.ForEach(imp => ImposterNameList = $"{ImposterNameList} {imp.Nickname}");
                ImpostersSet.ForEach(imp => imp.Broadcast(5, $"< color =#db140d>You are a imposter whit {ImposterNameList}...\nKill everyone (whit your one shot com15) to win.</color>"));
            }
        }
    }
}
