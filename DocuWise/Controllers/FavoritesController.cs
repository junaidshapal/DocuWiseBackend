using DocuWise.Data.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DocuWise.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        private string GetUserId()
        {
            //return User.FindFirstValue(ClaimTypes.NameIdentifier);
            return User.FindFirst("uid")?.Value;
        }

        [HttpPost("{documentId}")]
        public async Task<IActionResult> AddFavorite(int documentId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found in token.");

            await _favoriteService.AddFavoriteAsync(userId, documentId);
            return Ok(new { message = "Document added to favorites." });
        }

        [HttpDelete("{documentId}")]
        public async Task<IActionResult> RemoveFavorite(int documentId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found in token.");

            await _favoriteService.RemoveFavoriteAsync(userId, documentId);
            return Ok(new { message = "Document removed from favorites." });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found in token.");

            var favorites = await _favoriteService.GetFavoriteDocumentsAsync(userId);
            return Ok(favorites);
        }

        [HttpGet("check/{documentId}")]
        public async Task<IActionResult> IsFavorite(int documentId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isFav = await _favoriteService.IsFavoriteAsync(documentId, userId);
            return Ok(isFav);
        }

    }
}
