using BitcoinBetting.Core.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinBetting.Core.Services
{
    public class RequestProvider : IRequestProvider
    {
        private readonly JsonSerializerSettings serializerSettings;

        CookieContainer cookieContainer;

        public RequestProvider()
        {
            this.serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            };

            serializerSettings.Converters.Add(new StringEnumConverter());

            cookieContainer = new CookieContainer();
        }

        public async Task<TResult> GetAsync<TResult>(string uri)
        {
            CookieContainer cookieContainer = new CookieContainer();

            using (var httpClient = CreateHttpClient(ref cookieContainer))
            {
                HttpResponseMessage response = null;

                try
                {
                    response = await httpClient.GetAsync(uri);
                }
                catch (Exception e)
                {
                    throw new HttpRequestException("Network error");
                }

                await HandleResponse(response);

                string serialized = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);
                this.GetCookie(cookieContainer);

                return result;
            }
        }

        public async Task<TResult> PostAsync<TData, TResult>(string uri, TData data)
        {
            CookieContainer cookieContainer = new CookieContainer();

            using (var httpClient = CreateHttpClient(ref cookieContainer))
            {
                var content = new StringContent(JsonConvert.SerializeObject(data));

                HttpResponseMessage response = null;

                try
                {
                    response = await httpClient.PostAsync(uri, content);
                }
                catch(Exception e)
                {
                    throw new HttpRequestException("Network error");
                }

                await HandleResponse(response);

                string serialized = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);
                this.GetCookie(cookieContainer);

                return result;
            }
        }

        public async Task DeleteAsync(string uri)
        {
            CookieContainer cookieContainer = new CookieContainer();

            using (var httpClient = CreateHttpClient(ref cookieContainer))
            {
                try
                {
                    await httpClient.DeleteAsync(uri);
                }
                catch (Exception e)
                {
                    throw new HttpRequestException("Network error");
                }

                this.GetCookie(cookieContainer);
            } 
        }

        private HttpClient CreateHttpClient(ref CookieContainer cookieContainer)
        {
            cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (string.IsNullOrWhiteSpace(GlobalSetting.Instance.AuthToken))
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(".AspNetCore.Identity.Application", GlobalSetting.Instance.AuthToken);
            }

            return httpClient;
        }

        private void GetCookie(CookieContainer cookieContainer)
        {
            IEnumerable<Cookie> responseCookies = cookieContainer.GetCookies(new Uri(GlobalSetting.Instance.RegisterEndpoint)).Cast<Cookie>();
            foreach (Cookie cookie in responseCookies)
            {
                if (cookie.Name == ".AspNetCore.Identity.Application")
                {
                    GlobalSetting.Instance.AuthToken = cookie.Value;
                }
            }
        }

        private async Task HandleResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpRequestException("Access denied");
                }

                throw new HttpRequestException("Server error, try again");
            }
        }
    }
}
