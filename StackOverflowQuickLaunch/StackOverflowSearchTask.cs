using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Aberus.StackOverflowQuickLaunch
{
    class StackOverflowSearchTask : VsSearchTask 
    {
        readonly StackOverflowSearchProvider searchProvider;

        public StackOverflowSearchTask(StackOverflowSearchProvider provider, uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchProviderCallback pSearchCallback) 
            : base(dwCookie, pSearchQuery, pSearchCallback)
        {
            this.searchProvider = provider;
        }

        /// <summary>
        /// Override to start the search
        /// </summary>
        protected async override void OnStartSearch()
        {
            var sortQuery = "relevance";
            var sort = StackOverflowQuickLaunchPackage.Instance.Sort;
            if (sort != null && sort.HasValue)
                sortQuery = sort.Value.ToString().ToLowerInvariant();
            //// Get the tokens count in the query
            //uint tokenCount = this.SearchQuery.GetTokens(0, null);
            //// Get the tokens
            //IVsSearchToken[] tokens = new IVsSearchToken[tokenCount];
            //this.SearchQuery.GetTokens(tokenCount, tokens);
            var cancellationSource = new CancellationTokenSource();
            var searchResult = (StackOverflowSearchResult)null;

            var client = new HttpClient(
                new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });
            
            using (var response = await client.GetAsync("http://api.stackexchange.com/2.2/search/excerpts?order=desc&sort=" + sortQuery + "&site=stackoverflow&q=" + WebUtility.UrlEncode(SearchQuery.SearchString.Trim()), cancellationSource.Token))
            using (var receiveStream = await response.Content.ReadAsStreamAsync())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(StackOverflowSearchResult));
                searchResult = serializer.ReadObject(receiveStream) as StackOverflowSearchResult;
            }

            // Check if the search was canceled
            if (this.TaskStatus == VSConstants.VsSearchTaskStatus.Stopped)
            {
                if (!cancellationSource.IsCancellationRequested)
                {
                    cancellationSource.Cancel();
                    if (cancellationSource != null)
                        cancellationSource.Dispose();
                }
                // The completion was already notified by the base.OnStopSearch, there is nothing else to do
                return;
            }

            if (searchResult != null && searchResult.Items.Count != 0)
            {
                var results = searchResult.Items.Take(100).ToArray();

                // Since we know how many items we have, we can report progress
                for (int itemIndex = 0; itemIndex < results.Length; itemIndex++)
                {
                    var itemResult = new StackOverflowSearchItemResult(
                        (results[itemIndex].ItemType == ItemType.Question ? "Q: " : "A: ") + WebUtility.HtmlDecode(results[itemIndex].Title),
                        FormatExcerpt(WebUtility.HtmlDecode(results[itemIndex].Excerpt)).Trim(),
                        "http://stackoverflow.com/questions/" + results[itemIndex].QuestionId,
                        new WinFormsIconUIObject(Resources.StackOverflow),
                        searchProvider);

                    // Create and report new result
                    SearchCallback.ReportResult(this, itemResult);

                    // Keep track of how many results we have found, and the base class will use this number when calling the callback to report completion
                    SearchResults++;

                    // Since we know how many items we have, we can report progress
                    SearchCallback.ReportProgress(this, (uint) (itemIndex + 1), (uint) results.Length);
                }
            }
            else
            {
                // Create and report new result
                SearchCallback.ReportResult(this, 
                    new StackOverflowSearchItemResult("Search Stack Overflow for '" + SearchQuery.SearchString + "'", null,
                        "http://stackoverflow.com/search?q=" + WebUtility.UrlEncode(SearchQuery.SearchString.Trim()),
                        null,
                        searchProvider));

                // Since we know how many items we have, we can report progress
                SearchCallback.ReportComplete(this, 1);
            }

            // Now call the base class - it will set the task status to complete and will callback to report search complete
            base.OnStartSearch();
        }

        private string FormatExcerpt(string excerpt)
        {
            var removedNewLines = Regex.Replace(excerpt, @"(\t|\n|\r|\s){1,}", " ");
            string pattern = "<span class=\"highlight\">|</span>";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.Replace(removedNewLines, String.Empty);
        }

        protected new IVsSearchProviderCallback SearchCallback
        {
            get { return (IVsSearchProviderCallback) base.SearchCallback; }
        }

        // No need to override OnStopSearch in this case, we'll check the task status to see if the search was canceled
    }
}
