using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtGallery.Models
{
    public class Image
    {
        public static string imagesPath = "https://localhost:44397/Content/Images/";

        [ Key]
        public int imageId {
            get; set;
        }

        public string imageName {
            get; set;
        }

        public string imageOldName {
            get; set;
        }

        public string imageExt {
            get; set;
        }
        public bool isMainImage {
            get; set;
        }

        // An image is of one pieceDto.
        [ForeignKey( "Piece" )]
        public int pieceId {
            get; set;
        }
        public virtual Piece Piece {
            get; set;
        }
    }

    public class ImageDto
    {
        public int imageId {
            get; set;
        }

        [DisplayName( "File Name" )]
        public string imageName {
            get; set;
        }

        public string imageOldName {
            get; set;
        }

        public string imageExt {
            get; set;
        }

        [DisplayName( "Is Primary Image" )]
        public bool isMainImage {
            get; set;
        }

        public int pieceId {
            get; set;
        }

        public string imagesPath {
            get; set;
        }
    }
}