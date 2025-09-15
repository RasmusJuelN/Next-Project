using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.DTO
{
    public class ClassStudentsDTO
    {
        public string ClassName { get; set; } = string.Empty;
        public List<string> Students { get; set; } = new();
    }
}
