using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mjjames.DataContexts;
using System.Configuration;
using System.Web;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Repositories;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
    public class Site
    {

        public string UrlBase { get; private set; }
        internal int Key { get; private set; }
        public string HostName { get; private set; }
        public string Name { get; private set; }

        private double _cacheMinutes = 15;

        public IReadOnlyDictionary<string, KeyValueDto> KeyValues { get; private set; }

        public T Setting<T>(string key) where T : IConvertible
        {
            if (!KeyValues.ContainsKey(key))
            {
                return default(T);
            }
            try
            {
                return (T)Convert.ChangeType(KeyValues[key].Value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }


        public string Setting(string key)
        {
            return Setting<string>(key);
        }

        /// <summary>
        /// Takes the current uri and returns one suitable for finding a site with
        /// </summary>
        internal Site()
        {
            //get our uri
            var uri = HttpContext.Current.Request.Url;
            //we can only have a maximum of 1 path segment so take our uri and create a new one from it with only the first url segment if it has one
            var newUri = new UriBuilder(uri);
            //take the path and only take the first segment
            newUri.Path = "/" + (newUri.Uri.Segments.Length > 1 ? newUri.Uri.Segments[1] : "");
            //ensure path ends /
            if (!newUri.Path.EndsWith("/"))
            {
                newUri.Path = newUri.Path + "/";
            }
            //remove the querystring
            newUri.Query = "";
            //Init the site object
            Init(newUri.Uri);
        }

        internal Site(Uri siteUri)
        {
            Init(siteUri);
        }

        private void Init(Uri siteUri)
        {
            if (HttpContext.Current.Request.IsLocal)
            {
                _cacheMinutes = 0.5;
            }
            PopulateSite(siteUri);
            PopulateKeyValues();
        }

        private void PopulateSite(Uri siteUri)
        {
            var cachedSite = HttpContext.Current.Cache["site-" + siteUri.ToString()] as mjjames.DataEntities.Site;
            if (cachedSite != null)
            {
                PopulateSiteDetails(siteUri, cachedSite, false);
            }
            else
            {
                PopulateSiteFromDb(siteUri);
            }
        }

        private void PopulateSiteFromDb(Uri siteUri)
        {
            using (var dc = new CMSDataContext(ConfigurationManager.ConnectionStrings["ourDatabase"].ConnectionString))
            {

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

                    //failing that just load the first site taht is active
                    if (site == null)
                    {
                        site = dc.Sites.FirstOrDefault(s => s.active);
                    }
                }

                //if we now have a site populate the site details
                if (site != null)
                {
                    PopulateSiteDetails(siteUri, site);
                }
            }
        }

        private void PopulateKeyValues()
        {
           if (Key == 0)
            {
                KeyValues = new Dictionary<string, KeyValueDto>();
                return;
            }

            var cacheKey = "keyvalues-site-" + Key;
            KeyValues = HttpContext.Current.Cache[cacheKey] as IReadOnlyDictionary<string, KeyValueDto>;
            if (KeyValues != null)
            {
                return;
            }
            KeyValues = new KeyValueRepository().ByLink(Key, "sitelookup").ToDictionary(kv => kv.lookup.lookup_id, kv => new KeyValueDto(kv.keyvalue_key, kv.lookup.title, kv.value));
            HttpContext.Current.Cache.Add(cacheKey, KeyValues, null, DateTime.Now.AddMinutes(_cacheMinutes), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
        }

        /// <summary>
        /// takes the site uri and the matched site and populates the object
        /// </summary>
        /// <param name="siteUri"></param>
        /// <param name="site"></param>
        private void PopulateSiteDetails(Uri siteUri, mjjames.DataEntities.Site site, bool cacheSite = true)
        {
            Key = site.site_key;
            Name = site.name;
            HostName = site.hostname;
            UrlBase = new Uri(site.hostname).LocalPath;

            if (cacheSite)
            {
                HttpContext.Current.Cache.Add("site-" + siteUri.ToString(), site, null, DateTime.Now.AddMinutes(_cacheMinutes), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
            }
        }

    }
}
