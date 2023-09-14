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
using static System.Net.Mime.MediaTypeNames;
using protokolDMX512;


namespace DMX512_analyzator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] toSend = new byte[513];
        Protocol device1 = new Protocol();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            device1.start();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            device1.stop(); //setter
        }

        public byte[] getTextBoxes() //protokol.cs by musel mít includnutej hlavni namespace což je asi blbost
        {
            toSend[0] = 0x00;
            toSend[1] = 0xFF;
            toSend[2] = 0xFF;
            toSend[3] = 0xFF;
            return toSend;
        }
    }
    
}
//co kdyby měl založit class uživatel?