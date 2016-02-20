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

        [Category("My Category")]
        [DisplayName("My Integer Option")]
        [Description("My integer option")]
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

        private int myVar;

        [Category("My Category")]
        [DisplayName("My Integer Option1")]
        [Description("My integer option1")]
        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
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
        [Description("(none)")]
        None = 0,
        Activity, 
        Votes, 
        Creation, 
        Relevance 
    }; 
}
