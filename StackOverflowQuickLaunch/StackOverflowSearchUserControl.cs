using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

namespace Aberus.StackOverflowQuickLaunch
{
    public partial class StackOverflowSearchUserControl : UserControl
    {
        public StackOverflowSearchUserControl()
        {
            InitializeComponent();
           // this.Initialize();
        }

        public void Initialize()
        {
            this.comboBox1.DataSource = EnumHelper.GetValues(typeof(Sort));
            this.comboBox1.DisplayMember = "Value";
            this.comboBox1.ValueMember = "Key";
            this.comboBox1.SelectedItem = OptionsPage.Sort;

            this.numericUpDown1.Value = OptionsPage.ShowResults;

            this.checkBox1.Checked = OptionsPage.AlwayShowLink;
        }

        public StackOverflowSearchOptionPage OptionsPage { get; set; }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Sort sort;
            Enum.TryParse<Sort>(comboBox1.SelectedValue.ToString(), out sort);

            OptionsPage.Sort = sort;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            OptionsPage.ShowResults = (int)numericUpDown1.Value;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            OptionsPage.AlwayShowLink = checkBox1.Checked;
        }
    }

    /// <span class="code-SummaryComment"><summary></span>
    /// Provides a static utility object of methods and properties to interact
    /// with enumerated types.
    /// <span class="code-SummaryComment"></summary></span>
    public static class EnumHelper
    {
        /// <span class="code-SummaryComment"><summary></span>
        /// Gets the <span class="code-SummaryComment"><see cref="DescriptionAttribute" /> of an <see cref="Enum" /></span>
        /// type value.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="value">The <see cref="Enum" /> type value.</param></span>
        /// <span class="code-SummaryComment"><returns>A string containing the text of the</span>
        /// <span class="code-SummaryComment"><see cref="DescriptionAttribute"/>.</returns></span>
        public static string GetDescription(Enum @enum)
        {
            if (@enum == null)
            {
                throw new ArgumentNullException("enumType");
            }

            Type type = @enum.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enum");
            }

            string description = @enum.ToString();
            FieldInfo fieldInfo = type.GetField(description);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Converts the <span class="code-SummaryComment"><see cref="Enum" /> type to an <see cref="IList" /> </span>
        /// compatible object.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="type">The <see cref="Enum"/> type.</param></span>
        /// <span class="code-SummaryComment"><returns>An <see cref="IList"/> containing the enumerated</span>
        /// type value and description.<span class="code-SummaryComment"></returns></span>
        public static IList GetValues(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException("type");
            }

            ArrayList list = new ArrayList();
            Array enumValues = Enum.GetValues(enumType);

            foreach (Enum value in enumValues)
            {
                list.Add(new KeyValuePair<Enum, string>(value, GetDescription(value)));
            }

            return list;
        }
    }

}
