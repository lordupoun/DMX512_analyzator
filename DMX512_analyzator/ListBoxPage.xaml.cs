﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Globalization;
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

namespace DMX512_analyzator
{
	/// <summary>
	/// Interaction logic for ListBoxPage.xaml
	/// </summary>
	public partial class ListBoxPage : Page
	{
		bool ready;
		int format;
		Protocol[] protocolArray = new Protocol[256];
		private RadioButton[] radioArray;
		Dictionary<string, ProtocolSend> protocolSendDictionary = new Dictionary<String, ProtocolSend>();
        Dictionary<string, ProtocolReceive> protocolReceiveDictionary = new Dictionary<String, ProtocolReceive>();
        //private String selectedPort;
		//private byte selectedFunction;
        public String SelectedPort { get; set; } //Musí být zde, protože instance je založené ve třídě
        public byte SelectedFunction { get; set; } //ref to nejde jednoduše udělat
        public ListBoxPage(Dictionary<String, ProtocolSend> protocolSendDictionary, Dictionary<String, ProtocolReceive> protocolReceiveDictionary, RadioButton[] radioArray, String port,byte function)
		{
			this.protocolSendDictionary = protocolSendDictionary;
            this.protocolReceiveDictionary = protocolReceiveDictionary;
            this.radioArray = radioArray;
			this.SelectedPort = port;
			SelectedFunction = function;
			InitializeComponent();
			ready = true;
			//var mainWindow = (MainWindow)Application.Current.MainWindow;
		}
        public ListBoxPage(Dictionary<String, ProtocolSend> protocolSendDictionary, Dictionary<String, ProtocolReceive> protocolReceiveDictionary, RadioButton[] radioArray, String port)
        {
            this.protocolSendDictionary = protocolSendDictionary;
            this.protocolReceiveDictionary = protocolReceiveDictionary;
            this.radioArray = radioArray;
            this.SelectedPort = port;
            InitializeComponent();
            ready = true;
            //var mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			textBoxA.Text = Convert.ToString(ScrollBarA.Value);
		}

		private void ScrollBarA_KeyUp(object sender, KeyEventArgs e)
		{
			//textBoxA.Text=Convert.ToString(int.Parse(textBoxA.Text) - 1);
		}

		private void ScrollBarA_KeyDown(object sender, KeyEventArgs e)
		{
			//textBoxA.Text = Convert.ToString(int.Parse(textBoxA.Text) +1);
		}

		private void textBoxA_TextChanged(object sender, TextChangedEventArgs e) //vymyslet jak to bude fungovat když bude zaškrtlej radioBox
		{
			ScrollBarA.Value = (int.Parse(textBoxA.Text));
			//int.TryParse(textBoxA.Text, ScrollBarA.Value);

            TextBox boxChanged = (TextBox)sender;
			/*if (byte.TryParse(boxChanged.Text, NumberStyles.HexNumber, null, out device1.toSend[ScrollBarA.Value]) == false) //ošetření dělá Parse, v případě chyby vrátí nulu jako Convert jen je vhodnější
            {
                MessageBox.Show("opravit");
            }*/
			//textBoxB.Text = Convert.ToString(protocolArray[0].getToSendValue(int.Parse(textBoxA.Text)));
			if (ready == true)
			{
				Refresh();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedFunction == 1)
			{ 
			if (radioArray[0].IsChecked == true)
			{
				protocolSendDictionary[SelectedPort].SendHex(textBoxB, int.Parse(textBoxA.Text));
			}
			else if (radioArray[1].IsChecked == true)
			{
				protocolSendDictionary[SelectedPort].SendDec(textBoxB, int.Parse(textBoxA.Text));
			}
			else if (radioArray[2].IsChecked == true)
			{
				protocolSendDictionary[SelectedPort].SendBin(textBoxB, int.Parse(textBoxA.Text));
			}
		}
			/*if (selectedFunction == 1)
			{
				if (radioArray[0].IsChecked == true)
				{
					protocolSendDictionary[SelectedPort].SendHex(textBoxB, int.Parse(textBoxA.Text));
				}
				else if (radioArray[1].IsChecked == true)
				{
					protocolSendDictionary[SelectedPort].SendDec(textBoxB, int.Parse(textBoxA.Text));
				}
				else if (radioArray[2].IsChecked == true)
				{
					protocolSendDictionary[SelectedPort].SendBin(textBoxB, int.Parse(textBoxA.Text));
				}
			}*/
            }
		public void setFormat(int format)
		{
			this.format = format;
		}

		private void ScrollBarA_PreviewTouchUp(object sender, TouchEventArgs e)
		{

		}

		private void ScrollBarA_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			//textBoxA.Text = Convert.ToString(int.Parse(textBoxA.Text) - 1);
		}

		private void ScrollBarA_PreviewTouchDown(object sender, StylusButtonEventArgs e)
		{
		}

		private void ScrollBarA_PreviewTouchUp(object sender, StylusButtonEventArgs e)
		{

		}

		private void ScrollBarA_PreviewStylusButtonDown(object sender, StylusButtonEventArgs e)
		{
			MessageBox.Show("");
			textBoxA.Text = Convert.ToString(int.Parse(textBoxA.Text) + 1);
		}
		public void Refresh()
		{
			if (SelectedFunction == 1)
			{
				if (radioArray[0].IsChecked == true)
				{
					textBoxB.Text = Convert.ToString(protocolSendDictionary[SelectedPort].getToSendValue(int.Parse(textBoxA.Text)), 16);
				}
				if (radioArray[1].IsChecked == true)
				{
					textBoxB.Text = Convert.ToString(protocolSendDictionary[SelectedPort].getToSendValue(int.Parse(textBoxA.Text)));
				}
				if (radioArray[2].IsChecked == true)
				{
					textBoxB.Text = Convert.ToString(protocolSendDictionary[SelectedPort].getToSendValue(int.Parse(textBoxA.Text)), 2);
				}
			}
            if (SelectedFunction == 0)
            {
                if (radioArray[0].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(protocolReceiveDictionary[SelectedPort].getReceivedValue(int.Parse(textBoxA.Text)), 16);
                }
                if (radioArray[1].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(protocolReceiveDictionary[SelectedPort].getReceivedValue(int.Parse(textBoxA.Text)));
                }
                if (radioArray[2].IsChecked == true)
                {
                    textBoxB.Text = Convert.ToString(protocolReceiveDictionary[SelectedPort].getReceivedValue(int.Parse(textBoxA.Text)), 2);
                }
            }
        }
	}
}
