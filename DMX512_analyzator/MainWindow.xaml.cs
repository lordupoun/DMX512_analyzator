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
using protokolDMX512;
using System.Globalization;

/**
 * ToDo:
 * Nebudou komunikovat stránka se stránkou ale stránky s třídou API (Víc COM by se vždy posílal argument, kterého se to týká)
 * Předělat Grafiku, Grid, nastavit MinMax
 * Setter pro toSend[]
 * Nastavení COM portu automaticky
 * Pomalý režim -> odešle jen při stisknutí tlačítka?
 * **/
namespace DMX512_analyzator
{   //UI běží na stejném vlákně jako kód
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] toSend = new byte[513];
        Protocol device1 = new Protocol();
        bool windowLoaded = false;
        TextBox[] textBoxArray;//=new TextBox[513];
    public MainWindow()
        {
            InitializeComponent();
            textBoxArray= mainGrid.Children.OfType<TextBox>().Cast<TextBox>().ToArray(); //Castování kolekce na array
            windowLoaded = true;
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            device1.Start();
            //setDataToSend(); //pak asi i vymazat
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            device1.Stop(); //setter
        }

        public void setDataToSend() //<----- data na odeslání - vhodné pro event k tlačítku //vždy by se měl aktualizovat jen ten jeden určitej byte, jen jedno určený okýnko (ale ideálně jednou funkcí) //protokol.cs by musel mít includnutej hlavni namespace což je asi blbost //načtení dat buď s enterem nebo se změnou textu
        {
            for(int i=0; i<8;i++)
            {
                //device1.toSend[i]=Convert.ToByte(textBoxArray[i].Text, 16);//<-------------------------Taky možnost - ale hází out of range
                if (byte.TryParse(textBoxArray[i].Text, NumberStyles.HexNumber, null, out device1.toSend[i])==false)
                {
                    MessageBox.Show(textBoxArray[i].Name);
                }
            }
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {
            //setDataToSend();
            //mainFrame.Content = new CustomValuePage();
            mainFrame.Navigate(new ListBoxPage());
        }

        private void text_changed(object sender, TextChangedEventArgs e) //<- data na odeslání při změně textu
        {
            TextBox boxChanged = (TextBox)sender;     
            if (windowLoaded == true)//zabrani padu - pak odstranit
            {
                if(radioHex.IsChecked==true) //------------tyhle řádky by nemusely být duplicitně //-----------------------------Přehodit do jiné třídy
                { 
                    if (byte.TryParse(boxChanged.Text, NumberStyles.HexNumber, null, out device1.toSend[Array.IndexOf(textBoxArray, boxChanged)]) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
                    {
                        MessageBox.Show("opravit");
                    }
                }
                if (radioDec.IsChecked == true)
                {
                    if (byte.TryParse(boxChanged.Text, out device1.toSend[Array.IndexOf(textBoxArray, boxChanged)]) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
                    {
                        MessageBox.Show("opravit");
                    }
                }
                if (radioBin.IsChecked == true)
                {
                    device1.toSend[Array.IndexOf(textBoxArray, boxChanged)]= Convert.ToByte(boxChanged.Text, 2);
                }
                testBtn.Background = new SolidColorBrush(Color.FromArgb(device1.toSend[1], device1.toSend[2], device1.toSend[3], device1.toSend[4]));
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