using DocuWise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Services.IServices
{
    public interface IDocumentService
    {
        Task<IEnumerable<Document>> GetAllAsync();
        Task<Document?> GetByIdAsync(int id);
        Task<IEnumerable<Document>> GetByUserIdAsync(string userId);
        Task AddAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(Document document);
    }
}
