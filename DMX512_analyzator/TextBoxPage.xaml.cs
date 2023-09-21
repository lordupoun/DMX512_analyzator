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
	/// Interaction logic for TextBoxPage.xaml
	/// </summary>
	public partial class TextBoxPage : Page
	{
		bool windowLoaded;
		int format;
		private TextBox[] textBoxArray = new TextBox[513];
		private Protocol[] protocolArray = new Protocol[256];
		private RadioButton[] radioArray;
		public TextBoxPage(Protocol[] protocolArray, RadioButton[] radioArray)
		{
			this.protocolArray = protocolArray;
			this.radioArray = radioArray;	
			InitializeComponent();
			textBoxArray = mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce na array
			windowLoaded = true;
		}
		private void text_changed(object sender, TextChangedEventArgs e) //<- data na odeslání při změně textu
		{
			TextBox boxChanged = (TextBox)sender;
			if (windowLoaded == true)//zabrani padu - pak odstranit
			{
				if (radioArray[0].IsChecked == true) //------------tyhle řádky by nemusely být duplicitně //-----------------------------Přehodit do jiné třídy
				{
					if (protocolArray[0].SendHex(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //když bude celá metoda pryč, nemůžu přistupovat k TextBoxArray, když jen část, nemůžu ji sdílet...
					{
						MessageBox.Show("opravit"); //out device1.toSend[Array.IndexOf(textBoxArray, boxChanged)
					}
				}
				else if (radioArray[1].IsChecked == true)
				{
					if (protocolArray[0].SendDec(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
					{
						MessageBox.Show("opravit");
					}
				}
				else if (radioArray[2].IsChecked == true)
				{
					protocolArray[0].SendBin(boxChanged, Array.IndexOf(textBoxArray, boxChanged));
				}
				//testBtn.Background = new SolidColorBrush(Color.FromArgb(protocolArray[0].toSend[1], protocolArray[0].toSend[2], protocolArray[0].toSend[3], protocolArray[0].toSend[4]));
			}

		}
	}
}
