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
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
//using static System.Net.Mime.MediaTypeNames;
//using protokolDMX512;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel;
/**FAQ:
 * Proč jsem na předávání použil objekt - protože třídy od WPF nejdou pořádně dědit a interface neumožňuje vložení proměnných (navíc i kdyby, byly by stále vázané k instanci)
 * řekni si zda nepoužíváš if zbytečně!
 * **/

/**
 * ToDo:
 * 
 * Čtení
 * Tab pro textboxpage ...
 * Opravit ListBox Page +1
 * Dodělat Grafiku
 * Pomalý režim -> odešle jen při stisknutí tlačítka? - ne
 * **/
//Event je vlastně něco jako přerušení, nicméně přímo v kódu s vytvořeným GUI to tady nepotřebuju (Je to potřeba buď na uživatelskou reakci, nebo při dokončení něčeho na co se čeká)
namespace DMX512_analyzator
{
	public interface IBasePage
	{
		/// <summary>Znovu načte stránku.</summary>
		void Refresh();
		/// <summary>Nastaví GUI stránky na přijímání signálu.</summary>
		void SetToReceive();
		/// <summary>Nastaví GUI stránky na odesílání signálu a znovu načte hodnoty.</summary>
		void SetToSend();
	}
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public class UserSettings //Ideálěn Singleton obsahující všechna nastavení mainWindow -- spojuje stránky; protože třídy z WPF nejdou jednoduše dědit (a stejně by to nešlo, protože bych musel měnit parametry dvou instancí najednou)
	{
		public Dictionary<string, Protocol> ProtocolDictionary { get; set; } //TODO: Odebrat set
		public RadioButton[] RadioArray { get; set; }
		public String SelectedPort { get; set; } //TODO: Přidat otazníčky
		public int SelectedFunction { get; set; }//když předám jako radioButtony, čas HW se prakticky neušetří, asi to můžu přehodit zpět tak jak to bylo, nebo naopak přehodit SelectedPort, aby vše bylo stejně - ale to pak bude všude psaný přidělování, který je ve vlastnostech objektu stejně už jednou přidělený
	}

	public partial class MainWindow : Window
	{
		bool windowLoaded = false;
		RadioButton[] radioArray = new RadioButton[3];
		RadioButton[] radioFunctionArray = new RadioButton[3];
		Dictionary<string, Protocol> protocolDictionary = new Dictionary<String, Protocol>(); //TODO: Zrušit - může se nastavovat přímo
		ListBoxPage listBoxPage;
		TextBoxPage textBoxPage;
		IBasePage CurrentPage; //Interface stránek (Page) - určuje právě otevřenou stránku
		UserSettings userSettings = new UserSettings();
		public MainWindow()
		{
			InitializeComponent();
			radioArray[0] = radioHex;
			radioArray[1] = radioDec;
			radioArray[2] = radioBin;
			RefreshPorts();
			portBox.SelectedIndex = 0;
			try
			{
				protocolDictionary.Add((String)portBox.SelectedValue, new Protocol((String)portBox.SelectedValue));
			}
			catch
			{
				MessageBox.Show("Zapojte prosím analyzátor do USB.");
			}
			windowLoaded = true;
			userSettings.ProtocolDictionary = protocolDictionary;
			userSettings.RadioArray = radioArray;
			userSettings.SelectedPort = (String)portBox.SelectedValue; //Předávám Stringem, kvůli nadbytku explicitního castování v jiném případě
			userSettings.SelectedFunction = 0;
			textBoxPage = new TextBoxPage(userSettings);
			listBoxPage = new ListBoxPage(userSettings);
			mainFrame.Navigate(textBoxPage); //Default
			CurrentPage = textBoxPage; //Default
			CurrentPage.SetToReceive(); //Default
		}

