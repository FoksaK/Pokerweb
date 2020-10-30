using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pokerweb.Models
{
    public class Player
    {
        public bool Played { get; set; } = false;
        public string PlayerName { get; set; }
        public bool Founder { get; set; } = false;
        public int LastMoney { get; set; } = 2000;
        public int Money { get; set; }
        public int MoneyFinal { get { return LastMoney + Money; } }
        public List<string> Cards { get; set; }
        public bool InGame { get; set; } = true;
        public string Adress { get; set; }
    }
}
