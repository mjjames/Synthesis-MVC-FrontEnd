using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DataEntities
{
	public class Podcast
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Filename { get; set; }
		public DateTime Published { get; set; }
		public bool Active { get; set; }
	}
}
