using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.Data.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
