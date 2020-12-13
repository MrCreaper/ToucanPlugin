using System;
using System.Linq;
using Exiled.API.Features;

namespace ToucanPlugin.Gamemodes
{
    public class Scp682
    {
        public void Setup()
        {
            Random rnd = new Random();
            Player Scp682 = Exiled.API.Features.Player.List.ToList().Find(x => x.Id == rnd.Next(0, Player.List.Count()));
            Exiled.API.Features.Player.List.ToList().ForEach(p =>
            {
                if(Scp682 != p)
                    p.SetRole(RoleType.NtfCommander);
            });
            Scp682.SetRole(RoleType.Scp93953);
            Scp682.Position = RoleType.ChaosInsurgency.GetRandomSpawnPoint();
        }
    }
}
