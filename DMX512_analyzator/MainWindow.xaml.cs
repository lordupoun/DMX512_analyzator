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
 * Nebudou komunikovat stránka se stránkou ale stránky s třídou API (Víc COM by se vždy posílal argument, kterého se to týká)
 * Předělat Grafiku, Grid, nastavit MinMax
 * Setter pro toSend[]
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
        //Protocol device1 = new Protocol();
		//Protocol device1 = (Protocol)Application.Current.Properties[new Protocol()]; //Princip více COM portů? - pokud jenom změním COM port, přestane mi vysílat data na původní COM. Musím vytvořit novou instanci, ideálně asi v array[256] (max. Windows), kde se podle toho bude indexovat a hledat. Nabídka COM Port *int*. Tlačítko "vytvořit" spustí novou instanci a případně se bude dát přepnout zpět. GUI se vykreslí podle toSend[] a tlačítka podle smyčky loop (běží/neběží)
		bool windowLoaded = false;
        TextBox[] textBoxArray;//=new TextBox[513];
        public MainWindow()
        {
			//Application["Name"] = "pandian";
			protocolArray[0] = new Protocol();
			InitializeComponent();
            textBoxArray= mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce na array
            windowLoaded = true;
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
            for(int i=0; i<8;i++)
            {
                //device1.toSend[i]=Convert.ToByte(textBoxArray[i].Text, 16);//<-------------------------Taky možnost - ale hází out of range
                if (byte.TryParse(textBoxArray[i].Text, NumberStyles.HexNumber, null, out protocolArray[0].toSend[i])==false)
                {
                    MessageBox.Show(textBoxArray[i].Name);
                }
            }
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new ListBoxPage(protocolArray)); //kdybych při tom změnil instanci, světla by problikla
        }

        private void text_changed(object sender, TextChangedEventArgs e) //<- data na odeslání při změně textu
        {
            TextBox boxChanged = (TextBox)sender;     
            if (windowLoaded == true)//zabrani padu - pak odstranit
            {
                if(radioHex.IsChecked==true) //------------tyhle řádky by nemusely být duplicitně //-----------------------------Přehodit do jiné třídy
                {
                    if (protocolArray[0].SendHex(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) ==false) //když bude celá metoda pryč, nemůžu přistupovat k TextBoxArray, když jen část, nemůžu ji sdílet...
                    {
                        MessageBox.Show("opravit"); //out device1.toSend[Array.IndexOf(textBoxArray, boxChanged)
					}
                }
                else if (radioDec.IsChecked == true)
                {
					if (protocolArray[0].SendDec(boxChanged, Array.IndexOf(textBoxArray, boxChanged)) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
					{
                        MessageBox.Show("opravit");
                    }
                }
                else if (radioBin.IsChecked == true)
                {
					protocolArray[0].SendBin(boxChanged, Array.IndexOf(textBoxArray, boxChanged));
                }
                testBtn.Background = new SolidColorBrush(Color.FromArgb(protocolArray[0].toSend[1], protocolArray[0].toSend[2], protocolArray[0].toSend[3], protocolArray[0].toSend[4]));
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