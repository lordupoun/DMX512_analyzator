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
//Event je vlastně něco jako přerušení, nicméně přímo v kódu s vytvořeným GUI to tady nepotřebuju (Je to potřeba buď na uživatelskou reakci, nebo při dokončení něčeho na co se čeká)
namespace DMX512_analyzator
{   //UI běží na stejném vlákně jako kód
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public class UserSettings //Ideálěn Singleton obsahující všechna nastavení mainWindow -- spojuje stránky;; protože třídy z WPF nejdou jednoduše dědit (a stejně by to nešlo, protože bych musel měnit parametry dvou instancí najednou)
	{
		public Dictionary<string, Protocol> ProtocolDictionary { get; set; } //TODO: Odebrat set
		//public Dictionary<string, ProtocolReceive> ProtocolReceiveDictionary { get; set; }
		public RadioButton[] RadioArray { get; set; } //RadioFormatArray
		public String SelectedPort { get; set; } //TODO: Přidat otazníčky
		public int SelectedFunction { get; set; }//když předám jako radioButtony, čas HW se prakticky neušetří, asi to můžu přehodit zpět tak jak to bylo, nebo naopak přehodit SelectedPort, aby vše bylo stejně - ale to pak bude všude psaný přidělování, který je ve vlastnostech objektu stejně už jednou přidělený
    }
	
	public partial class MainWindow : Window //Zachovat Window - v něm mít akorát tlačítka a přepínátka -> tlačítka Start Stop ovládají přímo Protocol class, informace o zaškrtlé volbě se bude přenášet spolu s COM portem v přetížení metody -> vliv na rychlost? asi žádný, informace o stavu radioButton bude uložená přímo v proměnné
	{
		//int format = 1; 
						//Protocol device1 = (Protocol)Application.Current.Properties[new Protocol()]; //Princip více COM portů? - pokud jenom změním COM port, přestane mi vysílat data na původní COM. Musím vytvořit novou instanci, ideálně asi v array[256] (max. Windows), kde se podle toho bude indexovat a hledat. Nabídka COM Port *int*. Tlačítko "vytvořit" spustí novou instanci a případně se bude dát přepnout zpět. GUI se vykreslí podle toSend[] a tlačítka podle smyčky loop (běží/neběží)
		bool windowLoaded = false;
		RadioButton[] radioArray = new RadioButton[3];
        RadioButton[] radioFunctionArray = new RadioButton[3];
        Dictionary<string, Protocol> protocolDictionary = new Dictionary<String, Protocol>(); //TODO: Zrušit - může se nastavovat přímo
        //Dictionary<string, ProtocolReceive> protocolReceiveDictionary = new Dictionary<String,ProtocolReceive>(); //odesílání - jeden aktivní port je pro odesílání i přijímání stejnej - nejdřív je povypínám a pak k Start tlačítku přidám podmínku, že jestli už je port otevřenej, tak ať pokračuje
        ListBoxPage listBoxPage;
		TextBoxPage textBoxPage;
		IBasePage CurrentPage; //Interface stránek (Page) - určuje právě otevřenou stránku
        UserSettings userSettings = new UserSettings();
		//Měl bych instanci Receive a instanci Send - v nich metodu Function - když by se volalo Function volal bych jen SelectedInstance.Function - což by bylo buď Receive nebo Send, stejně jako u Current Page... ale pomohlo by mi to u ostatních CheckBoxů? NEPOMOHLO! Protože oni jsou posunutý ke konkrétní stránce, ve který se odehrává danej Event (text_Changed), protože ten se neodehrává v mainWindow
        //Pokud ale bude SelectedInstance.Function tak program musí vědět co je SelectedInstance (+paměť) a když budu děla ify, jen někde jinde, tak si nepomůžu
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
                //protocolReceiveDictionary.Add((String)portBox.SelectedValue, new ProtocolReceive((String)portBox.SelectedValue)); //Warning: Default = Recieve, Change when changing default radioButton check Page neví jaká je aktuální hodnota // vytvoří se až se překlikne
                protocolDictionary.Add((String)portBox.SelectedValue, new Protocol((String)portBox.SelectedValue));
            }
			catch
			{
				MessageBox.Show("Zapojte prosím analyzátor do USB.");				
			}
			windowLoaded = true;
			userSettings.ProtocolDictionary = protocolDictionary;
            //userSettings.ProtocolReceiveDictionary = protocolReceiveDictionary;
            userSettings.RadioArray = radioArray;
            userSettings.SelectedPort = (String)portBox.SelectedValue; //Předávám Stringem, kvůli nadbytku explicitního castování v jiném případě
            userSettings.SelectedFunction = 0; //Do budoucna předávat jako pole radioButtonů - ale je to asi jedno, eventy tam jsou tak jako tak, to samý radioArray
            //textBoxPage = new TextBoxPage(protocolSendDictionary, protocolReceiveDictionary, radioArray, (String)portBox.SelectedValue, radioFunctionArray); //předat jako ref...
            //listBoxPage = new ListBoxPage(protocolSendDictionary, protocolReceiveDictionary, radioArray, (String)portBox.SelectedValue, radioFunctionArray);

            textBoxPage = new TextBoxPage(userSettings);
			listBoxPage = new ListBoxPage(userSettings);
            mainFrame.Navigate(textBoxPage); //Default
			CurrentPage = textBoxPage; //Default
			CurrentPage.SetToReceive(); //Default

