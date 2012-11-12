using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.DataContexts;
using System.Configuration;
using System.Web;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
	internal class Site
	{

		public string UrlBase { get; set; }
		public int Key { get; set; }
		public string HostName { get; set; }
		public string Name { get; set; }

		/// <summary>
		/// Takes the current uri and returns one suitable for finding a site with
		/// </summary>
		public Site(){
			//get our uri
			var uri = HttpContext.Current.Request.Url;
			//we can only have a maximum of 1 path segment so take our uri and create a new one from it with only the first url segment if it has one
			var newUri = new UriBuilder(uri);
			//take the path and only take everything from the beginning until the first /
			newUri.Path = newUri.Path.Substring(0, newUri.Path.IndexOf("/"));
			//remove the querystring
			newUri.Query = "";
			//Init the site object
			Init(newUri.Uri);
		}

		public Site(Uri siteUri)
		{
			Init(siteUri);
		}

		private void Init(Uri siteUri){
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

            //finally if we are local try and do a local domain check
            if (site == null && HttpContext.Current.Request.IsLocal)
            {
                var host = siteUri.ToString().Replace(".local", ".co.uk");
                site = (from s in dc.Sites
                        where s.hostname == host
						select s).FirstOrDefault();

                //last attempt
                if (site == null)
                {
                    host = siteUri.ToString().Replace(".local", ".org.uk");
                    site = (from s in dc.Sites
                            where s.hostname == host
                            select s).FirstOrDefault();
                }
                //if we still fail see if its a ksl site
                if (site == null)
                {
                    host = siteUri.ToString().Replace(".local", ".kslgarageservices.co.uk");
                    site = (from s in dc.Sites
                            where s.hostname == host
                            select s).FirstOrDefault();
                }
            }

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
