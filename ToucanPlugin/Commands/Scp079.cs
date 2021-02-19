using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MEC;
using Grenades;
using Mirror;
using UnityEngine;

namespace ToucanPlugin.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Scp079 : ICommand
    {
        List<Player> Robots = new List<Player>();
        public string Command { get; } = "079";

        public string[] Aliases { get; } = { "79" };

        public string Description { get; } = "Use 079 abilities";

        public bool Execute(ArraySegment<string> arguments, ICommandSender Sender, out string response)
        {
            List<string> args = new List<string>(arguments.Array);
            Player p = Player.List.ToList().Find(x => x.Sender == Sender);
            if (p.Role != RoleType.Scp079 && !Robots.Contains(p))
            {
                response = "Bruh";
                return false;
            }
            List<AbiltyRequirementData> ard = ToucanPlugin.Singleton.Config.Scp079Abilities;
            if (args.Count < 2)
            {
                string CmdList = "cmd - cost - lvl - desc";
                ard.ForEach(x => CmdList += $"\n{x.Cmd.PadRight(10)} - {x.Energy} - {x.Lvl} - {x.Desc}");
                response = CmdList;
                return true;
            }
            if (!ard.Exists(x => x.Cmd == args[1]))
            {
                response = "Invalid subcommand";
                return false;
            }
            AbiltyRequirementData abil = ard.Find(x => x.Cmd == args[1]);
            if (abil.Lvl > p.Level -1)
            {
                response = $"Too low access tier, required: {abil.Lvl}";
                return false;
            }
            if (abil.Energy > p.Energy)
            {
                response = $"Too low Energy, required: {abil.Energy}";
                return false;
            }
            p.Experience += abil.Xp;
            switch (ard.FindIndex(x => x.Cmd == args[1]))
            {
                default:
                    response = "Invalid subcommand";
                    return false;
                case 0: //Blackout
                    Map.TurnOffAllLights(10);
                    response = "Blackout...";
                    return true;
                case 1: //Fake death
                    Cassie.Message("SCP-079 successfully terminated by generator recontainment sequence.");
                    response = "Faking death announcment...";
                    return true;
                case 2: //Flash
                    UnityEngine.Vector3 relativeVelocity = p.Camera.head.position; // Thx AdminTools <3
                    GrenadeManager component1 = p.ReferenceHub.GetComponent<GrenadeManager>();
                    FlashGrenade component2 = UnityEngine.Object.Instantiate<GameObject>(((IEnumerable<GrenadeSettings>)component1.availableGrenades).FirstOrDefault<GrenadeSettings>((Func<GrenadeSettings, bool>)(g => g.inventoryID == ItemType.GrenadeFlash)).grenadeInstance).GetComponent<FlashGrenade>();
                    component2.InitData(component1, relativeVelocity, UnityEngine.Vector3.zero);
                    NetworkServer.Spawn(component2.gameObject);
                    response = "Flashing...";
                    return true;
                case 3: //Fake Mtf annc
                    char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
                    Cassie.Message($"MtfUnit NATO_{alpha[new System.Random().Next(0, alpha.Length -1)]} {new System.Random().Next()} HasEntered AllRemaining  NoSCPsLeft ");
                    response = "Faking mtf spawn announcment...";
                    return true;
                case 4: //scp
                    List<int> scpidlist = new List<int>();
                    Player.List.ToList().ForEach(x =>
                    {
                        if (x.Team == Team.SCP && x.Role != RoleType.Scp079)
                            scpidlist.Add(x.Id);
                    });
                    Player scp = Player.List.ToList().Find(fuck => fuck.Id == scpidlist[new System.Random().Next(0, scpidlist.Count)]);
                    p.Camera = scp.CurrentRoom.GetComponent<Camera079>();
                    response = "Dont loose him <3";
                    return true;
                case 5: //Robot
                    int RobotTime = 45;
                    Robots.Add(p);
                    p.SetRole(RoleType.Tutorial);
                    p.Position = Exiled.API.Extensions.CameraExtensions.Room(p.Camera).Position;
                    p.SendConsoleMessage($"Robot mode active. For {RobotTime}", "#fffff");
                    Timing.WaitForSeconds(RobotTime);
                    p.Kill();
                    p.SetRole(RoleType.Scp079);
                    response = "Back to camera mode.";
                    return true;
                case 6: //Scan
                    p.SendConsoleMessage("Scanning...", "#fffff");
                    string ScanResults = "Scan results: ";
                    Player.List.ToList().ForEach(x =>
                    {
                        if (x.Team != Team.SCP || x.Team != Team.RIP)
                            ScanResults += $"\n{x.Nickname,20} | {x.Role.ToString().PadLeft(5)} | {x.CurrentRoom.Zone.ToString().PadLeft(5)}";
                    });
                    response = ScanResults;
                    return true;
            }
        }
        public class AbiltyRequirementData
        {
            public string Cmd { get; set; }
            public int Lvl { get; set; }
            public float Energy { get; set; }
            public float Xp { get; set; }
            public float Cooldown { get; set; }
            public string Desc { get; set; }
        }
    }
}
