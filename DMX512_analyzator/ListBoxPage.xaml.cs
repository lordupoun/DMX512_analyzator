using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
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
            bool ready;
		Protocol[] protocolArray = new Protocol[256];
		public ListBoxPage(Protocol[] protocolArray)
        {
            this.protocolArray = protocolArray;
            InitializeComponent();
            ready = true;
            //var mainWindow = (MainWindow)Application.Current.MainWindow;
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
        
            TextBox boxChanged = (TextBox)sender;
            /*if (byte.TryParse(boxChanged.Text, NumberStyles.HexNumber, null, out device1.toSend[ScrollBarA.Value]) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
            {
                MessageBox.Show("opravit");
            }*/
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
				protocolArray[0].SendHex(textBoxB, int.Parse(textBoxA.Text));
		}
	}
}
