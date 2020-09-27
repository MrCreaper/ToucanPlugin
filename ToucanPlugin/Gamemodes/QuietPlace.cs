using System;
using System.Linq;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class QuietPlace
    {
        public void QuietPlacee()
        {
            Map.Broadcast(5, "Quite Place Gamemode started!\n<size=2>Idea by Symbol#0420 </size>");
            int teamCount = Player.List.Count() / 2;
            for (int i = 0; i < teamCount; i++)
            {
                Random rnd = new Random();
                if (rnd.Next(0, 1) == 0) //Get a random dog type
                {
                    Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.Scp93953);
                }
                else
                {
                    Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.Scp93989);
                }
            }
            for(int i = teamCount; i < Player.List.Count(); i++)
            {
                Player.List.ToList().Find(x => x.Id == i).SetRole(RoleType.Scientist);
            }
        }
    }
}
