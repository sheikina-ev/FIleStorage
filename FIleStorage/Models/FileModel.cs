using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIleStorage.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; } // Размер как строка (например, "100 KB")
        public string Path { get; set; }
        public DateTime? CreatedAt { get; set; } // Nullable для случаев с null
    }


}
