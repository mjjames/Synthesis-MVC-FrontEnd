using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.ActionResults
{
	class RSSActionResult : ActionResult
	{
		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.AddHeader("Content-Type", "text/xml");
		}
	}
}
