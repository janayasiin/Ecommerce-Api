using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
   public interface  ICheckoutService
    {
        Task<CheckOutResponse> ProcessCheckout(string userId, CheckoutRequest request);
        Task<CheckOutResponse> HandelSuccess(string sessionId);
    }
}
