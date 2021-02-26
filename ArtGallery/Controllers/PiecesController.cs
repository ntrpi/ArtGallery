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
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static PiecesController()
        {
            HttpClientHandler handler = new HttpClientHandler() {
                AllowAutoRedirect = false
            };
            client = new HttpClient( handler );

            // Change this to match your own local port number.
            client.BaseAddress = new Uri( "https://localhost:44397/api/" );

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue( "application/json" ) );

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);
        }

        // GET: Pieces
        public ActionResult Index()
        {
            string url = "PiecesData/GetPieceDtos";
            HttpResponseMessage response = client.GetAsync( url ).Result;
            if( !response.IsSuccessStatusCode ) {
                return View( new List<ViewPiece>() );
            }

            IEnumerable<PieceDto> pieceDtos = response.Content.ReadAsAsync<IEnumerable<PieceDto>>().Result;
            List<ViewPiece> viewPieces = new List<ViewPiece>();
            foreach( PieceDto pieceDto in pieceDtos ) {
                FormDto form = null;
                if( pieceDto.formId != null ) {
                    form = getFormDto( (int) pieceDto.formId );
                }
                ViewPiece viewPiece = new ViewPiece {
                    pieceDto = pieceDto,
                    formDto = form
                };
                viewPieces.Add( viewPiece );
            }
            return View( viewPieces );
        }

        private FormDto getFormDto( int formId )
        {
            string url = "FormsData/GetFormDto/" + formId;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            string jsonContent = response.Content.ReadAsStringAsync().Result;
            FormDto formDto = jss.Deserialize<FormDto>( jsonContent );
            return formDto;
        }

        private PieceDto getPieceDto( int id )
        {
            string url = "PiecesData/GetPieceDto/" + id;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            PieceDto pieceDto = response.Content.ReadAsAsync<PieceDto>().Result;
            return pieceDto;
        }

        private ViewPiece getViewPiece( PieceDto pieceDto )
        {
            ViewPiece viewPiece = new ViewPiece();
            viewPiece.pieceDto = pieceDto;
            if( pieceDto.formId != null ) {
                viewPiece.formDto = getFormDto( (int) pieceDto.formId );
            }
            return viewPiece;
        }
        private ViewPiece getViewPiece( int id )
        {
            PieceDto pieceDto = getPieceDto( id );
            return getViewPiece( pieceDto );
        }

        // GET: Pieces/Details/5
        public ActionResult Details( int id )
        {
            return View( getViewPiece( id ) );
        }

        // GET: Pieces/Create
        public ActionResult Create()
        {
            string url = "FormsData/GetFormDtos";
            HttpResponseMessage response = client.GetAsync( url ).Result;
            IEnumerable<FormDto> formDtos;
            if( !response.IsSuccessStatusCode ) {
                formDtos = new List<FormDto>();
            } else {
                formDtos = response.Content.ReadAsAsync<IEnumerable<FormDto>>().Result;
            }

            UpdatePiece updatePiece = new UpdatePiece();
            updatePiece.forms = formDtos;

            return View( updatePiece );
        }

        private HttpResponseMessage doPiecePostRequest( string url, Piece piece )
        {
            HttpContent content = new StringContent( jss.Serialize( piece ) );
            content.Headers.ContentType = new MediaTypeHeaderValue( "application/json" );
            HttpResponseMessage response = client.PostAsync( url, content ).Result;
            return response;
        }

        // POST: Pieces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create( Piece piece )
        {
            string url = "PiecesData/CreatePiece";
            Debug.WriteLine( jss.Serialize( piece ) );
            HttpContent content = new StringContent( jss.Serialize( piece ) );
            content.Headers.ContentType = new MediaTypeHeaderValue( "application/json" );
            HttpResponseMessage response = client.PostAsync( url, content ).Result;

            if( response.IsSuccessStatusCode ) {
                string jsonContent = response.Content.ReadAsStringAsync().Result;
                PieceDto pieceDto = jss.Deserialize<PieceDto>( jsonContent );
                return RedirectToAction( "Details", new {
                    pieceDto = getViewPiece( pieceDto )
                } );
            }

            return RedirectToAction( "Error" );
        }


        // GET: Pieces/Edit/5
        public ActionResult Edit( int id )
        {
            // TODO
            return RedirectToAction( "Details", new {
                id = id
            } );
        }

        // POST: Pieces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( [Bind( Include = "pieceId,pieceName,pieceDescription,length,width,height,piecePrice,formId" )] Piece piece )
        {
            string url = "PiecesData/UpdatePiece/" + piece.pieceId;
            HttpResponseMessage response = doPiecePostRequest( url, piece );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to update " + piece.pieceName;
                return View();
            }

            return RedirectToAction( "Details", new {
                id = piece.pieceId
            } );
        }

        // GET: Pieces/Delete/5
        public ActionResult Delete( int id )
        {
            ViewPiece viewPiece = getViewPiece( id );
            return View( viewPiece );
        }

        // POST: Pieces/Delete/5
        [HttpPost, ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed( int id )
        {
            string url = "PiecesData/DeletePiece/" + id;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            return RedirectToAction( "Index" );
        }
    }
}
