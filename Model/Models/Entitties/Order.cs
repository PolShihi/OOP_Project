using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.Entitties
{
    public class Order : Entity
    {
        public string Address { get; set; }
        public DateTime? Date { get; set; }
        public int? CeremonyId { get; set; }
        public Ceremony? Ceremony { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public List<ProductOrder> ProductOrders { get; set; } = new();
        public DeadPerson? DeadPerson { get; set; }

        [NotMapped]
        public decimal TotalPrice { get; set; }

        public decimal GetTotalPrice()
        {
            decimal totalPrice = 0;

            if (Ceremony is not null)
            {
                totalPrice += Ceremony.Price;
            }

            foreach (var order in ProductOrders)
            {
                totalPrice += order.GetTotalPrice();
            }

            return totalPrice;
        }
    }
}
