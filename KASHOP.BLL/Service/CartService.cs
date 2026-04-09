using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CartService : ICartService
 {  

    private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task AddToCart(AddtoCartRequest request, string UserId)
        {
            var ExistingItem = await _cartRepository.GetOne(c => c.ProductId == request.ProductId && c.UserId==UserId);

            if (ExistingItem != null)
            {
                ExistingItem.Count += request.Count;
                 await _cartRepository.UpdateAsync(ExistingItem);
            }
            else
            {
                var cartItem = request.Adapt<Cart>();
                cartItem.UserId= UserId;
                await _cartRepository.CreateAsync(cartItem);
            }


        }
    }
}
