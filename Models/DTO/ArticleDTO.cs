
using System;
namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
	public class ArticleDTO
	{
		public ArticleDTO()
		{
			Date = new DateRange();
		}

		public string Url { get; set; }

		public string Title { get; set; }

		public string Body { get; set; }

		public bool InFeed { get; set; }

		public string Description { get; set; }

		public DateRange Date { get; private set; }



		public string ThumbnailImage { get; set; }
	}
	public class DateRange
	{
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
	}
}
