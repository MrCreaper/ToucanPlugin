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
        public void RealPeanutInfectione()
        {
            Map.Broadcast(5, "REAL Peanut Infection Gamemode started!");
            int teamCount = Player.List.Count() / 2;
            for (int i = 0; i <= teamCount; i++)
            {
                if(i == teamCount) Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.Scp173); 
                else
                Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.ClassD);
            }
        }
    }
}
