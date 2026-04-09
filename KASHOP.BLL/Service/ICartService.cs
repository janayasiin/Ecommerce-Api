using KASHOP.DAL.DTO.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface ICartService
    {
        Task AddToCart(AddtoCartRequest request, string UserId);
    }
}
