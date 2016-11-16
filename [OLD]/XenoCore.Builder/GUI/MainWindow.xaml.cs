using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XenoCore.Builder.Host;
using XenoCore.Builder.MVVM;
using XenoCore.Builder.Services;
using XenoCore.Desktop;
using XenoCore.Engine;
using XenoCore.Engine.Input;
using XenoCore.Engine.Logging;
using XenoCore.Engine.Screens;
using System.ComponentModel;

namespace XenoCore.Builder.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; private set; } = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;

            App.CurrentApp.ScreenManager.UpdateControls.Add(mainHost);

            App.CurrentApp.MainEditor = this;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            App.Current.Shutdown();
        }

        private void menuNewProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectSettingsWindow();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                ViewModel.NewProject(dialog.ViewModel.Settings, dialog.ViewModel.ProjectDir);
            }
        }

        private void menuOpenProject_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Builder Project Files|*.xcp";
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!ViewModel.OpenProject(dialog.FileName))
                    MessageBox.Show("Invalid project file!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    mainHost.Screen = ViewModel.CurrentScreen;
            }

        }

        private void menuProjectSettings_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProjectSettingsWindow();
            dialog.ViewModel.UseExistingProject(ViewModel.CurrentProject);
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                ViewModel.UpdateSettings();
            }
        }

        private void menuResources_Click(object sender, RoutedEventArgs e)
        {
            var resourcesWindow = new ResourcesWindow();
            resourcesWindow.Show();

        }
    }
}
