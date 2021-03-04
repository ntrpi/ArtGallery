using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.ViewModels
{
    public class ViewFormPieces
    {
        public FormDto formDto {
            get; set;
        }

        public IEnumerable<PieceDto> pieceDtos {
            get; set;
        }

        public Dictionary<int, ImageDto> imageDtos {
            get; set;
        }

    }
}