using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using ArtGallery.Models;
using ArtGallery.Models.ViewModels;

namespace ArtGallery.Controllers
{
    public class ImagesController : Controller
    {
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly ControllersHelper helper;

        static ImagesController()
        {
            helper = new ControllersHelper();
        }

        // GET: Images
        public ActionResult Index()
        {
            IEnumerable<ImageDto> imageDtos = helper.getImageDtos();
            return View( imageDtos );
        }

        // GET: Images/Details/5
        public ActionResult Details( int id )
        {
            return View( helper.getViewImage( id ) );
        }

        public ActionResult Create( int id )
        {
            UpdateImage updateImage = helper.getUpdateImage( id );
            return View( updateImage );
        }

        // POST: Image/Create
        // id == pieceId
        [HttpPost]
        public ActionResult Create( int id, HttpPostedFileBase imageData )
        {
            Debug.WriteLine( "Received image for piece id " + id );

            string url = "ImagesData/CreateImage/" + id;

            MultipartFormDataContent requestcontent = new MultipartFormDataContent();
            HttpContent imagecontent = null;
            try {
                imagecontent = new StreamContent( imageData.InputStream );
            } catch( Exception e ) {
                Debug.WriteLine( e );
                UpdateImage updateImage = helper.getUpdateImage( id );
                ViewBag.errorMessage = "Please choose an image for " + updateImage.pieceDto.pieceName + ".";
                return View( updateImage );
            }

            requestcontent.Add( imagecontent, "pieceImage", imageData.FileName );
            HttpResponseMessage response = helper.doMultiPartPostRequest( url, requestcontent );
            if( !response.IsSuccessStatusCode ) {
                UpdateImage updateImage = helper.getUpdateImage( id );
                ViewBag.errorMessage = "Unable to add image for " + updateImage.pieceDto.pieceName + ".";
                return View( updateImage );
            }

            try {
                int imageId = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction( "Details", new {
                    id = imageId
                } );
            } catch( Exception e ) {
                Debug.WriteLine( e );
                return RedirectToAction( "Index" );
            }
        }

        // GET: Images/Edit/5
        public ActionResult Edit( int id )
        {
            return View( helper.getUpdateImage( 0, id ) );
        }

        // GET: Images/Edit/5
        public ActionResult View( int id )
        {
            return View( helper.getUpdateImage( 0, id ) );
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( [Bind( Include = "imageId,imageName,imageOldName,imageExt,isMainImage,pieceId" )] Image image )
        {
            string url = "ImagesData/UpdateImage/" + image.imageId;
            HttpResponseMessage response = helper.doPostRequest( url, image );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to update image.";
                return View( helper.getUpdateImage( 0, image.imageId ) );
            }
            return RedirectToAction( "Details", new {
                id = image.imageId
            } );
        }

        // GET: Image/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm( int id )
        {
            return View( helper.getViewImage( id ) );
        }

        // POST: Image/Delete/5
        [HttpPost]
        public ActionResult Delete( int id, FormCollection collection )
        {
            string url = "ImagesData/DeleteImage/" + id;
            HttpResponseMessage response = helper.doPostRequest( url, "" );

            return RedirectToAction( "Index" );
        }
    }
}
