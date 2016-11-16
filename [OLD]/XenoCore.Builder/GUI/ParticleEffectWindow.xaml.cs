using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using XenoCore.Builder.Converters;
using XenoCore.Builder.Data;
using XenoCore.Builder.MVVM;
using XenoCore.Builder.Screens;
using XenoCore.Builder.Services;
using XenoCore.Engine.Entities;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Resources;
using XenoCore.Engine.Screens;
using XenoCore.Engine.World;

namespace XenoCore.Builder.GUI
{
    /// <summary>
    /// Interaction logic for ParticleEffectWindow.xaml
    /// </summary>
    public partial class ParticleEffectWindow : Window
    {
        public ParticleEditorViewModel ViewModel { get; private set; } = new ParticleEditorViewModel();

        public ParticleEffectWindow(ResourceObj effect)
        {
            InitializeComponent();

            DataContext = ViewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;


            ViewModel.Initialize(effect);

            host.Screen = ViewModel.Screen;
            App.CurrentApp.ScreenManager.UpdateControls.Add(host);

        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "EditingEmitter")
            {
                spEmitterSettings.Children.Clear();
                spModuleSettings.Children.Clear();

                if (ViewModel.EditingEmitter != null)
                {
                    UIFactory.GenerateUI(spEmitterSettings, ViewModel.EditingEmitter.EditorModel);
                }
            }
            if (e.PropertyName == "EditingModule")
            {
                spModuleSettings.Children.Clear();
                var module = ViewModel.EditingModule;
                if (module != null)
                {
                    UIFactory.GenerateUI(spModuleSettings, module);
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel.Modified)
            {
                var result = System.Windows.MessageBox.Show("Save changes and continiue?","Closing",
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Information );
               switch(result)
                {
                    case MessageBoxResult.Yes:
                        ViewModel.Save();
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.No:
                        //TODO - Restore previous state (or just edit a copy)

                        break;
                }


            }
            //  e.Cancel = true;
            base.OnClosing(e);
            //  Hide();
        }
        protected override void OnClosed(EventArgs e)
        {
            App.CurrentApp.ScreenManager.UpdateControls.Remove(host);
            host.Dispose();
            base.OnClosed(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ViewModel.Save();
            }


        }

        private void btnRemoveModule_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveSelectedModule();
        }
        private void btnAddModule_Click(object sender, RoutedEventArgs e)
        {
            host.Screen.Paused = true;
            ParticleModuleWindow dialog = new ParticleModuleWindow();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                ViewModel.AddModule(dialog.ViewModel.SelectedModule.Type);
            }
            host.Screen.Paused = false;

        }
        private void btnAddEmitter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.AddEmitter();
        }
        private void btnRemoveEmitter_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RemoveSelectedEmitter();
        }


    }
}
