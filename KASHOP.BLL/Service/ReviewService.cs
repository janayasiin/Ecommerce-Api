using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class ReviewService : IReviewService
    {


        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository, IOrderRepository orderRepository)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
        }

        public async Task<bool> AddReview(string userId, AddReviewRequest request)
        {
            var purchaseOrder = await _orderRepository.GetOne(
                filter: o => o.UserId == userId && o.OrderStatus == DAL.Models.OrderStatusEnum.Delivered &&
                o.OrderItems.Any(oi => oi.ProductId == request.ProductId),
                includes:
                new[]
                {
                    nameof(DAL.Models.Order.OrderItems)
                }

                );
            if (purchaseOrder == null) return false;
            var AlreadyReviews = await _reviewRepository.GetOne(

                filter: r => r.UserdId == userId && r.ProductId == request.ProductId
                );

            if (AlreadyReviews != null) return false;
            var review = request.Adapt<Review>();
            review.UserdId= userId;
            await _reviewRepository.CreateAsync( review );

            return true;


        }
    }
}
