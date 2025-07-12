using DocuWise.Data.Data;
using DocuWise.Data.Models;
using DocuWise.Data.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocuWise.Data.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllDocumentsAsync()
        {
            return await _context.Documents.ToListAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByUserIdAsync(string userId)
        {
            return await _context.Documents
                .Where(d => d.UserId == userId)
                .ToListAsync();
        }

        public async Task AddDocumentAsync(Document document)
        {
            await _context.Documents.AddAsync(document);
        }

        public async Task UpdateDocumentAsync(Document document)
        {
            _context.Documents.Update(document);
            await Task.CompletedTask;
        }

        public async Task DeleteDocumentAsync(Document document)
        {
            _context.Documents.Remove(document);
            await Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}

