using DocuWise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Repositories.IRepositories
{
    public interface IFavoriteRepository
    {
        Task AddFavoriteAsync(Favorite favorite);
        Task RemoveFavoriteAsync(string userId, int documentId);
        Task<IEnumerable<Document>> GetFavoriteDocumentsAsync(string userId);
        Task<bool> IsFavoriteAsync(int documentId, string userId);

    }
}
