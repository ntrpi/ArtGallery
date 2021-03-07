using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace ArtGallery.Models
{
    public class Form
    {
        [Key]
        public int formId {
            get; set;
        }

        public string formName {
            get; set;
        }

        public string formDescription {
            get; set;
        }

        public ICollection<Piece> pieces {
            get; set;
        }
    }

    public class FormDto
    {
        [DisplayName( "Form" )]
        public int formId {
            get; set;
        }

        [DisplayName( "Form Type" )]
        public string formName {
            get; set;
        }

        [DisplayName( "Description" )]
        public string formDescription {
            get; set;
        }

    }
}