using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IUserManagementService
    {
        Task<List<UserListResponse>> GetAllUsers();
        Task<UserDetailsResponse?> GetUser(string userId);
        Task<bool> ChangeRole(string userId, string role);
        Task<bool> DeleteUSer(string userId);
        Task<bool> ToggleBlockUser(string userId);
    
    
    }
}
