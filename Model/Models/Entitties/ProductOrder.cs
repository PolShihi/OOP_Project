using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.Entitties
{
    public class ProductOrder : Entity
    {
        public int Amount { get; set; }
        public string Comment { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        public decimal GetTotalPrice()
        {
            if (Product is null)
            {
                return 0;
            }

            return Amount * Product.Price;
        }
    }
}
