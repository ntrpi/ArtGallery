using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ArtGallery.Models;

namespace ArtGallery.Controllers
{
    public class FormsDataController: ApiController
    {
        private ArtGalleryDbContext db = new ArtGalleryDbContext();

        /// <summary>
        /// A utility function to create a FormDto object with the data from a Form object.
        /// </summary>
        /// <param name="form">A Form object.</param>
        /// <returns>A FormDto object containing the information in form.</returns>
        private FormDto getFormDtoFromForm( Form form )
        {
            FormDto formDto = new FormDto {
                formId = form.formId,
                formName = form.formName,
                formDescription = form.formDescription
            };
            return formDto;
        }

        /// <summary>
        /// Get a FormDto created with the information in the Form in the database with the given id.
        /// </summary>
        /// <param name="id">An integer representing the id of the required Form.</param>
        /// <returns>A FormDto object.</returns>
        /// <example>
        /// GET: api/FormsData/5
        /// </example>
        [ResponseType( typeof( FormDto ) )]
        [HttpGet]
        public IHttpActionResult GetFormDto( int id )
        {
            Form form = db.forms.Find( id );
            if( form == null ) {
                return NotFound();
            }

            return Ok( getFormDtoFromForm( form ) );
        }

        /// <summary>
        /// We only want to send back FormDto objects, so this is a private utility function
        /// that retrieves all the Forms from the database, which will be converted to FormDtos.
        /// </summary>
        /// <returns>A List of Form objects.</returns>
        private IEnumerable<Form> getForms()
        {
            List<Form> forms = db.forms.ToList();
            return forms;
        }

        /// <summary>
        /// A private utility function to convert a collection of Form objects to FormDtos.
        /// </summary>
        /// <param name="forms">A collection of Form objects.</param>
        /// <returns>A collection of FormDto objects created with the data in the Forms that were passed in.</returns>
        private IEnumerable<FormDto> getFormDtos( IEnumerable<Form> forms )
        {
            List<FormDto> formDtos = new List<FormDto>();
            foreach( var form in forms ) {
                formDtos.Add( getFormDtoFromForm( form ) );
            }
            return formDtos;
        }

        /// <summary>
        /// Get a collection of FormDto objects that represent all the Forms in the database.
        /// </summary>
        /// <returns>A collection of FormDto objects.</returns>
        /// <example>
        /// GET: api/FormsData
        /// </example>
        [HttpGet]
        public IEnumerable<FormDto> GetFormDtos()
        {
            IEnumerable<Form> forms = getForms();
            return getFormDtos( forms );
        }

        /// <summary>
        /// Updates a Form in the database given information about the form.
        /// </summary>
        /// <param name="id">The form id.</param>
        /// <param name="form">A Form object, received as POST data.</param>
        /// <returns>If the update is successful, a NoContent status code is returned. 
        /// Otherwise, a NotFound ActionResult is returned.</returns>
        /// <example>
        /// POST: api/FormsData/UpdateForm/5
        /// FORM DATA: Form JSON Object
        /// </example>
        [ResponseType( typeof( void ) )]
        [HttpPost]
        public IHttpActionResult UpdateForm( int id, [FromBody] Form form )
        {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            if( id != form.formId ) {
                return BadRequest();
            }

            db.Entry( form ).State = EntityState.Modified;

            try {
                db.SaveChanges();
            } catch( DbUpdateConcurrencyException ) {
                if( !FormExists( id ) ) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return StatusCode( HttpStatusCode.NoContent );
        }

        /// <summary>
        /// Creates a Form in the database given information about the form.
        /// </summary>
        /// <param name="form">A Form object, received as POST data.</param>
        /// <returns>If the creation is successful, an ActionResult with the id of the form is returned. 
        /// Otherwise, a BadRequest ActionResult is returned.</returns>
        /// <example>
        /// POST: api/FormsData/CreateForm
        /// FORM DATA: Form JSON Object
        /// </example>
        [ResponseType( typeof( int ) )]
        [HttpPost]
        public IHttpActionResult CreateForm( [FromBody] Form form )
        {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            db.forms.Add( form );
            db.SaveChanges();

            return CreatedAtRoute( "DefaultApi", new {
                id = form.formId
            }, form );
        }

        /// <summary>
        /// Deletes the Form in the database with the given id.
        /// </summary>
        /// <param name="id">The form id.</param>
        /// <returns>If the update is successful, an Ok status code is returned. 
        /// Otherwise, a NotFound ActionResult is returned.</returns>
        /// <example>
        /// DELETE: api/FormsData/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteForm( int id )
        {
            Form form = db.forms.Find( id );
            if( form == null ) {
                return NotFound();
            }

            db.forms.Remove( form );
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose( bool disposing )
        {
            if( disposing ) {
                db.Dispose();
            }
            base.Dispose( disposing );
        }

        private bool FormExists( int id )
        {
            return db.forms.Count( e => e.formId == id ) > 0;
        }
    }
}