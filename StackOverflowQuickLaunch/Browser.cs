using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Aberus.StackOverflowQuickLaunch
{
    public static class Browser
    {
        public static void Open(string url, bool openInInternalBrowser, bool newWindow = false)
        {
            if(openInInternalBrowser)
            {
                uint navigateOptions = newWindow ? (uint)__VSWBNAVIGATEFLAGS.VSNWB_ForceNew : 0u;

                var webBrowsingService = Package.GetGlobalService(typeof(IVsWebBrowsingService)) as IVsWebBrowsingService;
                webBrowsingService.Navigate(url, navigateOptions, out IVsWindowFrame ppFrame);
            }
            else
            {
                Process.Start(url);
            }
        }
    }
}
