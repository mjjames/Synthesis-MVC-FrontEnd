using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Interfaces
{
    public interface ISiteSettings
    {
        T GetSetting<T>(string key) where T : IConvertible;
    }
}
