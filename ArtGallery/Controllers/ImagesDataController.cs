using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Http.Description;
using System.Diagnostics;
using System.Web.Script.Serialization;
using ArtGallery.Models;

namespace ArtGallery.Controllers
{
    public class ImagesDataController : ApiController
    {
        private ArtGalleryDbContext db = new ArtGalleryDbContext();

        private ImageDto getImageDtoFromImage( Image image )
        {
            ImageDto imageDto = new ImageDto {
                imageId = image.imageId,
                imageName = image.imageName,
                imageExt = image.imageExt,
                isMainImage = image.isMainImage,
                pieceId = image.pieceId,
                imagesPath = Image.imagesPath
            };
            if( image.imageOldName == null ) {
                imageDto.imageOldName = image.imageName;
            }
            return imageDto;
        }

        // GET: api/ImagesData/5
        [ResponseType( typeof( ImageDto ) )]
        [HttpGet]
        public IHttpActionResult GetImageDto( int id )
        {
            Image image = db.images.Find( id );
            if( image == null ) {
                return NotFound();
            }

            return Ok( getImageDtoFromImage( image ) );
        }

        private IEnumerable<Image> getImages()
        {
            List<Image> images = db.images.ToList();
            return images;
        }

        private IEnumerable<Image> getImagesForPiece( int pieceId )
        {
            List<Image> images = db.images.Where( i => i.pieceId == pieceId ).ToList();
            return images;
        }

        private IEnumerable<ImageDto> getImageDtos( IEnumerable<Image> images )
        {
            List<ImageDto> imageDtos = new List<ImageDto>();
            foreach( var image in images ) {
                imageDtos.Add( getImageDtoFromImage( image ) );
            }
            return imageDtos;
        }

        // GET: api/ImagesData
        [HttpGet]
        public IEnumerable<ImageDto> GetImageDtos()
        {
            IEnumerable<Image> images = getImages();
            return getImageDtos( images );
        }

        [HttpGet]
        public IEnumerable<ImageDto> GetImageDtosForPiece( int id ) // id == pieceId
        {
            IEnumerable<Image> images = getImagesForPiece( id );
            return getImageDtos( images );
        }

        private string getImageLocalPath( string imageName, string imageExt )
        {
            string imagePath = Path.Combine( HttpContext.Current.Server.MapPath( "~/Content/Images/" ), imageName + '.' + imageExt );
            return imagePath;
        }

        // PUT: api/ImagesData/5
        [HttpPost]
        public IHttpActionResult UpdateImage( int id, [FromBody] Image image )
        {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            if( id != image.imageId ) {
                return BadRequest();
            }

            string imageOldName = image.imageOldName;
            image.imageOldName = image.imageName;

            try {
                db.Entry( image ).State = EntityState.Modified;
                db.SaveChanges();

                // Update the local file.
                string oldPath = getImageLocalPath( imageOldName, image.imageExt );
                string newPath = getImageLocalPath( image.imageName, image.imageExt );
                System.IO.File.Move( oldPath, newPath );

            } catch( DbUpdateConcurrencyException ) {
                if( !ImageExists( id ) ) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return Ok( id );
        }

        // POST: api/ImagesData/CreateImage
        // FORM DATA: Image JSON Object
        // id == pieceId
        [HttpPost]
        public IHttpActionResult CreateImage( int id )
        {
            // Check that the HttpContext contains more than one part.
            if( !Request.Content.IsMimeMultipartContent() ) {
                return new TextResult( "Missing image file.", Request );
            }
            Debug.WriteLine( "Received multipart image data." );

            //Check if a file is posted
            int numfiles = HttpContext.Current.Request.Files.Count;
            if( numfiles != 1 || HttpContext.Current.Request.Files[ 0 ] == null ) {
                return new TextResult( "No file posted.", Request );
            }
            Debug.WriteLine( "Files Received: " + numfiles );

            var imageFile = HttpContext.Current.Request.Files[ 0 ];
            // Check if the file is empty.
            if( imageFile.ContentLength == 0 ) {
                return new TextResult( "File is emtpy.", Request );
            }

            // Create an image object.
            Image image = new Image();
            image.pieceId = id;

            // Check file extension.
            var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
            image.imageExt = Path.GetExtension( imageFile.FileName ).Substring( 1 );
            if( !valtypes.Contains( image.imageExt ) ) {
                return new TextResult( "File does not have an image extension.", Request );
            }

            try {
                // Create a file name.
                image.imageName = "pieceId_" + image.pieceId + "_"
                    + Stopwatch.GetTimestamp();

                // Create a complete file path.
                string imagePath = getImageLocalPath( image.imageName, image.imageExt );

                // Save the image.
                imageFile.SaveAs( imagePath );

            } catch( Exception e ) {
                Debug.WriteLine( "Image was not saved successfully." );
                Debug.WriteLine( "Exception:" + e );

                return new TextResult( "Unable to save file.", Request );
            }

            //Will Validate according to data annotations specified on model
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            db.images.Add( image );
            db.SaveChanges();

            // Not using this right now, but leaving it because I plan to.
            return Ok( image.imageId );
        }

        // DELETE: api/ImagesData/5
        [HttpPost]
        public IHttpActionResult DeleteImage( int id )
        {
            Image image = db.images.Find( id );
            if( image == null ) {
                return NotFound();
            }

            db.images.Remove( image );
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImageExists(int id)
        {
            return db.images.Count(e => e.imageId == id) > 0;
        }
    }
}