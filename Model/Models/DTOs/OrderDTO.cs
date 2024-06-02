using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.DTOs
{
    public class OrderDTO
    {
        [Required]
        public string Adress { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int? CeremonyId { get; set; }

        [Required]
        public int ClientId { get; set; }
    }
}
