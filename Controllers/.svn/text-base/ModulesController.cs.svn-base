using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	[HandleError]
	public class ModulesController : Controller
	{
		public ActionResult BibleGatewayVerseOfTheDay(BibleGateway.BibleTranslation version)
		{
			var verseoftheday = new BibleGateway.BibleGatewayWrapper().GetVerseOfTheDay(version);

			return View("VerseOfTheDay", verseoftheday);
		}
	}
}
