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
    }
}
