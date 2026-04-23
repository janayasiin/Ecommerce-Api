using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IEmailSender _emailSender;

      

        public CheckoutService(ICartRepository cartRepository, IProductRepository productRepository, UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccesor, IOrderRepository orderRepository , ICartService cartService , IEmailSender emailSender)

        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _httpContextAccesor = httpContextAccesor;
            _orderRepository = orderRepository;
            _cartService = cartService;
            _emailSender=emailSender;
         

        }

        public async Task<CheckOutResponse> ProcessCheckout(string userId, CheckoutRequest request)
        {
            var cartItems = await _cartRepository.GetAllAsync(filter: c => c.UserId == userId, includes: new[] { nameof(Cart.Product), $"{nameof(Cart.Product)}.{nameof(DAL.Models.Product.Translations)}" });
            if (!cartItems.Any())
            {
                return new CheckOutResponse
                {
                    Success = false,
                    Errors = "Cart is Empty"
                };

            }
            var user = await _userManager.FindByIdAsync(userId);

            var city = request.City ?? user.City;
            if (city is null)
            {
                return new CheckOutResponse
                {
                    Success = false,
                    Errors = "city is requeired "
                };
            }

            var street = request.Street ?? user.Street;
            if (street is null)
            {
                return new CheckOutResponse
                {
                    Success = false,
                    Errors = "street is requeired "
                };
            }
            var PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            if (PhoneNumber is null)
            {
                return new CheckOutResponse
                {
                    Success = false,
                    Errors = "PhoneNumber is requeired "
                };
            }
            //var product = await _productRepository.GetAllAsync(filter

            foreach (var item in cartItems)
            {
                if (item.Count > item.Product.Quantity)
                {
                    return new CheckOutResponse
                    {
                        Success = false,
                        Errors = "Dosn't have enough stock"
                    };
                }

            }
            var order = new Order()
            {
                UserId = userId,
                City = city,
                Street = street,
                PhoneNumber = PhoneNumber,
                paymentMethod = request.PaymentMethod,
                AmountPaid = cartItems.Sum(c => c.Product.Price * c.Count),
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Count,
                    UnitPrice = c.Product.Price,
                    TotalPrice = c.Product.Price * c.Count
                }).ToList()


            };

            await _orderRepository.CreateAsync(order);
            if (request.PaymentMethod == PaymentMethodEnum.Cash)
                return new CheckOutResponse
                {
                    Success = true
                };
            if (request.PaymentMethod == PaymentMethodEnum.Visa)
            {

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",

                    SuccessUrl = $"{_httpContextAccesor.HttpContext.Request.Scheme}://{_httpContextAccesor.HttpContext.Request.Host}/api/checkouts/success?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_httpContextAccesor.HttpContext.Request.Scheme}://{_httpContextAccesor.HttpContext.Request.Host}/api/checkouts/cancel",
                    LineItems = new List<SessionLineItemOptions>()



                };

                foreach (var item in cartItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Translations.FirstOrDefault(t => t.Language == "en").Name,

                            },
                            UnitAmount = (long)item.Product.Price * 100,
                        },
                        Quantity = item.Count,
                    });

                }
                var service = new SessionService();
                var session = service.Create(options);
                order.StripSessionId = session.Id;
                await _orderRepository.UpdateAsync(order);
                    
                return new CheckOutResponse
                {
                    Success = true,
                    StripeUrl = session.Url
                };
            }

            return new CheckOutResponse
            {
                Success = false,
                Errors = "Invalid Payment method"
            };


        }

        public async Task<CheckOutResponse> HandelSuccess(string sessionId) 
        {
            var order = await _orderRepository.GetOne(O => O.StripSessionId == sessionId , new[] {nameof(Order.OrderItems) , $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}." +
                $"{nameof(DAL.Models.Product.Translations)}"}  );
            order.OrderStatus = OrderStatusEnum.Paid;
            await _orderRepository.UpdateAsync(order);
            await _cartService.ClearCart(order.UserId);
              var user = await _userManager.FindByIdAsync(order.UserId);
            await _emailSender.SendEmailAsync(user.Email, "Order confirmes", "<h2> your order has beed placed succefuly</h2>");

            var lowStockProducts = await _productRepository.DecreaseQuantityAsync(order.OrderItems);
            foreach(var item in lowStockProducts)
            {
                await _emailSender.SendEmailAsync("janayasiin@gmail.com", "low stock alert ", $"<h2>product " +
                    $"{item.Translations.FirstOrDefault(t => t.Language == "en").Name} current quantity : {item.Quantity}</h2>");

            }
         

            return new CheckOutResponse()
            {

                Success = true,
                OrderId = order.Id
            };



        }
    }
    } 
