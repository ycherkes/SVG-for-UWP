using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using SvgConverterCore.Utils;
using SvgForUWPConverter.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;


namespace SvgForUWPConverter.Controls
{
    public partial class SvgConverterToolWindowControl : UserControl
    {
        private readonly WritableSettingsStore _settingsStore;

        private static readonly DTE2 Dte;

        static SvgConverterToolWindowControl()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(DTE));
        }

        public SvgConverterToolWindowControl()
        {
            InitializeComponent();

            ThreadHelper.ThrowIfNotOnUIThread();
            var settingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            _settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

        private void ChooseInputFolder_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "Select input folder.",
                SelectedPath = ViewModel.InputFolder ?? _settingsStore.GetString("External Tools", "SvgConverter-InputFolderPath", FindAssetsPath())
            };

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK) return;

            ViewModel.InputFolder = folderBrowserDialog.SelectedPath;

            _settingsStore.SetString("External Tools", "SvgConverter-InputFolderPath", folderBrowserDialog.SelectedPath);
        }

        private string FindAssetsPath()
        {
            var projectPath = GetProjectPath();
            if (string.IsNullOrEmpty(projectPath))
            {
                return DteExtensions.GetSolutionPath();
            }
            var directoryName = Path.GetDirectoryName(projectPath) + "\\Assets\\";
            return string.IsNullOrEmpty(directoryName) ? projectPath : directoryName;
        }

        private void ProjectsOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedProject = GetSelectedProject();
        }

        private async void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Projects = await GetProjectListAsync();
                if (ViewModel.Projects.Count > 0)
                {
                    Projects.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }
        }

        private static async Task<ObservableCollection<Project>> GetProjectListAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var result = new ObservableCollection<Project>(Dte.Solution
                                                              .Projects
                                                              .Cast<Project>()
                                                              .SelectMany(GetAllProjects));
            return result;
        }

        private static IEnumerable<Project> GetAllProjects(Project project)
        {
            const string vsProjectKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";

            ThreadHelper.ThrowIfNotOnUIThread();

            if (project == null || project.Kind == "{66A2671D-8FB5-11D2-AA7E-00C04F688DDE}") // "Miscellaneous Files"
                return Array.Empty<Project>();

            if (project.Kind != vsProjectKindSolutionFolder)
                return new[] { project };

            var projectItems = project.ProjectItems.Cast<ProjectItem>().ToArray();

            var subProjects = projectItems.Select(x => x.SubProject)
                                          .SelectMany(GetAllProjects)
                                          .ToArray();

            return subProjects;
        }

        private void ChooseOutputFolderOnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "Select output folder for Visual Assets (normally the \"Assets\" folder).",
                SelectedPath = ViewModel.OutputFolder ?? FindAssetsPath()
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                ViewModel.OutputFolder = folderBrowserDialog.SelectedPath;
            }
        }

        private async void ConvertOnClick(object sender, RoutedEventArgs e)
        {
            Error.Text = string.Empty;
            ProgressBar.Visibility = Visibility.Visible;
            try
            {
                await SvgConverter.Convert(ViewModel, fileName =>
                {
                    if (ViewModel.AddItemsToProject)
                    {
                        ViewModel.SelectedProject?.ProjectItems.AddFromFile(fileName);
                    }
                });
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }

            ProgressBar.Visibility = Visibility.Hidden;
        }


        private string GetProjectPath()
        {
            return GetSelectedProject()?.FullName;
        }

        private Project GetSelectedProject()
        {
            return Projects.SelectedIndex >= 0 ? ViewModel.Projects[Projects.SelectedIndex] : null;
        }
    }
}