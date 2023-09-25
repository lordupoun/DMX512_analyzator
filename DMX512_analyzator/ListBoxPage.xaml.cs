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
        int format;
		Protocol[] protocolArray = new Protocol[256];
		private RadioButton[] radioArray;
		Dictionary<string, Protocol> protocolDictionary = new Dictionary<String, Protocol>();
        private ComboBox portBox;
		public ListBoxPage(Dictionary<String, Protocol> protocolDictionary, RadioButton[] radioArray, ComboBox portBox)
        {
			this.protocolDictionary = protocolDictionary;
			this.radioArray = radioArray;
            this.portBox = portBox;
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
            textBoxA.Text=Convert.ToString(int.Parse(textBoxA.Text) - 1);
        }

        private void ScrollBarA_KeyDown(object sender, KeyEventArgs e)
        {
			textBoxA.Text = Convert.ToString(int.Parse(textBoxA.Text) +2);
		}

        private void textBoxA_TextChanged(object sender, TextChangedEventArgs e) //vymyslet jak to bude fungovat když bude zaškrtlej radioBox
        {
        
            TextBox boxChanged = (TextBox)sender;
            /*if (byte.TryParse(boxChanged.Text, NumberStyles.HexNumber, null, out device1.toSend[ScrollBarA.Value]) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
            {
                MessageBox.Show("opravit");
            }*/
            //textBoxB.Text = Convert.ToString(protocolArray[0].getToSendValue(int.Parse(textBoxA.Text)));
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            if (radioArray[0].IsChecked==true)
            {
				protocolDictionary[(String)portBox.SelectedValue].SendHex(textBoxB, int.Parse(textBoxA.Text));
			}
			else if(radioArray[1].IsChecked == true)
			{
				protocolDictionary[(String)portBox.SelectedValue].SendDec(textBoxB, int.Parse(textBoxA.Text));
			}
			else if(radioArray[2].IsChecked == true)
			{
				protocolDictionary[(String)portBox.SelectedValue].SendBin(textBoxB, int.Parse(textBoxA.Text));
			}

		}
        public void setFormat(int format)
        {
            this.format = format;
        }
	}
}
