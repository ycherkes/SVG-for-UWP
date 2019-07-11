using System;
using System.ComponentModel.Design;
using System.Threading;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SvgForUWPConverter
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SvgConverterToolWindowCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("6457631e-14a7-4f36-a534-cfdf27d78140");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgConverterToolWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private SvgConverterToolWindowCommand(AsyncPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));

            if (!(ServiceProvider.GetService(typeof(IMenuCommandService)) is OleMenuCommandService commandService)) return;

            var menuCommandId = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(ShowToolWindowAsync, menuCommandId);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SvgConverterToolWindowCommand Instance
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
        public static void Initialize(AsyncPackage package)
        {
            Instance = new SvgConverterToolWindowCommand(package);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private async void ShowToolWindowAsync(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var window = await _package.FindToolWindowAsync(typeof(SvgConverterToolWindow), 0, true, CancellationToken.None);
            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
