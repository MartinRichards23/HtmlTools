using System;
using System.Collections.Generic;
using System.Text;

namespace HtmlTools
{
    public static class NetTools
    {
        /// <summary>
        /// Determines if Url is a relative one
        /// </summary>
        public static bool IsAbsoluteUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }

        /// <summary>
        /// Gets an absolute Url from a relative one and its base url
        /// </summary>
        public static string MakeAbsoluteUrl(string baseUrl, string relativeUrl)
        {
            UriBuilder builder = new UriBuilder(baseUrl);
            return MakeAbsoluteUrl(builder.Uri, relativeUrl);
        }

        /// <summary>
        /// Gets an absolute Url from a relative one and its base url
        /// </summary>
        public static string MakeAbsoluteUrl(Uri baseUri, string relativeUrl)
        {
            if (IsAbsoluteUrl(relativeUrl))
                return relativeUrl;

            Uri frameUri = new Uri(baseUri, relativeUrl);
            return frameUri.ToString();
        }
    }
}
