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

        public HttpResponseMessage doPostRequest( string url, Object obj )
        {
            HttpContent content = new StringContent( jss.Serialize( obj ) );
            content.Headers.ContentType = new MediaTypeHeaderValue( "application/json" );
            HttpResponseMessage response = client.PostAsync( url, content ).Result;
            return response;
        }

        public FormDto getFormDto( int formId )
        {
            string url = "FormsData/GetFormDto/" + formId;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            string jsonContent = response.Content.ReadAsStringAsync().Result;
            FormDto formDto = jss.Deserialize<FormDto>( jsonContent );
            return formDto;
        }

        public PieceDto getPieceDto( int id )
        {
            string url = "PiecesData/GetPieceDto/" + id;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            if( !response.IsSuccessStatusCode ) {
                return null;
            }

            PieceDto pieceDto = response.Content.ReadAsAsync<PieceDto>().Result;
            return pieceDto;
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

        public ImageDto getImageDto( int imageId )
        {
            string url = "ImagesData/GetImageDto/" + imageId;
            HttpResponseMessage response = client.GetAsync( url ).Result;
            return getImageDto( response );
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
    }
}