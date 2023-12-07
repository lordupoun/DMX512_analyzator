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
	public partial class TextBoxPage : Page,IBasePage
	{
		bool pageLoaded;
		private TextBox[] textBoxArray = new TextBox[513];		
		private UserSettings userSettings; //Předává hodnoty všem Page v jednom objektu
        TextBox boxChanged;

		public TextBoxPage(UserSettings userSettings)
		{
			this.userSettings=userSettings;
            InitializeComponent();
            textBoxArray = mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce textboxů na array
            pageLoaded = true;
        }
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
			//Refresh();
        }
		
        private void text_changed(object sender, TextChangedEventArgs e) //Event změny textu v textboxu
		{
			
			if (pageLoaded == true&& userSettings.SelectedFunction == 1)//Zabraňuje pádu, TODO: Najít alternativu
			{
				
					boxChanged = (TextBox)sender;
					if (userSettings.RadioArray[0].IsChecked == true) //Hexadecimální; v případě problémů s výkonem, lze dosadit příme hodnoty, dosazení do protocolDictionary a radioArray[0].IsChecked
					{
						if (userSettings.ProtocolDictionary[userSettings.SelectedPort].SendHex(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) 
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
                    //testBtn.Background = new SolidColorBrush(Color.FromArgb(protocolArray[0].toSend[1], protocolArray[0].toSend[2], protocolArray[0].toSend[3], protocolArray[0].toSend[4])); - příkaz pro zobrazení barvy z hodnot
                }

		}
		public void Refresh()
		{
			if(userSettings.SelectedFunction==1)
			{
				if (userSettings.RadioArray[0].IsChecked == true)
				{                   
                    for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(i), 16); //TODO: Je opraveno?										
					}					
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
            else if (userSettings.SelectedFunction == 0) 
			{
                if (userSettings.RadioArray[0].IsChecked == true)
                {                 
                    for (int i = 0; i < textBoxArray.Length; i++)
                    {						
                        textBoxArray[i].Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getReceivedValue(i), 16); //Sem to při odesílání nemá chodit				
                    }
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
        }
	}
}
//je vhodné přepisovat buňky? teď to funguje tak, že při změně režimu se přepíše obsah textBoxů do toSend, i přesto, že jsou tam shodný hodnoty