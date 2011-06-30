using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	public class CalendarModel
	{
		public DateTime Date { get; set; }
		public List<NavigationItem> MainNavigation { get; set; }
		public List<NavigationItem> FooterNavigation { get; set; }
		/// <summary>
		/// All of the events for the provided date
		/// </summary>
		public List<CalendarEntryDTO> Events { get; set; }
		/// <summary>
		/// The first event if a listing page or the actual event if a details page
		/// </summary>
		public CalendarEntryDTO Event{get;set;}
		/// <summary>
		/// whether or not this is a listing or details page
		/// </summary>
		public bool IsDetailsPage{get;set;}
		public string PageTitle{get;set;}
	}
}
