using Microsoft.AspNetCore.SignalR;
using Pokerweb.Data;
using Pokerweb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Pokerweb.Hubs
{
    public class ChatHub : Hub
    {
        //on connected
        public void Connected(string key, string username)
        {
            int _key = Convert.ToInt32(key);
            Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key);
            Player player = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.PlayerName == username);

            if ((player.Address == string.Empty) || (player.Left == true))
            {
                player.Address = Context.ConnectionId;
                player.Left = false;
            }
            
            string addressFounder = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.Founder == true).Address;

            foreach (var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players)
            {
                Clients.Client(x.Address).SendAsync("ReceiveMessage");
            }


            if (room.Players.Count >= 3 && !room.InGame) 
            {
                Clients.Client(addressFounder).SendAsync("ShowPlaybutton");
            }
            
        }

        //game start
        public void StartMessage(string _key)
        {
            int key = Convert.ToInt32(_key);

            Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key);

            room.PrepareNextRound();

            List<string> playersInGame = new List<string>();

            foreach(Player player in room.Players)
            {
                if (player.InGame)
                {
                    playersInGame.Add(player.PlayerName);
                }
            }

            int first = room.Players.FindIndex(x => x.PlayerName == playersInGame[0]);
            int second = room.Players.FindIndex(x => x.PlayerName == playersInGame[1]);

            room.Players[first].Money = 5;
            room.Players[second].Money = 10;

            room.Players[first].Played = true;
            room.Players[second].Played = true;

            room.Last = second;

            PlayMessage(_key, playersInGame[1]);

            room.InGame = true;
        }

        //player has played so this is executed
        public void PlayMessage(string _key, string username)
        {
            int key = Convert.ToInt32(_key);

            Player player = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Find(x => x.PlayerName == username);

            int i = Convert.ToInt32(PlaySignal(username, key).Item1);
            bool ended = PlaySignal(username, key).Item2;

            if (NewRoundIsNext(key, i, RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.FindIndex(x => x.PlayerName == username))
                && !ended)
            {
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Played = true;

                Clients.Client(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Address).SendAsync("ReceivePlayMessage");
            }
            else if (ended)
            {
                GameEnded(key, 1);
            }

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).TimeStamp = DateTime.UtcNow;

            foreach (var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players)
            {
            Clients.Client(x.Address).SendAsync("ReceiveMessage");
            }
        }

        //fold button clicked
        public void FoldMessage(string key, string username)
        {
            int _key = Convert.ToInt32(key);

            RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.PlayerName == username).InGame = false;

            PlayMessage(key, username);
        }

        //check message clicked
        public void CheckMessage(string key, string username)
        {
            int _key = Convert.ToInt32(key);
            Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key);

            ProcessMoney(_key, username);

            PlayMessage(key, username);
        }

        //raise message clicked
        public void RaiseMessage(string key, string username, string money)
        {
            int _key = Convert.ToInt32(key);
            int _money = Convert.ToInt32(money);

            Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key);

            Player player = room.Players.Find(x => x.PlayerName == username);

            ProcessMoney(_key, username, _money);

            PlayMessage(key, username);
        }

        //on leave
        public void LeaveMessage(string key, string username, string isPlaying)
        {
            int _key = Convert.ToInt32(key);

            Player player = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.Find(x => x.PlayerName == username);
            int index = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == _key).Players.FindIndex(x => x.PlayerName == username);

            player.InGame = false;
            player.Left = true;

            if(isPlaying == "true")
            {
                PlayMessage(key, username);
            }
        }

        //------------------------------------------ Helping functions ------------------------------------------
        //get money to check
        private void ProcessMoney(int key, string username, int money = 0)
        {
            Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key);
            Player player = room.Players.Find(x => x.PlayerName == username);
            int previous = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Last;

            int roundChange = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[previous].Money - player.Money;
            roundChange += money;

            player.Money += roundChange;

            if (player.MoneyFinal < 0)
            {
                player.NonFailed = false;
                player.InGame = false;
                player.LastMoney = 0;
                player.Money = 0;
            }
            else
            {
                room.Last = room.Players.FindIndex(x => x.PlayerName == username);
            }
        }

        //executed when game is ended
        private void GameEnded(int key, int Case)
        {
            //natural end
            if (Case == 0)
            {
                List<string> winners = new List<string>();
                winners.AddRange(EvaluateRound(key));

                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Winners.AddRange(winners);

                int prize = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Sum / winners.Count;

                foreach (string winnerName in winners)
                {
                    Player player = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Find(x => x.PlayerName == winnerName);
                    player.LastMoney = player.MoneyFinal + prize;
                }
            }
            //last stand
            else
            {
                Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key);
                Player player = room.Players.Find(x => x.InGame == true);
                string winner = player.PlayerName;
                room.Winners.Add(winner);
                player.LastMoney = player.MoneyFinal + RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Sum;
                room.endedCase = 0;

                List<Player> playersClone = new List<Player>();
                playersClone.AddRange(room.Players);

                if (playersClone.FindAll(x => x.NonFailed == true).Count <= 1)
                {
                    GameAbsolutlyEnded(room);
                }
            }

            foreach (var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players)
            {
                Clients.Client(x.Address).SendAsync("ReceiveMessage");
            }

            if (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Count >= 3)
            {
                Clients.Client(RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players.Find(x => x.Founder == true).Address).SendAsync("ShowPlaybutton");
            }
        }

        //execute when game has absolutelly ended
        public void GameAbsolutlyEnded(Room room)
        {
            room.endedCase = -1;
            room.InGame = false;
        }

        //check if round has ended
        public bool NewRoundIsNext(int key, int i, int y)
        {
            int z = new int();
            z = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Round;

            if (((RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Money) 
                == (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[y].Money))
                && (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players[i].Played == true))
            {
                z++;           

                if (z >= 4)
                {
                    GameEnded(key, 0);

                    return false;
                }

                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Round = z;

                foreach ( var x in RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key).Players)
                {
                    x.Played = false;
                }
            }

            return true;
            
        }

        //get next playing(non-folded) player
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

        //round evaluating function calling external Evaluation.cs
        private List<string> EvaluateRound(int key)
        {
            Room room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == key);
            List<Player> survivors = room.Players.Where(x => x.InGame == true).ToList();
            List<string> roomsCards = room.Cards;

            room.endedCase = Evaluation.Evaluate(survivors, roomsCards).Item2;
            
            return Evaluation.Evaluate(survivors, roomsCards).Item1;
        }

    }

}