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

namespace DMX512_analyzator
{
    /// <summary>
    /// Interaction logic for ListBoxPage.xaml
    /// </summary>
    public partial class ListBoxPage : Page
    {
        public ListBoxPage()
        {
            InitializeComponent();
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            textBoxA.Text = Convert.ToString(ScrollBarA.Value);
        }

        private void ScrollBarA_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void ScrollBarA_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBoxA_TextChanged(object sender, TextChangedEventArgs e)
        {
            ScrollBarA.Value = byte.Parse(textBoxA.Text);
        }
    }
}
