using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;

namespace ToucanPlugin
{
    public class ReservedSlots
    {
        readonly private string ReservedSlotsLocation = $"{Paths.AppData}\\SCP Secret Laboratory\\config\\{Server.Port}\\UserIDReservedSlots.txt";
        private List<string> ReservedSlotsRaw { get; set; } = new List<string> { };
        public static List<string> ReservedSlotsUsers { get; set; } = new List<string> { };
        public void Read()
        {
            string[] whitelistRaw = File.ReadAllLines(ReservedSlotsLocation);
            foreach (string line in whitelistRaw)
            {
                ReservedSlotsRaw.Add(line);
                if (!line.StartsWith("#")) ReservedSlotsUsers.Add(line);
            }
        }
        public void Add(string User, string Comment = null)
        {
            using (StreamWriter file =
new StreamWriter(ReservedSlotsLocation, true))
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
                new StreamWriter(ReservedSlotsLocation))
            {
                foreach (string line in ReservedSlotsRaw)
                {
                    if (!line.Contains(User)) file.WriteLine(line);
                }
            }
            Read();
        }
    }
}
