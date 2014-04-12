using System;
using System.Linq;
using System.Web.Mvc;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;
using System.Net.Mail;
using System.Configuration;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using mjjames.MVC_MultiTenant_Controllers_and_Models.ActionFilters;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Controllers
{
	[HandleError]
    [PasswordProtectedSiteFilter]
	public class FormsController : Controller
	{
	    private readonly PageModelRepository _pageModelRepository;
        public FormsController()
        {
            _pageModelRepository = new PageModelRepository(new Site());
        }

		//
		// GET: /ContactUsIndex/
		/// <summary>
		/// Renders the contact form
		/// </summary>
		/// <returns></returns>
		public ActionResult ContactUsIndex()
		{
			//bit of a trick here - load the page action result and then pull just the model out and pass that to our view
            return View(_pageModelRepository.FromId("contactus"));
		}
		/// <summary>
		/// sends the contact us form
		/// </summary>
		/// <returns></returns>
		[CaptchaValidator]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ContactUsIndex(string name, string email, string message, bool captchaValid)
		{
		    var pageData = _pageModelRepository.FromId("contactus");

			ViewData["name"] = name;
			ViewData["email"] = email;
			ViewData["message"] = message;

			var bSendEmail = false;
			var objEmail = new MailMessage();

			// Validation
			if (!captchaValid)
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
            var bcc = ConfigurationManager.AppSettings["enquiryEmailBCC"];
            if (!String.IsNullOrWhiteSpace(bcc))
            {
                objEmail.Bcc.Add(bcc);
            }
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
			var smtp = new SmtpClient { Host = "localhost" };

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

		/// <summary>
		/// Processes a callback
		/// </summary>
		/// <param name="contactName"></param>
		/// <param name="contactNumber"></param>
		/// <param name="timeSlot"></param>
		/// <param name="email">actually the current date, the name email is to trick bots</param>
		/// <returns></returns>
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult CallBack(string contactName, string contactNumber, string timeSlot, string email)
		{
			//if the date returned by the form is not todays date we are spam
			if (!DateTime.Now.ToString("d/M/yyyy").Equals(email))
			{
				return View("Callback");
			}

			var messageBody = String.Format("Name: {0}\r\nNumber: {1}\r\nTime Slot: {2})", contactName, contactNumber, timeSlot);
			var callbackMessage = new MailMessage(ConfigurationManager.AppSettings["callbackFormFromAddress"], ConfigurationManager.AppSettings["callbackFormToAddress"], "IDEA Callback Request", messageBody)
			{
				IsBodyHtml = false
			};

			var smtp = new SmtpClient { Host = "localhost" };
			smtp.Send(callbackMessage);

			return View("CallbackComplete");

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
