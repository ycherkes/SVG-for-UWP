using System;
using System.Collections.ObjectModel;
using System.IO;
using EnvDTE;
using SvgConverterCore.VieModels;
using SvgForUWPConverter.Extensions;


namespace SvgForUWPConverter.ViewModels
{
    public class SvgUpdatingViewModel : SvgUpdatingViewModelBase
    {
        private ObservableCollection<Project> _projects;
        private Project _selectedProject;
        private bool _addItemsToProject;

        public SvgUpdatingViewModel()
        {
            AddItemsToProject = true;
            _projects = new ObservableCollection<Project>();
        }

        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAddingAvailable));
                OnPropertyChanged(nameof(AddItemsToProject));
            }
        }

        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAddingAvailable));
                OnPropertyChanged(nameof(AddItemsToProject));
            }
        }

        public bool AddItemsToProject
        {
            get => IsAddingAvailable && _addItemsToProject;
            set
            {
                _addItemsToProject = value;
                OnPropertyChanged();
            }
        }

        public bool IsAddingAvailable => _selectedProject != null 
                                         && ! string.IsNullOrEmpty(OutputFolder) 
                                         && OutputFolder.IndexOf(Path.GetDirectoryName(DteExtensions.GetSolutionPath()) ?? "@", 
                                                                 StringComparison.CurrentCultureIgnoreCase) == 0;

        public override string OutputFolder
        {
            get => base.OutputFolder;
            set
            {
                base.OutputFolder = value;
                OnPropertyChanged(nameof(IsAddingAvailable));
                OnPropertyChanged(nameof(AddItemsToProject));
            }
        }
    }
}
