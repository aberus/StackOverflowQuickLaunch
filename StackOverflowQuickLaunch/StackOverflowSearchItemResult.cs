using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;

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

        public void InvokeAction()
        {
            // This function is called when the user selects the item result from the Quick Launch popup
            if (StackOverflowQuickLaunchPackage.Instance.OptionPage.OpenInInternalBrowser)
            {
                var navigateOptions = StackOverflowQuickLaunchPackage.Instance.OptionPage.OpenInNewTab ? 
                    vsNavigateOptions.vsNavigateOptionsNewWindow : vsNavigateOptions.vsNavigateOptionsDefault;

                var dte = StackOverflowQuickLaunchPackage.GetGlobalService(typeof(DTE)) as DTE;
                dte.ItemOperations.Navigate(Url, navigateOptions);
            }
            else
            {
                System.Diagnostics.Process.Start(Url);
            }
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
