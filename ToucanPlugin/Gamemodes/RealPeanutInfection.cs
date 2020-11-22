using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class RealPeanutInfection
    {
        public static List<Player> Nuts { get; set; } = new List<Player>();
        public static List<Player> DClass { get; set; } = new List<Player>();
        public void RealPeanutInfection_()
        {
            Map.Broadcast(5, "REAL Peanut Infection Gamemode started!");
            int teamCount = Player.List.Count();
            for (int i = 0; i <= teamCount; i++)
            {
                Player fucker = Player.List.ToList().Find(x => x.Id == i);
                if (i == teamCount)
                {
                    Nuts.Add(fucker);
                    fucker.SetRole(RoleType.Scp173);
                }
                else
                {
                    DClass.Add(fucker);
                    fucker.SetRole(RoleType.ClassD);
                }
            }
        }
    }
}
