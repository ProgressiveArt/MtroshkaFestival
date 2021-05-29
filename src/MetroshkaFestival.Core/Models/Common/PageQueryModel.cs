using System;
using System.ComponentModel.DataAnnotations;

namespace MetroshkaFestival.Core.Models.Common
{
    public class PageQueryModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Min value is 1")]
        public int NumPage { get; set; } = 1;

        [Range(1, Int32.MaxValue, ErrorMessage = "Min value is 1")]
        public int RecordsCountPerPage { get; set; } = 25;
    }
}