		private void ButtonStart_Click(object sender, RoutedEventArgs e)
		{
			if (radioSend.IsChecked == true)
			{
				protocolDictionary[(String)portBox.SelectedValue].StartSending(); //TODO: Ošetřit vyjímku
			}
			else if (radioReceive.IsChecked == true)
			{
				protocolDictionary[(String)portBox.SelectedValue].StartReceiving();
			}
			buttonStart.IsEnabled = false;
			buttonStop.IsEnabled = true;
		}

		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
			if (radioSend.IsChecked == true)
			{
				protocolDictionary[(String)portBox.SelectedValue].StopSending();
			}
			else if (radioReceive.IsChecked == true)
			{
				protocolDictionary[(String)portBox.SelectedValue].StopReceiving();
			}
			buttonStart.IsEnabled = true;
			buttonStop.IsEnabled = false;
		}

		private void ChangeToListBoxPage(object sender, RoutedEventArgs e)
		{
			mainFrame.Navigate(listBoxPage);
			CurrentPage = (IBasePage)listBoxPage;
			CurrentPage.Refresh();
		}

		private void ChangeToTextBoxPage(object sender, RoutedEventArgs e)
		{
			mainFrame.Navigate(textBoxPage);
			CurrentPage = (IBasePage)textBoxPage;
			CurrentPage.Refresh();
		}

		private void RadioHex_Checked(object sender, RoutedEventArgs e) //smazat
		{
			if (windowLoaded == true)
			{
				CurrentPage.Refresh();
			}
		}

		private void radioBin_Checked(object sender, RoutedEventArgs e)
		{
			CurrentPage.Refresh();
		}

		private void radioDec_Checked(object sender, RoutedEventArgs e)
		{
			CurrentPage.Refresh();
		}

		private void refreshButton_Click(object sender, RoutedEventArgs e)
		{
			RefreshPorts(); //TODO: Ošetřit přepnutí při běhu
		}
		private void RefreshPorts()
		{
			portBox.Items.Clear();
			foreach (String i in SerialPort.GetPortNames())
			{
				portBox.Items.Add(i);
			}
			portBox.SelectedIndex = 0;
		}

		private void portBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //Vybere nový port (jinou instanci)
		{
			//Předá aktuálně zvolený port ostatním stránkám (layoutům); předává se pomocí property, protože String nelze předat referencí
			//Pozor! Musí být editováno pro všechny layouty
			if (windowLoaded == true && portBox.SelectedValue != null)//Event se triggruje ještě když nemá Selcted Value, proto nesmí být null; 
			{
				userSettings.SelectedPort = (String)portBox.SelectedValue;
				if (protocolDictionary.ContainsKey((String)portBox.SelectedValue) == false)
				{
					protocolDictionary.Add((String)portBox.SelectedValue, new Protocol((String)portBox.SelectedValue));
				}

				if (radioSend.IsChecked == true)
				{

					if (protocolDictionary[(String)portBox.SelectedValue].Sending == true)//TODO: ElseIf
					{
						buttonStart.IsEnabled = false;
						buttonStop.IsEnabled = true;
					}
					else
					{
						buttonStart.IsEnabled = true;
						buttonStop.IsEnabled = false;
					}


				}
				else if (radioReceive.IsChecked == true) //COM port není null
				{
					if (protocolDictionary[(String)portBox.SelectedValue].Receiving == true)
					{
						buttonStart.IsEnabled = false;
						buttonStop.IsEnabled = true;
					}
					else
					{
						buttonStart.IsEnabled = true;
						buttonStop.IsEnabled = false;
					}

				}
				CurrentPage.Refresh();
			}
		}
		private void radioSend_Checked(object sender, RoutedEventArgs e)
		{

			userSettings.SelectedFunction = 1; //TODO: Předělat zpět na array checkboxu
			CurrentPage.SetToSend();
			if (protocolDictionary[(String)portBox.SelectedValue].Sending == true)//TODO: ElseIf
			{
				buttonStart.IsEnabled = false;
				buttonStop.IsEnabled = true;
			}
			else
			{
				buttonStart.IsEnabled = true;
				buttonStop.IsEnabled = false;
			}
		}

