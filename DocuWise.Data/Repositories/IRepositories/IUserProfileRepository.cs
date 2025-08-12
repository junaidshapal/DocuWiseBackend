using DocuWise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Repositories.IRepositories
{
    public interface IUserProfileRepository
    {
        Task<User> GetByIdAsync(string userId);
        Task UpdateAsync(User user);
        Task<bool> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    }

}
