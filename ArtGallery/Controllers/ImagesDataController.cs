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

        /// <summary>
        /// A utility function to create an ImageDto object with the data from an Image object.
        /// </summary>
        /// <param name="image">An Image object.</param>
        /// <returns>An ImageDto object containing the information in the image.</returns>
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

        /// <summary>
        /// Get an ImageDto created with the information in the Image in the database with the given id.
        /// </summary>
        /// <param name="id">An integer representing the id of the required Image.</param>
        /// <returns>An ImageDto object.</returns>
        /// <example>
        /// GET: api/ImagesData/GetImageDto/5
        /// </example>
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

        /// <summary>
        /// We only want to send back ImageDto objects, so this is a private utility function
        /// that retrieves all the Images from the database, which will be converted to ImageDtos.
        /// </summary>
        /// <returns>A List of Image objects.</returns>
        private IEnumerable<Image> getImages()
        {
            List<Image> images = db.images.ToList();
            return images;
        }

        /// <summary>
        /// Get only the Images that have the given pieceId value.
        /// </summary>
        /// <param name="pieceId">The id of the Piece foreign key.</param>
        /// <returns>A collection of Image objects.</returns>
        private IEnumerable<Image> getImagesForPiece( int pieceId )
        {
            List<Image> images = db.images.Where( i => i.pieceId == pieceId ).ToList();
            return images;
        }

        /// <summary>
        /// A private utility function to convert a collection of Image objects to ImageDtos.
        /// </summary>
        /// <param name="forms">A collection of Image objects.</param>
        /// <returns>A collection of ImageDto objects created with the data in the Images that were passed in.</returns>
        private IEnumerable<ImageDto> getImageDtos( IEnumerable<Image> images )
        {
            List<ImageDto> imageDtos = new List<ImageDto>();
            foreach( var image in images ) {
                imageDtos.Add( getImageDtoFromImage( image ) );
            }
            return imageDtos;
        }

        /// <summary>
        /// Get a collection of ImageDto objects that represent all the Images in the database.
        /// </summary>
        /// <returns>A collection of ImageDto objects.</returns>
        /// <example>
        /// GET: api/ImagesData/GetImageDtos
        /// </example>
        [HttpGet]
        public IEnumerable<ImageDto> GetImageDtos()
        {
            IEnumerable<Image> images = getImages();
            return getImageDtos( images );
        }


        /// <summary>
        /// Get a collection of ImageDto objects that have the given pieceId.
        /// </summary>
        /// <param name="id">The id of the Piece foreign key.</param>
        /// <returns>A collection of ImageDto objects.</returns>
        /// <example>
        /// GET: api/ImagesData/GetImageDtos/5
        /// </example>
        [HttpGet]
        public IEnumerable<ImageDto> GetImageDtosForPiece( int id ) // id == pieceId
        {
            IEnumerable<Image> images = getImagesForPiece( id );
            return getImageDtos( images );
        }

        /// <summary>
        /// Utility function to construct the local path of an image.
        /// </summary>
        /// <param name="imageName">The image file name.</param>
        /// <param name="imageExt">The image file extension.</param>
        /// <returns>The local path of the image.</returns>
        private string getImageLocalPath( string imageName, string imageExt )
        {
            string imagePath = Path.Combine( HttpContext.Current.Server.MapPath( "~/Content/Images/" ), imageName + '.' + imageExt );
            return imagePath;
        }

        /// <summary>
        /// Updates an Image in the database given information about the image.
        /// </summary>
        /// <param name="id">The image id.</param>
        /// <param name="image">An Image object, received as POST data.</param>
        /// <returns>If the update is successful, a NoContent status code is returned. 
        /// Otherwise, if there is a problem with the model, a BadRequest ActionResult is returned, or a NotFound ActionResult if the image was not updated.</returns>
        /// <example>
        /// POST: api/ImagesData/UpdateForm/5
        /// FORM DATA: Image JSON Object
        /// </example>
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

            // Try to update the database.
            try {
                db.Entry( image ).State = EntityState.Modified;
                db.SaveChanges();
            } catch( DbUpdateConcurrencyException e ) {
                Debug.WriteLine( e.Message );
                Debug.WriteLine( e );
                if( !ImageExists( id ) ) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            try {
                // Try to move the image file. Do not overwrite the file if it already exists.
                string oldPath = getImageLocalPath( imageOldName, image.imageExt );
                string newPath = getImageLocalPath( image.imageName, image.imageExt );
                File.Copy( oldPath, newPath, false );

            } catch( IOException e ) {
                Debug.WriteLine( e.Message );
                Debug.WriteLine( e );

                // If we couldn't move the file, we need to undo the changes to the database.
                image.imageOldName = imageOldName;
                // Try to update the database.
                try {
                    db.Entry( image ).State = EntityState.Modified;
                    db.SaveChanges();
                } catch( DbUpdateConcurrencyException duce ) {
                    Debug.WriteLine( duce.Message );
                    Debug.WriteLine( duce );
                    if( !ImageExists( id ) ) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return NotFound();
            }

            return Ok( id );
        }

        /// <summary>
        /// Creates an Image in the database given information about the image.
        /// </summary>
        /// <param name="id">The id of the Piece foreign key.</param>
        /// <param>The image file data as multipart content.</param>
        /// <returns>If the creation is successful, an ActionResult with the id of the form is returned. 
        /// Otherwise, a BadRequest ActionResult is returned.</returns>
        /// <example>
        /// POST: api/ImagesData/CreateImage/5
        /// FORM DATA: Image JSON Object
        /// </example>
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

        /// <summary>
        /// Deletes the Image in the database with the given id.
        /// </summary>
        /// <param name="id">The image id.</param>
        /// <returns>If the update is successful, an Ok status code is returned. 
        /// Otherwise, a NotFound ActionResult is returned.</returns>
        /// <example>
        /// DELETE: api/ImagesData/5
        /// </example>
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