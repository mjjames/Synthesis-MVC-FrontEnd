using System;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	[HandleError]
	public class ModulesController : Controller
	{
		/// <summary>
		/// Gets the BibleGatewayVerseOfTheDay PartialView
		/// </summary>
		/// <param name="version">BibleGateway Bible Version</param>
		/// <returns>PartialView</returns>
		public ActionResult BibleGatewayVerseOfTheDay(BibleGateway.BibleTranslation version)
		{
			var verseoftheday = new BibleGateway.BibleGatewayWrapper().GetVerseOfTheDay(version);

			return View("VerseOfTheDay", verseoftheday);
		}

		
	}
}
