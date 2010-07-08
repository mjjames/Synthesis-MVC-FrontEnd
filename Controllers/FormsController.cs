using System;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using System.Net.Mail;
using System.Configuration;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	[HandleError]
	public class FormsController : Controller
	{
		readonly PageRepository _pages = new PageRepository();
		readonly NavigationRepository _navs = new NavigationRepository();


		//
		// GET: /ContactUsIndex/
		/// <summary>
		/// Renders the contact form
		/// </summary>
		/// <returns></returns>
		public ActionResult ContactUsIndex()
		{
			//bit of a trick here - load the page action result and then pull just the model out and pass that to our view

			var pageData = BuildPage(_pages.Get("contactus"));

			return View(pageData);
		}

		//converts a Page into a PageModel
		private PageModel BuildPage(DataEntities.Page ourPage)
		{
			if(ourPage == null)
			{
				return new PageModel
				       	{
				       		FooterNavigation = _navs.GetFooterNavigation().ToList(),
							MainNavigation = _navs.GetMainNavigation().ToList()
				       	};
			}

			PageModel newPage = new HomePageModel
			{
				AccessKey = ourPage.accesskey,
				Active = ourPage.active,
				Body = ourPage.body,
				FooterNavigation = _navs.GetFooterNavigation().ToList(),
				LastModified = ourPage.lastmodified,
				LinkURL = ourPage.linkurl,
				MainNavigation = _navs.GetMainNavigation().ToList(),
				MetaDescription = ourPage.metadescription,
				MetaKeywords = ourPage.metakeywords,
				NavTitle = ourPage.navtitle,
				PageFKey = ourPage.page_fkey,
				PageKey = ourPage.page_key,
				PageUrl = ourPage.page_url,
				PageID = ourPage.pageid,
				Password = ourPage.password,
				PasswordProtect = ourPage.passwordprotect,
				ShowInFeaturedNav = ourPage.showinfeaturednav,
				ShowInFooter = ourPage.showinfooter,
				ShowInNav = ourPage.showinnav,
				ShowOnHome = ourPage.showonhome,
				SortOrder = ourPage.sortorder,
				ThumbnailImage = ourPage.thumbnailimage,
				Title = ourPage.title
			};
			if (!String.IsNullOrEmpty(newPage.ThumbnailImage))
			{
				newPage.ThumbnailImage = System.IO.Path.Combine(ConfigurationManager.AppSettings["uploaddir"], newPage.ThumbnailImage);
			}
			return newPage;
		}

		/// <summary>
		/// sends the contact us form
		/// </summary>
		/// <returns></returns>
		[CaptchaValidator]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ContactUsIndex(string name, string email, string message, bool captchaValid)
		{
			var pageData = BuildPage(_pages.Get("contactus"));

			ViewData["name"] = name;
			ViewData["email"] = email;
			ViewData["message"] = message;

			var bSendEmail = false;
			var objEmail = new MailMessage();

			// Validation
			if(!captchaValid)
				ViewData.ModelState.AddModelError("captcha", "Please Complete the Captcha");
			if (string.IsNullOrEmpty(name))
				ViewData.ModelState.AddModelError("name", "Please enter your name");
			if (string.IsNullOrEmpty(email))
				ViewData.ModelState.AddModelError("email", "Please enter your e-mail");
			if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
				ViewData.ModelState.AddModelError("email", "Please enter a valid e-mail");
			if (string.IsNullOrEmpty(message))
				ViewData.ModelState.AddModelError("message", "Please enter a message");

			// Send e-mail?
			if (!ViewData.ModelState.IsValid)
			{
				return View(pageData);
			}
#if DEBUG
			objEmail.To.Add("mike+debug@mjjames.co.uk");
#else
			objEmail.To.Add(ConfigurationManager.AppSettings["enquiryEmailTo"]);
			objEmail.Bcc.Add(ConfigurationManager.AppSettings["enquiryEmailBCC"]);
#endif
			try
			{
// ReSharper disable AssignNullToNotNullAttribute
				// we validate email earlier on when we do our model validation
				objEmail.From = new MailAddress(email, name);
				objEmail.CC.Add(email);
// ReSharper restore AssignNullToNotNullAttribute

				bSendEmail = true;
			}
			catch (Exception)
			{
				pageData.Body = "<h2> Invalid Email Address, please try again </h2> " + String.Format("<p>Supplied Email: {0}  <br /> Supplied Name: {1}</p>", email, name);
			}
			objEmail.Subject = ConfigurationManager.AppSettings["enquiryEmailSubject"];
			objEmail.Body = String.Format("{0} \r\n Name: {1} \r\n Email: {2}", message, name, email);
			objEmail.Priority = MailPriority.High;
			//send the message
			var smtp = new SmtpClient {Host = "localhost"};

			if (bSendEmail)
			{
				try
				{
					smtp.Send(objEmail);
					pageData.Body = "<h2> Your enquiry has been sent sucessfully - Thank You </h2>";
				}
				catch (Exception exc)
				{
					pageData.Body = "<h2> Your Enquiry Failed to Send: </h2>";
					pageData.Body += objEmail.Body;
#if DEBUG
					pageData.Body += exc.ToString();
#endif
				}
			}


			return View("FormComplete", pageData);
		}

	}

	public class CaptchaValidatorAttribute : ActionFilterAttribute
	{
		private const string ChallengeFieldKey = "recaptcha_challenge_field";
		private const string ResponseFieldKey = "recaptcha_response_field";

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var captchaChallengeValue = filterContext.HttpContext.Request.Form[ChallengeFieldKey];
			var captchaResponseValue = filterContext.HttpContext.Request.Form[ResponseFieldKey];
			var captchaValidtor = new Recaptcha.RecaptchaValidator
									  {
										  PrivateKey = ConfigurationManager.AppSettings["recaptchaPrivateKey"],
										  RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
										  Challenge = captchaChallengeValue,
										  Response = captchaResponseValue
									  };

			var recaptchaResponse = captchaValidtor.Validate();

			// this will push the result value into a parameter in our Action  
			filterContext.ActionParameters["captchaValid"] = recaptchaResponse.IsValid;

			base.OnActionExecuting(filterContext);
		}
	}
}
