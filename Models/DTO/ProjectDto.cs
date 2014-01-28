using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
    public class ProjectDto
    {
            
            public string Description {get;set;}
            public DisplayDate DisplayDate {get;set;}
            public MediaInfo MediaInfo {get;set;}
            public string Title {get;set;}
            public string Url {get;set;}
            public Dictionary<string, KeyValueDto> KeyValues { get; set; }
            public string MetaDescription { get; set; }
            public string PageTitle { get; set; }
            public string ThumbnailImage { get; set; }

    }
}
