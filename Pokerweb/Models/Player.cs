using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pokerweb.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public string Role { get; set; }
        public bool Host { get; set; }
        public TimestampAttribute Timestamp { get; set; }
        public int Money { get; set; }
        public List<string> Cards { get; set; }
        public int Round { get; set; }
    }
}
