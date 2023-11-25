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
		private Dictionary<string, ProtocolSend> protocolSendDictionary = new Dictionary<String, ProtocolSend>(); //doufám že se předává odkazem
        private Dictionary<string, ProtocolReceive> protocolReceiveDictionary = new Dictionary<String, ProtocolReceive>();
        private RadioButton[] radioArray;
        public String SelectedPort { get; set; }
        public byte SelectedFunction { get; set; } //ref to nejde jednoduše udělat
        //private String selectedPort; //Pro port se využívá String - vychází z koncepce pojmenovávání COM portů (více futureproof oproti číslům)
        TextBox boxChanged;


        public TextBoxPage(Dictionary<String, ProtocolSend> protocolSendDictionary, Dictionary<String, ProtocolReceive> protocolReceiveDictionary, RadioButton[] radioArray, String port,byte function)//vybranej port předat jako ref String, to samý radioButton ref int
		{
			this.protocolSendDictionary = protocolSendDictionary;
            this.protocolReceiveDictionary = protocolReceiveDictionary;
            this.radioArray = radioArray;
			SelectedFunction = function;
			SelectedPort = port;
			InitializeComponent();
			textBoxArray = mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce textboxů na array
			windowLoaded = true;
			//Refresh();
		}
		private void text_changed(object sender, TextChangedEventArgs e) //Event změny textu v textboxu
		{
			
			if (windowLoaded == true&& SelectedFunction == 1)//Zabraňuje pádu, TODO: Najít alternativu
			{
				//if (selectedFunction == 0)
				//{
					boxChanged = (TextBox)sender;
					if (radioArray[0].IsChecked == true) //Hexadecimální; v případě problémů s výkonem, lze dosadit příme hodnoty místo portBox.SelectedVaulue castování(String) a dosazení do protocolDictionary a radioArray[0].IsChecked
					{
						if (protocolSendDictionary[SelectedPort].SendHex(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //když bude celá metoda pryč, nemůžu přistupovat k TextBoxArray, když jen část, nemůžu ji sdílet...
						{
							//Upozornění na chybnou hodnotu
						}
					}
					else if (radioArray[1].IsChecked == true) //Decimální
					{
						if (protocolSendDictionary[SelectedPort].SendDec(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější (rychlejší)
						{
							//Upozornění na chybnou hodnotu
						}
					}
					else if (radioArray[2].IsChecked == true) //Binární
					{
						protocolSendDictionary[SelectedPort].SendBin(boxChanged, Array.IndexOf(textBoxArray, boxChanged));
					}
				//}
				/*else if (selectedFunction == 1)
				{
					boxChanged = (TextBox)sender;
					if (radioArray[0].IsChecked == true) //Hexadecimální; v případě problémů s výkonem, lze dosadit příme hodnoty místo portBox.SelectedVaulue castování(String) a dosazení do protocolDictionary a radioArray[0].IsChecked
					{
						//if (protocolReceiveDictionary[SelectedPort].SendHex(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //když bude celá metoda pryč, nemůžu přistupovat k TextBoxArray, když jen část, nemůžu ji sdílet...
						// {
						//     //Upozornění na chybnou hodnotu
						// }
					}
					else if (radioArray[1].IsChecked == true) //Decimální
					{
						//if (protocolReceiveDictionary[SelectedPort].SendDec(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější (rychlejší)
						//{
						//     //Upozornění na chybnou hodnotu
						//}
					}
					else if (radioArray[2].IsChecked == true) //Binární
					{
						//protocolReceiveDictionary[SelectedPort].SendBin(boxChanged, Array.IndexOf(textBoxArray, boxChanged));
					}
				}*/
                    //testBtn.Background = new SolidColorBrush(Color.FromArgb(protocolArray[0].toSend[1], protocolArray[0].toSend[2], protocolArray[0].toSend[3], protocolArray[0].toSend[4])); - příkaz pro zobrazení barvy z hodnot
                }

		}
		public void Refresh()
		{
			if(SelectedFunction==1)
			{
				if (radioArray[0].IsChecked == true)
				{
					/*String hexString = BitConverter.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSend());
					String[] hexStringArray = hexString.Split("-");*/
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(protocolSendDictionary[SelectedPort].getToSendValue(i), 16); //fixnout
					
						//textBoxArray[i].Text = hexStringArray[i];					
					}
					//MessageBox.Show(SelectedPort);
				}
				if (radioArray[1].IsChecked == true)
				{
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(protocolSendDictionary[SelectedPort].getToSendValue(i));
					
					}
				}
				if (radioArray[2].IsChecked == true)
				{
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(protocolSendDictionary[SelectedPort].getToSendValue(i), 2);

					}
				}
            }
            else if (SelectedFunction == 0)
			{
                if (radioArray[0].IsChecked == true)
                {
                    /*String hexString = BitConverter.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSend());
					String[] hexStringArray = hexString.Split("-");*/
                    for (int i = 0; i < textBoxArray.Length; i++)
                    {
                        textBoxArray[i].Text = Convert.ToString(protocolReceiveDictionary[SelectedPort].getReceivedValue(i), 16); //fixnout

                        //textBoxArray[i].Text = hexStringArray[i];					
                    }
                    //MessageBox.Show(SelectedPort);
                }
                if (radioArray[1].IsChecked == true)
                {
                    for (int i = 0; i < textBoxArray.Length; i++)
                    {
                        textBoxArray[i].Text = Convert.ToString(protocolReceiveDictionary[SelectedPort].getReceivedValue(i));

                    }
                }
                if (radioArray[2].IsChecked == true)
                {
                    for (int i = 0; i < textBoxArray.Length; i++)
                    {
                        textBoxArray[i].Text = Convert.ToString(protocolReceiveDictionary[SelectedPort].getReceivedValue(i), 2);

                    }
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
//je vhodné přepisovat buňky? teď to funguje tak, že při změně režimu se přepíše obsah textBoxů do toSend, i přesto, že jsou tam shodný hodnoty