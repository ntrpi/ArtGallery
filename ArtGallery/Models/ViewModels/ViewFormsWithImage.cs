using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.ViewModels
{
    public class ViewFormsWithImage
    {
        public IEnumerable<FormDto> formDtos {
            get; set;
        }

        public Dictionary<int, ImageDto> imageDtos {
            get; set;
        }
    }
}