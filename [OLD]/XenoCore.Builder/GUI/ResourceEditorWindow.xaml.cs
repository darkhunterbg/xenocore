using FontDialogSample;
using System;
using System.Collections.Generic;
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

namespace XenoCore.Builder.GUI
{
    /// <summary>
    /// Interaction logic for ResourceEditor.xaml
    /// </summary>
    public partial class ResourceEditorWindow : Window
    {
        public EditorResourceModel ViewModel { get; private set; }

        public ResourceEditorWindow(ResourceObj resouce)
        {

            InitializeComponent();

            spFont.Visibility = Visibility.Collapsed;

            switch (resouce.Type)
            {
                case ResourceType.SpriteFont:
                    {
                        spFont.Visibility = Visibility.Visible;
                        ViewModel = new SpriteFontEditorViewModel(resouce);
                        spFont.DataContext = ViewModel;
                    };
                    break;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveResource();
          

            DialogResult = true;
        }

        private void btnSelectFont_Click(object sender, RoutedEventArgs e)
        {
            var vm = ViewModel as SpriteFontEditorViewModel;
            var dialog = new FontChooser();
            dialog.SelectedFontFamily = new FontFamily(vm.FontName);

            dialog.SelectedFontSize = vm.FontSize;
            dialog.SelectedFontWeight = vm.FontWeight;
            dialog.SelectedFontStyle = vm.FontStyle;
            if (dialog.ShowDialog().GetValueOrDefault())
            {
    
                vm.FontName = dialog.SelectedFontFamily.ToString();
                vm.FontSize = dialog.SelectedFontSize;
                vm.FontWeight = dialog.SelectedFontWeight;
                vm.FontStyle = dialog.SelectedFontStyle;

            }
        }
    }
}
