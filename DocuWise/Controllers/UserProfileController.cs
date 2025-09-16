using DocuWise.Data.Services.IServices;
using DocuWise.DTOs.DTOs.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DocuWise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ← require an authenticated user
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                User.FindFirst("uid")?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Missing user id claim.");

            var profile = await _userProfileService.GetProfileAsync(userId);
            if (profile is null) return NotFound();

            return Ok(profile);
        }


        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updated = await _userProfileService.UpdateProfileAsync(userId, dto);
            return updated ? Ok("Profile updated") : BadRequest("Update failed");
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _userProfileService.ChangePasswordAsync(userId, dto);
            return result ? Ok("Password changed") : BadRequest("Password change failed");
        }

        //[HttpPost("upload-picture")]
        //public async Task<IActionResult> UploadPicture([FromForm] UpdateProfilePictureDTO dto)
        //{
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (dto.ProfileImage == null || dto.ProfileImage.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var filePath = Path.Combine("wwwroot/profile_pictures", $"{Guid.NewGuid()}_{dto.ProfileImage.FileName}");
        //    using var stream = new FileStream(filePath, FileMode.Create);
        //    await dto.ProfileImage.CopyToAsync(stream);

        //    var relativePath = filePath.Replace("wwwroot/", "/");
        //    var updated = await _userProfileService.UpdateProfilePictureAsync(userId, relativePath);
        //    return updated ? Ok(new { imageUrl = relativePath }) : BadRequest("Failed to update profile picture.");
        //}
        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadPicture([FromForm] UpdateProfilePictureDTO dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (dto.ProfileImage == null || dto.ProfileImage.Length == 0)
                return BadRequest("No file uploaded.");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_pictures");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // ✨ Use only GUID + extension to avoid long names
            var extension = Path.GetExtension(dto.ProfileImage.FileName);
            var shortFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, shortFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.ProfileImage.CopyToAsync(stream);

            var relativePath = $"/profile_pictures/{shortFileName}";
            var updated = await _userProfileService.UpdateProfilePictureAsync(userId, relativePath);

            return updated ? Ok(new { imageUrl = relativePath }) : BadRequest("Failed to update profile picture.");
        }
    }
}
