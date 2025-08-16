using DocuWise.Data.Models;
using DocuWise.Data.Repositories.IRepositories;
using DocuWise.Data.Services.IServices;
using DocuWise.DTOs.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _repository;

        public UserProfileService(IUserProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserProfileDTO> GetProfileAsync(string userId)
        {
            var user = await _repository.GetByIdAsync(userId);
            return new UserProfileDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                LastLogin = user.LastLogin,
                ProfilePictureUrl = user.ProfilePictureUrl
            };
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileDTO dto)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            await _repository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO dto)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            return await _repository.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        }

        public async Task<bool> UpdateProfilePictureAsync(string userId, string imageUrl)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            user.ProfilePictureUrl = imageUrl;
            await _repository.UpdateAsync(user);
            return true;
        }
    }

}
