using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtGallery.Models;

namespace ArtGallery.Models.ViewModels
{
    public class UpdatePiece
    {
        public PieceDto piece {
            get; set;
        }

        public IEnumerable<FormDto> forms {
            get; set;
        }

    }
}