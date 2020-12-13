using System;
using System.Linq;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class LivingNerd
    {
        public void Setup()
        {
            Random rnd = new Random();
            Player Nerd = Player.List.ToList().Find(x => x.Id == rnd.Next(0, Player.List.Count()));
            Player.List.ToList().ForEach(p =>
            {
                if (Nerd != p)
                    p.SetRole(RoleType.ClassD);
            });
            Nerd.SetRole(RoleType.Scientist);
            for(int i = 0; i < 10; i++)
            Nerd.AddItem(ItemType.GunLogicer);
        }
    }
}
