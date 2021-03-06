﻿using System;
using System.Collections.Generic;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
	public class PageDTO
	{
		public int PageKey { get; set; }
		public int? PageFKey { get; set; }
		public string PageID { get; set; }

		public string NavTitle { get; set; }
		public string Title { get; set; }

		public char? AccessKey { get; set; }
		public string MetaDescription {get;set;}
        public string MetaKeywords { get; set; }
        public string PageTitle { get; set; }

		public string Body { get; set; }

		public string ThumbnailImage { get; set; }

		public bool? ShowInNav { get;set;}

		public bool? ShowInFeaturedNav { get; set; }

		public bool? ShowOnHome { get; set;}
		public bool? ShowInFooter { get; set;}
		public bool? Active{ get; set;}

		public int? SortOrder;

		public string LinkURL { get; set;}

		public DateTime LastModified { get; set; }

		public string PageUrl { get; set; }

		public bool? PasswordProtect { get; set; }

		public string Password { get; set;}

		public int? SiteFKey { get; set;}

        public Dictionary<string, KeyValueDto> KeyValues { get; set; }
        public IEnumerable<MediaDTO> GalleryImages { get; set; }

	}
}
