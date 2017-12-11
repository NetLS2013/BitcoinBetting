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
using BitcoinBetting.Core.Models;
using BitcoinBetting.Core.Views.Account;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Services
{
    public class RequestProvider : IRequestProvider
    {
        private readonly JsonSerializerSettings serializerSettings;

        CookieContainer cookieContainer;

        public RequestProvider()
        {
            serializerSettings = new JsonSerializerSettings
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
                    throw new HttpRequestException("Network error", e);
                }
                
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var resultRefresh = await RefreshTokenAsync();

                    if (resultRefresh)
                    {
                        return await GetAsync<TResult>(uri);
                    }
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
                    throw new HttpRequestException("Network error", e);
                }
                
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var resultRefresh = await RefreshTokenAsync();

                    if (resultRefresh)
                    {
                        return await PostAsync<TData, TResult>(uri, data, cookies);
                    }
                }

                await HandleResponse(response);

                string serialized = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<TResult>(serialized, serializerSettings);

                return result;
            }
        }
        
        public async Task PostAsync(string uri)
        {
            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage response = null;
                
                try
                {
                    response = await httpClient.PostAsync(uri, null);
                }
                catch (Exception e)
                {
                    throw new HttpRequestException("Network error", e);
                }
                
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var resultRefresh = await RefreshTokenAsync();

                    if (resultRefresh)
                    {
                        await PostAsync(uri);
                    }
                }
                
                await HandleResponse(response);
            }
        }

        public async Task DeleteAsync(string uri)
        {
            using (var httpClient = CreateHttpClient())
            {
                HttpResponseMessage response = null;
                
                try
                {
                    response = await httpClient.DeleteAsync(uri);
                }
                catch (Exception e)
                {
                    throw new HttpRequestException("Network error", e);
                }
                
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var resultRefresh = await RefreshTokenAsync();

                    if (resultRefresh)
                    {
                        await DeleteAsync(uri);
                    }
                }
                
                await HandleResponse(response);
            }
        }
        
        private async Task<bool> RefreshTokenAsync()
        {
            var result = await PostAsync<RefreshTokenModel, ResultModel>(GlobalSetting.Instance.RefreshTokenEndpoint, 
                new RefreshTokenModel { RefreshToken = (string) Application.Current.Properties["refresh_token"]});

            if (!result.Result)
            {
                Device.BeginInvokeOnMainThread(() => {
                    Application.Current.MainPage = new NavigationPage(new LoginPage());
                });
            }
            else
            {
                Application.Current.Properties["token"] = result.Token;
                Application.Current.Properties["refresh_token"] = result.RefreshToken;
            }

            return result.Result;
        }

        private HttpClient CreateHttpClient(List<KeyValuePair<string, string>> cookies = null)
        {
            HttpClient httpClient = null;
            
            if (cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    cookieContainer.Add(new Cookie(cookie.Key, cookie.Value) { Domain = new Uri(GlobalSetting.Instance.BaseEndpoint).Host });
                }

                httpClient = new HttpClient(new HttpClientHandler { CookieContainer = cookieContainer });
            }
            else
            {
                httpClient = new HttpClient();
            }

            if (Application.Current.Properties.ContainsKey("token"))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", (string) Application.Current.Properties["token"]);
            }

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("device_id", (string) Application.Current.Properties["device_id"]);
            
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
