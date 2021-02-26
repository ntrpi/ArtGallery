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

        private FormDto getFormDtoFromForm( Form form )
        {
            FormDto formDto = new FormDto {
                formId = form.formId,
                formName = form.formName,
            };
            return formDto;
        }

        // GET: api/FormsData/5
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

        private IEnumerable<Form> getForms()
        {
            List<Form> forms = db.forms.ToList();
            return forms;
        }

        private IEnumerable<FormDto> getFormDtos( IEnumerable<Form> forms )
        {
            List<FormDto> formDtos = new List<FormDto>();
            foreach( var form in forms ) {
                formDtos.Add( getFormDtoFromForm( form ) );
            }
            return formDtos;
        }

        // GET: api/FormsData
        [HttpGet]
        public IEnumerable<FormDto> GetFormDtos()
        {
            IEnumerable<Form> forms = getForms();
            return getFormDtos( forms );
        }

        // PUT: api/FormsData/5
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

        // POST: api/FormsData
        [ResponseType( typeof( Form ) )]
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

        // DELETE: api/FormsData/5
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