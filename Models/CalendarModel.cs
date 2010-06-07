using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.Models;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class CalendarModel
	{
		public DateTime Date { get; set; }
		public List<NavigationItem> MainNavigation { get; set; }
		public List<NavigationItem> FooterNavigation { get; set; }
	}
}
