using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.DataContexts;
using System.Configuration;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	internal class Site
	{

		public string UrlBase { get; set; }
		public int Key { get; set; }
		public string HostName { get; set; }
		public string Name { get; set; }

		public Site(Uri siteUri)
		{
			var dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString);
			//first find the site that matches the provided uri
			var site = (from s in dc.Sites
						where s.hostname == siteUri.ToString()
						select s).FirstOrDefault();

			//if we have one populate our site information
			if (site != null)
			{
				PopulateSiteDetails(siteUri, site);
				//we are done so quit
				return;
			}
			
			//if we failed to match try and find a site that matches just the hostname of the provided uri
			site = (from s in dc.Sites
					where s.hostname == siteUri.Host
					select s).FirstOrDefault();
			
			//if we now have a site populate the site details
			if (site != null)
			{
				PopulateSiteDetails(siteUri, site);
			}

		}

		/// <summary>
		/// takes the site uri and the matched site and populates the object
		/// </summary>
		/// <param name="siteUri"></param>
		/// <param name="site"></param>
		private void PopulateSiteDetails(Uri siteUri, mjjames.DataEntities.Site site)
		{
			Key = site.site_key;
			Name = site.name;
			HostName = siteUri.Host;
			//our url base is either all the url path segments or just /
			UrlBase = siteUri.Segments.Any() ? String.Join("/", siteUri.Segments) : "/";
		}
	}
}
