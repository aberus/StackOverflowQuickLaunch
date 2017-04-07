using System;
using System.Runtime.InteropServices;
using Microsoft.Internal.VisualStudio.PlatformUI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Aberus.StackOverflowQuickLaunch
{
    // A global search provider declared statically in registry needs the following:
    // 1) a class implementing IVsSearchProvider interface
    // 2) the provider class specifying a Guid attribute of the search provider (provider_identifier)
    // 3) the provider class type declared on the Package-derived class using ProvideSearchProvider attribute
    // 4) the package must derive from ExtensionPointPackage for automatic extenstion creation.
    //    An alternate solution is for the package to implement IVsPackageExtensionProvider and create the search provider when CreateExtensionPoint(typeof(IVsSearchProvider).GUID, provider_identifier) is called.
    //
    // Declare the search provider guid, to be used during registration 
    // and during provider's automatic creation as an extension point
    [Guid(GuidList.guidStackOverflowSearchProviderString)]
    internal sealed class StackOverflowSearchProvider : IVsSearchProvider
    {
        public Guid Category
        {
            get 
            { 
                return new Guid(GuidList.guidStackOverflowSearchProviderString); 
            }
        }

        public string Description
        {
            get 
            { 
                return Resources.SearchProviderDescription; 
            }
        }

        public string DisplayText
        {
            get 
            { 
                return Resources.SearchProviderDisplayText; 
            }
        }

        public void ProvideSearchSettings(IVsUIDataSource pSearchOptions)
        {
            // Let's mark the results from this provider non-cacheable. 
            // Providers with non-cacheable results will be called on CreateItemResult() by the MostRecentlyExecuted provider
            // to re-create recent items in order to verify if they still match new search queries.
            // For performance reason though it's better to use the default setting and let the MRE provider cache the strings
            // of the item result and do check if it matches the new queries. 

            // Whenever we need to change the settings (e.g. set SearchResultsCacheable = false), 
            // we need to do it through IVsUIDataSource members, or by using helpers/utilities classes.
            // We cannot cast the pSearchOption to SearchProviderSettingsDataSource and use its members directly unless 
            // the extension always targets only the latest version of Visual Studio. 
            //Utilities.SetValue(pSearchOptions, SearchProviderSettingsDataSource.PropertyNames.SearchResultsCacheable, false);
        }

        public string Shortcut
        {
            get 
            { 
                return "stack"; 
            }
        }

        public string Tooltip
        {
            get 
            { 
                return null; 
            }
        }

        public IVsSearchItemResult CreateItemResult(string lpszPersistenceData)
        {
            string[] data = lpszPersistenceData.Split('|');
            if (data.Length != 3)
                return null;

            string name = data[0];
            string url = data[1];
            string description = data[2];
            if (string.IsNullOrWhiteSpace(url))
                return null;

            return new StackOverflowSearchItemResult(name, description, url, new WinFormsIconUIObject(Resources.StackOverflow), this);
        }

        public IVsSearchTask CreateSearch(uint dwCookie, IVsSearchQuery pSearchQuery, IVsSearchProviderCallback pSearchCallback)
        {
            if (dwCookie == VSConstants.VSCOOKIE_NIL)
                return null;
            
            return new StackOverflowSearchTask(this, dwCookie, pSearchQuery, pSearchCallback);
        }
    }
}
