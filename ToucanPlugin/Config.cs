using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace ToucanPlugin
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public string ToucanServerIP { get; set; } = "";
        [Description("Can buy stuff")]
        public bool CanBuy { get; set; } = true;
        [Description("Sets the for when someone joins the server. {players} will be replaced with the players name")]
        public string JoinedMessage { get; set; } = "{player} has joined the server.";
        [Description("Sets the for when someone leaves the server. {players} will be replaced with the players name")]
        public string LeftMessage { get; set; } = "{player} has left the server.";
        [Description("Sets the message when the round starts")]
        public string RoundStartMessage { get; set; } = "";
        [Description("Reflect team damage")]
        public bool ReflectTeamDMG { get; set; } = false;
        [Description("Crouching...")]
        public bool CrouchingEnabled { get; set; } = false;
        [Description("Reflect team damage")]
        public Vector3 CrouchingSize { get; set; } = new Vector3(1, 0.4f, 1);
        [Description("Default Kick message, when the kicker didnt add a reason to why")]
        public string DefaultKickReason { get; set; } = "[Kicked by {kicker}] No Reason";
        [Description("Default Kick All message, when the kicker didnt add a reason to why")]
        public string DefaultKickAllReason { get; set; } = "[Everyone was Kicked by {kicker}] No Reason";
        [Description("Default Ban message, when the banner didnt add a reason to why")]
        public string DefaultBanReason { get; set; } = "[Banned by {banner}] No Reason";
        [Description("Can the inch by inch thing be cured whit scp500")]
        public bool InchByInchCureable { get; set; } = true;
        [Description("Replace advertisment names")]
        public bool ReplaceAdvertismentNames { get; set; } = true;
        [Description("Thing to replace advertisment part of a name whit")]
        public string ReplaceAdvertismentNamesWhit { get; set; } = "[ADVERTISMENT DENIED]";
        public List<string> ADThing { get; set; } = new List<string> { ".com", ".tf", "ttv/", "YT", ".money", "csgo" };
        public bool MentionRoles { get; set; } = false;
        public string FivePlayerRole { get; set; } = "";
        public string TenPlayerRole { get; set; } = "";
        public string FifteenPlayerRole { get; set; } = "";
        [Description("Detonate the alpha warhead at the end of the round.")]
        public bool DetonateAtRoundEnded { get; set; } = false;
        [Description("Read the github wiki to know more")]
        public List<Handlers.CustomSquadSpawns> CustomSquads { get; set; } = new List<Handlers.CustomSquadSpawns>
        {
            new Handlers.CustomSquadSpawns() {
                Name="The UIU",
                Team=Respawning.SpawnableTeamType.NineTailedFox,
                ReplaceChance = 100,
                Role = RoleType.FacilityGuard,
                MaxHealth = -1,
                MaxAdrenalin = 30,
                MaxEnergy = 60,
                Items = new List<int> { 13, 34 },
                CommanderItems = new List<int> { 13, 34, 7 },
                PreSetSpawnPos = RoleType.None,
                SpawnPos =new Handlers.XYZ(){X=176,Y=984,Z=39},
                SquadMaxSize = 0,
                MaxSCPKills = 2,
                MinSCPKills = 0,
                CassieAnnc =$"the u i u HasEntered"
            },
            new Handlers.CustomSquadSpawns() {
                Name="NU7 (Hammer Down)",
                Team=Respawning.SpawnableTeamType.NineTailedFox,
                ReplaceChance = 100,
                Role = RoleType.NtfLieutenant,
                MaxHealth = -1,
                MaxAdrenalin = 85,
                MaxEnergy = 120,
                Items = new List<int> { 16, 30, 7, 14, 34, 12, 19, 27 },
                CommanderItems = new List<int> { },
                PreSetSpawnPos = RoleType.None,
                SpawnPos =new Handlers.XYZ() {X=0f,Y=1005f,Z=-10f},
                SquadMaxSize = 0,
                MaxSCPKills = 15,
                MinSCPKills = 10,
                CassieAnnc =$"MTFUNIT n u 7 HasEntered"
            },
            new Handlers.CustomSquadSpawns() {
                Name="Red Right Hand",
                Team=Respawning.SpawnableTeamType.NineTailedFox,
                ReplaceChance = 100,
                Role = RoleType.NtfCommander,
                MaxHealth = 200,
                MaxAdrenalin = 300,
                MaxEnergy = 999,
                Items = new List<int> { 24, 30, 8, 17, 12, 19, 27 },
                CommanderItems = new List<int> { },
                PreSetSpawnPos = RoleType.NtfCommander,
                SpawnPos =new Handlers.XYZ() {X=0,Y=0,Z=0},
                SquadMaxSize = 0,
                MaxSCPKills = -1,
                MinSCPKills = 16,
                CassieAnnc =$"RED RIGHT HAND HasEntered . be advised"
            },
        };
        public List<Handlers.CustomPersonelSpawns> CustomPersonel { get; set; } = new List<Handlers.CustomPersonelSpawns>
        {
            new Handlers.CustomPersonelSpawns() {
                Name="Janitor",
                Role=RoleType.ClassD,
                PlayerCountNeeded=4,
                ReplaceChance=10,
                MaxHealth=110,
                MaxEnergy=120,
                MaxAdrenalin=100,
                Items=new List<int> { 0, 34, 35 },
                PreSetSpawnPos=RoleType.None,
                PreSetSpawnPosRoom=RoomType.LczToilets,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>Janior</color>... LIFi SUCZ</i>",
            },
            new Handlers.CustomPersonelSpawns() {
                Name="Mayor Scientist",
                Role=RoleType.Scientist,
                PlayerCountNeeded=5,
                ReplaceChance=10,
                MaxHealth=90,
                MaxEnergy=90,
                MaxAdrenalin=120,
                Items=new List<int> { },
                PreSetSpawnPos=RoleType.None,
                PreSetSpawnPosRoom=RoomType.LczToilets,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>Mayor Scientist</color>.</i>",
            },
            new Handlers.CustomPersonelSpawns() {
                Name="Containment Engineer",
                Role=RoleType.ClassD,
                PlayerCountNeeded=6,
                ReplaceChance=10,
                MaxHealth=110,
                MaxEnergy=120,
                MaxAdrenalin=100,
                Items=new List<int> { 0, 34, 35 },
                PreSetSpawnPos=RoleType.None,
                PreSetSpawnPosRoom=RoomType.LczToilets,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>Containment Engineer</color>. You had <color=yellow>one</color> job.</i>",
            },
        };
        public bool CanMedicMTFSpawn { get; set; } = false;
        public List<int> MTFMedicItems { get; set; } = new List<int> { 17, 33, 33, 33, 34, 12, 13 };
        public bool CanChaosHackerSpawn { get; set; } = false;
        public List<int> ChaosHackerItems { get; set; } = new List<int> { 23, 14, 15, 12 };
        public bool Random008Spawn { get; set; } = false;
        [Description("Start the round automaticly after a minute")]
        public bool LonelyRound { get; set; } = false;
        public bool RandomGamemodes { get; set; } = false;
        public int RandomGamemodeChance { get; set; } = 10;
        public List<GamemodeChances> GamemodeChances { get; set; } = new List<GamemodeChances>
        {
            new GamemodeChances() {
                QuietPlace = 33,
                PeanutInfection = 33,
                AmongUs = 34,
                //CandyRush = 25,
            }
        };
    }
}
