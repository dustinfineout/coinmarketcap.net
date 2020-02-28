﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace CoinMarketCap.Client
{
    public abstract class ApiClientBase
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["CoinMarketCap.ApiKey"];

        private readonly bool _sandbox =
            ConfigurationManager.AppSettings["CoinMarketCap.Sandbox"]?.ToLowerInvariant().Equals("true") ?? false;

        private const string ApiBaseUrlPro = "https://pro-api.coinmarketcap.com/v1/";
        private const string ApiBaseUrlSandbox = "https://sandbox-api.coinmarketcap.com/v1/";

        private string ApiBaseUrl =>
            _sandbox
                ? ApiBaseUrlSandbox
                : ApiBaseUrlPro;

        protected T ApiRequest<T>(string endpoint, Dictionary<string, string> parameters) where T : class
        {           
            var url = new UriBuilder($"{ApiBaseUrl}{endpoint}");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var param in parameters
                .Where(param => !string.IsNullOrWhiteSpace(param.Value)))
            {
                queryString[param.Key] = param.Value;
            }
            url.Query = queryString.ToString();

            string responseJson;
            using (var client = new WebClient())
            {
                client.Headers.Add("X-CMC_PRO_API_KEY", _apiKey);
                client.Headers.Add("Accepts", "application/json");

                try
                {
                    responseJson = client.DownloadString(url.ToString());
                }
                catch (WebException ex)
                {
                    if (ex.Response == null)
                    {
                        throw;
                    }
                    using (var response = ex.Response)
                    {
                        var dataRs = response.GetResponseStream();
                        if (dataRs == null)
                        {
                            throw;
                        }

                        using (var reader = new StreamReader(dataRs))
                        {
                            responseJson = reader.ReadToEnd();
                        }
                    }
                }
            }
            
            return 
                string.IsNullOrWhiteSpace(responseJson)
                ? null
                : JsonConvert.DeserializeObject<T>(responseJson);;
        }
    }
}
