using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.GoogleCalendarWrapper;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Factories
{
	/// <summary>
	/// Provides factory methods for loading external modules
	/// </summary>
	public class ExternalModulesFactory
	{
		/// <summary>
		/// Returns a Google Calendar Provider
		/// </summary>
		/// <param name="calendarUrl">URL to the calendar feed</param>
		/// <returns></returns>
		public static IGoogleCalendar GetCalendarInstance(string calendarUrl)
		{
#if DEBUG
			//if in debug mode and no url provided always return the test wrapper which has sample data
			if (string.IsNullOrWhiteSpace(calendarUrl))
			{
				return new TestGoogleCalendar();
			}
#endif

			//return the full Google Calendar Wrappeer
			return new GoogleCalendar(calendarUrl);
		}
	}
}
