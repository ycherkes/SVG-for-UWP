using System.ComponentModel;
using System.Runtime.CompilerServices;
using SvgConverterCore.Annotations;

namespace SvgConverterCore.VieModels
{
    public class SvgUpdatingViewModelBase : INotifyPropertyChanged
    {
        private string _inputFolder;

        private string _outputFolder;

        private bool _removeInlinedStyles;
        private bool _overwriteExistingFiles;
        
        public bool OverwriteExistingFiles
        {
            get => _overwriteExistingFiles;
            set
            {
                _overwriteExistingFiles = value;
                OnPropertyChanged();
            }
        }

        public bool RemoveInlinedStyles
        {
            get => _removeInlinedStyles;
            set
            {
                _removeInlinedStyles = value;
                OnPropertyChanged();
            }
        }

        public virtual string OutputFolder
        {
            get => _outputFolder;
            set
            {
                _outputFolder = value;
                OnPropertyChanged();
            }
        }

        public string InputFolder
        {
            get => _inputFolder;
            set
            {
                _inputFolder = value;
                OnPropertyChanged();
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
