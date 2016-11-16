using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using XenoCore.Builder.Data;
using XenoCore.Builder.MVVM;
using XenoCore.Builder.Screens;
using XenoCore.Builder.Services;
using XenoCore.ContentPipeline;
using XenoCore.Engine.Graphics;
using XenoCore.Engine.Particles;
using XenoCore.Engine.Resources;

namespace XenoCore.Builder.GUI
{
    /// <summary>
    /// Interaction logic for ResourcesWindow.xaml
    /// </summary>
    public partial class ResourcesWindow : Window
    {
        private ParticleEffectScreen particleEffectScreen;
        private TextureScreen textureScreen;
        private FontScreen fontScreen;

        public ResourcesWindowViewModel ViewModel { get; private set; } = new ResourcesWindowViewModel();

        public ResourcesWindow(ResourceType? typeFilter = null)
        {
            InitializeComponent();

            DataContext = ViewModel;
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel.Initialize(typeFilter);

            App.CurrentApp.ScreenManager.UpdateControls.Add(host);

            particleEffectScreen = new ParticleEffectScreen()
            {
                 ShowStats = false,
            };
            fontScreen = new FontScreen();
            textureScreen = new TextureScreen();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selection")
            {
                host.Screen = null;
                var item = ViewModel.Selection as ResourceObjModel;
                if (item == null)
                {
                    return;
                }

                try
                {
                    switch (item.Resource.Type)
                    {
                        case ResourceType.Texture:
                            {
                                var t = GraphicsService.Cache.GetTexture(item.Resource.XnbPath);
                                host.Screen = textureScreen;
                                textureScreen.SetTexture(t);
                                break;
                            }
                        case ResourceType.SpriteFont:
                            {
                                host.Screen = fontScreen;
                                var f = GraphicsService.Cache.GetFont(item.Resource.XnbPath);
                                fontScreen.SetFont(f);
                                break;
                            }
                        case ResourceType.ParticleEffect:
                            {
                                host.Screen = particleEffectScreen;
                                particleEffectScreen.SetParticleEffect(item.Resource.Instance as ParticleEffectDescription);
                                break;
                            }
                    }
                }
                catch
                {

                }
            }

        }

        protected override void OnClosed(EventArgs e)
        {
            ViewModel.Uninitialize();
            //  ViewModel = null;

            App.CurrentApp.ScreenManager.UpdateControls.Remove(host);

            host.Dispose();
            base.OnClosed(e);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.NewValue as ResourceModel;
            if (item != ViewModel.Selection)
                ViewModel.Selection = item;
        }

        private void OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static TreeViewItem VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }

        private void tvResources_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bool? dialogResult = null;

            var resource = ViewModel.Selection as ResourceObjModel;

            if (resource == null)
                return;

            host.Screen.Paused = true;

            switch (resource.Resource.Type)
            {
                case ResourceType.SpriteFont:
                    {
                        var dialog = new ResourceEditorWindow(resource.Resource);
                        dialogResult = dialog.ShowDialog();
                    }
                    break;
                case ResourceType.ParticleEffect:
                    {
                        var dialog = new ParticleEffectWindow(resource.Resource);
                        dialogResult = dialog.ShowDialog();
                    }
                    break;
            }

   

            host.Screen.Paused = false;


            //if (dialogResult.GetValueOrDefault())
            {
                //refresh
                ViewModel.Selection = null;
                ViewModel.Selection = resource;
            }
     


        }

        private void miDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Selection == null)
                return;


            ViewModel.DeleteSelectedItem();
        }

        private void tvResources_KeyUp(object sender, KeyEventArgs e)
        {
            if (ViewModel.Selection == null)
                return;

            switch (e.Key)
            {
                case Key.F2:
                    {

                        if (!ViewModel.Selection.Editing)
                            ViewModel.Selection.Editing = true;
                        break;
                    }
                case Key.Enter:
                    {
                        if (ViewModel.Selection.Editing)
                            ViewModel.RenameSelectedItem();
                        break;
                    }
                case Key.Escape:
                    {
                        if (ViewModel.Selection.Editing)
                            ViewModel.Selection.Editing = false;
                        break;
                    }
                case Key.Delete:
                    {
                        if (!ViewModel.Selection.Editing)
                            ViewModel.DeleteSelectedItem();
                        break;
                    }
            }
        }

        private void miRename_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Selection == null)
                return;

            ViewModel.Selection.Editing = true;

        }

        private void miAddItems_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Multiselect = true;
            dialog.CheckFileExists = true;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ViewModel.AddNewItems(dialog.FileNames);
            }
        }

        private void tvResources_Drop(object sender, DragEventArgs e)
        {
            if (ViewModel.Selection == null)
            {
                ViewModel.Selection = ViewModel.Resources.First();

            }

            var data = e.Data.GetData(DataFormats.FileDrop) as String[];
            if (data != null)
                ViewModel.AddNewItems(data);
        }

        private void miNewItem_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Selection == null)
                return;

            var s = e.OriginalSource as MenuItem;
            var model = s.Tag as NewResourceModel;
            ViewModel.New(model.Type);
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            var dir = ViewModel.Selection as ResourceDirModel;
            if (dir != null)
                Process.Start(dir.Directory.AbsolutePath);
            else
            {
                var item = ViewModel.Selection as ResourceObjModel;
                Process.Start(item.Resource.AbsolutePath);
            }
        }
    }
}
