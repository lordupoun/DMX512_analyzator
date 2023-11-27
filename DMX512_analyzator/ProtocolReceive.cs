using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Controls;

namespace DMX512_analyzator //mohla by dědit společné atributy
{
    public class ProtocolReceive : Protocol1
    {
        public byte[] toReceive = new byte[513]; //Buď můžu editovat tenhle byte, nebo dávat byte do metody Send //zanechat private a dát setter
         //do konstruktoru přidat COM port atd.....
        
        
        SerialPort sp = new SerialPort();
        public ProtocolReceive() //Zvolení portu vytvoří novou instanci (pokud ještě není vytvořena)
        {

        }
        public ProtocolReceive(String port) //Zvolení portu vytvoří novou instanci (pokud ještě není vytvořena)
        {
            //Started = false;
            //Port = port;
        }
        private async Task Receive()
        {
            sp.Read(toReceive, 0, toReceive.Length);
            
        }
        public async Task Start()
        {
            //Started = true;
            //sp.PortName = Port; //Nastavení COM portu v rámci konstruktoru
            sp.BaudRate = 250000;
            sp.Parity = Parity.None;
            sp.DataBits = 8;
            sp.StopBits = StopBits.Two;
            sp.Handshake = Handshake.None;
            sp.ReadTimeout = 500;
            sp.WriteTimeout = 500;
            sp.Open(); //přidat try catch pokud se neotevře
            //while (Started == true) //možná přidat do send
            //{
            //    await Receive();
            //}
        }
        public void Stop()
        {
            //Started = false;
            sp.Close();
        }
        public byte getReceivedValue(int index)
        {
            /*if (toSend[index] == null)
				return 0;
			else*/
            return toReceive[index];
        }

    }
}

