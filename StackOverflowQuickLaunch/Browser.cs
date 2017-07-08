using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace Aberus.StackOverflowQuickLaunch
{
    public static class Browser
    {
        public static void Open(string url, bool openInInternalBrowser, bool newWindow = false)
        {
            if(openInInternalBrowser)
            {
                var navigateOptions = newWindow ? vsNavigateOptions.vsNavigateOptionsNewWindow : vsNavigateOptions.vsNavigateOptionsDefault;

                var dte = StackOverflowQuickLaunchPackage.GetGlobalService(typeof(DTE)) as DTE;
                dte.ItemOperations.Navigate(url, navigateOptions);
            }
            else
            {
                System.Diagnostics.Process.Start(url);
            }
        }
    }
}
