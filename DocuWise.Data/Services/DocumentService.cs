using DocuWise.Data.Repositories.IRepositories;
using DocuWise.Data.Services.IServices;
using DocuWise.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using DocuWise.DTOs.DTOs;


namespace DocuWise.Data.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IWebHostEnvironment _env;



        public DocumentService(IDocumentRepository documentRepository, IWebHostEnvironment env)
        {
            _env = env;
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

        public async Task<(string summary, string keywords, string category)> AnalyzeTextWithAI(string text)
        {
            using var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)
            };

            var payload = new { text = text };
            var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            //var response = await httpClient.PostAsync("http://localhost:5000/analyze-text", jsonContent);
            var response = await httpClient.PostAsync("http://localhost:5000/analyze", jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("AI server error: " + error);
                return ("AI text analysis failed", "N/A", "Unknown");
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            var aiResult = JsonSerializer.Deserialize<AIResponseDto>(resultJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return (
                aiResult?.Summary ?? "No summary",
                string.Join(", ", aiResult?.Keywords ?? new List<string>()),
                aiResult?.Category ?? "Unknown"
              );
        }

    }
}
