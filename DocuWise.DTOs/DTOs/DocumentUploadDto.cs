using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.DTOs.DTOs
{
    public class DocumentUploadDto
    {
        public IFormFile File { get; set; }
        public string Title { get; set; }
    }
}
