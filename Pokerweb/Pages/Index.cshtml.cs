﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Pokerweb.Data;
using Pokerweb.Models;
using System;
using System.Collections.Generic;

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
            GenerateKey(out Key);

            string N = Request.Form[nameof(NameIn)];

            if (N.Length > 0 && N.Length < 50)
            {
                RoomsDbContext.RoomsList.Add(new Room { KeyNumber = Key });
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == Key).AddPlayer(new Player { PlayerName = N, Founder = true });
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
                Message = "Incorrect këy";
                return Page();
            }


            if (Ks.Length == 6 && IsInDatabase(K) == true && N.Length > 0 && N.Length < 50 && (AlreadyUsed(K, N) == false))
            {
                RoomsDbContext.RoomsList.Find(x => x.KeyNumber == K).AddPlayer(new Player { PlayerName = N });

                return RedirectToPage("GamePage", new { key = K, name = N });
            }
            else
            {
                Message = "Incorrect këy, nebo badný jméno";
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

        private void GenerateKey(out int k)
        {
            var Rand = new Random();
            k = 0;
            for (int i = 0; i < 6; i++)
            {
                k = k * 10 + Rand.Next(0, 10);
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