		private void radioReceive_Checked(object sender, RoutedEventArgs e)
		{
			if (windowLoaded == true)
			{
				userSettings.SelectedFunction = 0;
				CurrentPage.SetToReceive();
				if (protocolDictionary[(String)portBox.SelectedValue].Receiving == true)
				{
					buttonStart.IsEnabled = false;
					buttonStop.IsEnabled = true;
				}
				else
				{
					buttonStart.IsEnabled = true;
					buttonStop.IsEnabled = false;
				}
			}
		}


	}
}
//co kdyby měl založit class uživatel?
//načtení
/*IEnumerable<TextBox> TextboxCollection = rootControl.Children.OfType<TextBox>();
            foreach(TextBox tb in TextboxCollection)
            {
                tb.Text = "Text";
            }*/
//kliknutí na radioButton překonvertuje obsah všech buněk na příslušnej formát
//PORT by se mohl předávat přes metodu set() -> nemusel by se na všechno předávat odkaz, ale zase je potřeba řešit jestli ten Page už existuje -> teď zbytečně předávám celej portBox, ale aspoň pokaždý nemusím nastavovat setPort() nebo setHex <- ale zase je to asi pomalejší
//Nemusel bych hrabat v Mapě pokaždé - a potom pročistit zbytek kódu -> dát pryč to, co je v konstruktoru
//Objekt by nemusel existovat hned, ale až potom co zadám výzvu -> to znamená změním design -> vadí něco, že existuje hned? stejně nic nedělá jen zabírá paměť - stejně ten program musí vědět, že nějakej objekt existuje
/*...Send : Communicate tzn. Function
Send.ToDictionary
Receive.ToDictionary
-> vyžaduje aby byla vždy nastavená proměnná SelectedComminicate = Send/Receive
->přidat do eventu ButtonChecked
Stejne tak bych musel mít udělaný
Hexadec : Format
SelectedFormat=Hex
SelectedFormat.Send
Ale ono se to kombinuje - tzn.
Odešli v hex
Prijmi v dec - bylo by toho hodne a bylo by to zmatecny a o tom oop neni

Jedine ze by byla jedna velka trida, kde je definovano vse - ale to by tam pak byly ify, ne? Ano a hlavne by to razeni postradalo logiku, protoze takhle predavam akorat veci z MainWindow do TextBoxPage a jinak by MW i TBP museli volat jednu třídu navíc. Ta vy navíc musel mít instanci, která by se musela předávat v konstruktoru. Pak by se volalo
Instance.udelej(formatTakovy, funkceTakova, portTakovy) - přemýšlej nad tím - to by byl vlastně abstraction layer - programatorovi by se to lépe programovalo ale byli by tam zase samé ify - mohl bych to dělat i teď asi, od TBP mám instanci - a taky bych to dělal, jenže potřebuju nastavovat dvěma třídám najednou, a to můžu jen přes objekt (bez duplicity) a nebo přes IBasePage


... Nebo:
Na selectedPage není vlastnost
Ale na IsChecked ano - mohu využívat aniž bych musel vytvářet pamatující proměnné
---> nic v proměnných, vše ve vlastnostech

Proč mám send a receive solo? Asi zbytecne

Protocol
Bool sending
Bool receiving

Kdybych nepredaval pres objekt, ale metodama, musek bych mit metodu pro kazdej spustenej layout.
A i kdyby byly zastreseny abstract classou (coz nemuzou, je tam jen interface bez prennych)

1)kouknout jestli spojit send a receive
2)kouknout jestli by interfacem nešli zprehlednit metody
Nejsou sendy v page vsude stejny? Nejsou, rozdily by dosadit v ramci metod, ale ty metody v page jsou stejne vazany na event. I kdyz by to slo, byl by to zmatek
*/