using OfficeDevPnP.Core.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Utilities
{
    /// <summary>
    /// PnP http client which implements setting of User-Agent + retry mechanismn on throttling
    /// </summary>
    public class PnPHttpProvider : HttpClient
    {
        readonly int retryCount;
        readonly int delay;
        private string userAgent;

        /// <summary>
        /// Constructor without HttpMessageHandler
        /// </summary>
        /// <param name="retryCount">Number of retries, defaults to 10</param>
        /// <param name="delay">Incremental delay increase in milliseconds</param>
        /// <param name="userAgent">User-Agent string to set</param>
        public PnPHttpProvider(int retryCount = 10, int delay = 500, string userAgent = null) : this(new HttpClientHandler(), retryCount, delay, userAgent)
        {
        }

        /// <summary>
        /// Constructor with HttpMessageHandler
        /// </summary>
        /// <param name="innerHandler">HttpMessageHandler instance to pass along</param>
        /// <param name="retryCount">Number of retries, defaults to 10</param>
        /// <param name="delay">Incremental delay increase in milliseconds</param>
        /// <param name="userAgent">User-Agent string to set</param>
        public PnPHttpProvider(HttpMessageHandler innerHandler, int retryCount = 10, int delay = 500, string userAgent = null) : this(innerHandler, false, retryCount, delay, userAgent)
        {
        }

        /// <summary>
        /// Constructor with HttpMessageHandler
        /// </summary>
        /// <param name="innerHandler">HttpMessageHandler instance to pass along</param>
        /// <param name="retryCount">Number of retries, defaults to 10</param>
        /// <param name="delay">Incremental delay increase in milliseconds</param>
        /// <param name="userAgent">User-Agent string to set</param>
        /// <param name="disposeHandler">Declares whether to automatically dispose the internal HttpHandler instance</param>
        public PnPHttpProvider(HttpMessageHandler innerHandler, bool disposeHandler, int retryCount = 10, int delay = 500, string userAgent = null) : base(innerHandler, disposeHandler)
        {
            this.retryCount = retryCount;
            this.delay = delay;
            this.userAgent = userAgent;

            // Use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        }

        /// <summary>
        /// Perform async http request
        /// </summary>
        /// <param name="request">Http request to execute</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Response object from http request</returns>
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Retry logic variables
            int retryAttempts = 0;
            int backoffInterval = this.delay;

            HttpRequestMessage workrequest = request;

            // Loop until we need to retry
            while (retryAttempts < this.retryCount)
            {
                try
                {
                    // Add the PnP User Agent string
                    workrequest.Headers.UserAgent.TryParseAdd(string.IsNullOrEmpty(userAgent) ? $"{PnPCoreUtilities.PnPCoreUserAgent}" : userAgent);

                    // Make the request
                    Task<HttpResponseMessage> result = base.SendAsync(workrequest, cancellationToken);

                    if (result != null && result.Result != null && (result.Result.StatusCode == (HttpStatusCode)429 || result.Result.StatusCode == (HttpStatusCode)503))
                    {
                        // And return the response in case of success
                        Log.Warning(Constants.LOGGING_SOURCE, CoreResources.GraphExtensions_SendAsyncRetry, $"{backoffInterval}");

                        //Add delay for retry
                        Task.Delay(backoffInterval).Wait();

                        //Add to retry count and increase delay.
                        retryAttempts++;
                        backoffInterval = backoffInterval * 2;

                        workrequest = workrequest.CloneRequest();
                    }
                    else
                    {
                        return result;
                    }
                }
                // Or handle any ServiceException
                catch (Exception ex)
                {
                    // Check if the is an InnerException
                    // And if it is a WebException
                    var wex = ex.InnerException as WebException;
                    if (wex != null)
                    {
                        var response = wex.Response as HttpWebResponse;
                        // Check if request was throttled - http status code 429
                        // Check is request failed due to server unavailable - http status code 503
                        if (response != null &&
                            (response.StatusCode == (HttpStatusCode)429 || response.StatusCode == (HttpStatusCode)503))
                        {
                            Log.Warning(Constants.LOGGING_SOURCE, CoreResources.GraphExtensions_SendAsyncRetry, backoffInterval);

                            //Add delay for retry
                            Task.Delay(backoffInterval).Wait();

                            //Add to retry count and increase delay.
                            retryAttempts++;
                            backoffInterval = backoffInterval * 2;
                            workrequest = workrequest.CloneRequest();
                        }
                        else
                        {
                            Log.Error(Constants.LOGGING_SOURCE, CoreResources.GraphExtensions_SendAsyncRetryException, wex.ToString());
                            throw;
                        }
                    }
                    throw;
                }
            }

            throw new Microsoft.SharePoint.Client.ClientContextExtensions.MaximumRetryAttemptedException($"Maximum retry attempts {this.retryCount}, has be attempted.");
        }
    }

    //Reference: https://stackoverflow.com/questions/18000583/re-send-httprequestmessage-exception/18014515#18014515
    internal static class PnPHttpRequestCloneExtension
    {
        public static HttpRequestMessage CloneRequest(this HttpRequestMessage request)
        {
            var clone = new HttpRequestMessage(request.Method, request.RequestUri)
            {
                Content = request.Content.CloneRequest(),
                Version = request.Version
            };
            foreach (KeyValuePair<string, object> prop in request.Properties)
            {
                clone.Properties.Add(prop);
            }
            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
            {
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return clone;
        }

        public static HttpContent CloneRequest(this HttpContent content)
        {
            if (content == null) return null;

            var ms = new MemoryStream();
            content.CopyToAsync(ms).ConfigureAwait(false).GetAwaiter().GetResult();
            ms.Position = 0;

            var clone = new StreamContent(ms);
            foreach (KeyValuePair<string, IEnumerable<string>> header in content.Headers)
            {
                clone.Headers.Add(header.Key, header.Value);
            }
            return clone;
        }
    }
}
