using System;
using System.Collections.Generic;
using System.IO;

namespace ToucanPlugin
{
    public class ReservedSlots
    {
        readonly private string ReservedSlotsLocation = @"C:\Users\Kelvin Kersna\AppData\Roaming\SCP Secret Laboratory\config\7777\UserIDReservedSlots.txt";
        private List<String> ReservedSlotsRaw { get; set; } = new List<string> { };
        public List<String> ReservedSlotsUsers { get; set; } = new List<string> { };
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
                if (Comment != null) file.WriteLine($"#{Comment}\n{User}");
                else
                    file.WriteLine($"{User}");
            }
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
        }
    }
}
