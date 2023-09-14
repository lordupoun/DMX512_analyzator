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
        Protocol device1 = new Protocol();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {            
           device1.Start();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            device1.Stop(); //setter
        }
    }
}
//co kdyby měl založit class uživatel?