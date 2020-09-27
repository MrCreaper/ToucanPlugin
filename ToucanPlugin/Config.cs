using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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
        public bool MentionRoles { get; set; } = false;
        public string FivePlayerRole { get; set; } = "";
        public string TenPlayerRole { get; set; } = "";
        public string FifteenPlayerRole { get; set; } = "";
        [Description("Detonate the alpha warhead at the end of the round.")]
        public bool DetonateAtRoundEnded { get; set; } = false;
        [Description("Have a 1 in a 100 % chance to turn into a zombie when killed")]
        public bool Random008Spawn { get; set; } = false;

        public bool CanUIUSpawn { get; set; } = false;
        public List<int> UIUSpawnItems { get; set; } = new List<int> { 13, 34 };
        public bool CanHammerDownSpawn { get; set; } = false;
        public List<int> HammerDownSpawnItems { get; set; } = new List<int> { 16, 30, 7, 14, 34, 12, 19, 27 };
        public bool CanRedHandSpawn { get; set; } = false;
        public List<int> RedHandSpawnItems { get; set; } = new List<int> { 24, 30, 8, 17, 12, 19, 27 };
        public bool CanMedicMTFSpawn { get; set; } = false;
        public List<int> MTFMedicItems { get; set; } = new List<int> { 17, 33, 33, 33, 34, 12, 13 };
        public bool CanJanitorSpawn { get; set; } = false;
        public List<int> JanitorItems { get; set; } = new List<int> { 0, 34, 35 };
        public bool CanChaosHackerSpawn { get; set; } = false;
        public List<int> ChaosHackerItems { get; set; } = new List<int> { 23, 14, 15, 12 };
        [Description("Start the round automaticly after a minute")]
        public bool LonelyRound { get; set; } = false;
    }
}
