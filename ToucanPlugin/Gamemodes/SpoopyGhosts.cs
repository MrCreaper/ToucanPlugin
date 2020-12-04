using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Exiled.API.Extensions;
using UnityEngine;
using ToucanPlugin.Commands;

namespace ToucanPlugin.Gamemodes
{
    class SpoopyGhosts
    {
        public static List<Player> InvisScpList = new List<Player>();
        public void SpoopyGhosts_()
        {
            Player.List.ToList().ForEach(p =>
            {
                if (p.Team == Team.SCP)
                    InvisScpList.Add(p);
                else
                    if (p.Inventory.items.Count != 8)
                    p.AddItem(ItemType.SCP268);
                else
                    Exiled.API.Extensions.Item.Spawn(ItemType.SCP268, 1, p.Position);
            });
            InvisScpList.ForEach(p =>
            {
                if (!p.IsInvisible)
                    p.IsInvisible = true;
            });
        }
        public static void InvisScpDead(Player p)
        {
            if (InvisScpList.Contains(p) && GamemodeLogic.RoundGamemode == GamemodeType.SpoopyGhosts)
                InvisScpList.Remove(p);
        }
    }
}
