﻿using System;
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

/**
 * ToDo:
 * Najít a vymazat
 * ----Nebudou komunikovat stránka se stránkou ale stránky s třídou API (Víc COM by se vždy posílal argument, kterého se to týká) -> Hotovo - předávají si informace v přetížení
 * ----Přidat podporu více COM - mrknout na volný COM porty do systému
 * ---Přidat features - přebírání hodnot v designech - načtení hodnot pro příslušnej COM a design, konverze hodnot
 * Čtení
 * To co je v konstruktoru přepsat do setterů
 * Tab pro textboxpage ...
 * Opravit ListBox Page +1
 * ---Původní Layout hodit jako Page, tlačítka nechat v rámci Window - někde k tomu mám komentář
 * Předělat Grafiku, Grid, nastavit MinMax
 * Dodělat Grafiku
 * ---Setter pro toSend[]
 * Nastavení COM portu automaticky
 * Pomalý režim -> odešle jen při stisknutí tlačítka?
 * Přepínač sledovat/zapisovat
 * Pro případ LISTu -> Do listu uchovávat Protocoly (vzít vybranej COM v tabulce z něj vzít číslo, zkontrolovat jestli neexistuje a případně na pozici čísla vytvořit novou instanci), hledat je podle názvu "COM4", nalezenej uložit do proměnný která je připsaná všem metodám (aby nemusel hledat pro každej protokolArray[0] -> všechny najít a nahradit -> respektive passnout proměnnou pro page (kdybych předával mapu, bylo by to takový blbý asi)) -> List je závislej na knihovnách a může se měnit -> je míň robustní teoreticky
 * Pro případ Array -> Do array uchovávat Protocoly, hledat je podle čísla a předávat číslo, oboje obsahuje pouze reference; RAM vs CPU ------- u pole není potřeba trápit ani pamět ani CPU s dalšími operacemi, obzvlášť když přesně vím kolik paměti budu chtít (a není to moc, vzheldem k tomu, že je to reference) -> ale ARRAY způsob se vztahuje k názvu COM -> ale nemůžu hledat Contains(nějakej Protocol) -> na to by vlastně byla lepší Mapa
 * **/
//příště možná postupovat po tlačítkách (pro jednotlivý eventy vytvořit tlačítka s funkcemi
namespace DMX512_analyzator
{   //UI běží na stejném vlákně jako kód
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window //Zachovat Window - v něm mít akorát tlačítka a přepínátka -> tlačítka Start Stop ovládají přímo Protocol class, informace o zaškrtlé volbě se bude přenášet spolu s COM portem v přetížení metody -> vliv na rychlost? asi žádný, informace o stavu radioButton bude uložená přímo v proměnné
	{
		//int format = 1; 
						//Protocol device1 = (Protocol)Application.Current.Properties[new Protocol()]; //Princip více COM portů? - pokud jenom změním COM port, přestane mi vysílat data na původní COM. Musím vytvořit novou instanci, ideálně asi v array[256] (max. Windows), kde se podle toho bude indexovat a hledat. Nabídka COM Port *int*. Tlačítko "vytvořit" spustí novou instanci a případně se bude dát přepnout zpět. GUI se vykreslí podle toSend[] a tlačítka podle smyčky loop (běží/neběží)
		bool windowLoaded = false;
		RadioButton[] radioArray = new RadioButton[3];	
		Dictionary<string, ProtocolSend> protocolSendDictionary = new Dictionary<String, ProtocolSend>();
        Dictionary<string, ProtocolReceive> protocolReceiveDictionary = new Dictionary<String,ProtocolReceive>(); //odesílání - jeden aktivní port je pro odesílání i přijímání stejnej - nejdřív je povypínám a pak k Start tlačítku přidám podmínku, že jestli už je port otevřenej, tak ať pokračuje
		//byte selectedFunction=0;
        //int currentPage = 1;
        //bool test;
        //String selectedPort;
        ListBoxPage listBoxPage;
		TextBoxPage textBoxPage;
		 //Nevidí ho, get a set jsou jako metody a nedostanou se k nim
		public MainWindow()
		{
			
			InitializeComponent();
			radioArray[0] = radioHex;
			radioArray[1] = radioDec;
			radioArray[2] = radioBin;
			
			RefreshPorts();
			portBox.SelectedIndex = 0;
			//selectedPort= (String)portBox.SelectedValue;
            try
			{
                //protocolSendDictionary.Add((String)portBox.SelectedValue, new ProtocolSend((String)portBox.SelectedValue)); //vytvoří mapu otevřených prostředí  (otevřených COM portů)
                protocolReceiveDictionary.Add((String)portBox.SelectedValue, new ProtocolReceive((String)portBox.SelectedValue)); //Page neví jaká je aktuální hodnota // vytvoří se až se překlikne
            }
			catch
			{
				MessageBox.Show("Zapojte prosím analyzátor do USB.");				
			}
			windowLoaded = true;
			textBoxPage = new TextBoxPage(protocolSendDictionary, protocolReceiveDictionary, radioArray, (String)portBox.SelectedValue, 0); //předat jako ref...
			listBoxPage = new ListBoxPage(protocolSendDictionary, protocolReceiveDictionary, radioArray, (String)portBox.SelectedValue, 0);
			mainFrame.Navigate(textBoxPage);
		}

		private void ButtonStart_Click(object sender, RoutedEventArgs e)
		{
			if (radioSend.IsChecked == true)
			{
                protocolSendDictionary[(String)portBox.SelectedValue].Start(); //TODO: Ošetřit vyjímku
			}
			else if (radioReceive.IsChecked == true)
			{
                protocolReceiveDictionary[(String)portBox.SelectedValue].Start();
            }            
            buttonStart.IsEnabled = false;
			buttonStop.IsEnabled = true;
			//MessageBox.Show(Convert.ToString(portBox.SelectedValue));
		}

		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
            if (radioSend.IsChecked == true)
            {
                protocolSendDictionary[(String)portBox.SelectedValue].Stop(); //setter
            }
            else if (radioReceive.IsChecked == true)
            {
                protocolReceiveDictionary[(String)portBox.SelectedValue].Stop();
            }
            buttonStart.IsEnabled = true;
			buttonStop.IsEnabled = false;
		}

