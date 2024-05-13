using System;
//using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System.Globalization;
using System.Windows;
using System.Net.Sockets;
using System.Reflection;
using System.IO;

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
        
		//Přijímání:
        const int packetSize = 520;
        static byte[] receivedBytes = new byte[packetSize * 2];
        static byte[] packet = new byte[packetSize];
        static bool findFirstPacket = true;
        static int index = 0;
        static int availableBytes;
        int packetsDropped = 0;
        private Action<byte[]> packetReceivedCallback;
        private Action<int> packetDropped;
        //---------
        //static StreamWriter writer;
        //static bool test = true;





        public Protocol(String port, Action<byte[]> packetReceivedCallback, Action<int> packetDropped) //Zvolení portu vytvoří novou instanci třídy Protocol (pokud ještě není vytvořena)
		{
			Sending = false;
			Receiving = false;
            this.packetReceivedCallback = packetReceivedCallback;
            this.packetDropped = packetDropped;
            //bool isOpen=false;
            this.port = port;
			/*foreach(byte i in toReceive)
			{
				toReceive[i] = 0;
			}*/
		}

        /// <summary>Odesílací algoritmus.</summary>
        private async Task Send()
		{
			sp.Write(toSend, 0, toSend.Length);
			await Task.Delay(30); //přidat režim kompatibility -> 1ms pro FTDI; 30ms pro ostatní
			sp.BreakState = true; //dřív než pošle, přepíše?? //přepne se do nuly
			await Task.Delay(1); //30 aby to fungovalo i pro další převodníky//lze použít i timer
			sp.BreakState = false; //přidat ještě jednou Delay(MAB)
			 //-------------------               
		}

        /// <summary>Započne odesílání.</summary>
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
                sp.ReadBufferSize = 8192;
                sp.Open(); //přidat try catch pokud se neotevře
			}
			Sending = true;

			while (Sending == true)
			{
				await Send();
			}
		}

        /// <summary>Započne přijímání.</summary>
        public void StartReceiving()
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
                sp.ReadBufferSize = 8192;
                sp.Open(); //přidat try catch pokud se neotevře              
            }
            sp.DataReceived += SerialPort_DataReceived; //zde se děje problém!            
            Receiving = true;
            /*while (Receiving == true)
			{
				//await Receive();
			}*/
		}

        /// <summary>Ukončí přijímání.</summary>
        public void StopReceiving()
		{
		Receiving = false;
        sp.DataReceived -= SerialPort_DataReceived;
        if (Sending==false)
			{ 
			sp.Close(); //uzavře port pokud se již nepoužívá            
            }
		}

        /// <summary>Odhlásí přijímací event (zastaví přijímací smyčku) - používá se v případě, že uživatel neprohlíží GUI
		/// - tím pádem zůstane COM port se statusem aktivního přijímání, ale algoritmus přijímání na pozadí neběží, aby nezatěžovalo aplikaci.
		/// Při zvolení původního COM portu nebo režimu přijímání, se opět spustí (viz StartReceivingEvent())</summary>
        public void StopReceivingEvent()
        {
            sp.DataReceived -= SerialPort_DataReceived;
        }

        /// <summary>Přihlásí přijímací event (spustí přijímací smyčku)</summary>
        public void StartReceivingEvent()
        {
            sp.DataReceived += SerialPort_DataReceived;
        }

        /// <summary>Ukončí odesílání</summary>
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
            return toReceive[index];
		}
        /// <summary>Přijímací algoritmus. Metoda vyvolaná eventem pro přijímání dat. Event se automaticky unsubscribne, pokud není v GUI používán.</summary>
        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			await Task.Run(() =>
			{
				SerialPort serialPort = (SerialPort)sender;
				availableBytes = serialPort.BytesToRead;//zde je důležitá posloupnost -> není pak to dál v kódu stejně nepoužívám
				if (findFirstPacket == true)
				{
					if (availableBytes > receivedBytes.Length)//dát >= //Pokud jsou k dispozici byty k přečtení a jejich součet nebude větší než 1040 (bacha, musím přidat aby se to vždy dostalo k součtu
					{
						index = 0;
						bool packetFound = false;
						serialPort.Read(receivedBytes, 0, receivedBytes.Length);
						//writer.WriteLine("vytisk 1.");
						//writer.WriteLine(BitConverter.ToString(receivedBytes));

						for (int i = 0; i < receivedBytes.Length / 2; i++)
						{   // 0 1 518 519
							if (receivedBytes[i] == 121 && receivedBytes[i + 1] == 122 && receivedBytes[i + packetSize - 2] == 131 && receivedBytes[i + packetSize - 1] == 132) //Co pokud nic nesplní podmínku //if nesplnena podminka za forem, resetovat
							{
								Array.Copy(receivedBytes, i, packet, 0, packetSize); //Zkopíruje 520 do paketu
								Array.Copy(receivedBytes, i + packetSize, receivedBytes, 0, packetSize - i); //zarovná zbytek na nultou pozici - zkusit to nezarovnávat
								index = i;
								packetFound = true;
								findFirstPacket = false;
								//writer.WriteLine("pravy 2. paket");
								//writer.WriteLine(BitConverter.ToString(packet));
								//Console.WriteLine("Received a complete frame:"); //ukládání do souboru je potřeba komprimovat
								//Console.WriteLine(BitConverter.ToString(packet));
								//writer.WriteLine(BitConverter.ToString(packet));
								packetReceivedCallback?.Invoke(packet);
							}
						}                  //nezdržuje se to někde při array copy?
						if (packetFound == false)
						{
                            //Console.WriteLine("Paket nenalezen");
                            //serialPort.ReadExisting();
                            packetsDropped += 1;
                            packetDropped?.Invoke(packetsDropped);
                            findFirstPacket = true;
						}
					}
				}
				else
				{//index označuje přesně to, co chybí z následujícího bytu
				 //Původní plán byl takovej, že by to vždycky doplňovalo do 1040 a vždycky to hledalo -> tím pádem i kdyby se to rozhodilo, našel by se v tom novým poli bez zásadních výpadků, ale tohle je lepší
				 //Console.WriteLine("už nehledá první byte");
					if (availableBytes > index) //bacha tady je, pokud bytes je větší -- není problém, přečtu jen tolik, kolik chci
					{
						//bool packetFound = false;//+1? - nemusí být, počítá se od nuly
						serialPort.Read(receivedBytes, packetSize - index, index); //Vymazává to ta data včas (nepřečte to 2x to samý?)
																				   //writer.WriteLine("vytisk 3.");
																				   //writer.WriteLine(BitConverter.ToString(receivedBytes));
																				   //for (int i = 0; i < receivedBytes.Length / 2; i++)
																				   //{   // 0 1 518 519
						if (receivedBytes[0] == 121 && receivedBytes[1] == 122 && receivedBytes[packetSize - 2] == 131 && receivedBytes[packetSize - 1] == 132) //Co pokud nic nesplní podmínku //if nesplnena podminka za forem, resetovat
						{
							Array.Copy(receivedBytes, 0, packet, 0, 520); //přečte 520 bytů
																		  //Array.Copy(receivedBytes, i + 520, receivedBytes, 0, receivedBytes.Length - i - 520); //zarovná zbytek na nultou pozici - zkusit to nezarovnávat
																		  //index = i; //index přidat jako paměť?
																		  //packetFound = true;

							//writer.WriteLine(BitConverter.ToString(packet));
							//Console.WriteLine("Received a complete frame:");
							//Console.WriteLine(BitConverter.ToString(packet));
							//writer.WriteLine("pravy 4. paket");
							//writer.WriteLine(BitConverter.ToString(packet));
							index = packetSize; //nepřidat víc kódu abych to nemusel přepisovat pokaždé?
												//for(int i=0; i<packet.Length; i++)
												//packet[i] = 0;
							packetReceivedCallback?.Invoke(packet);
						}
						else
						{
							//Console.WriteLine("Paket nenalezen");
							//serialPort.ReadExisting();
							packetsDropped += 1;
							packetDropped?.Invoke(packetsDropped);
							findFirstPacket = true;
							//DroppedPacket
						}
					}
				}
			});
		}
        /// <summary>Vyresetuje počítadlo nerozpoznaných paketů.</summary>
        public void ResetPacketsDropped()
		{
			packetsDropped = 0;
            packetDropped?.Invoke(packetsDropped);
        }

    }
}
//...