using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace Aberus.StackOverflowQuickLaunch
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "0.9.0", IconResourceID = 400)]
    // Declare the package guid
    [Guid(GuidList.guidStackOverflowQuickLaunchQuickLaunchPkgString)]
    // Declare a global search provider supported by this package
    // This will write the necessary registry keys for the provider's declaration and will be used to create the provider
    // when the package extensibility point for IVsSearchProvider is requested
    [ProvideSearchProvider(typeof(StackOverflowSearchProvider), "Stack Overflow Search Provider")]
    [ProvideOptionPage(typeof(StackOverflowSearchOptionPage), "StackOverflowSearchProvider", "SearchSettings", 101, 102, true, new[] { "Stack Overflow", "StackOverflow", "Search" })]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // Derive the package class from ExtensionPointPackage instead of Package
    // This will add support for automatic creation of the search provider as an extensibility point
    public sealed class StackOverflowQuickLaunchPackage : ExtensionPointPackage
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public StackOverflowQuickLaunchPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            Instance = this;
            StackOverflowSearchErrorListCommand.Initialize(this);
        }

        public static StackOverflowQuickLaunchPackage Instance { get; private set; }

        public StackOverflowSearchOptionPage OptionPage
        {
            get
            {
                var optionsPage = (StackOverflowSearchOptionPage)GetDialogPage(typeof(StackOverflowSearchOptionPage));
                return optionsPage;
            }
        }

        #endregion
    }
}
