using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.DTO.Response
{
    public class CartResponse
    {
        public decimal Price { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Discount { get; set; }
        public int count { get; set; }
        public decimal SubTotal => (count * (Price-(Price * Discount)/100));
    }
}
