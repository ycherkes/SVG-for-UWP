using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SvgConverterCore.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace SvgForUWPConverter
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class ConvertSvgCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 256;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("e1ab6f74-358a-4950-9918-2c9b55485582");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertSvgCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private ConvertSvgCommand(Package package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            if (!(ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)) return;

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandId) {Text = "Inline Svg Styles"};
            menuItem.BeforeQueryStatus += MenuItem_BeforeQueryStatus;
            commandService.AddCommand(menuItem);
        }

        private void MenuItem_BeforeQueryStatus(object sender, EventArgs e)
        {
            if (!(sender is OleMenuCommand menuCommand)) return;

            var documents = GetSelectedDocuments();

            menuCommand.Visible = false;
            menuCommand.Enabled = false;


            //foreach (var uiHierarchyItem in documents)
            //{
            //    DebugHelper.IdentifyInternalObjectTypes(uiHierarchyItem);
            //}

            if(documents.All(x => !(x.Object is ProjectItem) || !IsSvgFile(((ProjectItem) x.Object).Name) ))  return;

            menuCommand.Visible = true;
            menuCommand.Enabled = true;
        }


        private bool IsSvgFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || fileName.Length < 4) return false;

            var returnedActiveDocumentType = Path.GetExtension(fileName);
            var isSvgFile = returnedActiveDocumentType.Equals(".svg", StringComparison.CurrentCultureIgnoreCase);

            return isSvgFile;
        }

        private IEnumerable<UIHierarchyItem> GetSelectedDocuments()
        {
            var dte = (EnvDTE80.DTE2) ServiceProvider.GetService(typeof(DTE));
            var selectedItems = ((UIHierarchy)dte.Windows.Item(EnvDTE.Constants.vsWindowKindSolutionExplorer).Object).SelectedItems as IEnumerable<UIHierarchyItem>;

            return selectedItems;
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static ConvertSvgCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new ConvertSvgCommand(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void MenuItemCallback(object sender, EventArgs e)
        {
            var documents = GetSelectedDocuments().Select(x => x.Object).OfType<ProjectItem>().ToArray();

            if (!documents.Any()) return;

            try
            {
                foreach (var document in documents)
                {
                    await SvgConverter.ConvertFile(document.Properties.Item("FullPath").Value.ToString());
                }
                
            }
            catch (Exception exception)
            {
                VsShellUtilities.ShowMessageBox(
                    ServiceProvider,
                    $"Styles inlining failed!. Message: {exception.Message}",
                    "Error.",
                    OLEMSGICON.OLEMSGICON_INFO,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
    }
}
