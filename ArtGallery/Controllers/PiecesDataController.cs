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
    public class PiecesDataController: ApiController
    {
        private ArtGalleryDbContext db = new ArtGalleryDbContext();

        /// <summary>
        /// A utility function to create an PieceDto object with the data from a Piece object.
        /// </summary>
        /// <param name="piece">A Piece object.</param>
        /// <returns>An PieceDto object containing the information in the piece.</returns>
        private PieceDto getPieceDtoFromPiece( Piece piece )
        {
            PieceDto pieceDto = new PieceDto {
                pieceId = piece.pieceId,
                pieceName = piece.pieceName,
                pieceDescription = piece.pieceDescription,
                height = piece.height,
                length = piece.length,
                width = piece.width,
                piecePrice = piece.piecePrice,
                formId = piece.formId
            };
            return pieceDto;
        }

        /// <summary>
        /// Get an PieceDto created with the information in the Piece in the database with the given id.
        /// </summary>
        /// <param name="id">An integer representing the id of the required Piece.</param>
        /// <returns>An PieceDto object.</returns>
        /// <example>
        /// GET: api/PiecesData/GetPieceDto/5
        /// </example>
        [ResponseType( typeof( PieceDto ) )]
        [HttpGet]
        public IHttpActionResult GetPieceDto( int id )
        {
            Piece piece = db.pieces.Find( id );
            if( piece == null ) {
                return NotFound();
            }

            return Ok( getPieceDtoFromPiece( piece ) );
        }

        public int GetLatestPieceId()
        {
            return db.pieces.Max( p => p.pieceId );
        }

        /// <summary>
        /// We only want to send back PieceDto objects, so this is a private utility function
        /// that retrieves all the Pieces from the database, which will be converted to PieceDtos.
        /// </summary>
        /// <returns>A List of Piece objects.</returns>
        private IEnumerable<Piece> getPieces()
        {
            List<Piece> pieces = db.pieces.ToList();
            return pieces;
        }

        /// <summary>
        /// Get only the Pieces that have the given formId value.
        /// </summary>
        /// <param name="pieceId">The id of the Piece foreign key.</param>
        /// <returns>A collection of Piece objects.</returns>
        private IEnumerable<Piece> getPiecesForForm( int formId )
        {
            List<Piece> pieces = db.pieces.Where( p => p.formId == formId ).ToList();
            return pieces;
        }

        /// <summary>
        /// A private utility function to convert a collection of Piece objects to PieceDtos.
        /// </summary>
        /// <param name="forms">A collection of Piece objects.</param>
        /// <returns>A collection of PieceDto objects created with the data in the Pieces that were passed in.</returns>
        private IEnumerable<PieceDto> getPieceDtos( IEnumerable<Piece> pieces )
        {
            List<PieceDto> pieceDtos = new List<PieceDto>();
            foreach( var piece in pieces ) {
                pieceDtos.Add( getPieceDtoFromPiece( piece ) );
            }
            return pieceDtos;
        }

        /// <summary>
        /// Get a collection of PieceDto objects that represent all the Pieces in the database.
        /// </summary>
        /// <returns>A collection of PieceDto objects.</returns>
        /// <example>
        /// GET: api/PiecesData/GetPieceDtos
        /// </example>
        [HttpGet]
        public IEnumerable<PieceDto> GetPieceDtos()
        {
            IEnumerable<Piece> pieces = getPieces();
            return getPieceDtos( pieces );
        }

        /// <summary>
        /// Get a collection of PieceDto objects that have the given formId.
        /// </summary>
        /// <param name="id">The id of the Form foreign key.</param>
        /// <returns>A collection of PieceDto objects.</returns>
        /// <example>
        /// GET: api/PiecesData/GetPieceDtosForForm/5
        /// </example>
        [HttpGet]
        public IEnumerable<PieceDto> GetPieceDtosForForm( int id ) // id == formId
        {
            IEnumerable<Piece> pieces = getPiecesForForm( id );
            return getPieceDtos( pieces );
        }

        /// <summary>
        /// Updates a Piece in the database given information about the form.
        /// </summary>
        /// <param name="id">The form id.</param>
        /// <param name="piece">A Piece object, received as POST data.</param>
        /// <returns>If the update is successful, a NoContent status code is returned. 
        /// Otherwise, a NotFound ActionResult is returned.</returns>
        /// <example>
        /// POST: api/PiecesData/UpdatePiece/5
        /// FORM DATA: Piece JSON Object
        [ResponseType( typeof( void ) )]
        [HttpPost]
        public IHttpActionResult UpdatePiece( int id, [FromBody] Piece piece )
        {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            if( id != piece.pieceId ) {
                return BadRequest();
            }

            db.Entry( piece ).State = EntityState.Modified;

            try {
                db.SaveChanges();
            } catch( DbUpdateConcurrencyException ) {
                if( !PieceExists( id ) ) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return StatusCode( HttpStatusCode.NoContent );
        }

        /// <summary>
        /// Creates a Piece in the database given information about the form.
        /// </summary>
        /// <param name="piece">A Piece object, received as POST data.</param>
        /// <returns>If the creation is successful, an ActionResult with the id of the form is returned. 
        /// Otherwise, a BadRequest ActionResult is returned.</returns>
        /// <example>
        /// POST: api/PiecesData/CreatePiece
        /// FORM DATA: Piece JSON Object
        /// </example>
        [ResponseType( typeof( int ) )]
        [HttpPost]
        public IHttpActionResult CreatePiece( [FromBody] Piece piece )
        {
            if( !ModelState.IsValid ) {
                return BadRequest( ModelState );
            }

            db.pieces.Add( piece );
            db.SaveChanges();

            return CreatedAtRoute( "DefaultApi", new {
                id = piece.pieceId
            }, piece );
        }

        /// <summary>
        /// Deletes the Piece in the database with the given id.
        /// </summary>
        /// <param name="id">The piece id.</param>
        /// <returns>If the update is successful, an Ok status code is returned. 
        /// Otherwise, a NotFound ActionResult is returned.</returns>
        /// <example>
        /// DELETE: api/PiecesData/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeletePiece( int id )
        {
            Piece piece = db.pieces.Find( id );
            if( piece == null ) {
                return NotFound();
            }

            db.pieces.Remove( piece );
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

        private bool PieceExists( int id )
        {
            return db.pieces.Count( e => e.pieceId == id ) > 0;
        }
    }
}