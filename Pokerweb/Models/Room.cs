using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pokerweb.Models
{
    public class Room
    {
        public Room()
        {
            List<string> barva = new List<string>() { "kr_", "sr_", "ka_", "pi_" };
            List<string> hodnota = new List<string>() { "1","2","3","4","5","6","7","8","9","10", "j", "q", "k"};

            foreach (string b in barva)
            {
                foreach (string h in hodnota)
                {
                    cardsList.Add(b + h);
                }
            }

            Shuffle(ref cardsList);

            Queue<string> cards = new Queue<string>(cardsList);
            Packet = cards;

            Cards = GetChunk(5);

        }

        //properties
        public List<Player> Players = new List<Player>();
        public List<string> Winners { get; set; } = new List<string>();
        public Queue<string> Packet { get; set; }
        public int Round { get; set; } = 0;
        public int Last { get; set; }
        public int KeyNumber { get; set; }
        public List<string> Cards { get; set; }
        public bool InGame { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public string PagePartialHelper { get; set; }
        public int Sum { 
            get
            {
                int i = 0;
                foreach(Player player in Players)
                {
                    i += player.Money;
                }
                return -i;
            } 
        }

        //metod
        public void AddPlayer(Player player)
        {
            player.Cards = GetChunk(2);
            this.Players.Add(player);
        }

        public void PrepareNextRound()
        {
            Shuffle(ref cardsList);

            Queue<string> cards = new Queue<string>(cardsList);
            Packet = cards;

            Cards = GetChunk(5);

            foreach (Player player in Players)
            {
                if (!Winners.Contains(player.PlayerName))
                {
                    player.LastMoney = player.MoneyFinal;
                }

                player.Cards = GetChunk(2);
                player.Money = 0;
                player.Played = false;
                player.InGame = true;
            }

            Winners.Clear();
            Rotate(ref Players);
            Round = 0;
        }
        public Room ShallowCopy()
        {
            return (Room)this.MemberwiseClone();
        }

        //private
        private Random rnd = new Random();
        private List<string> cardsList = new List<string>();
        private void Shuffle<T>(ref List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        private void Rotate<T>( ref List<T> items)
        {
            T nItem;
            int count = items.Count;
            nItem = items[count - 1];
            items.RemoveAt(count - 1);
            items.Insert(0, nItem);
        }
        private List<string> GetChunk(int count)
        {
            List<string> chunk = new List<string>();
            for(int i = 0; i < count; i++)
            {
                chunk.Add(this.Packet.Dequeue());
            }
            return chunk;
        }
    }
}
