using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
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
    private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository , IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task <bool> AddToCart(AddtoCartRequest request, string UserId)
        {
            var product =await _productRepository.GetOne(p=>p.Id ==request.ProductId);
            if (product == null) return false;
            var ExistingItem = await _cartRepository.GetOne
                (c => c.ProductId == request.ProductId && c.UserId==UserId);
            var currentcount = ExistingItem?.Count ?? 0;
            var newCount= currentcount+request.Count;
            if(newCount>product.Quantity)
            {
                return false;

            }

            if (ExistingItem != null)
            {
                ExistingItem.Count =newCount;
                 await _cartRepository.UpdateAsync(ExistingItem);
            }
            else
            {
                var cartItem = request.Adapt<Cart>();
                cartItem.UserId= UserId;
                await _cartRepository.CreateAsync(cartItem);
            }

            return true;
        }

        public  async Task<bool> ClearCart(string userId)
        {
            var items = await _cartRepository.GetAllAsync(filter: c=>c.UserId==userId);

            if(!items.Any()) return false;

            await _cartRepository.DeleteRangeAsync(items);
            return true;
        }

        public async  Task<List<CartResponse>> GetCart(string userId)
        {

            var items = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId,
                includes: new string[] { nameof(Cart.Product) ,$"{nameof(Cart.Product)}.{nameof(Product.Translations)}"}); 
            

           return items.Adapt<List<CartResponse>>();

        }

        public async Task<bool> RemoveItem(int productId, string userId)
        {
          var item = await _cartRepository.GetOne(c=>c.UserId == userId && c.ProductId==productId); 
            if (item == null) { return false; }




            return await _cartRepository.DeleteAsync(item);


        }

        public async Task<bool> UpdateQuantity(int productId, int count, string userId)
        {
            var item = await _cartRepository.GetOne(c=>c.UserId==userId && c.ProductId==productId);
            if (item is null) { return false; }
            var product= await _productRepository.GetOne(p=>p.Id==productId);
            if (count > product.Quantity) return false;
            item.Count=count;
            return await _cartRepository.UpdateAsync(item);
        }
    }
}
