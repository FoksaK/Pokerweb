using Microsoft.AspNetCore.SignalR;
using Pokerweb.Data;
using Pokerweb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Pokerweb.Hubs
{
    public class ChatHub : Hub
    {

        public void Connected(string key, string username)
        {
            int _key = Convert.ToInt32(key);
            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.PlayerName == username).Adress = Context.ConnectionId;

            foreach (var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players)
            {
                Clients.Client(x.Adress).SendAsync("ReceiveMessage");
            }

            if (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Count >= 3) 
            {
                Clients.Client(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.Founder == true).Adress).SendAsync("ShowPlaybutton");
            }
            
        }

        public void StartMessage(string _key)
        {
            int key = Convert.ToInt32(_key);
            string username;

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[1].Money -= 5;
            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[2].Money -= 10;

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[1].Played = true;
            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[2].Played = true;

            username = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[2].PlayerName;
            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Last = 2;

            PlayMessage(_key, username);

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).InGame = true;
        }

        public void PlayMessage(string _key, string username)
        {
            int key = Convert.ToInt32(_key);

            int i = Convert.ToInt32(PlaySignal(username, key).Item1);
            bool ended = PlaySignal(username, key).Item2;

            if (NewRoundIsNext(key, i, RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.FindIndex(x => x.PlayerName == username))
                && !ended)
            {
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Played = true;

                Clients.Client(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Adress).SendAsync("ReceivePlayMessage");
            }
            else
            {
                GameEnded(key, 1);
            }

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).TimeStamp = DateTime.UtcNow;

            foreach (var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players)
            {
            Clients.Client(x.Adress).SendAsync("ReceiveMessage");
            }
        }

        public void FoldMessage(string key, string username)
        {
            int _key = Convert.ToInt32(key);

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.PlayerName == username).InGame = false;

            PlayMessage(key, username);
        }

        public void CheckMessage(string key, string username)
        {
            int _key = Convert.ToInt32(key);

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.PlayerName == username).Money
               -= MoneyToCheck(_key, username);

            PlayMessage(key, username);

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Last =
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.FindIndex(x => x.PlayerName == username);

        }

        public void RaiseMessage(string key, string username, string money)
        {
            int _key = Convert.ToInt32(key);
            int _money = Convert.ToInt32(money);

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.PlayerName == username).Money -= (_money + MoneyToCheck(_key, username));

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Last =
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.FindIndex(x => x.PlayerName == username);

            PlayMessage(key, username);

        }

        private int MoneyToCheck(int key, string username)
        {
            int previous = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Last;
            int lenght = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count;


            return RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Find(x => x.PlayerName == username).Money
                - RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[previous].Money;
        }

        private void GameEnded(int key, int Case)
        {
            //natural end
            if(Case == 0)
            {
                EvaluateRound(key);
                //udělat scénu na konec.
            }
            //last stand
            else
            {
                Player player = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Find(x => x.InGame == true);
                string winner = player.PlayerName;
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Winners.Add(winner);
                player.LastMoney = player.MoneyFinal + RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Sum;
            }

            foreach (var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players)
            {
                Clients.Client(x.Adress).SendAsync("ReceiveMessage");
            }

            Thread.Sleep(10000);

            if (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count >= 3)
            {
                Clients.Client(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Find(x => x.Founder == true).Adress).SendAsync("ShowPlaybutton");
            }

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).PrepareNextRound();
        }

        public void GameAbsolutlyEnded(int key, int Case)
        {
            //zdeeeeeeeeeeeee, nějak zapojit to, když dojdou pěníze a zbyde málo nebo jak to má být. Potom kompletní restart

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).InGame = false;
        }

        public bool NewRoundIsNext(int key, int i, int y)
        {
            if (((RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Money) 
                == (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[y].Money))
                && (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Played == true))
            {
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Round++;

                foreach ( var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players)
                {
                    x.Played = false;
                }
            }

            int z = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Round;

            if (z >= 3)
            {
                GameEnded(key, 0);

                return false;
            }

            return true;
            
        }

        private (int?, bool) PlaySignal(string username, int key)
        {
            int index = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.FindIndex(x => x.PlayerName == username);
            int? finalIndex = null;
            int count = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count;
            int countInGame = 0;

            for (int i = 0; i < count; i++)
            {
                index++;
                index %= count;

                if (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[index].InGame == true)
                {
                    countInGame++;
                    
                    if (finalIndex == null)
                    {
                        finalIndex = index;
                    }
                }
            }

            if (countInGame <= 1)
            {
                return (finalIndex, true);
            }

            return (finalIndex, false);
        }

        private void EvaluateRound( int key)
        {
            Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key);
            List<Player> survivors = room.Players.Where(x => x.InGame == true).ToList();
            List<string> roomsCards = room.Cards;
            //pro každého hráče vyhodnotit co má. Přiřadit to k němu v dictionary s číslem síly a pak vyhodnostit nejlepší. 
            //na konci sse získá sublist ze slovníku pro nejvyšší hodnotu. Podle počtu se to vydělí. Nakonec se peníze foreach in list přiřadí.
            foreach(Player player in survivors)
            {
                List<string> playersCards= player.Cards;
            }
        }

    }

}