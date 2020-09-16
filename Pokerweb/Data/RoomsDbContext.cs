using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Pokerweb.Data
{
    public static class RoomsDbContext
    {
        public static List<Models.Room> RoomsList = new List<Models.Room>();

        public static string s = string.Empty;

        public static bool prvni = true;
    }
}