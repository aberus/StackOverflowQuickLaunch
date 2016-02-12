using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Aberus.StackOverflowQuickLaunch;

namespace TestSerializer
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = "I&#39;m using\necho date(&#39;H:i:s&#39;).&quot; this step time\\n&quot;;\n in order to know how much time needs for each function in order to be executed.  \n\nHow can I know the time with <span class=\"highlight\">microseconds</span> also?\n &hellip;";
            var excerpt = WebUtility.HtmlDecode(a);
            var removedNewLines = Regex.Replace(excerpt, @"(\t|\n|\r|\s){1,}", " ");

            string pattern = "<span class=\"highlight\">|</span>";
             var rx = new Regex(pattern, RegexOptions.IgnoreCase);
             var e = rx.Replace(removedNewLines, String.Empty);




            var cancellationSource = new CancellationTokenSource();
            var searchResult = (StackOverflowSearchResult)null;

            var client = new HttpClient(
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });

            using (var response = client.GetAsync("http://api.stackexchange.com/2.2/search/excerpts?order=desc&sort=relevance&site=stackoverflow&q=" + WebUtility.UrlEncode("microseconds")).Result)
            using (var receiveStream = response.Content.ReadAsStreamAsync().Result)
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(StackOverflowSearchResult)
                    //,
                    //new DataContractJsonSerializerSettings
                    //{
                    //    DateTimeFormat = new DateTimeFormat("e", new EpochDateFormatProvider()),
                    //}
                    );

                searchResult = serializer.ReadObject(receiveStream) as StackOverflowSearchResult;
            }
        }
    }
}
