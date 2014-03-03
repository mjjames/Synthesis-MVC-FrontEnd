using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
    public class DownloadDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public Dictionary<string, KeyValueDto> KeyValues { get; set; }
    }
}
