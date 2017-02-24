using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using BigDays.Pixabay.Models;
using System.IO;

namespace BigDays.Pixabay
{
    public class PixabayService
    {
        //https://pixabay.com/api/docs/

        private string BASE_URL = "https://pixabay.com/api/?";
        private string UserName = "VasPasha";
        private string Key = "4616986-8f565151db57d59f5e06a4f06";
		private HttpClient httpClient;

        public PixabayService()
        {
			httpClient = GetHttpClient();
        }

		public async Task<PixabayResult> Init(string searchText )
        { 
			var result = await httpClient.GetStringAsync($"{BASE_URL}key={Key}&q={searchText}&response_group=image_details&image_type=photo&orientation=vertical&=min_width=100&min_height=120&safesearch=true&per_page=36");
           	var model = JsonConvert.DeserializeObject<PixabayResult>(result);
	        return model;
        }


        #region Helper methods    
        private HttpClient GetHttpClient(bool useHttps = false)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(BASE_URL),
                Timeout = TimeSpan.FromSeconds(25)
            };

            //// add language to header
            //httpClient.DefaultRequestHeaders.AcceptLanguage.Add( new StringWithQualityHeaderValue( Language.ToString() ) );          

            return httpClient;
        }

        #endregion


        private string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        

        public static object DeserializeFromStream(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize(jsonTextReader);
            }
        }
    }
}