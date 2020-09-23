using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Pokerweb.Data;

namespace Pokerweb.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {

            if (RoomsDbContext.prvni == true)
            {
                RoomsDbContext.prvni = false;
                RoomsDbContext.s = Context.ConnectionId;
            }

            await Clients.Client(RoomsDbContext.s).SendAsync("ReceiveMessage", user, message);
              
        }
        
    }

}