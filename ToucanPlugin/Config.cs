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
        public bool Debug { get; set; } = false;
        [Description("Can buy stuff from the store?")]
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
        public Handlers.XYZ CrouchingSize { get; set; } = new Handlers.XYZ() { X = 1, Y = 0.4f, Z = 1 };
        [Description("Force 106 be contained by the femur breaker")]
        public bool Force106Femur { get; set; } = false;
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
        public List<string> ADBlacklist { get; set; } = new List<string> { ".com", ".tf", "ttv/", "YT", ".money", "csgo" };
        public List<Handlers.PlayerCountMentionsClass> PlayerCountMentions { get; set; } = new List<Handlers.PlayerCountMentionsClass>()
        {
            new Handlers.PlayerCountMentionsClass() {
                PlayerCount=-1,
                RoleID="Example Role id, idk",
            },
        };
        public Dictionary<string, string> PlayerRoleMentions { get; set; } = new Dictionary<string, string>();
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
                Enabled=true,
                Name="Janitor",
                Role=RoleType.ClassD,
                PlayerCount=4,
                ReplaceChance=10,
                MaxHealth=110,
                MaxEnergy=120,
                MaxAdrenalin=100,
                Items=new List<int> { 0, 34, 35 },
                PreSetSpawnPosRole=RoleType.None,
                PreSetSpawnPosRoom=RoomType.Unknown,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>Janior</color>... LIFi SUCZ</i>",
            },
            new Handlers.CustomPersonelSpawns() {
                Enabled=true,
                Name="Mayor Scientist",
                Role=RoleType.Scientist,
                PlayerCount=5,
                ReplaceChance=10,
                MaxHealth=90,
                MaxEnergy=90,
                MaxAdrenalin=120,
                Items=new List<int> { },
                PreSetSpawnPosRole=RoleType.None,
                PreSetSpawnPosRoom=RoomType.LczToilets,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>Mayor Scientist</color>.</i>",
            },
            new Handlers.CustomPersonelSpawns() {
                Enabled=false,
                Name="Containment Engineer",
                Role=RoleType.ClassD,
                PlayerCount=6,
                ReplaceChance=10,
                MaxHealth=110,
                MaxEnergy=120,
                MaxAdrenalin=100,
                Items=new List<int> { 0, 34, 35 },
                PreSetSpawnPosRole=RoleType.None,
                PreSetSpawnPosRoom=RoomType.Unknown,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>Containment Engineer</color>. You had <color=yellow>one</color> job.</i>",
            },
            new Handlers.CustomPersonelSpawns() {
                Enabled=false,
                Name="MTF Medic",
                Role=RoleType.NtfLieutenant,
                PlayerCount=6,
                MaxSCPKills=999,
                MinSCPKills=10,
                ReplaceChance=10,
                MaxHealth=80,
                MaxEnergy=100,
                MaxAdrenalin=175,
                Items=new List<int> { 17, 33, 33, 33, 34, 12, 13 },
                PreSetSpawnPosRole=RoleType.None,
                PreSetSpawnPosRoom=RoomType.Unknown,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>MTF Medic</green>. Your grenades heal people, dont forget to spread the medkits.</i>",
            },
                new Handlers.CustomPersonelSpawns() {
                Enabled=false,
                Name="Chaos Hacker",
                Role=RoleType.ChaosInsurgency,
                PlayerCount=6,
                MaxSCPKills=20,
                MinSCPKills=5,
                ReplaceChance=10,
                MaxHealth=110,
                MaxEnergy=120,
                MaxAdrenalin=100,
                Items=new List<int> { 0, 34, 35 },
                PreSetSpawnPosRole=RoleType.None,
                PreSetSpawnPosRoom=RoomType.Unknown,
                SpawnPos=new Handlers.XYZ() {X=0,Y=0,Z=0},
                Hint=$"<i>You are a <color=yellow>Chaos Hacker</green>. You can hack doors, your sprint is you AP.</i>",
            },
        };
        public bool Random008Spawn { get; set; } = false;
        [Description("Start the round automaticly after a minute")]
        public bool LonelyRound { get; set; } = false;
        public Dictionary<GamemodeType, int> GamemodeChances { get; set; } = new Dictionary<GamemodeType, int>();
        [Description("SCP's that can speak using alternative speak")]
        public List<RoleType> AltSpeakScps { get; private set; } = new List<RoleType>() { RoleType.Scp049, RoleType.Scp0492, RoleType.Scp079, RoleType.Scp096, RoleType.Scp106, RoleType.Scp173, RoleType.Scp93953, RoleType.Scp93989 };
        [Description("CI hacker ability costs")]
        public List<Commands.Hack.CIHackingAbilityConfig> HackingAbility { get; set; } = new List<Commands.Hack.CIHackingAbilityConfig> {
            new Commands.Hack.CIHackingAbilityConfig(){
                Name="Intercom",
                Cost=60
            },
            new Commands.Hack.CIHackingAbilityConfig(){
                Name="Lights",
                Cost=75
            },
            new Commands.Hack.CIHackingAbilityConfig(){
                Name="Radio",
                Cost=65
            },
        };
        [Description("scp-079 abilitis and costs")]
        public List<Commands.Scp079.AbiltyRequirementData> Scp079Abilities { get; set; } = new List<Commands.Scp079.AbiltyRequirementData>
        {
            new Commands.Scp079.AbiltyRequirementData(){
                Cmd="blackout",
                Lvl=3,
                Energy=125,
                Xp=15f,
                Cooldown=10,
                Desc="Deactivates for 10 seconds all Lights in Heavy/Light",
            },
            new Commands.Scp079.AbiltyRequirementData(){
                Cmd="death",
                Lvl=3,
                Energy=100,
                Xp=20,
                Cooldown=20,
                Desc="Sends a fake Scp death Announcement",
            },
            new Commands.Scp079.AbiltyRequirementData(){
                Cmd="flash",
                Lvl=1,
                Energy=200,
                Xp=5,
                Cooldown=2,
                Desc="Explodes a FlashBang at the Camera => flashing all players looking at it",
            },
            new Commands.Scp079.AbiltyRequirementData(){
                Cmd="mtf",
                Lvl=0,
                Energy=0,
                Xp=0f,
                Cooldown=0,
                Desc="Sends a fake mtf spawn Announcement",
            },
            new Commands.Scp079.AbiltyRequirementData(){
                Cmd="scp",
                Lvl=2,
                Energy=50,
                Xp=1,
                Cooldown=0,
                Desc="changes your current camera to a one near an other Scp",
            },
            new Commands.Scp079.AbiltyRequirementData(){
                Cmd="robot",
                Lvl=3,
                Energy=100,
                Xp=10,
                Cooldown=0,
                Desc="Allows Scp079 to change his Role to a robot",
            },
            new Commands.Scp079.AbiltyRequirementData(){
                Cmd="scan",
                Lvl=0,
                Energy=0,
                Xp=0f,
                Cooldown=0,
                Desc="Gives a List of all Players with they Role and in which Zone they are",
            },
        };
    }
}
