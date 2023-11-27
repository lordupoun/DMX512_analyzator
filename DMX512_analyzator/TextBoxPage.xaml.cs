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
    /// 
    public interface IBasePage
    {
        void Refresh();
		void SetToReceive();
		void SetToSend();
	}
	public partial class TextBoxPage : Page,IBasePage
	{
		bool pageLoaded;
		int format;
		private TextBox[] textBoxArray = new TextBox[513];
		//private Protocol[] protocolArray = new Protocol[256];
		//private Dictionary<string, ProtocolSend> protocolSendDictionary = new Dictionary<String, ProtocolSend>(); //doufám že se předává odkazem
        //private Dictionary<string, ProtocolReceive> protocolReceiveDictionary = new Dictionary<String, ProtocolReceive>();
        //private RadioButton[] radioArray;
        //private RadioButton[] radioFunctionArray;
        //public String SelectedPort { get; set; }
		private UserSettings userSettings;
        //public byte SelectedFunction { get; set; } //ref to nejde jednoduše udělat
        //private String selectedPort; //Pro port se využívá String - vychází z koncepce pojmenovávání COM portů (více futureproof oproti číslům)
        TextBox boxChanged;

		public TextBoxPage(UserSettings userSettings)//vybranej port předat jako ref String, to samý radioButton ref int
		{
			this.userSettings=userSettings; //musí se předávat takto? neviděla by na to ta třída i normálně? neviděla by tu instanci kde jsou hodnoty?
            InitializeComponent();
            textBoxArray = mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce textboxů na array
            pageLoaded = true;
        }
        /*    public TextBoxPage(Dictionary<String, ProtocolSend> protocolSendDictionary, Dictionary<String, ProtocolReceive> protocolReceiveDictionary, RadioButton[] radioArray, String port, RadioButton[] radioFunctionArray)//vybranej port předat jako ref String, to samý radioButton ref int
		{
			this.protocolSendDictionary = protocolSendDictionary;
            this.protocolReceiveDictionary = protocolReceiveDictionary;
            this.radioArray = radioArray;
			this.radioFunctionArray = radioFunctionArray;
			SelectedPort = port;
			InitializeComponent();
			textBoxArray = mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce textboxů na array
			windowLoaded = true;
			//Refresh();
		}*/
        public void SetToReceive()
        {
            for (int i = 0; i < textBoxArray.Length; i++)
			{
				textBoxArray[i].IsReadOnly = true;
                textBoxArray[i].Background = Brushes.LightGray;
                textBoxArray[i].Text = "0";
            }
        }
        public void SetToSend()
        {
            for (int i = 0; i < textBoxArray.Length; i++)
            {
                textBoxArray[i].IsReadOnly = false;
                textBoxArray[i].Background = Brushes.Transparent;
            }
			Refresh();
        }
		//Odešle data z textboxu na základě toho jaký je zvolený formát
        private void text_changed(object sender, TextChangedEventArgs e) //Event změny textu v textboxu
		{
			
			if (pageLoaded == true&& userSettings.SelectedFunction == 1)//Zabraňuje pádu, TODO: Najít alternativu
			{
				//if (selectedFunction == 0)
				//{
					boxChanged = (TextBox)sender;
					if (userSettings.RadioArray[0].IsChecked == true) //Hexadecimální; v případě problémů s výkonem, lze dosadit příme hodnoty místo portBox.SelectedVaulue castování(String) a dosazení do protocolDictionary a radioArray[0].IsChecked
					{
						if (userSettings.ProtocolDictionary[userSettings.SelectedPort].SendHex(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //když bude celá metoda pryč, nemůžu přistupovat k TextBoxArray, když jen část, nemůžu ji sdílet...
						{
							//Upozornění na chybnou hodnotu
						}
					}
					else if (userSettings.RadioArray[1].IsChecked == true) //Decimální
					{
						if (userSettings.ProtocolDictionary[userSettings.SelectedPort].SendDec(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější (rychlejší)
						{
							//Upozornění na chybnou hodnotu
						}
					}
					else if (userSettings.RadioArray[2].IsChecked == true) //Binární
					{
						userSettings.ProtocolDictionary[userSettings.SelectedPort].SendBin(boxChanged, Array.IndexOf(textBoxArray, boxChanged));
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
			if(userSettings.SelectedFunction==1)
			{
				if (userSettings.RadioArray[0].IsChecked == true)
				{
                    //MessageBox.Show(Convert.ToString(userSettings.SelectedFunction));
                    /*String hexString = BitConverter.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSend());
					String[] hexStringArray = hexString.Split("-");*/
                    MessageBox.Show(Convert.ToString(userSettings.SelectedPort));
                    MessageBox.Show(Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(0), 16));
                    for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(i), 16); //fixnout...
						//
						//textBoxArray[i].Text = hexStringArray[i];					
					}
					//MessageBox.Show(SelectedPort);
				}
				if (userSettings.RadioArray[1].IsChecked == true)
				{
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(i));
					
					}
				}
				if (userSettings.RadioArray[2].IsChecked == true)
				{
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(i), 2);

					}
				}
            }
            else if (userSettings.SelectedFunction == 0) //TODO: fungovalo by předání ref? tzn. int zadám do konstruktoru jako referenci a uložím do proměnné, čímž se mi tam drží stále ta samá hodnota jako pointer? ChatGPT říkal, že ne. - vytvoři třídu s konstruktorem, vložit tam ref int jako přetížení uložit ho do proměnné, vypsat, změnit proměnnou v předchozí třídě, znovu vypsat
			{
                if (userSettings.RadioArray[0].IsChecked == true)
                {
                    /*String hexString = BitConverter.ToString(protocolDictionary[(String)portBox.SelectedValue].getToSend());
					String[] hexStringArray = hexString.Split("-");*/
                    MessageBox.Show(Convert.ToString(userSettings.SelectedFunction));
                    for (int i = 0; i < textBoxArray.Length; i++)
                    {
						
                        textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getReceivedValue(i), 16); //Sem to při odesílání nemá chodit

                        //textBoxArray[i].Text = hexStringArray[i];					
                    }
                    //MessageBox.Show(SelectedPort);
                }
                if (userSettings.RadioArray[1].IsChecked == true)
                {
                    for (int i = 0; i < textBoxArray.Length; i++)
                    {
                        textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getReceivedValue(i));

                    }
                }
                if (userSettings.RadioArray[2].IsChecked == true)
                {
                    for (int i = 0; i < textBoxArray.Length; i++)
                    {
                        textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getReceivedValue(i), 2);

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