using DocuWise.DTOs.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Services.IServices
{
    public interface IUserProfileService
    {
        Task<UserProfileDTO> GetProfileAsync(string userId);
        Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO dto);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO dto);
        Task<bool> UpdateProfilePictureAsync(string userId, string imageUrl);
    }

}
