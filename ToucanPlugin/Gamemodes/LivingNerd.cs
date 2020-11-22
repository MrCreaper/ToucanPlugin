using System;
using System.Linq;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class LivingNerd
    {
        public void LivingNerd_()
        {
            Random rnd = new Random();
            Player Nerd = Exiled.API.Features.Player.List.ToList().Find(x => x.Id == rnd.Next(0, Player.List.Count()));
            Exiled.API.Features.Player.List.ToList().ForEach(p =>
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
