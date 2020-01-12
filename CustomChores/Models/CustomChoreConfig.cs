using System.Collections.Generic;

namespace LeFauxMatt.CustomChores.Models
{
    public class CustomChoreConfig
    {
        public string ChoreName { get; set; }
        public double Chance { get; set; }

        public CustomChoreConfig(string choreName, double chance)
        {
            ChoreName = choreName;
            Chance = chance;
        }
    }
}
