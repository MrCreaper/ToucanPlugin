/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using LiteNetLib;
using UnityEngine;

namespace ToucanPlugin.Gamemodes
{
    public class CandyRush
    {
        public static List<string> DefuserList { get; set; } = new List<string>();
        public static List<string> BommerList { get; set; } = new List<string>();
        public void CandyRush_()
        {
            int Defusers = (int)Math.Floor(Player.List.Count() / 5d);
            int DefusersSpawned = 0;
            List<int> DefuserItems = new List<int> { 30, 21 };
            List<Player> playerList = Player.List.ToList();
            Log.Debug($"Spwaning {Defusers} Defusers");
            playerList.ForEach(p =>
            {
                if (Defusers != DefusersSpawned)
                {
                    // Defuser
                    DefuserList.Add(p.UserId);
                    p.SetRole(RoleType.Scientist);
                    DefuserItems.ForEach(num => p.Inventory.AddNewItem((ItemType)num));
                    p.ShowHint($"You are a <color=yellow>Defuser</color>\nKill the suicide bommers to win!", 5);
                    DefusersSpawned++;
                }
                else
                {
                    // Bommer
                    BommerList.Add(p.UserId);
                    p.SetRole(RoleType.ClassD);
                    p.Inventory.AddNewItem(ItemType.PinkCandy);
                    p.ShowHint($"<color=#0d9ddb>You are a Bommer</color>\nKill the Defuser's by suicide bomming to win!", 5);
                }
            });
        }
    }
}*/
