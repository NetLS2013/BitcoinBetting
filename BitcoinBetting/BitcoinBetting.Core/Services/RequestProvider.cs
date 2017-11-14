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
            using (var httpClient = CreateHttpClient())
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

                return result;
            }
        }

        public async Task<TResult> PostAsync<TData, TResult>(string uri, TData data, List<KeyValuePair<string, string>> cookies = null)
        {
            using (var httpClient = CreateHttpClient(cookies))
            {
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                try
                {
                    response = await httpClient.PostAsync(uri, content);
                }
                catch (Exception e)
                {
                    throw new HttpRequestException("Network error");
                }

                await HandleResponse(response);

                string serialized = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);

                return result;
            }
        }

        public async Task DeleteAsync(string uri)
        {
            using (var httpClient = CreateHttpClient())
            {
                try
                {
                    await httpClient.DeleteAsync(uri);
                }
                catch (Exception e)
                {
                    throw new HttpRequestException("Network error");
                }
            }
        }

        private HttpClient CreateHttpClient(List<KeyValuePair<string, string>> cookies = null)
        {
            HttpClient httpClient = null;
            if (cookies != null)
            {
                CookieContainer cookieContainer = new CookieContainer();

                foreach (var cookie in cookies)
                {
                    cookieContainer.Add(new Cookie(cookie.Key, cookie.Value) { Domain = new Uri(GlobalSetting.Instance.BaseEndpoint).Host });
                }

                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                httpClient = new HttpClient(handler);
            }
            else
            {
                httpClient = new HttpClient();
            }

            if (!string.IsNullOrWhiteSpace(GlobalSetting.Instance.AuthToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalSetting.Instance.AuthToken);
            }

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
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
