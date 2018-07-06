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
using XenoCore.Builder.MVVM;

namespace XenoCore.Builder.GUI
{
    /// <summary>
    /// Interaction logic for ParticleSettingAddWindow.xaml
    /// </summary>
    public partial class ParticleModuleWindow : Window
    {
        public ParticleModuleViewModel ViewModel { get; private set; } = new ParticleModuleViewModel();

        public ParticleModuleWindow()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedModule = ((RadioButton)sender).Tag as ModuleTypeModel;
        }
    }
}
