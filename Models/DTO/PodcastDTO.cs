using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
	public class PodcastDTO
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string FileName { get; set; }
		public DateTime? Published { get; set; }
	}
}