			//MessageBox.Show((String)mainFrame.CurrentSource);
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
			//MessageBox.Show(Convert.ToString(portBox.SelectedValue));
		}

		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
            if (radioSend.IsChecked == true)
            {
                protocolDictionary[(String)portBox.SelectedValue].StopSending(); //setter
            }
            else if (radioReceive.IsChecked == true)
            {
                protocolDictionary[(String)portBox.SelectedValue].StopReceiving();
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
		{ //zakomentoval jsem
			//listBoxPage = new ListBoxPage(protocolSendDictionary, protocolReceiveDictionary , radioArray, (String)portBox.SelectedValue); //TODO: Destruktor; nakopírovat do funkce (aby nebyl vícekrát v kódu)
			mainFrame.Navigate(listBoxPage);
			CurrentPage =(IBasePage)listBoxPage; //Případně hodit jen proměnnou, ale to by zase potom nefungovalo potvrzování bez ifů (teoreticky by to mohl být jen string, ale to v syntaxi nejde, stejně by tam musel být if
			//currentPage = 2;
		}

		private void ChangeToTextBoxPage(object sender, RoutedEventArgs e)
		{
			//textBoxPage = new TextBoxPage(protocolDictionary, radioArray, portBox);
			mainFrame.Navigate(textBoxPage);
			CurrentPage = (IBasePage)textBoxPage;
            CurrentPage.Refresh();
            //currentPage = 1;
        }

		private void RadioHex_Checked(object sender, RoutedEventArgs e) //smazat
		{
			//textBoxPage.Refresh();
			//format = 1;
			if(windowLoaded==true) //pro default only...
			{
                CurrentPage.Refresh();
            }
			//mainFrame.Navigate.setFormat(1);<-----------------------------------------Tohle musím opravit...
		}

		private void radioBin_Checked(object sender, RoutedEventArgs e)
		{
            CurrentPage.Refresh();
            //format = 3;
            /*if (windowLoaded == true)
				refreshContent();*/
        }

		private void radioDec_Checked(object sender, RoutedEventArgs e)
		{
            //TODO: Přidat switch - aby se nepřepisovaly všechny layouty//doifovat <-- Pokud je zapnutá stránka -> refrehsnout to či ono
            CurrentPage.Refresh();//EquippedPage.Refresh(); Page CurrentPage=textBoxPage - nejde musely by být extendnutý; RefreshCurrentPage() - kde jsou manuálně nadefinovaný, přidat Warning; mainFrame.Content otázka -> Frame.Source?; - šlo by to vyřešit přes CurrentSource ale switch bude jednoduší; ne switch
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
            if (windowLoaded == true && portBox.SelectedValue != null)//Event se triggruje ještě když nemá Selcted Value, proto nesmí být null; 
            {
             userSettings.SelectedPort = (String)portBox.SelectedValue;
                if (protocolDictionary.ContainsKey((String)portBox.SelectedValue) == false)
                {
                    protocolDictionary.Add((String)portBox.SelectedValue, new Protocol((String)portBox.SelectedValue)); //Používat tohle---------------------------------------------------
                    //protocolReceiveDictionary.Add((String)portBox.SelectedValue, new ProtocolReceive((String)portBox.SelectedValue)); //Přidá port do obou, kvůli jednoduchosti, až později se řeší jestli odesílá nebo ne - při editaci můžou být stále použity oba                                                                                                  //MessageBox.Show((String)portBox.SelectedValue);//Tady to ještě funguje
                }

                if (radioSend.IsChecked == true) //Celé předělat do metod - třída function -> CurrentFunction.PortChange(); šel by vytvořit objekt userPropertiesSend a userPropertiesRecieve, který by měly definovaný metody pro funkci -> Ale to je BLBOST! Logiku tlačítek musím udělat tam kde jsou tlačítka
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

            userSettings.SelectedFunction = 1;
            CurrentPage.SetToSend();
            //if (protocolSendDictionary.ContainsKey((String)portBox.SelectedValue) == false)
            //{
              //  userSettings.ProtocolSendDictionary.Add((String)portBox.SelectedValue, new ProtocolSend((String)portBox.SelectedValue)); //Používat tohle---------------------------------------------------
                                                                                                                                         //MessageBox.Show((String)portBox.SelectedValue);//Tady to ještě funguje
            //}
            //Defaultní funkce je Receive
            foreach (Protocol running in protocolDictionary.Values)
			{
				//running.Stop();
			}
            buttonStart.IsEnabled = true;
            buttonStop.IsEnabled = false;
        }

        private void radioRecieve_Checked(object sender, RoutedEventArgs e)
        {
			if(windowLoaded==true)
			{ 
            userSettings.SelectedFunction = 0; //TODO: Vrátit na předávání jako WPF objekty; je vázáno k instanci, musím předávat objektem
				//metoda zakázat textboxy
				CurrentPage.SetToReceive();
                //if (protocolReceiveDictionary.ContainsKey((String)portBox.SelectedValue) == false)
               // {
                //    protocolReceiveDictionary.Add((String)portBox.SelectedValue, new ProtocolReceive((String)portBox.SelectedValue));
               // }
                //currentlyUsedID.Lock() Místo textBoxPage.Lock
                //textBoxPage.SelectedPort = (String)portBox.SelectedValue;
                foreach (Protocol running in protocolDictionary.Values)
            {
                //running.Stop();
            }
                buttonStart.IsEnabled = true;
                buttonStop.IsEnabled = false;
            }
        }
    }
	//DALO BY SE!! ---- skladovat aktuálně uloženou stránku a pak 
    
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