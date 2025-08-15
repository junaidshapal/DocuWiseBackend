using DocuWise.Data.Models;
using DocuWise.Data.Repositories.IRepositories;
using DocuWise.Data.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task AddFavoriteAsync(string userId, int documentId)
        {
            var favorite = new Favorite
            {
                UserId = userId,
                DocumentId = documentId
            };

            await _favoriteRepository.AddFavoriteAsync(favorite);
        }

        public async Task RemoveFavoriteAsync(string userId, int documentId)
        {
            await _favoriteRepository.RemoveFavoriteAsync(userId, documentId);
        }

        public async Task<IEnumerable<Document>> GetFavoriteDocumentsAsync(string userId)
        {
            return await _favoriteRepository.GetFavoriteDocumentsAsync(userId);
        }

        public async Task<bool> IsFavoriteAsync(int documentId, string userId)
        {
            return await _favoriteRepository.IsFavoriteAsync(documentId, userId);
        }
    }
}
