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
        private static readonly ControllersHelper helper;

        static FormsController()
        {
            helper = new ControllersHelper();
        }


        // GET: Forms
        public ActionResult Index()
        {
            string url = "FormsData/GetFormDtos";
            HttpResponseMessage response = helper.doGetRequest( url );
            if( !response.IsSuccessStatusCode ) {
                return View( new List<FormDto>() );
            }
            IEnumerable<FormDto> formDtos = response.Content.ReadAsAsync<IEnumerable<FormDto>>().Result;
            return View( helper.getFormDtos() );
        }

        // GET: Forms/Details/5
        public ActionResult Details(int id)
        {
            FormDto formDto = helper.getFormDto( id );
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

        // POST: Forms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Form form )
        {
            string url = "FormsData/CreateForm";
            HttpResponseMessage response = helper.doPostRequest( url, form );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to add form.";
                return View();
            }

            FormDto formDto = helper.getFormDto( response );
            return RedirectToAction( "Index" );
        }

        // GET: Forms/Edit/5
        public ActionResult Edit(int id)
        {
            return View( helper.getFormDto( id ) );
        }

        // POST: Forms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "formId,formName")] Form form)
        {
            string url = "FormsData/UpdateForm/" + form.formId;
            HttpResponseMessage response = helper.doPostRequest( url, form );
            if( !response.IsSuccessStatusCode ) {
                ViewBag.errorMessage = "Unable to update form.";
                return View();
            }

            return RedirectToAction( "Details", new {
                id = form.formId
            } );
        }

        // GET: Form/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm( int id )
        {
            FormDto formDto = helper.getFormDto( id );
            return View( formDto );
        }

        // POST: Form/Delete/5
        [HttpPost]
        public ActionResult Delete( int id, FormCollection collection )
        {
            string url = "FormsData/DeleteForm/" + id;
            HttpResponseMessage response = helper.doPostRequest( url, "" );

            return RedirectToAction( "Index" );
        }
    }
}
