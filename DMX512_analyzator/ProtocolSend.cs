﻿using System;
//using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System.Globalization;
using System.Windows;

namespace DMX512_analyzator
{
	public class Protocol
	{
		private byte[] toSend = new byte[513]; //Buď můžu editovat tenhle byte, nebo dávat byte do metody Send //zanechat private a dát setter
		private byte[] toReceive = new byte[513];
		private String port;
		public bool Sending { get; private set; }
		public bool Receiving { get; private set; }
		//private bool isOpen;
		//private bool loop; //do konstruktoru přidat COM port atd.....
		//String port;
		SerialPort sp = new SerialPort();
		//public Protocol() //Zvolení portu vytvoří novou instanci (pokud ještě není vytvořena)
		//{
		//
		//}
		public Protocol(String port) //Zvolení portu vytvoří novou instanci (pokud ještě není vytvořena)
		{
			Sending = false;
			Receiving = false;
			//bool isOpen=false;
            this.port = port;
		}
		private async Task Send()
		{
			//MessageBox.Show("test");
			sp.Write(toSend, 0, toSend.Length);
			await Task.Delay(30); //přidat režim kompatibility -> 1ms pro FTDI; 30ms pro ostatní
			sp.BreakState = true; //dřív než pošle, přepíše??
			await Task.Delay(1); //30 aby to fungovalo i pro další převodníky//lze použít i timer
			sp.BreakState = false; //přidat ještě jednou Delay(MAB)
			 //------------------- Mohlo by být await na Write Zde najdou uplatnění zejména IO bound operace, které nám pomohou snížit počet vláken v aplikaci. V případě tohoto typu operací není žádná činnost prováděna na dalším vlákně vaší aplikace, ale čeká se na odpověď jiného systému (např. databáze).                
		}
		public async Task StartSending()
		{
			//
			if (sp.IsOpen == false)
			{
				//sp.IsOpen = true;
				sp.PortName = port; //Nastavení COM portu v rámci konstruktoru
				sp.BaudRate = 250000;
				sp.Parity = Parity.None;
				sp.DataBits = 8;
				sp.StopBits = StopBits.Two;
				sp.Handshake = Handshake.None;
				sp.ReadTimeout = 500;
				sp.WriteTimeout = 500;
				sp.Open(); //přidat try catch pokud se neotevře
			}
			Sending = true;
			
			//for (int i = 0; i < toSend.Length; i++)
			//	toSend[i] = 0;
			while (Sending == true) //možná přidat do send
			{
				await Send();
			}
		}
		public async Task StartReceiving()
		{
			//
			if (sp.IsOpen == false)
			{
				//sp.IsOpen = true;
				sp.PortName = port; //Nastavení COM portu v rámci konstruktoru
				sp.BaudRate = 250000;
				sp.Parity = Parity.None;
				sp.DataBits = 8;
				sp.StopBits = StopBits.Two;
				sp.Handshake = Handshake.None;
				sp.ReadTimeout = 500;
				sp.WriteTimeout = 500;
				sp.Open(); //přidat try catch pokud se neotevře
			}
			while (Sending == true) //možná přidat do send
			{
				await Receive();
			}
		}
		public async Task Receive()
		{
			await Task.Delay(30);
		}
//for (int i = 0; i < toSend.Length; i++)
//	toSend[i] = 0;

		
		public void StopReceiving()
		{
		Receiving = false;
			if(Sending==false)
			{ 
            //isOpen = false;
			sp.Close();
			}
		}
		public void StopSending()
		{
		Sending = false;
			if(Receiving==false)
			{ 
			//Started = false;
			sp.Close();
			}
		}
	//Předělat BoxChanged na string
	/// <summary>Odešle hexadecimální obsah textboxu jako byte s int číslem. Vrací true v případě úspěchu, false v případě neúspěchu.</summary>
	public bool SendHex(TextBox BoxChanged, int index)
		{
			return byte.TryParse(BoxChanged.Text, NumberStyles.HexNumber, null, out toSend[index]); //out getToSend();
		}

		/// <summary>Odešle dekadický obsah textboxu na příslušný byte. Vrací true v případě úspěchu, false v případě neúspěchu.</summary>
		public bool SendDec(TextBox BoxChanged, int index)
		{
			return byte.TryParse(BoxChanged.Text, out toSend[index]);
		}

		/// <summary>Odešle binární obsah textboxu na příslušný byte. Vrací true v případě úspěchu, false v případě neúspěchu.</summary>
		public void SendBin(TextBox BoxChanged, int index)
		{
			toSend[index] = Convert.ToByte(BoxChanged.Text, 2);
		}
		/// <summary>Odešle byte s příslušným indexem, v případě, že je null, odešle nulu</summary>
		public byte getToSendValue(int index)//TODO: Předělat na property
		{
			/*if (toSend[index] == null)
				return 0;
			else*/
				return toSend[index];
		}
		public byte getReceivedValue(int index)//TODO: Předělat na property
		{
			/*if (toSend[index] == null)
				return 0;
			else*/
			return toSend[index];
		}
		/*public byte[] getToSend()
		{
			//if (toSend[index] == null)
			//	return 0;
			//else
			return toSend;
		}*/
	}
}
//metody třídy prostředí budou vyžadovat číslo portu a podle toho se bude využívat instance
//výběrem COM portu se vždycky zobrazí jeho stav a příslušný informace