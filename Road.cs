//CAB301 - Assignment 3A - Road class
// Maolin Tang and Connor McLaughlin 
// 2025


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment3A
{
    public class Road
    {
        public string Suburb1 { get; set; }
        public string Suburb2 { get; set; }
        public uint Distance { get; set; }

        public Road(string suburb1, string suburb2, uint distance)
        {
            Suburb1 = suburb1;
            Suburb2 = suburb2;
            Distance = distance;
        }

        public override string ToString()
        {
            return $"{Suburb1} ↔ {Suburb2}: {Distance} km";
        }
    }


}
