using System;
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
            int pageSize = 40;
            bool alwaysShowLink = false;

            var options = StackOverflowQuickLaunchPackage.Instance.OptionPage;
            if (options != null)
            {
                sortQuery = options.Sort.ToString().ToLowerInvariant();
                pageSize = options.ShowResults;
                alwaysShowLink = options.AlwayShowLink;
            }

            //// Get the tokens count in the query
            //uint tokenCount = SearchQuery.GetTokens(0, null);
            //// Get the tokens
            //IVsSearchToken[] tokens = new IVsSearchToken[tokenCount];
            //SearchQuery.GetTokens(tokenCount, tokens);
            
            var cancellationSource = new CancellationTokenSource();
            var searchResult = (StackOverflowSearchResult)null;
            try
            {
               using(var client = new HttpClient(
                    new HttpClientHandler
                    {
                        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                    }
                ))
                using (var response = await client.GetAsync("https://api.stackexchange.com/2.2/search/excerpts?order=desc&pagesize=" + pageSize + "&sort=" + sortQuery + "&site=stackoverflow&q=" + WebUtility.UrlEncode(SearchQuery.SearchString.Trim()), cancellationSource.Token))
                using (var receiveStream = await response.Content.ReadAsStreamAsync())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(StackOverflowSearchResult));
                    searchResult = serializer.ReadObject(receiveStream) as StackOverflowSearchResult;
                }
            }
            catch(Exception ex)
            {
                this.ErrorCode = ex.HResult;
                this.SetTaskStatus(VSConstants.VsSearchTaskStatus.Error);
                this.SearchCallback.ReportComplete(this, 0);

                return;
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

            if (searchResult.ErrorId.HasValue)
            {
                this.SetTaskStatus(VSConstants.VsSearchTaskStatus.Error);
                this.SearchCallback.ReportComplete(this, this.SearchResults);
                return;
            }

            bool anyResults = false;

            if (searchResult != null && searchResult.Items.Length != 0)
            {
                anyResults = true;
                var results = searchResult.Items.Take(pageSize).ToArray();

                // Since we know how many items we have, we can report progress
                for (int itemIndex = 0; itemIndex < results.Length; itemIndex++)
                {
                    var itemResult = new StackOverflowSearchItemResult(
                        (results[itemIndex].ItemType == ItemType.Question ? "Q: " : "A: ") + WebUtility.HtmlDecode(results[itemIndex].Title),
                        FormatExcerpt(WebUtility.HtmlDecode(results[itemIndex].Excerpt)).Trim(),
                        "https://stackoverflow.com/questions/" + results[itemIndex].QuestionId,
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
            
            if(!anyResults || alwaysShowLink)
            {
                // Create and report new result
                SearchCallback.ReportResult(this, 
                    new StackOverflowSearchItemResult($"Search Online on Stack Overflow for '{SearchQuery.SearchString}'",
                        string.Empty,
                        "https://stackoverflow.com/search?q=" + WebUtility.UrlEncode(SearchQuery.SearchString.Trim()),
                        null,
                        searchProvider));

                // Only one result
                SearchCallback.ReportComplete(this, 1);
            }

            // Now call the base class - it will set the task status to complete and will callback to report search complete
            base.OnStartSearch();
        }

        private string FormatExcerpt(string excerpt)
        {
            var highlightRegex = new Regex("<span class=\"highlight\">|</span>", RegexOptions.IgnoreCase);
            var removedNewLines = Regex.Replace(excerpt, @"(\t|\n|\r|\s){1,}", " ");
            return highlightRegex.Replace(removedNewLines, string.Empty);
        }

        protected new IVsSearchProviderCallback SearchCallback
        {
            get 
            { 
                return (IVsSearchProviderCallback) base.SearchCallback; 
            }
        }

        // No need to override OnStopSearch in this case, we'll check the task status to see if the search was canceled
    }
}
