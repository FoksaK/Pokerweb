using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Pokerweb.Models
{
    public class Room
    {
        public int KeyNumber { get; set; }
        public List<Player> Players { get; set; }
        public List<string> Cards { get; set; }
        public int Sum { get; set; }
        public bool InGame { get; set; }
    }
}
