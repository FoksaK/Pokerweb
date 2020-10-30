using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Pokerweb.Data;
using Pokerweb.Models;
using System;
using System.Collections.Generic;

/// <summary>
/// předělat playsignal()
/// oddělit kola - opakování kol, konce her
/// Vyřešit, aby byl vždycky k uživateli s přístupem jen jeden člověk. 
/// Dodělat css a základ grafiky. Na login použít bootstrap, na zbytek asi taky nějaký tabulkový, float left zobrazení.
/// Vyřešit, že se někdy nezaznamenával příhoz při raise.
/// Měnění názvu tlačítek při hře.
/// Omezení při připojení hráče - zákaz když ingame == true a když je hráčů nad 12
/// Bot na mazání v DB, kde bude timstamp velký
/// </summary>

namespace Pokerweb.Pages
{
    public class GamePageModel : PageModel
    {
        public string Name { get; set; }
        public int Key { get; set; }
        public Room Room { get; set; }
        public Player Player { get; set; }

        public void OnGet(string key, string name)
        {
            Key = Convert.ToInt32(key);
            Name = name;
            Player = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == Key).Players.Find(x => x.PlayerName == name);
        }

        public PartialViewResult OnGetPlayersPartial(string key, string name)
        {
            Key = Convert.ToInt32(key);
            Room = RoomsDbContext.RoomsList.Find(x => x.KeyNumber == Key);
            Room room = Room.ShallowCopy();
            room.PagePartialHelper = name;
            PartialViewResult _resultPartialPage = new PartialViewResult()
            {
                ViewName = "_PlayersPartial",
                ViewData = new ViewDataDictionary<Room>(ViewData, room),
            };
            return _resultPartialPage;
        }


    }
}