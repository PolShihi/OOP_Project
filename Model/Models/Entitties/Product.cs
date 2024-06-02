using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyModel.Models.Entitties
{
    public class Product : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; }
        public int ReorderLevel { get; set; }


        public List<ProductOrder> ProductOrders { get; set; } = new();


        public bool IsReorderNeeded { get => Amount <= ReorderLevel;}
    }
}
