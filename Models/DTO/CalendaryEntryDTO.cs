using System;
namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO
{
	public class CalendarEntryDTO
	{
        public string Description { get; set; }
        public string Identifier { get; set; }
		public string Location{ get; set;}
        public string Title { get; set; }
		
        public DateTime Start { get; set;}
		public DateTime End { get; set;}
        
    }
}
