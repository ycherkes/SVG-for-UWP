using System;
using System.Windows;
using System.Windows.Forms;
using SvgConverterCore.Utils;
using UserControl = System.Windows.Controls.UserControl;

namespace SvgConverterDesktop
{
    public partial class SvgConverterControl : UserControl
    {
        public SvgConverterControl()
        {
            InitializeComponent();
        }

        private void ChooseInputFolder_OnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "Select input folder.",
                SelectedPath = ViewModel.InputFolder ?? (string)Properties.Settings.Default["InputFolder"] 
            };
           
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                ViewModel.InputFolder = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default["InputFolder"] = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private void ChooseOutputFolderOnClick(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = "Select output folder.",
                SelectedPath = ViewModel.OutputFolder ?? (string)Properties.Settings.Default["OutputFolder"]
            };

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.ViewModel.OutputFolder = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default["OutputFolder"] = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }
        }

        private async void ConvertOnClick(object sender, RoutedEventArgs e)
        {
            Error.Text = string.Empty;
            this.ProgressBar.Visibility = Visibility.Visible;
            try
            {
                await SvgConverter.Convert(ViewModel, fileName => { });
            }
            catch (Exception ex)
            {
                Error.Text = ex.Message;
            }
            
            this.ProgressBar.Visibility = Visibility.Hidden;
        }
        
    }
}