using mjjames.MVC_MultiTenant_Controllers_and_Models.Interfaces;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models
{
    public class DatabaseSiteSettings : ISiteSettings
    {
        private Site _site;
        public DatabaseSiteSettings(Site site)
        {
            _site = site;
        }

        public T GetSetting<T>(string key) where T : IConvertible
        {
            return _site.Setting<T>(key);
        }

        internal Site Site { get { return _site; } }
    }
}
