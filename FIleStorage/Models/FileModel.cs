using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIleStorage.Models
{
    public class FileModel
    {
        public int Id { get; set; }         // id файла
        public string Name { get; set; }     // Имя файла
        public string Extension { get; set; } // Расширение файла
        public long Size { get; set; }       // Размер файла
        public string Path { get; set; }     // Путь к файлу
        public int UserId { get; set; }      // Идентификатор пользователя
        public DateTime CreatedAt { get; set; } // Дата создания
        public DateTime UpdatedAt { get; set; } // Дата последнего обновления
    }
}
