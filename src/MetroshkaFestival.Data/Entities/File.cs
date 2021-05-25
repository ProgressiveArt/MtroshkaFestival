using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MetroshkaFestival.Data.Entities
{
    public class File
    {
        public int Id { get; set; }
        public string OriginalFileName { get; set; }
        public string Path { get; set; }
        public int Length { get; set; }
        public string ContentType { get; set; }
        public DateTime CreationDate { get; set; }

        [NotMapped]
        public string Url => $"/files/{Path}";
    }
}