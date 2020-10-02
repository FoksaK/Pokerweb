using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pokerweb.Models
{
    public class Player
    {
        public bool Played { get; set; } = false;
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public string Role { get; set; }
        public TimestampAttribute Timestamp { get; set; }
        private int LastMoney { get; set; } = 2000;
        public int Money { get; set; }
        public int MoneyFinal { get { return LastMoney + Money; } }
        public List<string> Cards { get; set; }
        public bool InGame { get; set; } = true;

        public string Adress { get; set; }
    }
}
