//------------------------------------------------------------------------------
// <copyright file="StackOverflowSearchErrorListCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Net;
using System.Text.RegularExpressions;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace Aberus.StackOverflowQuickLaunch
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class StackOverflowSearchErrorListCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("0c9cc537-086a-460b-ab7b-a21734629537");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackOverflowSearchErrorListCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private StackOverflowSearchErrorListCommand(Package package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));

            var commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static StackOverflowSearchErrorListCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new StackOverflowSearchErrorListCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = this.ServiceProvider.GetService(typeof(EnvDTE.DTE)) as DTE2;
            if (dte != null)
            {
                var errorList = dte.ToolWindows.ErrorList as IErrorList;
                var selected = errorList?.TableControl?.SelectedEntry;
                if (selected != null && selected.TryGetValue("text", out object text))
                {
                    string description = (string)text;
                    if(((StackOverflowQuickLaunchPackage)package).OptionPage.UseGenericSearch)
                        description = Regex.Replace(description, "'.*'", "''", RegexOptions.IgnoreCase);

                    string searchQuery = selected.TryGetValue("errorcode", out object code) ? $"{code}: {description}" : description;
                    string url = "https://stackoverflow.com/search?q=" + WebUtility.UrlEncode(searchQuery);
                    Browser.Open(url,
                        ((StackOverflowQuickLaunchPackage)package).OptionPage.OpenInInternalBrowser,
                        ((StackOverflowQuickLaunchPackage)package).OptionPage.OpenInNewTab);
                }
            }
        }
    }
}
