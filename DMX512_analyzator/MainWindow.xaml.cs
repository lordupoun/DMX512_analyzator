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
        TextBox[] textBoxArray=new TextBox[513];
        public MainWindow()
        {
            InitializeComponent();
            windowLoaded = true;
            /*for(int i=0;i<8;i++)
            {
                textBoxArray[i]=textBox1;
            }*/
            textBoxArray[0] = textBox0;
            textBoxArray[1] = textBox1;
            textBoxArray[2] = textBox2;
            textBoxArray[3] = textBox3;
            textBoxArray[4] = textBox4;
            textBoxArray[5] = textBox5;
            textBoxArray[6] = textBox6;
            textBoxArray[7] = textBox7; //pro získání byte ze stringu lze použít encoding, co je lepší? Parse/Convert/Encoding
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            device1.Start();
            setDataToSend(); //pak asi i vymazat
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            device1.Stop(); //setter
        }

        public void setDataToSend() //vždy by se měl aktualizovat jen ten jeden určitej byte, jen jedno určený okýnko (ale ideálně jednou funkcí) //protokol.cs by musel mít includnutej hlavni namespace což je asi blbost //načtení dat buď s enterem nebo se změnou textu
        {
            for(int i=0; i<8; i++)
            {
                //bool parseSuccess = byte.Parse(textBoxArray[i].Text, NumberStyles.HexNumber, out device1.toSend[i]);
                //bool test = byte.TryParse(textBoxArray[i].Text, NumberStyles.HexNumber, out device1.toSend[i]
                
                //device1.toSend[i]=Convert.ToByte(textBoxArray[i].Text, 16);//<-------------------------Taky možnost - ale hází out of range
                if (byte.TryParse(textBoxArray[i].Text, NumberStyles.HexNumber, null, out device1.toSend[i])==false)
                {
                    MessageBox.Show("opravit");
                }
                //device1.toSend[i] = Convert.FromHexString(textBoxArray[i].Text);
                //device1.toSend[i] = (byte)textBoxArray[i].Text;
            }
            /*device1.toSend[0] = byte.Parse(textBox0.Text); //přenastavit pro různá zařízení
            device1.toSend[1] = byte.Parse(textBox1.Text, NumberStyles.HexNumber);
            device1.toSend[2] = byte.Parse(textBox2.Text, NumberStyles.HexNumber);//.ToByte(textBox1.Text, 16); <- promine null
            device1.toSend[3] = byte.Parse(textBox3.Text, NumberStyles.HexNumber);// Convert.ToByte(textBox1.Text, 16);
            device1.toSend[4] = byte.Parse(textBox4.Text, NumberStyles.HexNumber);*/
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {
            setDataToSend();
        }

        private void text_changed(object sender, TextChangedEventArgs e)
        {
            if(windowLoaded==true)//zabrani padu
            setDataToSend();
        }
    }
    
}
//co kdyby měl založit class uživatel?
//načtení