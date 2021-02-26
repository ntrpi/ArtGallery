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

        // GET: api/PiecesData/5
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

        private IEnumerable<Piece> getPieces()
        {
            List<Piece> pieces = db.pieces.ToList();
            return pieces;
        }

        private IEnumerable<Piece> getPiecesForForm( int formId )
        {
            List<Piece> pieces = db.pieces.Where( p => p.formId == formId ).ToList();
            return pieces;
        }

        private IEnumerable<PieceDto> getPieceDtos( IEnumerable<Piece> pieces )
        {
            List<PieceDto> pieceDtos = new List<PieceDto>();
            foreach( var piece in pieces ) {
                pieceDtos.Add( getPieceDtoFromPiece( piece ) );
            }
            return pieceDtos;
        }

        // GET: api/PiecesData
        [HttpGet]
        public IEnumerable<PieceDto> GetPieceDtos()
        {
            IEnumerable<Piece> pieces = getPieces();
            return getPieceDtos( pieces );
        }

        [HttpGet]
        public IEnumerable<PieceDto> GetPieceDtosForForm( int formId )
        {
            IEnumerable<Piece> pieces = getPiecesForForm( formId );
            return getPieceDtos( pieces );
        }

        // PUT: api/PiecesData/5
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

        // POST: api/PiecesData
        [ResponseType( typeof( Piece ) )]
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

        // DELETE: api/PiecesData/5
        [ResponseType( typeof( Piece ) )]
        public IHttpActionResult DeletePiece( int id )
        {
            Piece piece = db.pieces.Find( id );
            if( piece == null ) {
                return NotFound();
            }

            db.pieces.Remove( piece );
            db.SaveChanges();

            return Ok( piece );
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