using System;
using System.Linq;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class QuietPlace
    {
        public void QuietPlace_()
        {
            int teamCount = Player.List.ToList().Count;
            for (int i = 0; i < teamCount; i++)
            {
                Random rnd = new Random();
                if (rnd.Next(0, 1) == 0) //Get a random dog type
                    Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.Scp93953);
                else
                    Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.Scp93989);
            }
            for(int i = teamCount; i < Player.List.Count(); i++)
                Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.Scientist);
        }
    }
}
