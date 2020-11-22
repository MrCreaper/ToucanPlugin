using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;

namespace ToucanPlugin
{
    public class Whitelist
    {
        public static bool Whitelisted { get; set; } = false;
        readonly private string WhitelistLocation = $"C:/Users/Kelvin Kersna/AppData/Roaming/SCP Secret Laboratory/config/{Server.Port}/UserIDWhitelist.txt";
        private List<string> WhitelistUsersRaw { get; set; } = new List<string> { };
        public static List<string> WhitelistUsers { get; set; } = new List<string> { };
        public void Read()
        {
            WhitelistUsers.Clear();
            string[] whitelistRaw = File.ReadAllLines(WhitelistLocation);
            foreach (string line in whitelistRaw)
            {
                WhitelistUsersRaw.Add(line);
                if (!line.StartsWith("#")) WhitelistUsers.Add(line);
            }
        }
        public void Add(string User, string Comment = null)
        {
            using (StreamWriter file =
new StreamWriter(WhitelistLocation, true))
            {
                if (Comment != null) 
                    file.WriteLine($"\n{User} #{Comment}");
                else
                    file.WriteLine($"{User}");
            }
            Read();
        }
        public void Remove(string User)
        {
            using (StreamWriter file =
                new StreamWriter(WhitelistLocation))
            {
                foreach (string line in WhitelistUsersRaw)
                {
                    if (!line.Contains(User)) file.WriteLine(line);
                }
            }
            Read();
        }
        public void KickAllNoneWhite()
        {// Sounds racist ik
            Player.List.ToList().ForEach(p =>
            {
                if (!WhitelistUsers.Contains(p.UserId))
                    p.Kick("Sorry the server is right now whitelisted. Come back later!");
            });
        }
    }
}