		private void test_Click(object sender, RoutedEventArgs e) 
		{
			foreach (String i in SerialPort.GetPortNames())
			{
				MessageBox.Show(i);
			}
		}

		private void ChangeToListBoxPage(object sender, RoutedEventArgs e) 
		{
			listBoxPage = new ListBoxPage(protocolSendDictionary, protocolReceiveDictionary , radioArray, (String)portBox.SelectedValue); //TODO: Destruktor; nakopírovat do funkce (aby nebyl vícekrát v kódu)
			mainFrame.Navigate(listBoxPage); 
			//currentPage = 2;
		}

		private void ChangeToTextBoxPage(object sender, RoutedEventArgs e)
		{
			//textBoxPage = new TextBoxPage(protocolDictionary, radioArray, portBox);
			mainFrame.Navigate(textBoxPage);
			textBoxPage.Refresh();
			//currentPage = 1;
		}

		private void RadioHex_Checked(object sender, RoutedEventArgs e) //smazat
		{
			//textBoxPage.Refresh();
			//format = 1;
			if(windowLoaded==true) //pro default only...
			{ 
				textBoxPage.Refresh();
				listBoxPage.Refresh();
			}
			//mainFrame.Navigate.setFormat(1);<-----------------------------------------Tohle musím opravit...
		}

		private void radioBin_Checked(object sender, RoutedEventArgs e)
		{
			textBoxPage.Refresh();
			listBoxPage.Refresh();
			//format = 3;
			/*if (windowLoaded == true)
				refreshContent();*/
		}

		private void radioDec_Checked(object sender, RoutedEventArgs e)
		{
			textBoxPage.Refresh(); //doifovat <-- Pokud je zapnutá stránka -> refrehsnout to či ono
			listBoxPage.Refresh();
			//format = 2;
			/*if (windowLoaded == true)
				refreshContent();*/
		}

		private void refreshButton_Click(object sender, RoutedEventArgs e)
		{
			RefreshPorts();
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
            //Protocol.SelectedPort =(String)portBox.SelectedValue; //Přiřadí instanci daný 

            //Předá aktuálně zvolený port ostatním stránkám (layoutům); předává se pomocí property, protože String nelze předat referencí
            //Pozor! Musí být editováno pro všechny layouty
            if (windowLoaded == true)
            {
            listBoxPage.SelectedPort = (String)portBox.SelectedValue;
            textBoxPage.SelectedPort = (String)portBox.SelectedValue;


				if ((String)portBox.SelectedValue != null && radioSend.IsChecked == true)
				{
				
					if (protocolSendDictionary.ContainsKey((String)portBox.SelectedValue) == false)
					{
						protocolSendDictionary.Add((String)portBox.SelectedValue, new ProtocolSend((String)portBox.SelectedValue));
                        MessageBox.Show((String)portBox.SelectedValue);
                    }
					if (protocolSendDictionary[(String)portBox.SelectedValue].Started == true)
					{
						buttonStart.IsEnabled = false;
						buttonStop.IsEnabled = true;
					}
					else
					{
						buttonStart.IsEnabled = true;
						buttonStop.IsEnabled = false;
					}
					textBoxPage.Refresh();
					listBoxPage.Refresh();
				}
				else if ((String)portBox.SelectedValue != null && radioReceive.IsChecked == true) //COM port není null
				{
					if (protocolReceiveDictionary.ContainsKey((String)portBox.SelectedValue) == false)
					{
						protocolReceiveDictionary.Add((String)portBox.SelectedValue, new ProtocolReceive((String)portBox.SelectedValue));
					}
					if (protocolReceiveDictionary[(String)portBox.SelectedValue].Started == true)
					{
						buttonStart.IsEnabled = false;
						buttonStop.IsEnabled = true;
					}
					else
					{
						buttonStart.IsEnabled = true;
						buttonStop.IsEnabled = false;
					}
					textBoxPage.Refresh();
					listBoxPage.Refresh();
				}
            }
        }

        private void radioSend_Checked(object sender, RoutedEventArgs e)
        {
			
            listBoxPage.SelectedFunction = 1;
			textBoxPage.SelectedFunction = 1;
            protocolSendDictionary.Add((String)portBox.SelectedValue, new ProtocolSend((String)portBox.SelectedValue));//Defaultní funkce je Receive
            foreach (ProtocolReceive running in protocolReceiveDictionary.Values)
			{
				running.Stop();
			}
			
        }

        private void radioRecieve_Checked(object sender, RoutedEventArgs e)
        {
			
            listBoxPage.SelectedFunction = 0;
			textBoxPage.SelectedFunction = 0;
            textBoxPage.SelectedPort = (String)portBox.SelectedValue;
            foreach (ProtocolSend running in protocolSendDictionary.Values)
            {
                running.Stop();
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