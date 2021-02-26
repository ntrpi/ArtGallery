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
    public class FormsController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        static FormsController()
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


        // GET: Forms
        public ActionResult Index()
        {
            string url = "FormsData/GetFormDtos";
            HttpResponseMessage response = client.GetAsync( url ).Result;
            if( !response.IsSuccessStatusCode ) {
                return View( new List<FormDto>() );
            }
            IEnumerable<FormDto> pieceDtos = response.Content.ReadAsAsync<IEnumerable<FormDto>>().Result;
            return View( pieceDtos );
        }

        private FormDto getFormDto( HttpResponseMessage response )
        {
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            string jsonContent = response.Content.ReadAsStringAsync().Result;
            FormDto formDto = jss.Deserialize<FormDto>( jsonContent );
            return formDto;
        }

        private FormDto getFormDto( int formId )
        {
            string url = "FormsData/GetFormDto/" + formId;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            return getFormDto( response );
        }

        // GET: Forms/Details/5
        public ActionResult Details(int id)
        {
            FormDto formDto = getFormDto( id );
            if( formDto == null ) {
                return HttpNotFound();
            }
            return View( formDto );
        }


        // GET: Forms/Create
        public ActionResult Create()
        {
            return View();
        }

        private HttpResponseMessage doFormPostRequest( string url, Form form )
        {
            HttpContent content = new StringContent( jss.Serialize( form ) );
            content.Headers.ContentType = new MediaTypeHeaderValue( "application/json" );
            HttpResponseMessage response = client.PostAsync( url, content ).Result;
            return response;
        }


        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Form form )
        {
            string url = "FormsData/CreateForm";
            HttpResponseMessage response = doFormPostRequest( url, form );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to add form.";
                return View();
            }

            FormDto formDto = getFormDto( response );
            return Index();
        }

        // GET: Forms/Edit/5
        public ActionResult Edit(int id)
        {
            return View( getFormDto( id ) );
        }

        // POST: Forms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "formId,formName")] Form form)
        {
            string url = "FormsData/UpdateForm";
            HttpResponseMessage response = doFormPostRequest( url, form );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to update form.";
                return View();
            }

            FormDto formDto = getFormDto( response );
            return RedirectToAction( "Details", new {
                formDto = formDto
            } );
        }

        // GET: Form/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm( int id )
        {
            FormDto formDto = getFormDto( id );
            return View( formDto );
        }

        // POST: Form/Delete/5
        [HttpPost]
        public ActionResult Delete( int id, FormCollection collection )
        {
            string url = "FormsData/DeleteForm/" + id;

            HttpContent content = new StringContent( "" );
            HttpResponseMessage response = client.PostAsync( url, content ).Result;

            return RedirectToAction( "Index" );
        }
    }
}
