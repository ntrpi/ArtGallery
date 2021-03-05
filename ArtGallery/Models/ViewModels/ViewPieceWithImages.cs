using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.ViewModels
{
    public class ViewPieceWithImages
    {
        public PieceDto pieceDto {
            get; set;
        }

        public FormDto formDto {
            get; set;
        }

        public IEnumerable<ImageDto> imageDtos {
            get; set;
        } 
    }
}