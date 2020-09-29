using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Pokerweb.Data;
using Pokerweb.Hubs;
using Pokerweb.Models;


//udělat ReceivePlayMessage v js, aby se to nějak dalo už vyzoušet jestli to volá samostatnýho
//Dodělat do modelu Player kolik jednotlivec přihodil, udělat funkci check v Hubu a jedem.
//Pak dělat dál na js. Když se stiskne tlačítko play, zavolá to ReceivePlayMessage pro prvního, co vždycky bude host

namespace Pokerweb.Pages
{
    public class GamePageModel : PageModel
    {
        public string Message { get; set; }

        public string Name { get; set; }

        public string Key { get; set; }

        public void OnGet(string key, string name)
        {
            Message = key + " " + RoomsDbContext.RoomsList[0].Players[0].PlayerName;

            Key = key;
            Name = name;

        }


        public Room _Room { get; set; }

       

        public PartialViewResult OnGetPlayersPartial()
        {

           _Room = RoomsDbContext.RoomsList[0];
            PartialViewResult _resultPartialPage = new PartialViewResult()
            {
                ViewName = "_PlayersPartial",
                ViewData = new ViewDataDictionary<Room>(ViewData, _Room),
            };
            return _resultPartialPage;
        }


    }
}