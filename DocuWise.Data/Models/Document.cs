using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? FilePath { get; set; }
        public string? Summary { get; set; }
        public string? Keywords { get; set; }
        public string? Category { get; set; }
        public DateTime UploadDate { get; set; }

        // Foreign Key
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
