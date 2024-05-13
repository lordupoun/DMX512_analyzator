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
	public partial class TextBoxPage : Page, IBasePage
	{
		bool pageLoaded;
		private TextBox[] textBoxArray = new TextBox[513];
		private UserSettings userSettings; //Předává hodnoty všem Page v jednom objektu
		TextBox boxChanged;
		int pageOffset = 0;

		public TextBoxPage(UserSettings userSettings)
		{
			this.userSettings = userSettings;
			InitializeComponent();
			textBoxArray = mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce textboxů na array
			pageLoaded = true;
		}
		public void SetToReceive()
		{
			for (int i = 0; i < textBoxArray.Length; i++)
			{
				textBoxArray[i].IsReadOnly = true;
				textBoxArray[i].Background = Brushes.WhiteSmoke;
			}
			Refresh();//Receive jej sice refreshne sám, ale v případě, že není zapnutý tam zůstane trčet hodnota z Send
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
		public void SetSendReceive_Auto()//automatická funkce, která upraví rozhraní dle aktuální vybraného režimu příjem/odesílání
		{
			if (userSettings.SelectedFunction == 0)
			{
				SetToReceive();

			}
			if (userSettings.SelectedFunction == 1)
			{
				SetToSend();
			}
		}
		private void text_changed(object sender, TextChangedEventArgs e) //Event změny textu v textboxu
		{

			if (pageLoaded == true && userSettings.SelectedFunction == 1)//Zabraňuje pádu, TODO: Najít alternativu
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
        /// <summary>Obnoví data v GUI podle vybraných parametrů.</summary>
        public void Refresh()
		{
			if (userSettings.SelectedFunction == 1)
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
		} //textBoxArray[i].Text = Convert.ToString(packet[i+2]); //Sem to při odesílání nemá chodit
		/// <summary>Vypíše přijatý paket do GUI.</summary>
		public void ShowPacket(byte[] packet)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				if (userSettings.RadioArray[0].IsChecked == true)
				{
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = packet[i + 2 + pageOffset].ToString("X2");

					}
				}
				if (userSettings.RadioArray[1].IsChecked == true)
				{
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(packet[i + 2 + pageOffset]); //Sem to při odesílání nemá chodit	
					}
				}
				if (userSettings.RadioArray[2].IsChecked == true)
				{
					for (int i = 0; i < textBoxArray.Length; i++)
					{
						textBoxArray[i].Text = Convert.ToString(packet[i + 2 + pageOffset], 2).PadLeft(8, '0');
					}
				}
			});
		}

		private void ForwardButton_Click(object sender, RoutedEventArgs e)
		{
            pageOffset += 64; //původní byte bude 64, ale odstraní se
            SetPagePosition();
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
            pageOffset -= 64; //původní byte bude 64, ale odstraní se
            SetPagePosition();
        }

        /// <summary>Chování GUI po přepnutí na zobrazení dalších bytů.</summary>
        private void SetPagePosition()
		{
			pageNumber.Content = (pageOffset+64)/64+"/8";
			l0.Content=(1+pageOffset)+"-"+(8 + pageOffset);
            l1.Content = (9 + pageOffset) + "-" + (16 + pageOffset);
            l2.Content = (17 + pageOffset) + "-" + (24 + pageOffset);
            l3.Content = (25 + pageOffset) + "-" + (32 + pageOffset);
            l4.Content = (33 + pageOffset) + "-" + (40 + pageOffset);
            l5.Content = (41 + pageOffset) + "-" + (48 + pageOffset);
            l6.Content = (49 + pageOffset) + "-" + (56 + pageOffset);
            l7.Content = (57 + pageOffset) + "-" + (64 + pageOffset);
            if (pageOffset+64 > 64)
            {
                textBox0.Visibility = System.Windows.Visibility.Hidden;
				l0.Visibility= System.Windows.Visibility.Visible;
                BackButton.IsEnabled = true;
            }
            else
            {
                textBox0.Visibility = System.Windows.Visibility.Visible;
                l0.Visibility = System.Windows.Visibility.Hidden;
                BackButton.IsEnabled = false;                
            }
			if(pageOffset+64>511)
			{
                ForwardButton.IsEnabled = false;
            }
			else
			{
                ForwardButton.IsEnabled = true;
            }
        }
    }
}
//je vhodné přepisovat buňky? teď to funguje tak, že při změně režimu se přepíše obsah textBoxů do toSend, i přesto, že jsou tam shodný hodnoty