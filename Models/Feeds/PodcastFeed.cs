using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Feeds
{
	public class PodcastFeed
	{
		public string Title { get; set; }
		public string Link { get; set; }
		public string Description { get; set; }
		public DateTime BuildDate { get; set; }
		public DateTime PubDate { get; set; }
		public IEnumerable<PodcastDTO> FeedItems { get; set; }
	}
}
