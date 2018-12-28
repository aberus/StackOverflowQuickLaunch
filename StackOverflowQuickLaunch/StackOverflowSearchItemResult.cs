using Microsoft.VisualStudio.Shell.Interop;

namespace Aberus.StackOverflowQuickLaunch
{
    class StackOverflowSearchItemResult : IVsSearchItemResult
    {
        public StackOverflowSearchItemResult(string name, string description, string url, IVsUIObject icon, IVsSearchProvider provider)
        {
            DisplayText = name;
            Url = url;
            Description = description;
            Icon = icon;
            PersistenceData = name + "|" + url + "|" + description;
            SearchProvider = provider;
        }

        public string Url
        {
            get;
            protected set;
        }

        #region IVsSearchItemResult

        public string Description
        { 
            get; 
            protected set; 
        }

        public string DisplayText 
        { 
            get; 
            protected set; 
        }

        public IVsUIObject Icon 
        { 
            get; 
            protected set; 
        }

        // This function is called when the user selects the item result from the Quick Launch popup
        public void InvokeAction()
        {
            Browser.Open(Url, 
                StackOverflowQuickLaunchPackage.Instance.OptionPage.OpenInInternalBrowser, 
                StackOverflowQuickLaunchPackage.Instance.OptionPage.OpenInNewTab);
        }

        public string PersistenceData { get; protected set; }
        //public string PersistenceData
        //{
        //    get
        //    {
        //        // This is used for the MRU list.  We need to be able to fully recreate the result data.
        //        return String.Join(Separator,
        //                            EscapePersistenceString(this.DisplayText),
        //                            EscapePersistenceString(this.Url),
        //                            EscapePersistenceString(this.Tooltip));
        //    }
        //}

        public IVsSearchProvider SearchProvider 
        { 
            get; 
            private set; 
        }

        public string Tooltip
        {
            get;
            private set;
        }

        #endregion IVsSearchItemResult
    }
}
