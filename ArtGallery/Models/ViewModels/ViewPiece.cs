using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArtGallery.Models.ViewModels
{
    public class ViewPiece
    {
        public PieceDto pieceDto {
            get; set;
        }

        public FormDto formDto {
            get; set;
        }
    }
}