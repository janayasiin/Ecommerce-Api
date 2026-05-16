using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class Review
    {
        public int Id {  get; set; }
        public string UserdId { get; set; }
        public ApplicationUser User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string comment { get; set; }
        public int Rate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
