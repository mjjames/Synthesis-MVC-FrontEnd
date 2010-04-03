using System;
using System.Linq;
using System.Web.Mvc;
using mjjames.Models;
using System.Net.Mail;
using System.Configuration;

namespace mjjames.Controllers
{
	[HandleError]
	public class FormsController : Controller
	{
		readonly NavigationRepository _navs = new NavigationRepository();

		//
		// GET: /ContactUsIndex/
		/// <summary>
		/// Renders the contact form
		/// </summary>
		/// <returns></returns>
		public ActionResult ContactUsIndex()
		{
			ViewData["MainNav"] = _navs.GetMainNavigation().ToList();
			ViewData["FooterNav"] = _navs.GetFooterNavigation().ToList();
			return View();
		}

		/// <summary>
		/// sends the contact us form
		/// </summary>
		/// <returns></returns>
		[CaptchaValidator]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ContactUsIndex(string name, string email, string message, bool captchaValid)
		{
			ViewData["MainNav"] = _navs.GetMainNavigation().ToList();
			ViewData["FooterNav"] = _navs.GetFooterNavigation().ToList();
		
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
				return View();
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
				ViewData["Header"] = "Invalid Email Address, please try again";

				ViewData["Message"] = String.Format("Supplied Email: {0}  <br /> Supplied Name: {1}", email, name);
			}
			objEmail.Subject = ConfigurationManager.AppSettings["enquiryEmailSubject"];
			objEmail.Body = String.Format("{0} \r\n Name: {1} \r\n Email: {2}", message, name, email);
			objEmail.Priority = MailPriority.High;
			//send the message
			var smtp = new SmtpClient {Host = "localhost"};
#if DEBUG
			//smtp.Host = "smtp.blueyonder.co.uk";
#endif
			if (bSendEmail)
			{
				try
				{
					smtp.Send(objEmail);
					ViewData["header"] = "Your enquiry has been sent sucessfully - Thank You";
				}
				catch (Exception exc)
				{
					ViewData["header"] = "Your Enquiry Failed to Send:";
					ViewData["message"] = objEmail.Body;
#if DEBUG
					ViewData["message"] += exc.ToString();
#endif
				}
			}


			return View("FormComplete");
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
