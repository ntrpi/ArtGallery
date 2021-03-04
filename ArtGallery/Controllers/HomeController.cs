using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ArtGallery.Models;
using ArtGallery.Models.ViewModels;

namespace ArtGallery.Controllers
{
    public class HomeController: Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly ControllersHelper helper;

        static HomeController()
        {
            helper = new ControllersHelper();
        }
        public ActionResult Index()
        {
            ViewFormsWithImage viewFormsWithImage = new ViewFormsWithImage();
            viewFormsWithImage.formDtos = helper.getFormDtos();
            Dictionary<int, ImageDto> imageDtos = new Dictionary<int, ImageDto>();
            viewFormsWithImage.imageDtos = imageDtos;

            foreach( var form in viewFormsWithImage.formDtos ) {

                PieceDto pieceDto = helper.getLatestPieceDtoForForm( form.formId );
                ImageDto primaryImage = helper.getPrimaryImageDtoForPiece( pieceDto.pieceId );
                if( primaryImage != null ) {
                    imageDtos.Add( (int) pieceDto.formId, primaryImage );
                }
            }

            return View( viewFormsWithImage );
        }

        public ActionResult Category( int id ) // id == formId
        {
            ViewFormPieces viewFormPieces = new ViewFormPieces();
            viewFormPieces.formDto = helper.getFormDto( id );

            IEnumerable<PieceDto> pieceDtos = helper.getPieceDtosForForm( id );
            viewFormPieces.pieceDtos = pieceDtos;

            Dictionary<int, ImageDto> imageDtos = new Dictionary<int, ImageDto>();
            foreach( var piece in pieceDtos ) {
                ImageDto primaryImage = helper.getPrimaryImageDtoForPiece( piece.pieceId );
                if( primaryImage != null ) {
                    imageDtos.Add( piece.pieceId, primaryImage );
                }
            }
            viewFormPieces.imageDtos = imageDtos;

            return View( viewFormPieces );
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}