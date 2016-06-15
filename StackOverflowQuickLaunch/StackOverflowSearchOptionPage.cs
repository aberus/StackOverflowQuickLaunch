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
        private Sort sort = Sort.Relevance;

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

        //[Category("My Category")]
        //[DisplayName("My Integer Option")]
        //[Description("My integer option")]
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
    }

    

    public enum Sort { 
        //[Description("(default)")]
        //None = 0,
        [Description("Active")]
        Activity = 4, 
        Votes = 3,
        [Description("Newest")]
        Creation = 2, 
        Relevance = 1,
    }; 
}
