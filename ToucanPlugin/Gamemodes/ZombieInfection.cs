using System;
using System.Linq;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class ZombieInfection
    {
        public void Setup()
        {
            Random rnd = new Random();
            Player Zomb = Player.List.ToList().Find(x => x.Id == rnd.Next(0, Player.List.Count()));
            Player.List.ToList().ForEach(p =>
            {
                if (Zomb != p)
                    p.SetRole(RoleType.ClassD);
            });
            Zomb.SetRole(RoleType.Scp0492);
            Zomb.Position = RoleType.ClassD.GetRandomSpawnPoint();
        }
        public void OnChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            if (ev.NewRole == RoleType.Scp0492 && GamemodeLogic.RoundGamemode == GamemodeType.ZombieInfection)
                ev.Player.ReferenceHub.characterClassManager.Classes.ToList().Find(x => x.roleId == RoleType.Scp0492).walkSpeed = 10f;
        }
    }
}
