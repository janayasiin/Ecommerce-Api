using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface ICartService
    {
     Task<bool> AddToCart(AddtoCartRequest request, string UserId);
        Task<List<CartResponse>> GetCart(string userId);
        Task<bool> UpdateQuantity (int  productId , int count , string userId);
        Task<bool> RemoveItem (int productId , string userId);
        Task<bool> ClearCart(string userId);
        
    
    
    
    }
}
