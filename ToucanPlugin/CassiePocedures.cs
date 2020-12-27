using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Enums;
using MEC;
using UnityEngine;

namespace ToucanPlugin
{
    public enum Procedures
    {
        None = 0,
        PE1 = 1,
        PE2 = 2,
        PE3 = 3,
        PE4 = 4,
        PE5 = 5,
        PE6 = 6,
        PE7 = 7,
        PE8 = 8,
        PE9 = 9,
        PB1 = 10,
        PB2 = 11,
        PB3 = 12,
        PB4 = 13,
        PL1 = 14,
        PL2 = 15,
        PL3 = 16,
        PS1 = 17,
        PS2 = 18,
        PS3 = 19,
    }
    class CassieProcedures
    {
        public void PE1(Room r)
        {
            r.gameObject.GetComponent<LightContainmentZoneDecontamination.DecontaminationController>().FinishDecontamination();
        }
        public void PB1(Room r)
        {
            r.Doors.ToList().ForEach(d =>
            {
                d.isOpen = false;
                d.locked = true;
            });
            r.Players.ToList().ForEach(p => p.EnableEffect<CustomPlayerEffects.Decontaminating>());
        }
        public void PB2(ZoneType z)
        {
            Door HvyEzCheckpoint = Map.Rooms.ToList().Find(x => x.Type == RoomType.HczEzCheckpoint).Doors.ToList().Find(door => door.doorType == Door.DoorTypes.Checkpoint);
            switch (z)
            {
                case ZoneType.Unspecified:
                default:
                    return;
                case ZoneType.LightContainment:
                    Map.StartDecontamination();
                    break;
                case ZoneType.HeavyContainment:
                    HvyEzCheckpoint.isOpen = false;
                    HvyEzCheckpoint.locked = true;
                    DecoAll(ZoneType.HeavyContainment);
                    Map.Lifts.ToList().ForEach(l =>
                    {
                        if (l.elevatorName == "" || l.elevatorName == "")
                            l.operative = false;
                    });
                    break;
                case ZoneType.Entrance:
                    HvyEzCheckpoint.isOpen = false;
                    HvyEzCheckpoint.locked = true;
                    DecoAll(ZoneType.Entrance);
                    PL1();
                    Map.Lifts.ToList().ForEach(l =>
                    {
                        if (l.elevatorName == "" || l.elevatorName == "")
                            l.operative = false;
                    });
                    break;
                case ZoneType.Surface:
                    Map.Lifts.ToList().ForEach(l =>
                    {
                        if (l.elevatorName == "" || l.elevatorName == "")
                            l.operative = false;
                    });
                    DecoAll(ZoneType.Surface);
                    break;
            }
        }
        private void DecoAll(ZoneType z)
        {
            Player.List.ToList().ForEach(p =>
            {
                if (p.CurrentRoom.Zone == z)
                    p.EnableEffect<CustomPlayerEffects.Decontaminating>();
            });
        }
        public void PB3(ZoneType z, int CountDownSec = 0)
        {
            Timing.WaitForSeconds(CountDownSec);
            PB2(z);
        }
        public void PB4()
        {
            Warhead.Start();
        }
        public void PL1(bool Enabled = true)
        {
            Door gA = Map.Rooms.ToList().Find(x => x.Type == RoomType.EzGateA).Doors.ToList().Find(door => door.doorType == Door.DoorTypes.HeavyGate);
            Door gB = Map.Rooms.ToList().Find(x => x.Type == RoomType.EzGateB).Doors.ToList().Find(door => door.doorType == Door.DoorTypes.HeavyGate);
            gA.isOpen = false;
            gA.locked = Enabled;
            gB.isOpen = false;
            gB.locked = Enabled;
        }
        public void PL2(bool Enabled = true)
        {
            Map.Doors.ToList().ForEach(d =>
            {
                if (d.doorType == Door.DoorTypes.Checkpoint || d.doorType == Door.DoorTypes.HeavyGate && d.DoorName.Contains("ent"))
                {
                    d.isOpen = !Enabled;
                    d.locked = Enabled;
                }
            });
        }
        public void PL3(bool Enabled = true)
        {
            Map.Doors.ToList().ForEach(d =>
            {
                d.isOpen = false;
                d.locked = Enabled;
            });
        }
        private bool PS1Enabled = false;
        private bool PS1Running = false;
        public void PS1(bool Enabled)
        {
            PS1Enabled = Enabled;
            if (PS1Running) return;
            PS1Running = true;
            for (; ; )
            {
                if (!PS1Enabled) {
                    PS1Running = false;
                    return;
                }
                Player.List.ToList().ForEach(p =>
                {
                    if (p.Team == Team.SCP && p.Role != RoleType.Scp079)
                    {
                        Camera079 cam = p.CurrentRoom.GetComponent<Camera079>();
                        cam.transform.LookAt(cam.transform.position + p.CameraTransform.rotation * Vector3.forward, p.CameraTransform.rotation * Vector3.up);
                    }
                });
                Timing.WaitForSeconds(5);
            }
        }
        private bool PS2Enabled = false;
        private bool PS2Running = false;
        private Dictionary<int, ZoneType> PS2AnncLastZones = new Dictionary<int, ZoneType>();
        public void PS2(bool Enabled = true)
        {
            PS2Enabled = Enabled;
            if (PS2Running) return;
            PS2Running = true;
            for (; ; )
            {
                if (!PS2Enabled)
                {
                    PS2Running = false;
                    return;
                }
                Player.List.ToList().ForEach(p =>
                {
                    if (PS2AnncLastZones.ContainsKey(p.Id))
                    {
                        if (p.Team != Team.SCP)
                            PS2AnncLastZones.Remove(p.Id);
                    }
                    else
                    {
                        if (p.Team == Team.SCP && p.Role != RoleType.Scp079)
                            PS2AnncLastZones.Add(p.Id, p.CurrentRoom.Zone);
                    }
                });
                Timing.WaitForSeconds(5);
            }
        }
        public void PS3(bool Enabled = true)
        {
            Map.TeslaGates.ToList().ForEach(t => t.enabled = Enabled);
        }
    }
}
