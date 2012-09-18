using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mjjames.MVC_MultiTenant_Controllers_and_Models.Models
{
    public class MediaInfo
    {
        public string PhotoGalleryId { get; set; }
        public string VideoId { get; set; }

        public IQueryable<DTO.MediaDTO> GalleryImages { get; set; }
    }
}
