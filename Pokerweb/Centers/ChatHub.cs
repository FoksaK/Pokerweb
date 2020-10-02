using Microsoft.AspNetCore.SignalR;
using Pokerweb.Data;
using Pokerweb.Models;
using System;

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

            Clients.Client(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players[0].Adress).SendAsync("ShowPlaybutton");
        }

        public void PlayMessage(string _key, string username)
        {
            int key = Convert.ToInt32(_key);

            int i = PlaySignal(username, key);

            

            if (NewRoundIsNext(key, i, RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.FindIndex(x => x.PlayerName == username)))
            {

                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Played = true;

                Clients.Client(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Adress).SendAsync("ReceivePlayMessage");

            }

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

        public void GameEnded(int Case)
        {
            //zdeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeee
        }
        
        //dodělat aby to pracovalo jen pokud má další hráč played = true
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

            if (z >= 4)
            {
                GameEnded(0);

                return false;
            }

            return true;
            
        }
        
        public int PlaySignal(string username, int key)
        {

            int index = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.FindIndex(x => x.PlayerName == username) + 1;

            if (index >= RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count)
            {
                index = 0;
            }

            Player player = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[index];

            int y = 0;

            IsIn(player);

            void IsIn(Player player)
            {
                y++;

                if (y > RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count + 1)
                {
                    GameEnded(1);
                    return;
                }

                if (player.InGame == false)
                {
                    index++;

                    if (index >= RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count)
                    {
                        index = 0;
                    }

                    IsIn(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[index]);
                }
                else if (player.InGame == true)
                {
                    if (index >= RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count)
                    {
                        index = 0;
                    }
                }

            }

            return index;
        }

    }

}