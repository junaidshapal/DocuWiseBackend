using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
