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

/**
 * ToDo:
 * ----Nebudou komunikovat stránka se stránkou ale stránky s třídou API (Víc COM by se vždy posílal argument, kterého se to týká) -> Hotovo - předávají si informace v přetížení
 * Přidat podporu více COM - mrknout na volný COM porty do systému
 * Přidat features - přebírání hodnot v designech, konverze hodnot
 * Opravit ListBox Page +1
 * ---Původní Layout hodit jako Page, tlačítka nechat v rámci Window - někde k tomu mám komentář
 * Předělat Grafiku, Grid, nastavit MinMax
 * Dodělat Grafiku
 * ---Setter pro toSend[]
 * Nastavení COM portu automaticky
 * Pomalý režim -> odešle jen při stisknutí tlačítka?
 * Přepínač sledovat/zapisovat
 * **/
namespace DMX512_analyzator
{   //UI běží na stejném vlákně jako kód
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window //Zachovat Window - v něm mít akorát tlačítka a přepínátka -> tlačítka Start Stop ovládají přímo Protocol class, informace o zaškrtlé volbě se bude přenášet spolu s COM portem v přetížení metody -> vliv na rychlost? asi žádný, informace o stavu radioButton bude uložená přímo v proměnné
    {
		Protocol[] protocolArray = new Protocol[256]; //co to uložit do souboru?		
		private byte[] toSend = new byte[513];
        int format = 1; //Nutno změnit při změně defaultního formátu
        //Protocol device1 = new Protocol();
		//Protocol device1 = (Protocol)Application.Current.Properties[new Protocol()]; //Princip více COM portů? - pokud jenom změním COM port, přestane mi vysílat data na původní COM. Musím vytvořit novou instanci, ideálně asi v array[256] (max. Windows), kde se podle toho bude indexovat a hledat. Nabídka COM Port *int*. Tlačítko "vytvořit" spustí novou instanci a případně se bude dát přepnout zpět. GUI se vykreslí podle toSend[] a tlačítka podle smyčky loop (běží/neběží)
		bool windowLoaded = false;
        //TextBox[] textBoxArray;//=new TextBox[513];
        RadioButton[] radioArray = new RadioButton[3];
        public MainWindow()
        {			
			//Application["Name"] = "pandian";
			protocolArray[0] = new Protocol();//<--------------------------------------------------------Najít a vymazat
            //radioArray = mainGrid.Children.OfType<RadioButton>().Cast<RadioButton>().ToArray();          
			InitializeComponent();
			radioArray[0] = radioHex;
			radioArray[1] = radioDec;
			radioArray[2] = radioBin;
			windowLoaded = true;
			mainFrame.Navigate(new TextBoxPage(protocolArray, radioArray));
		}

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            protocolArray[0].Start(); //tlačítko start bude v každým page, a při startu kontaktuje environment a sdělí mu číslo COM
            buttonStart.IsEnabled = false;
			buttonStop.IsEnabled = true;
			//setDataToSend(); //pak asi i vymazat
		}

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
			protocolArray[0].Stop(); //setter
			buttonStart.IsEnabled = true;
			buttonStop.IsEnabled = false;
		}

        public void setDataToSend() //<----- data na odeslání - vhodné pro event k tlačítku //vždy by se měl aktualizovat jen ten jeden určitej byte, jen jedno určený okýnko (ale ideálně jednou funkcí) //protokol.cs by musel mít includnutej hlavni namespace což je asi blbost //načtení dat buď s enterem nebo se změnou textu
        {
            /*for(int i=0; i<8;i++)
            {
                //device1.toSend[i]=Convert.ToByte(textBoxArray[i].Text, 16);//<-------------------------Taky možnost - ale hází out of range
                if (byte.TryParse(textBoxArray[i].Text, NumberStyles.HexNumber, null, out protocolArray[0].toSend[i])==false)
                {
                    MessageBox.Show(textBoxArray[i].Name);
                }
            }*/
        }

        private void test_Click(object sender, RoutedEventArgs e) //řeší se jestli je datový typ předanej jako hodnota nebo jako reference
        {
            mainFrame.Navigate(new ListBoxPage(protocolArray, radioArray)); //kdybych při tom změnil instanci, světla by problikla
        }

		private void ChangeToListBoxPage(object sender, RoutedEventArgs e) //řeší se jestli je datový typ předanej jako hodnota nebo jako reference
		{
			mainFrame.Navigate(new ListBoxPage(protocolArray, radioArray)); //kdybych při tom změnil instanci, světla by problikla
		}

		private void ChangeToTextBoxPage(object sender, RoutedEventArgs e) //řeší se jestli je datový typ předanej jako hodnota nebo jako reference
		{
			mainFrame.Navigate(new TextBoxPage(protocolArray, radioArray)); //kdybych při tom změnil instanci, světla by problikla
		}

		private void RadioHex_Checked(object sender, RoutedEventArgs e) //smazat
		{
            format = 1;
            //mainFrame.Navigate.setFormat(1);<-----------------------------------------Tohle musím opravit...
		}

		private void radioBin_Checked(object sender, RoutedEventArgs e)
		{
			format = 3;
		}

		private void radioDec_Checked(object sender, RoutedEventArgs e)
		{
			format = 2;
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