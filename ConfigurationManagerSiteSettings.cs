using mjjames.MVC_MultiTenant_Controllers_and_Models.Interfaces;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models
{
    public class ConfigurationManagerSiteSettings : ISiteSettings
    {
        private Site _site;
        public ConfigurationManagerSiteSettings()
        {
            _site = new Site();
        }

        public T GetSetting<T>(string key) where T : IConvertible
        {
            var settingKey = GetKey(key);
            if(!ConfigurationManager.AppSettings.AllKeys.Contains(settingKey)){
                settingKey = string.Format("{0}:{1}", Prefix, key);
            }

            try
            {
                return (T)Convert.ChangeType(ConfigurationManager.AppSettings[settingKey], typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        private string GetKey(string key)
        {
            return string.Format("{0}:{1}:{2}", Prefix, _site.Key, key);
        }

        private const string Prefix = "SiteSettings";
    }
}
