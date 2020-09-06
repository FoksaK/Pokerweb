using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pokerweb.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        public string PlayerName { get; set; }
    }
}
