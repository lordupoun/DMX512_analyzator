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

/**
 * ToDo:
 * Najít a vymazat
 * ----Nebudou komunikovat stránka se stránkou ale stránky s třídou API (Víc COM by se vždy posílal argument, kterého se to týká) -> Hotovo - předávají si informace v přetížení
 * ----Přidat podporu více COM - mrknout na volný COM porty do systému
 * Přidat features - přebírání hodnot v designech - načtení hodnot pro příslušnej COM a design, konverze hodnot
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
		Dictionary<string, Protocol> protocolDictionary = new Dictionary<String,Protocol>();
		//int currentPage = 1;
		//bool test;
		ListBoxPage listBoxPage;
		TextBoxPage textBoxPage;
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
			textBoxPage = new TextBoxPage(protocolDictionary, radioArray, portBox);
			mainFrame.Navigate(textBoxPage);
		}

		private void ButtonStart_Click(object sender, RoutedEventArgs e)
		{
			protocolDictionary[(String)portBox.SelectedValue].Start();
			buttonStart.IsEnabled = false;
			buttonStop.IsEnabled = true;
			//MessageBox.Show(Convert.ToString(portBox.SelectedValue));
		}

		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
			protocolDictionary[(String)portBox.SelectedValue].Stop(); //setter
			buttonStart.IsEnabled = true;
			buttonStop.IsEnabled = false;
		}

		public void setDataToSend() 
		{

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
			listBoxPage = new ListBoxPage(protocolDictionary, radioArray, portBox);
			mainFrame.Navigate(listBoxPage); 
			//currentPage = 2;
		}

		private void ChangeToTextBoxPage(object sender, RoutedEventArgs e)
		{
			
			mainFrame.Navigate(textBoxPage); 
			//currentPage = 1;
		}

		private void RadioHex_Checked(object sender, RoutedEventArgs e) //smazat
		{
			//textBoxPage.Refresh();
			//format = 1;
			if(windowLoaded==true)
				textBoxPage.Refresh();
			//mainFrame.Navigate.setFormat(1);<-----------------------------------------Tohle musím opravit...
		}

		private void radioBin_Checked(object sender, RoutedEventArgs e)
		{
				textBoxPage.Refresh();
			//format = 3;
			/*if (windowLoaded == true)
				refreshContent();*/
		}

		private void radioDec_Checked(object sender, RoutedEventArgs e)
		{
			textBoxPage.Refresh(); //doifovat
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

		private void portBox_SelectionChanged(object sender, SelectionChangedEventArgs e) //spouští se se startem programu
		{
			if(windowLoaded==true&&(String)portBox.SelectedValue!=null)
			{ 
				if(protocolDictionary.ContainsKey((String)portBox.SelectedValue)==false)
				{ 
					protocolDictionary.Add((String)portBox.SelectedValue, new Protocol((String)portBox.SelectedValue));
				}
					if (protocolDictionary[(String)portBox.SelectedValue].getLoop() == true)
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
			}
			//test = false;
		}
		void refreshContent() //kvůli eventu změna COM portu a změna rádia, kterej je dovolatelnej pouze odsud
		{
			/*//radioBoxy
			//Aktuální stránku
			//Aktuální port
			switch(currentPage)
			{
				case 1:
					mainFrame.Navigate(new TextBoxPage(protocolDictionary, radioArray, portBox));
					break;
				case 2:
					mainFrame.Navigate(new ListBoxPage(protocolDictionary, radioArray, portBox));
					break;
			}*/
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