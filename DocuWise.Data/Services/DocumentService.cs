using DocuWise.Data.Repositories.IRepositories;
using DocuWise.Data.Services.IServices;
using DocuWise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;

        public DocumentService(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task<IEnumerable<Document>> GetAllAsync()
        {
            return await _documentRepository.GetAllDocumentsAsync();
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _documentRepository.GetDocumentByIdAsync(id);
        }

        public async Task<IEnumerable<Document>> GetByUserIdAsync(string userId)
        {
            return await _documentRepository.GetDocumentsByUserIdAsync(userId);
        }

        public async Task AddAsync(Document document)
        {
            await _documentRepository.AddDocumentAsync(document);
            await _documentRepository.SaveAsync();
        }

        public async Task UpdateAsync(Document document)
        {
            await _documentRepository.UpdateDocumentAsync(document);
            await _documentRepository.SaveAsync();
        }

        public async Task DeleteAsync(Document document)
        {
            await _documentRepository.DeleteDocumentAsync(document);
            await _documentRepository.SaveAsync();
        }
    }
}
