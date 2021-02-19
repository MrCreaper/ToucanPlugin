using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ToucanPlugin.Handlers
{

    public class XYZ
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3 ToVector3(XYZ xyz)
        {
            return new Vector3(xyz.X, xyz.Y, xyz.Z);
        }
    }
    public class PlayerCountMentionsClass
    {
        public int PlayerCount { get; set; }
        public string RoleID { get; set; }
    }
    public enum AbilityType
    {
        None = 0,
        HealingGrenade = 1,
        DoorHacking = 2,
        IcomDisabling = 3,
        Blackout = 4,
        RadioDisabling = 4,
    }
    public class CustomSquadSpawns
    {
        public bool Enabled { get; set; }
        public string Name { get; set; } // Just for the user
        public Respawning.SpawnableTeamType Team { get; set; }
        public int ReplaceChance { get; set; } // Chance 1-100 for [name] to spawn insteed of [team] (0 for scpKill)
        public RoleType Role { get; set; }
        public int MaxHealth { get; set; }
        public int MaxAdrenalin { get; set; }
        public int MaxEnergy { get; set; }
        public List<int> Items { get; set; }
        public List<int> CommanderItems { get; set; }
        public RoleType PreSetSpawnPos { get; set; }
        public XYZ SpawnPos { get; set; }
        public int SquadMaxSize { get; set; }
        public int MaxSCPKills { get; set; }
        public int MinSCPKills { get; set; }
        public string CassieAnnc { get; set; }
    }
    public class CustomPersonelSpawns
    {
        public bool Enabled { get; set; }
        public string Name { get; set; } // Just for the user
        public RoleType Role { get; set; }
        public int PlayerCount { get; set; }
        public int MaxSCPKills { get; set; }
        public int MinSCPKills { get; set; }
        public int ReplaceChance { get; set; } // Chance 1-100 for [name] to spawn insteed of [team] (0 for scpKill)
        public int MaxHealth { get; set; }
        public int MaxEnergy { get; set; }
        public int MaxAdrenalin { get; set; }
        public List<int> Items { get; set; }
        public RoleType PreSetSpawnPosRole { get; set; }
        public RoomType PreSetSpawnPosRoom { get; set; }
        public XYZ SpawnPos { get; set; }
        public string Hint { get; set; }
        public List<AbilityType> Abilities { get; set; }
    }
}
