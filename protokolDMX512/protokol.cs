using System;
//using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;

namespace protokolDMX512
{
    public class Protocol
    {
        public byte[] toSend = new byte[513]; //Bu� m��u editovat tenhle byte, nebo d�vat byte do metody Send
        private bool loop; //do konstruktoru p�idat COM port atd.
        SerialPort sp = new SerialPort();
        private async Task Send()
        {           
                //sp.Write(nula, 0, 1);
                sp.BreakState = true;
                await Task.Delay(1); //lze pou��t i timer
                //Thread.Sleep(1);
                sp.BreakState = false;
                // sp.Write(jedna, 0, 1);
                //await Task.Delay(1);
                //Thread.Sleep(1);
                sp.Write(toSend, 0, toSend.Length); //------------------- Mohlo by b�t await na Write Zde najdou uplatn�n� zejm�na IO bound operace, kter� n�m pomohou sn�it po�et vl�ken v�aplikaci. V�p��pad� tohoto typu operac� nen� ��dn� �innost prov�d�na na dal��m vl�kn� va�� aplikace, ale �ek� se na odpov�� jin�ho syst�mu (nap�.�datab�ze). 
                /* /*await Task.Delay(1); //co to d�l�?
                 sp.BreakState = true;
                 if (test == true)
                 {
                     ts = timer.Elapsed;
                     test = false;
                 }
                 if (timer.Elapsed > ts + TimeSpan.FromMilliseconds(4))
                 {
                     Console.WriteLine("te�");
                     sp.BreakState = false;
                     sp.Write(odeslat, 0, odeslat.Length);
                     test = true;
                 }------------------------*/
                /*while (true)
                {
                    //sp.Write(nula, 0, 1);
                    sp.BreakState = true;
                    Thread.Sleep(1);
                    sp.BreakState = false;
                    // sp.Write(jedna, 0, 1);

                    Thread.Sleep(1);
                    sp.Write(odeslat, 0, odeslat.Length);
                }*/

            
        }
        public async Task Start()
        {
            loop = true;
            //Stopwatch timer = new Stopwatch();
            
           
            //byte[] nula = { 0x00 };
            //byte[] jedna = { 0xFF };
            //Console.WriteLine("Test DMX");
            //Console.ReadKey();
            //Thread.Sleep(5000);
            //Console.WriteLine("Dob�e");
            
            sp.PortName = "COM5";
            sp.BaudRate = 250000;
            sp.Parity = Parity.None;
            sp.DataBits = 8;
            sp.StopBits = StopBits.Two;
            sp.Handshake = Handshake.None;
            sp.ReadTimeout = 500;
            sp.WriteTimeout = 500;
            sp.Open(); //p�idat try catch pokud se neotev�e
            while (loop == true) //mo�n� p�idat do send
            {
               await Send();
            }
            //timer.Start();
            //TimeSpan ts;
            //ts = timer.Elapsed;
            //TimeSpan ts1 = new TimeSpan();
            //ts1 = TimeSpan.FromMilliseconds(1);
            //bool test = true;
            //bool loop = true;

        }
        public void Stop()
        {
            loop = false;          
            sp.Close();
        }
    }
}
/*static void udelay(long us)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        long v = (us * System.Diagnostics.Stopwatch.Frequency )/ 1000000;
        while (sw.ElapsedTicks < v)
        {
        }
    }*/