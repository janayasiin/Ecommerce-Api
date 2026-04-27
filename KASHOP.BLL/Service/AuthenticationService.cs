using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace KASHOP.BLL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration
            , IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) {
                return new LoginResponse() { Success = false, Message = "Invalid email" };

            }
            if (!await _userManager.IsEmailConfirmedAsync(user)) {
                return new LoginResponse() { Success = false, Message = "email is not confirmed" };
            }
            var result = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!result)

                return new LoginResponse() { Success = false, Message = "Invalid password" };
            var refreshToken = await GenerateRefreshToken(user);
            SetRefreshTokenCookies(refreshToken);
            return new LoginResponse() { Success = true, Message = "Success", AccessToken = await GenerateAccessToken(user) };


        }
        private async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            var userCLaims = new List<Claim>() {
            new Claim(ClaimTypes.NameIdentifier,user.Id) ,
            new Claim(ClaimTypes.Name,user.UserName) ,
            new Claim(ClaimTypes.Email,user.Email) ,
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
       issuer: _configuration["Jwt:Issuer"],
       audience: _configuration["Jwt:Audience"],
       claims: userCLaims,
       expires: DateTime.Now.AddMinutes(1),
       signingCredentials: credentials
       );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken (ApplicationUser user)
        {
            var refreshtoken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshtoken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(1);
            await _userManager.UpdateAsync(user);
            return refreshtoken;
        }

        private void SetRefreshTokenCookies (string refreshToken)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // true for production
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(5)

            });


        }
        public async Task<LoginResponse> RafreshTokenAsync ()
        {
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
            if (refreshToken == null)
            {
                new LoginResponse
                {
                    Success = false,
                    Message = "No refresh Token"
                };
            }
                var user = await _userManager.Users.FirstOrDefaultAsync(u=>u.RefreshToken== refreshToken);
                if(user.RefreshTokenExpiry<DateTime.UtcNow )
                {
                    new LoginResponse
                    {
                        Success = false,
                        Message = "refresh Token Expires"
                    };
                }

                var newRefreshToken = await GenerateRefreshToken(user);
                SetRefreshTokenCookies(newRefreshToken);

               return new LoginResponse
                {
                    Success = true,
                    Message = "Success",
                    AccessToken= await GenerateAccessToken(user)
                };



            }
         
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new RegisterResponse()
                {
                    Success = false,
                    Message = "Error",
                    Errors = result.Errors.Select(p => p.Description).ToList()

                };

            }
            await _userManager.AddToRoleAsync(user, "User");
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);
            var emailUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/Account/ConfirmEmail?token={token}&userId={user.Id}";
            await _emailSender.SendEmailAsync(user.Email, "Welcome", $"<h1> welcome {request.UserName}</h1>" +
             $"" + $"<a href ='{emailUrl}'> confirm </a>");
            return new RegisterResponse() { Success = true, Message = "success" };

        }

        public async Task<bool> ConfirmEmailAsync(string token, string userID)
        {
            var user = await _userManager.FindByIdAsync(userID);
            if (user == null) return false;
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return false;
            return true;


        }

        public async Task<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ForgotPasswordResponse()
                {
                    Success = false,
                    Message = "Email Not Found "
                };
            }

            var random = new Random();
            var code = random.Next(1000, 9999).ToString();
            user.CodeResetPassword = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);
            await _userManager.UpdateAsync(user);
            await _emailSender.SendEmailAsync(request.Email, "reset Password", $"<p>Code IS {code} </p>");
            return new ForgotPasswordResponse() { Success = true, Message = "code sent to your Email" };

        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Email Not Found "
                };
            }


            else if (user.CodeResetPassword != request.Code)

            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = " Invalid Code "
                };
            }
            else if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
            {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "  Code Expired "
                };

            }
            var isSamePassword = await _userManager.CheckPasswordAsync(user, request.NewPassword);
            if (isSamePassword) {
                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "New Password Must be Differnet from Old Password "
                };


            }
            var  token = await _userManager.GeneratePasswordResetTokenAsync(user); 
            var result = await _userManager.ResetPasswordAsync(user,token, request.NewPassword);

            if (!result.Succeeded) { 

                return new ResetPasswordResponse()
                {
                    Success = false,
                    Message = "Password reset faild  "
                };

            }
            await _emailSender.SendEmailAsync(request.Email, "change password", $"<p>your Passward is changed</p>");
            return new ResetPasswordResponse()
            {
                Success = true,
                Message = " password reset succefully  "
            };

        }
    } }
