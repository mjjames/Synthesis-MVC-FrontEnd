using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.DTO;
using mjjames.MVC_MultiTenant_Controllers_and_Models.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
    public class FlickrGalleryModel : ISitePage
    {
        public List<NavigationItem> MainNavigation { get; set; }
        public IList<NavigationItem> FooterNavigation { get; set; }
        public IList<NavigationItem> BreadcrumbNavigation { get; set; }
        public string Title { get; set; }
        public FlickrSetDto GalleryInfo { get; set; }
        public IEnumerable<FlickrPhotoDto> Photos { get; set; }
        public string PageTitle { get { return Title; } }
        public Site Site { get; internal set; }
    }
}
