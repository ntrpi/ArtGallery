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
            HttpResponseMessage response = helper.doGetRequest( "ImagesData/GetImageDtos" );
            if( !response.IsSuccessStatusCode ) {
                return View( new List<ImageDto>() );
            }
            IEnumerable<ImageDto> imageDtos = response.Content.ReadAsAsync<IEnumerable<ImageDto>>().Result;
            return View( imageDtos );
        }

        // GET: Images/Details/5
        public ActionResult Details( int id )
        {
            ImageDto imageDto = helper.getImageDto( id );
            if( imageDto == null ) {
                return HttpNotFound();
            }
            return View( imageDto );
        }

        public ActionResult Create( int id )
        {
            return View( helper.getPieceDto( id ) );
        }

        private HttpResponseMessage doImagePostRequest( string url, Image image )
        {
            HttpResponseMessage response = helper.doPostRequest( url, image );
            return response;
        }


        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Image image )
        {
            string url = "ImagesData/CreateImage";
            HttpResponseMessage response = doImagePostRequest( url, image );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to add image.";
                return View();
            }

            ImageDto imageDto = helper.getImageDto( response );
            return Index();
        }

        // GET: Images/Edit/5
        public ActionResult Edit( int id )
        {
            return View( helper.getImageDto( id ) );
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( [Bind( Include = "imageId,imageName" )] Image image )
        {
            string url = "ImagesData/UpdateImage";
            HttpResponseMessage response = doImagePostRequest( url, image );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to update image.";
                return View();
            }

            ImageDto imageDto = helper.getImageDto( response );
            return RedirectToAction( "Details", new {
                imageDto = imageDto
            } );
        }

        // GET: Image/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm( int id )
        {
            ImageDto imageDto = helper.getImageDto( id );
            return View( imageDto );
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
