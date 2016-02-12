using System.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace Aberus.StackOverflowQuickLaunch
{
    class StackOverflowSearchItemResult : IVsSearchItemResult
    {
        private string Url;

        public StackOverflowSearchItemResult(string name, string description, string url, IVsUIObject icon, IVsSearchProvider provider)
        {
            DisplayText = name;
            Url = url;
            Icon = icon;
            PersistenceData = name + "|" + Url;
            Description = description;
            SearchProvider = provider;
        }

        #region IVsSearchItemResult

        public Microsoft.VisualStudio.OLE.Interop.IDataObject DataObject
        {
            get { return null; }
        }

        public string Description  { get; protected set; }

        public string DisplayText { get; protected set; }

        public IVsUIObject Icon { get; protected set; }
/*        {
            get 
            {
                if (item.Icon == null)
                    return null;

                // If all items returned from the search provider use the same icon, consider using a static member variable 
                // (e.g. on the search provider class) to initialize and return the IVsUIObject - it will save time and memory 
                // creating these objects.

                // Helper classses in Microsoft.Internal.VisualStudio.PlatformUI can be used to construct IVsUIObject of VsUIType.Icon
                // Use Win32IconUIObject if you have a HICON, use WinFormsIconUIObject if you have a System.Drawing.Icon, or
                // use WpfPropertyValue.CreateIconObject() if you have a WPF ImageSource.
                return new WinFormsIconUIObject(item.Icon);
            }
        }*/

        public void InvokeAction()
        {
            Process.Start(Url);
            // This function is called when the user selects the item result from the Quick Launch popup
            //System.Windows.Forms.MessageBox.Show( string.Format(Resources.SearchProviderResultInvoked, this.Item.Name));
        }

        public string PersistenceData { get; protected set; }

        public IVsSearchProvider SearchProvider { get; private set; }

        public string Tooltip
        {
            get { return null; }
        }

        #endregion IVsSearchItemResult
    }
}
