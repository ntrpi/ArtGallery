using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.ViewModels
{
    public class UpdateImage
    {
        public ImageDto imageDto {
            get; set;
        }

        public PieceDto pieceDto {
            get; set;
        }
    }
}