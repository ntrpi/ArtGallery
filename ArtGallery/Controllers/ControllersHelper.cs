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

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);
        }

        public HttpResponseMessage doGetRequest( string url )
        {
            HttpResponseMessage response = client.GetAsync( url ).Result;
            return response;
        }

        public HttpResponseMessage doPostRequest( string url, Object obj )
        {
            HttpContent content = new StringContent( jss.Serialize( obj ) );
            Debug.WriteLine( jss.Serialize( obj ) );
            content.Headers.ContentType = new MediaTypeHeaderValue( "application/json" );
            HttpResponseMessage response = client.PostAsync( url, content ).Result;
            return response;
        }

        public HttpResponseMessage doMultiPartPostRequest( string url, MultipartFormDataContent requestcontent )
        {
            HttpResponseMessage response = client.PostAsync( url, requestcontent ).Result;
            return response;
        }

        
        public FormDto getFormDto( HttpResponseMessage response )
        {
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            string jsonContent = response.Content.ReadAsStringAsync().Result;
            FormDto formDto = jss.Deserialize<FormDto>( jsonContent );
            return formDto;
        }

        public FormDto getFormDto( int formId )
        {
            HttpResponseMessage response = doGetRequest( "FormsData/GetFormDto/" + formId );
            return getFormDto( response );
        }

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

        public PieceDto getPieceDto( int id )
        {
            HttpResponseMessage response = doGetRequest( "PiecesData/GetPieceDto/" + id );
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            PieceDto pieceDto = response.Content.ReadAsAsync<PieceDto>().Result;
            return pieceDto;
        }

        public IEnumerable<PieceDto> getPieceDtosForForm( int id )
        {
            HttpResponseMessage response = doGetRequest( "PiecesData/GetPieceDtosForForm/" + id );
            if( !response.IsSuccessStatusCode ) {
                return new List<PieceDto>();
            }
            IEnumerable<PieceDto> pieceDtos = response.Content.ReadAsAsync<IEnumerable<PieceDto>>().Result;
            return pieceDtos;
        }

        public PieceDto getLatestPieceDtoForForm( int id )
        {
            IEnumerable<PieceDto> pieceDtos = getPieceDtosForForm( id );
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
            return pieceDto;
        }

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

        public ImageDto getImageDto( HttpResponseMessage response )
        {
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            string jsonContent = response.Content.ReadAsStringAsync().Result;
            ImageDto imageDto = jss.Deserialize<ImageDto>( jsonContent );
            return imageDto;
        }

        public ImageDto getPrimaryImageDtoForPiece( int pieceId )
        {
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

        public ImageDto getImageDto( int imageId )
        {
            string url = "ImagesData/GetImageDto/" + imageId;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            return getImageDto( response );
        }

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

        public ViewPiece getViewPiece( PieceDto pieceDto )
        {
            ViewPiece viewPiece = new ViewPiece();
            viewPiece.pieceDto = pieceDto;
            if( pieceDto.formId != null ) {
                viewPiece.formDto = getFormDto( (int) pieceDto.formId );
            }
            return viewPiece;
        }
        public ViewPiece getViewPiece( int id )
        {
            PieceDto pieceDto = getPieceDto( id );
            return getViewPiece( pieceDto );
        }

        public UpdatePiece getUpdatePiece( PieceDto pieceDto )
        {
            UpdatePiece viewPiece = new UpdatePiece();
            viewPiece.piece = pieceDto;
            viewPiece.forms = getFormDtos();
            return viewPiece;
        }
        public UpdatePiece getUpdatePiece( int id )
        {
            PieceDto pieceDto = getPieceDto( id );
            return getUpdatePiece( pieceDto );
        }

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