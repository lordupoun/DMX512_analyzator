using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class ListBoxPage : Page, IBasePage
    {
		bool pageLoaded;		
        private UserSettings userSettings;
        public ListBoxPage(UserSettings userSettings)
        {
            this.userSettings = userSettings;//předá všechny informace z MainWindow
            InitializeComponent();
            pageLoaded = true;
        }

		public void SetToReceive()
		{
			textBoxB.IsEnabled = false;
			ConfirmButton.IsEnabled = false;
			Refresh(); //Receive jej sice refreshne sám, ale v případě, že není zapnutý tam zůstane trčet hodnota z Send
        }
		public void SetToSend()
		{
            textBoxB.IsEnabled = true;
            ConfirmButton.IsEnabled = true;
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
        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			textBoxA.Text = Convert.ToString(ScrollBarA.Value);
		}

		private void textBoxA_TextChanged(object sender, TextChangedEventArgs e) 
		{
			int parsedValue;
			if(int.TryParse(textBoxA.Text, out parsedValue))
			{
                ScrollBarA.Value = parsedValue;
				if(parsedValue>512)
				textBoxA.Text = "0";
            }
			else
			{
                ScrollBarA.Value = 0;
				textBoxA.Text = "0";
            }			
			//int.TryParse(textBoxA.Text, ScrollBarA.Value);

            TextBox boxChanged = (TextBox)sender;
			/*if (byte.TryParse(boxChanged.Text, NumberStyles.HexNumber, null, out device1.toSend[ScrollBarA.Value]) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
            {
                MessageBox.Show("opravit");
            }*/
			//textBoxB.Text = Convert.ToString(protocolArray[0].getToSendValue(int.Parse(textBoxA.Text)));
			if (pageLoaded == true)
			{
				Refresh();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (userSettings.SelectedFunction == 1)
			{ 
				if (userSettings.RadioArray[0].IsChecked == true)
				{
						userSettings.ProtocolDictionary[userSettings.SelectedPort].SendHex(textBoxB, int.Parse(textBoxA.Text));
				}
				else if (userSettings.RadioArray[1].IsChecked == true)
				{
						userSettings.ProtocolDictionary[userSettings.SelectedPort].SendDec(textBoxB, int.Parse(textBoxA.Text));
				}
				else if (userSettings.RadioArray[2].IsChecked == true)
				{
						userSettings.ProtocolDictionary[userSettings.SelectedPort].SendBin(textBoxB, int.Parse(textBoxA.Text));
				}
			}
        }

		private void ScrollBarA_PreviewStylusButtonDown(object sender, StylusButtonEventArgs e)
		{
			MessageBox.Show("");
			textBoxA.Text = Convert.ToString(int.Parse(textBoxA.Text) + 1);
		}
        /// <summary>Obnoví data v GUI podle uživatelem zvoleného nastavení.</summary>
        public void Refresh()
		{
			if (userSettings.SelectedFunction == 1)
			{
				if (userSettings.RadioArray[0].IsChecked == true)
				{
					textBoxB.Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(int.Parse(textBoxA.Text)), 16);
				}
				if (userSettings.RadioArray[1].IsChecked == true)
				{
					textBoxB.Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(int.Parse(textBoxA.Text)));
				}
				if (userSettings.RadioArray[2].IsChecked == true)
				{
					textBoxB.Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getToSendValue(int.Parse(textBoxA.Text)), 2);
				}
			}
            if (userSettings.SelectedFunction == 0)//Prozatím se nakopírují stejná data
            {				
                if (userSettings.RadioArray[0].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getReceivedValue(int.Parse(textBoxA.Text)), 16);
                }
                if (userSettings.RadioArray[1].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getReceivedValue(int.Parse(textBoxA.Text)));
                }
                if (userSettings.RadioArray[2].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(userSettings.ProtocolDictionary[userSettings.SelectedPort].getReceivedValue(int.Parse(textBoxA.Text)), 2);
                }
            }
        }
        /// <summary>Vypíše přijatý paket do GUI.</summary> //TODO: V přetížení předávat i offset čtecího algoritmu (délku hlavičky)
        public void ShowPacket(byte[] packet)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (userSettings.RadioArray[0].IsChecked == true)
                {
                    textBoxB.Text= Convert.ToString(packet[int.Parse(textBoxA.Text)+2], 16);
                }
                if (userSettings.RadioArray[1].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(packet[int.Parse(textBoxA.Text) + 2]);
                }
                if (userSettings.RadioArray[2].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(packet[int.Parse(textBoxA.Text) + 2], 2).PadLeft(8, '0');
                }
            });
        }
    }
}
