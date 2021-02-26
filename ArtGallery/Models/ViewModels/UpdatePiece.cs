using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtGallery.Models;

namespace ArtGallery.Models.ViewModels
{
    public class UpdatePiece
    {
        // For some reason changing the name of piece to pieceDto or forms to formDtos breaks the create functionality. I have absolutely no idea why.
        public PieceDto piece {
            get; set;
        }

        public IEnumerable<FormDto> forms {
            get; set;
        }

    }
}