using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace Aberus.StackOverflowQuickLaunch
{
    [Guid(GuidList.guidStackOverflowSearchProviderOptionString)]
    public class StackOverflowSearchOptionPage : DialogPage
    {
        Sort sort = Sort.Relevance;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Sort Sort
        {
            get
            {
                return sort;
            }
            set
            {
                if (value != sort)
                    sort = value;
            }
        }


        bool alwayShowLink = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AlwayShowLink
        {
            get
            {
                return alwayShowLink;
            }
            set
            {
                if (value != alwayShowLink)
                    alwayShowLink = value;
            }
        }

        int showResults = 40;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ShowResults
        {
            get
            {
                return showResults;
            }
            set
            {
                if (value != showResults)
                    showResults = value;
            }
        }

        bool openInInternalBrowser = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool OpenInInternalBrowser
        {
            get
            {
                return openInInternalBrowser;
            }
            set
            {
                if (value != openInInternalBrowser)
                    openInInternalBrowser = value;
            }
        }


        bool openInNewTab = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool OpenInNewTab
        {
            get
            {
                return openInNewTab;
            }
            set
            {
                if (value != openInNewTab)
                    openInNewTab = value;
            }
        }


        bool useGenericSearch = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseGenericSearch
        {
            get
            {
                return useGenericSearch;
            }
            set
            {
                if (value != useGenericSearch)
                    useGenericSearch = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override System.Windows.Forms.IWin32Window Window
        {
            get
            {
                var page = new StackOverflowSearchUserControl();
                page.OptionsPage = this;
                page.Initialize();
                return page;
            }
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }

    public enum Sort {
        Relevance = 1,
        [Description("Newest")]
        Creation = 2,
        [Description("Active")]
        Activity = 3,
        Votes = 4,
    }; 
}
