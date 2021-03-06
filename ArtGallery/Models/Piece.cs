﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtGallery.Models
{
    public class Piece
    {
        [Key]
        public int pieceId {
            get; set;
        }

        [Required]
        public string pieceName {
            get; set;
        }

        public string pieceDescription {
            get; set;
        }

        public decimal length {
            get; set;
        }

        public decimal width {
            get; set;
        }

        public decimal height {
            get; set;
        }

        public decimal piecePrice {
            get; set;
        }

        // A pieceDto has one form.
        [ForeignKey( "Form" )]
        public int? formId {
            get; set;
        }
        public virtual Form Form {
            get; set;
        }

        ICollection<Image> images {
            get; set;
        }

        //[DisplayName( "Techniques" )]
        //ICollection<Technique> techniques {
        //    get; set;
        //}
    }

    public class PieceDto
    {
        public static PieceDto getEmptyPieceDto( int formId )
        {
            return new PieceDto {
                formId = formId,
                pieceId = 0,
                pieceName = "",
                pieceDescription = "",
                length = 0,
                width = 0,
                height = 0,
                piecePrice = 0
            };
        }

        public int pieceId {
            get; set;
        }

        [DisplayName( "Name" )]
        public string pieceName {
            get; set;
        }

        [DisplayName( "Description" )]
        public string pieceDescription {
            get; set;
        }

        [DisplayName( "Length (cm)" )]
        public decimal length {
            get; set;
        }

        [DisplayName( "Width (cm)" )]
        public decimal width {
            get; set;
        }

        [DisplayName( "Height (cm)" )]
        public decimal height {
            get; set;
        }

        [DisplayName( "Price" )]
        [DataType( DataType.Currency )]
        public decimal piecePrice {
            get; set;
        }

        [DisplayName( "Form" )]
        public int? formId {
            get; set;
        }

        //[DisplayName( "Techniques" )]
        //public int techniques {
        //    get; set;
        //}
    }
}