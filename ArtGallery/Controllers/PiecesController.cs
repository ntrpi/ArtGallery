using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using ArtGallery.Models;
using ArtGallery.Models.ViewModels;

namespace ArtGallery.Controllers
{
    public class PiecesController: Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly ControllersHelper helper;

        static PiecesController()
        {
            helper = new ControllersHelper();
        }

        // GET: Pieces
        public ActionResult Index()
        {
            IEnumerable<PieceDto> pieceDtos = helper.getPieceDtos();
            List<ViewPiece> viewPieces = new List<ViewPiece>();
            foreach( PieceDto pieceDto in pieceDtos ) {
                FormDto form = null;
                if( pieceDto.formId != null ) {
                    form = helper.getFormDto( (int) pieceDto.formId );
                }
                ViewPiece viewPiece = new ViewPiece {
                    pieceDto = pieceDto,
                    formDto = form
                };
                viewPieces.Add( viewPiece );
            }
            return View( viewPieces );
        }

        // GET: Pieces/Details/5
        public ActionResult Details( int id )
        {
            return View( helper.getViewPiece( id ) );
        }

        // GET: Pieces/Create
        public ActionResult Create()
        {
            UpdatePiece updatePiece = new UpdatePiece();
            IEnumerable<FormDto> formDtos = helper.getFormDtos();
            updatePiece.forms = formDtos;

            return View( updatePiece );
        }

        // POST: Pieces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create( Piece piece )
        {
            string url = "PiecesData/CreatePiece";
            HttpResponseMessage response = helper.doPostRequest( url, piece );

            if( response.IsSuccessStatusCode ) {
                string jsonContent = response.Content.ReadAsStringAsync().Result;
                PieceDto pieceDto = jss.Deserialize<PieceDto>( jsonContent );
                return RedirectToAction( "Details", new {
                    id = pieceDto.pieceId
                } );
            }

            return RedirectToAction( "Error" );
        }


        // GET: Pieces/Edit/5
        public ActionResult Edit( int id )
        {
            return View( helper.getUpdatePiece( id ) );
        }

        // POST: Pieces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( [Bind( Include = "pieceId,pieceName,pieceDescription,length,width,height,piecePrice,formId" )] Piece piece )
        {
            string url = "PiecesData/UpdatePiece/" + piece.pieceId;
            HttpResponseMessage response = helper.doPostRequest( url, piece );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to update " + piece.pieceName;
                return View();
            }

            return RedirectToAction( "Details", new {
                id = piece.pieceId
            } );
        }

        // GET: Pieces/Delete/5
        public ActionResult DeleteConfirm( int id )
        {
            ViewPiece viewPiece = helper.getViewPiece( id );
            return View( viewPiece );
        }

        // POST: Piece/Delete/5
        [HttpPost]
        public ActionResult Delete( int id, FormCollection collection )
        {
            string url = "PiecesData/DeletePiece/" + id;
            HttpResponseMessage response = helper.doPostRequest( url, "" );
            return RedirectToAction( "Index" );
        }
    }
}
