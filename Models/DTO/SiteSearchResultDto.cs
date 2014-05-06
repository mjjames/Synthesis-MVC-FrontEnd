﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
    public class SiteSearchResultDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public DateTime? Published { get; set; }
    }
}
