using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pokerweb.Data;

namespace Pokerweb.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int Key)
        {
            RoomsDbContext.RoomsList[1].Players.Add(new Models.Player { Money = 50, PlayerName = "Dalibor" });

            await Clients.All.SendAsync("ReceiveMessage");
              
        }

    }

}