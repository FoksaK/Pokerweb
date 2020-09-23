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