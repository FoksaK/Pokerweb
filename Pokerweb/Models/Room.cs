using System.Collections.Generic;

namespace Pokerweb.Models
{
    public class Room
    {
        public int Round { get; set; } = 0;
        public int Last { get; set; }
        public int KeyNumber { get; set; }
        public List<Player> Players { get; set; }
        public List<string> Cards { get; set; }
        public int Sum { get; set; }
        public bool InGame { get; set; }
    }
}
