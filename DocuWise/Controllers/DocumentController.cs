using DocuWise.Data.Models;
using DocuWise.Data.Services;
using DocuWise.Data.Services.IServices;
using DocuWise.DTOs.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace DocuWise.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IWebHostEnvironment _env;

        public DocumentController(IDocumentService documentService, IWebHostEnvironment env)
        {
            _documentService = documentService;
            _env = env;
        }

        // POST: api/document/upload
        //[HttpPost("upload")]
        //[Consumes("multipart/form-data")]
        //public async Task<IActionResult> Upload([FromForm] DocumentUploadDto dto)
        //{
        //    if (dto.File == null || dto.File.Length == 0)
        //        return BadRequest("File is empty.");

        //    var uploadsFolder = Path.Combine(_env.ContentRootPath, "UploadedFiles");
        //    if (!Directory.Exists(uploadsFolder))
        //        Directory.CreateDirectory(uploadsFolder);

        //    var filePath = Path.Combine(uploadsFolder, Guid.NewGuid() + Path.GetExtension(dto.File.FileName));
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await dto.File.CopyToAsync(stream);
        //    }

        //    var (summary, keywords, category) = await AnalyzeFileWithAI(filePath);

        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var document = new Document
        //    {
        //        Title = dto.Title ?? Path.GetFileNameWithoutExtension(dto.File.FileName),
        //        FilePath = filePath,
        //        UploadDate = DateTime.UtcNow,
        //        UserId = userId,
        //        Summary = summary,
        //        Keywords = keywords,
        //        Category = category
        //    };

        //    await _documentService.AddAsync(document);
        //    return Ok(new { message = "Document uploaded", document.Id });
        //}


        [HttpPost("upload")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] DocumentUploadDto dto)
        {

            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"TYPE: {claim.Type} => VALUE: {claim.Value}");
            }

            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("File is empty.");

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "UploadedFiles");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.File.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.File.CopyToAsync(stream);
            }

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (string.IsNullOrEmpty(userId))
            //    return Unauthorized("Invalid token: UserId not found.");

            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token: UserId not found.");
            Console.WriteLine("Extracted UserId from JWT: " + userId);

            var (summary, keywords, category) = await AnalyzeFileWithAI(filePath);

            var document = new Document
            {
                Title = string.IsNullOrWhiteSpace(dto.Title)
                            ? Path.GetFileNameWithoutExtension(dto.File.FileName)
                            : dto.Title,
                FilePath = filePath,
                UploadDate = DateTime.UtcNow,
                UserId = userId,
                Summary = summary,
                Keywords = keywords,
                Category = category
            };

            await _documentService.AddAsync(document);
            return Ok(new { message = "Document uploaded", document.Id });
        }

        [HttpPost("text")]
        [Authorize]
        public async Task<IActionResult> UploadText([FromBody] TextUploadDto dto)
        {
            var userId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Invalid token: UserId not found.");

            if (string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest("Text content is empty.");

            var (summary, keywords, category) = await _documentService.AnalyzeTextWithAI(dto.Text);

            var document = new Document
            {
                Title = string.IsNullOrWhiteSpace(dto.Title) ? "Untitled Text Submission" : dto.Title,
                FilePath = null,
                UploadDate = DateTime.UtcNow,
                UserId = userId,
                Summary = summary,
                Keywords = keywords,
                Category = category
            };

                await _documentService.AddAsync(document);
            return Ok(new { message = "Text submitted successfully", document.Id });
        }

        // GET: api/document/user
        [HttpGet("user")]
        public async Task<IActionResult> GetUserDocuments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var documents = await _documentService.GetByUserIdAsync(userId);
            return Ok(documents);
        }

        // GET: api/document/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var doc = await _documentService.GetByIdAsync(id);
            if (doc == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (doc.UserId != userId)
                return Forbid();

            return Ok(doc);
        }

        // DELETE: api/document/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var doc = await _documentService.GetByIdAsync(id);
            if (doc == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (doc.UserId != userId)
                return Forbid();

            await _documentService.DeleteAsync(doc);
            return Ok(new { message = "Document deleted" });
        }
        private async Task<(string summary, string keywords, string category)> AnalyzeFileWithAI(string filePath)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(5)  // Increase timeout to 5 minutes
            };

            using var form = new MultipartFormDataContent();

            //var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            form.Add(new StreamContent(System.IO.File.OpenRead(filePath)), "file", Path.GetFileName(filePath));

            var response = await httpClient.PostAsync("http://localhost:5000/analyze", form);

            if (!response.IsSuccessStatusCode)
                return ("AI analysis failed", "N/A", "Unknown");

            var resultJson = await response.Content.ReadAsStringAsync();
            var aiResult = JsonSerializer.Deserialize<AIResponseDto>(resultJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            //fileStream.Close();

            return (
                aiResult.Summary,
                string.Join(", ", aiResult.Keywords),
                aiResult.Category
            );
        }

    }
}
