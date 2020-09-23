using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ToucanPlugin
{
    public class Whitelist
    {
        public bool Whitelisted { get; set; } = false;
        readonly private string WhitelistLocation = @"C:\Users\Kelvin Kersna\AppData\Roaming\SCP Secret Laboratory\config\7777\UserIDWhitelist.txt";
        private List<String> WhitelistUsersRaw { get; set; } = new List<string> { };
        public List<String> WhitelistUsers { get; set; } = new List<string> { };
        public void Read()
        {
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
                if(Comment != null) file.WriteLine($"#{Comment}\n{User}"); else
                file.WriteLine($"{User}");
            }
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
        }
    }
}
