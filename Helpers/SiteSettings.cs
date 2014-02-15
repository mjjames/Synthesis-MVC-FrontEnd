using mjjames.MVC_MultiTenant_Controllers_and_Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Helpers
{
    public static class SiteSettings
    {
        public static T SiteSetting<T>(this HtmlHelper helper, string settingName, ISiteSettings siteSettings = null) where T : IConvertible
        {
            if (siteSettings == null)
            {
                siteSettings = new ConfigurationManagerSiteSettings();
            }
            return siteSettings.GetSetting<T>(settingName);
        }

        public static string SiteSetting(this HtmlHelper helper, string settingName, ISiteSettings siteSettings = null)
        {
            if (siteSettings == null)
            {
                siteSettings = new ConfigurationManagerSiteSettings();
            }
            return siteSettings.GetSetting<string>(settingName);
        }
    }
}
