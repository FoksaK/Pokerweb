using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokerweb.Data;
using Pokerweb.Models;
using System;

namespace Pokerweb.Pages
{
    public class IndexModel : PageModel
    {


        public void OnGet()
        {

        }


        [BindProperty]
        public string KeyIn { get; set; }

        [BindProperty]
        public string NameIn { get; set; }
        public string Message { get; set; } = null;

        public int Key;


        public IActionResult OnPostNew()
        {
            Random random = new Random();
            Key = random.Next(100000, 999999);

            string N = Request.Form[nameof(NameIn)];

            if (N.Length > 0 && N.Length < 50)
            {
                RoomsDbContext.RoomsList.Add(new Room { KeyNumber = Key });
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == Key).AddPlayer(new Player { PlayerName = N, Founder = true });

                RoomsDbContext.DBFunction();

                return RedirectToPage("GamePage", new { key = Key, name = N });
            }
            else
            {
                Message = "Vadný jméno";

                return Page();
            }

        }
        public IActionResult OnPostIn()
        {
            string Ks = Request.Form[nameof(KeyIn)];
            string N = Request.Form[nameof(NameIn)];
            int K;

            if (Ks.Length == 6)
            {
                K = Convert.ToInt32(Ks);
            }
            else
            {
                Message = "Vadnej klíč";
                return Page();
            }


            if ((Ks.Length == 6 && IsInDatabase(K) == true && N.Length > 0) && (N.Length < 50 && (AlreadyUsed(K, N) == false))
                && (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == K).InGame == false) 
                && (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == K).Players.Count < 12))
            {
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == K).AddPlayer(new Player { PlayerName = N });

                return RedirectToPage("GamePage", new { key = K, name = N });
            }
            else
            {
                Message = "Vadnej klíč, nebo jméno, nebo možná ve hře, možná příliš hráčů";
                return Page();
            }


        }

        private bool AlreadyUsed(int K, string N)
        {
            if (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == K).Players.Find(x => x.PlayerName == N) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsInDatabase(int K)
        {
            if (RoomsDbContext.RoomsList.Find(x => x.KeyNumber == K) != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}