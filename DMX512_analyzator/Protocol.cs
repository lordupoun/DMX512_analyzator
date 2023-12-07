using System;
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
		private byte[] toSend = new byte[513];
		private byte[] toReceive = new byte[513];
		private String port;
		public bool Sending { get; private set; }
		public bool Receiving { get; private set; }
		SerialPort sp = new SerialPort();

		public Protocol(String port) //Zvolení portu vytvoří novou instanci (pokud ještě není vytvořena)
		{
			Sending = false;
			Receiving = false;
			//bool isOpen=false;
            this.port = port;
		}
		private async Task Send()
		{
			sp.Write(toSend, 0, toSend.Length);
			await Task.Delay(30); //přidat režim kompatibility -> 1ms pro FTDI; 30ms pro ostatní
			sp.BreakState = true; //dřív než pošle, přepíše??
			await Task.Delay(1); //30 aby to fungovalo i pro další převodníky//lze použít i timer
			sp.BreakState = false; //přidat ještě jednou Delay(MAB)
			 //-------------------               
		}
		public async Task StartSending()
		{
			//
			if (sp.IsOpen == false)//TODO: Odstranit, tlačítka uživatele nenechají kliknout když by bylo odesílání spuštěné ---- Pozor to není pravda, toto je ochrana před druhým otevření portu sp
			{
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

			while (Sending == true)
			{
				await Send();
			}
		}
		public async Task StartReceiving()
		{
			//
			if (sp.IsOpen == false)
			{
				sp.PortName = port;
				sp.BaudRate = 250000;
				sp.Parity = Parity.None;
				sp.DataBits = 8;
				sp.StopBits = StopBits.Two;
				sp.Handshake = Handshake.None;
				sp.ReadTimeout = 500;
				sp.WriteTimeout = 500;
				sp.Open(); //přidat try catch pokud se neotevře
			}
            Receiving = true;
            while (Receiving == true)
			{
				await Receive();
			}
		}
		public async Task Receive()
		{
			await Task.Delay(30);
		}

		
		public void StopReceiving()
		{
		Receiving = false;
			if(Sending==false)
			{ 
			sp.Close(); //uzavře port pokud se již nepoužívá
			}
		}
		public void StopSending()
		{
		Sending = false;
			if(Receiving==false)
			{ 
			sp.Close(); //uzavře port pokud se již nepoužívá
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
	}
}
//...