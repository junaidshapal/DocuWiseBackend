using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWise.DTOs.DTOs
{
    public class AIResponseDto
    {
        public string Summary { get; set; }
        public List<string> Keywords { get; set; }
        public string Category { get; set; }
    }
}
