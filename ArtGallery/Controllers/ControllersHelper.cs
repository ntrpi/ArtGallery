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
    // This class brings all the interactions with the data controllers into one class, reducing code
    // duplication and complexity.
    public class ControllersHelper
    {
        public JavaScriptSerializer jss = new JavaScriptSerializer();
        public static readonly HttpClient client;

        static ControllersHelper()
        {
            HttpClientHandler handler = new HttpClientHandler() {
                AllowAutoRedirect = false
            };
            client = new HttpClient( handler );

            // Change this to match your own local port number.
            client.BaseAddress = new Uri( "https://localhost:44397/api/" );

            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue( "application/json" ) );
        }

        /// <summary>
        /// Use the client member to perform a GET request on a given url.
        /// </summary>
        /// <param name="url">A string of the url for the GET request.</param>
        /// <returns>An HttpResponseMessage object containing the result of the request.</returns>
        public HttpResponseMessage doGetRequest( string url )
        {
            HttpResponseMessage response = client.GetAsync( url ).Result;
            return response;
        }

        /// <summary>
        /// Use the client member to perform a POST request. Use the JSON 
        /// serializer member to convert the obj parameter into JSON format
        /// to standardize the processing by the controller receiving the request.
        /// </summary>
        /// <param name="url">A string of the url for the POST request.</param>
        /// <param name="obj">An object to be put into JSON format to be processed by the receiving controller</param>
        /// <returns>An HttpResponseMessage object containing the result of the request.</returns>
        public HttpResponseMessage doPostRequest( string url, Object obj )
        {
            HttpContent content = new StringContent( jss.Serialize( obj ) );
            Debug.WriteLine( jss.Serialize( obj ) );
            content.Headers.ContentType = new MediaTypeHeaderValue( "application/json" );
            HttpResponseMessage response = client.PostAsync( url, content ).Result;
            return response;
        }

        /// <summary>
        /// Use the client member to perform a POST request. 
        /// </summary>
        /// <param name="url">A string of the url for the POST request.</param>
        /// <param name="requestcontent">A MultipartFormDataContent object, likely containing data from an uploaded file.</param>
        /// <returns>An HttpResponseMessage object containing the result of the request.</returns>
        public HttpResponseMessage doMultiPartPostRequest( string url, MultipartFormDataContent requestcontent )
        {
            HttpResponseMessage response = client.PostAsync( url, requestcontent ).Result;
            return response;
        }


        /// <summary>
        /// Retrieve the content from the response parameter and use the serializer to convert
        /// it into a FormDto object.
        /// </summary>
        /// <param name="response">An HttpResponseMessage that contains the result of a GET request for a FormDto.</param>
        /// <returns>If the response indicates a successful GET request, return a FormDto object. Otherwise return null.</returns>
        public FormDto getFormDto( HttpResponseMessage response )
        {
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            string jsonContent = response.Content.ReadAsStringAsync().Result;
            FormDto formDto = jss.Deserialize<FormDto>( jsonContent );
            return formDto;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for a Form with the given id. 
        /// </summary>
        /// <param name="formId">An integer holding the value of the requested Form's id.</param>
        /// <returns>If the response indicates a successful GET request, return a FormDto object. Otherwise return null.</returns>
        public FormDto getFormDto( int formId )
        {
            HttpResponseMessage response = doGetRequest( "FormsData/GetFormDto/" + formId );
            return getFormDto( response );
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for all the Forms in the database.
        /// </summary>
        /// <returns>An IEnumerable of type FormDto. If the GET request was unsuccessful or there are no Forms in the database, the IEnumerable will be empty.</returns>
        public IEnumerable<FormDto> getFormDtos()
        {
            string url = "FormsData/GetFormDtos";
            HttpResponseMessage response = doGetRequest( url );
            if( !response.IsSuccessStatusCode ) {
                return new List<FormDto>();
            }
            IEnumerable<FormDto> formDtos = response.Content.ReadAsAsync<IEnumerable<FormDto>>().Result;
            return formDtos;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for a Piece with the given id. 
        /// </summary>
        /// <param name="pieceId">An integer holding the value of the requested Piece's id.</param>
        /// <returns>If the response indicates a successful GET request, return a PieceDto object. Otherwise return null.</returns>
        public PieceDto getPieceDto( int pieceId )
        {
            HttpResponseMessage response = doGetRequest( "PiecesData/GetPieceDto/" + pieceId );
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            PieceDto pieceDto = response.Content.ReadAsAsync<PieceDto>().Result;
            return pieceDto;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for all the Pieces of a given Form in the database.
        /// </summary>
        /// <param name="formId">An integer holding the value of the requested Form's id.</param>
        /// <returns>An IEnumerable of type PieceDto. If the GET request was unsuccessful or there are no Pieces with that Form in the database, the IEnumerable will be empty.</returns>
        public IEnumerable<PieceDto> getPieceDtosForForm( int formId )
        {
            HttpResponseMessage response = doGetRequest( "PiecesData/GetPieceDtosForForm/" + formId );
            if( !response.IsSuccessStatusCode ) {
                return new List<PieceDto>();
            }
            IEnumerable<PieceDto> pieceDtos = response.Content.ReadAsAsync<IEnumerable<PieceDto>>().Result;
            return pieceDtos;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for the last Piece of a given Form entered in the database.
        /// </summary>
        /// <param name="formId">An integer holding the value of the requested Form's id.</param>
        /// <returns>If the GET request is successful and a Piece with the given form exists in the database, return a PieceDto. Otherwise, return null.</returns>
        public PieceDto getLatestPieceDtoForForm( int formId )
        {
            IEnumerable<PieceDto> pieceDtos = getPieceDtosForForm( formId );
            PieceDto pieceDto = null;
            foreach( var piece in pieceDtos ) {
                if( pieceDto == null ) {
                    pieceDto = piece;
                    continue;
                }
                if( piece.pieceId > pieceDto.pieceId ) {
                    pieceDto = piece;
                }
            }
            if( pieceDto == null ) {
                pieceDto = PieceDto.getEmptyPieceDto( formId );
            }
            return pieceDto;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for all the Pieces in the database.
        /// </summary>
        /// <returns>An IEnumerable of type PieceDto. If the GET request was unsuccessful or there are no Pieces in the database, the IEnumerable will be empty.</returns>
        public IEnumerable<PieceDto> getPieceDtos()
        {
            string url = "PiecesData/GetPieceDtos";
            HttpResponseMessage response = doGetRequest( url );
            if( !response.IsSuccessStatusCode ) {
                return new List<PieceDto>();
            }
            IEnumerable<PieceDto> pieceDtos = response.Content.ReadAsAsync<IEnumerable<PieceDto>>().Result;
            return pieceDtos;
        }

        /// Retrieve the content from the response parameter and use the serializer to convert
        /// it into an ImageDto object.
        /// </summary>
        /// <param name="response">An HttpResponseMessage that contains the result of a GET request for an ImageDto.</param>
        /// <returns>If the response indicates a successful GET request, return an ImageDto object. Otherwise return null.</returns>
        public ImageDto getImageDto( HttpResponseMessage response )
        {
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            string jsonContent = response.Content.ReadAsStringAsync().Result;
            ImageDto imageDto = jss.Deserialize<ImageDto>( jsonContent );
            return imageDto;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for the primary Image for the piece with the given id. 
        /// </summary>
        /// <param name="pieceId">An integer holding the value of the requested Piece's id.</param>
        /// <returns>If the response indicates a successful GET request, return an ImageDto object. Otherwise return null.</returns>
        /// <summary>
        public ImageDto getPrimaryImageDtoForPiece( int pieceId )
        {
            if( pieceId == 0 ) {
                return null;
            }
            IEnumerable<ImageDto> imagesForPiece = getImageDtos( pieceId );
            ImageDto primaryImage = null;
            foreach( var image in imagesForPiece ) {
                if( primaryImage == null ) {
                    primaryImage = image;
                    continue;
                }
                if( image.isMainImage ) {
                    primaryImage = image;
                    break;
                }
            }
            return primaryImage;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data for an Image with the given id. 
        /// </summary>
        /// <param name="imageId">An integer holding the value of the requested Image's id.</param>
        /// <returns>If the response indicates a successful GET request, return an ImageDto object. Otherwise return null.</returns>
        /// <summary>
        public ImageDto getImageDto( int imageId )
        {
            string url = "ImagesData/GetImageDto/" + imageId;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            return getImageDto( response );
        }


        /// <summary>
        /// Perform a GET request to retrieve the data for either all the Images in the database or the Images for a given Piece.
        /// </summary>
        /// <param name="pieceId">An optional integer parameter holding the requested Piece's id. A value of "0" is used to indicate that all Images should be retrieved.</param>
        /// <returns>An IEnumerable of type ImageDto. If the GET request was unsuccessful or there are no corresponding Images in the database, the IEnumerable will be empty.</returns>
        public IEnumerable<ImageDto> getImageDtos( int pieceId = 0 )
        {
            string url = "ImagesData/GetImageDtos";
            if( pieceId != 0 ) {
                url += "ForPiece/" + pieceId;
            }
            HttpResponseMessage response = doGetRequest( url );
            if( !response.IsSuccessStatusCode ) {
                return new List<ImageDto>();
            }
            IEnumerable<ImageDto> imageDtos = response.Content.ReadAsAsync<IEnumerable<ImageDto>>().Result;
            return imageDtos;
        }

        /// <summary>
        /// Perform a GET request to retrieve the data required to construct a ViewPiece object for the given PieceDto.
        /// </summary>
        /// <param name="pieceDto">The Piece for which the ViewPiece object is requested.</param>
        /// <returns>A ViewPiece object. The member values may be null if the GET requests were unsuccessful or there are no such members in the database.</returns>
        public ViewPiece getViewPiece( PieceDto pieceDto )
        {
            ViewPiece viewPiece = new ViewPiece();
            viewPiece.pieceDto = pieceDto;
            if( pieceDto.formId != null ) {
                viewPiece.formDto = getFormDto( (int) pieceDto.formId );
            }
            return viewPiece;
        }

        /// <summary>
        /// Perform required GET requests to retrieve the data required to construct a ViewPiece model for the given Piece.
        /// </summary>
        /// <param name="pieceId">An integer holding the requested Piece's id.</param>
        /// <returns>A ViewPiece object. The member values may be null if the GET requests were unsuccessful or there are no such members in the database.</returns>
        public ViewPiece getViewPiece( int pieceId )
        {
            PieceDto pieceDto = getPieceDto( pieceId );
            return getViewPiece( pieceDto );
        }

        /// <summary>
        /// Perform required GET requests to retrieve the data required to construct a ViewPieceWithImages model for the given Piece.
        /// </summary>
        /// <param name="pieceId">An integer holding the requested Piece's id.</param>
        /// <returns>A ViewPiece object. The member values may be null if the GET requests were unsuccessful or there are no such members in the database.</returns>
        public ViewPieceWithImages getViewPieceWithImages( int pieceId )
        {
            ViewPieceWithImages viewPieceWithImages = new ViewPieceWithImages();
            viewPieceWithImages.pieceDto = getPieceDto( pieceId );
            viewPieceWithImages.formDto = getFormDto( (int) viewPieceWithImages.pieceDto.formId );
            viewPieceWithImages.imageDtos = getImageDtos( pieceId );
            return viewPieceWithImages;
        }


        /// <summary>
        /// Perform a GET request to retrieve the data required to construct an UpdatePiece object for the given PieceDto.
        /// </summary>
        /// <param name="pieceDto">The Piece for which the UpdatePiece object is requested.</param>
        /// <returns>An UpdatePiece object. The member values may be null if the GET requests were unsuccessful or there are no such members in the database.</returns>
        public UpdatePiece getUpdatePiece( PieceDto pieceDto )
        {
            UpdatePiece viewPiece = new UpdatePiece();
            viewPiece.piece = pieceDto;
            viewPiece.forms = getFormDtos();
            return viewPiece;
        }

        /// <summary>
        /// Perform required GET requests to retrieve the data required to construct an UpdatePiece object for the given Piece.
        /// </summary>
        /// <param name="pieceId">An integer holding the requested Piece's id.</param>
        /// <returns>An UpdatePiece object. The member values may be null if the GET requests were unsuccessful or there are no such members in the database.</returns>
        public UpdatePiece getUpdatePiece( int pieceId )
        {
            PieceDto pieceDto = getPieceDto( pieceId );
            return getUpdatePiece( pieceDto );
        }

        /// <summary>
        /// Perform required GET requests to retrieve the data required to construct an UpdateImage object for an Image of a given Piece.
        /// </summary>
        /// <param name="pieceId">An integer holding the requested Piece's id.</param>
        /// <param name="imageId">An optional integer parameter holding the id of a specific Image. A value of "0" is used to indicate that no Image is required.</param>
        /// <returns>An UpdateImage object. The member values may be null if the GET requests were unsuccessful or there are no such members in the database, or if the imageId was 0.</returns>
        public UpdateImage getUpdateImage( int pieceId, int imageId = 0 )
        {
            UpdateImage updateImage = new UpdateImage();
            if( imageId != 0 ) {
                updateImage.imageDto = getImageDto( imageId );
                updateImage.pieceDto = getPieceDto( updateImage.imageDto.pieceId );
            } else if( pieceId != 0 ) {
                updateImage.pieceDto = getPieceDto( pieceId );
            }
            return updateImage;
        }

        /// <summary>
        /// Perform required GET requests to retrieve the data required to construct a ViewImage object for the given Image.
        /// </summary>
        /// <param name="imageId">An integer holding the requested Image's id.</param>
        /// <returns>A ViewImage object. The member values may be null if the GET requests were unsuccessful or there are no such members in the database.</returns>
        public ViewImage getViewImage( int imageId )
        {
            ImageDto imageDto = getImageDto( imageId );
            ViewImage viewImage = new ViewImage {
                imageDto = imageDto,
                pieceDto = getPieceDto( imageDto.pieceId )
            };
            return viewImage;
        }
    }
}