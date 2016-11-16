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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XenoCore.Builder.Controls
{
    /// <summary>
    /// Interaction logic for VectorEditor.xaml
    /// </summary>
    public partial class VectorEditor : UserControl
    {
        //private Vector vector;
        public double? MinValue
        {
            get
            {
                return txtX.Minimum;
            }
            set
            {

               txtY.Minimum = txtX.Minimum = value;
            }
        }
        public double? MaxValue
        {
            get
            {
                return txtX.Maximum;
            }
            set
            {
                txtY.Maximum = txtX.Maximum = value;
            }
        }

       public String Format
        {
            get
            {
                return txtX.FormatString;
            }
            set
            {
                txtY.FormatString = txtX.FormatString = value;
            }
        }
        
        public double? Increment
        {
            get
            {
                return txtX.Increment;
            }
            set
            {

                txtY.Increment = txtX.Increment = value;
            }
        }


        public Vector Value
        {
            get { return (Vector)DataContext; }
            set
            {
                DataContext = value;
                OnValueChanged?.Invoke(this, (Vector)DataContext);
            }
        }

        public event EventHandler<Vector> OnValueChanged;

        public VectorEditor()
        {
            DataContext = new Vector();
            InitializeComponent();
        }

        private void txtX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue == null)
                return;

            Vector v = Value;
            v.X = (double)e.NewValue;
            Value = v;
        }

        private void txtY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (e.NewValue == null)
                return;
            Vector v = Value;
            v.Y = (double)e.NewValue;
            Value = v;
        }
    }
}
