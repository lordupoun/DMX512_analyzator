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
		Dictionary<string, Protocol> protocolDictionary = new Dictionary<String, Protocol>(); //doufám že se předává odkazem
		private RadioButton[] radioArray;
		private ComboBox portBox;

		public TextBoxPage(Dictionary<String, Protocol> protocolDictionary, RadioButton[] radioArray, ComboBox portBox)
		{
			this.protocolDictionary = protocolDictionary;
			this.radioArray = radioArray;
			this.portBox = portBox;
			InitializeComponent();
			textBoxArray = mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce na array
			windowLoaded = true;
			//Refresh();
		}
		private void text_changed(object sender, TextChangedEventArgs e) //<- data na odeslání při změně textu
		{
			
			if (windowLoaded == true)//zabrani padu - pak odstranit
			{
				MessageBox.Show("box změněn");
				TextBox boxChanged = (TextBox)sender;
				if (radioArray[0].IsChecked == true) //------------tyhle řádky by nemusely být duplicitně //-----------------------------Přehodit do jiné třídy
				{
					if (protocolDictionary[(String)portBox.SelectedValue].SendHex(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //když bude celá metoda pryč, nemůžu přistupovat k TextBoxArray, když jen část, nemůžu ji sdílet...
					{
						//MessageBox.Show("opravit"); //out device1.toSend[Array.IndexOf(textBoxArray, boxChanged)
					}
				}
				else if (radioArray[1].IsChecked == true)
				{
					if (protocolDictionary[(String)portBox.SelectedValue].SendDec(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
					{
						//MessageBox.Show("opravit");
					}
				}
				else if (radioArray[2].IsChecked == true)
				{
					protocolDictionary[(String)portBox.SelectedValue].SendBin(boxChanged, Array.IndexOf(textBoxArray, boxChanged));
				}
				//testBtn.Background = new SolidColorBrush(Color.FromArgb(protocolArray[0].toSend[1], protocolArray[0].toSend[2], protocolArray[0].toSend[3], protocolArray[0].toSend[4]));
			}

		}
		public void Refresh()
		{
			if (radioArray[0].IsChecked == true)
			{
				/*String hexString = BitConverter.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSend());
				String[] hexStringArray = hexString.Split("-");*/
				for (int i = 0; i < textBoxArray.Length; i++)
				{
					textBoxArray[i].Text = Convert.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSendValue(i), 16);
					//textBoxArray[i].Text = hexStringArray[i];					
				}
			}
			if (radioArray[1].IsChecked == true)
			{
				for (int i = 0; i < textBoxArray.Length; i++)
				{
					textBoxArray[i].Text = Convert.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSendValue(i));
					
				}
			}
			if (radioArray[2].IsChecked == true)
			{
				for (int i = 0; i < textBoxArray.Length; i++)
				{
					textBoxArray[i].Text = Convert.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSendValue(i), 2);

				}
			}

			/*if (radioArray[0].IsChecked == true) //------------tyhle řádky by nemusely být duplicitně //-----------------------------Přehodit do jiné třídy
			{
				//MessageBox.Show("test");
				for(int i=0;i<textBoxArray.Length;i++) //překladač
				{					
					textBoxArray[i].Text = Convert.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSendValue(i));
					//MessageBox.Show(Convert.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSendValue(i)));
				}

			}
			else if (radioArray[1].IsChecked == true)
			{
				/*for (int i = 0; i < textBoxArray.Length; i++) //překladač
				{
					textBoxArray[i].Text = Convert.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSendValue(i));
				}*/
			/*}
			else if (radioArray[2].IsChecked == true)
			{
				/*for (int i = 0; i < textBoxArray.Length; i++) //překladač
				{
					textBoxArray[i].Text = Convert.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSendValue(i));
				}
			}*/
		}

		private void textBox0_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{

		}
	}
}
