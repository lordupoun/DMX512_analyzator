using System;
//using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using DMX

namespace protokolDMX512
{
    public class Protocol
    {
        //byte[] toSend = new byte[513]; //Buï mùžu editovat tenhle byte, nebo dávat byte do metody Send
        private bool loop; //do konstruktoru pøidat COM port atd.
        SerialPort sp = new SerialPort();
        public async Task send(byte[] toSend)
        {
            
                //sp.Write(nula, 0, 1);
                sp.BreakState = true;
                await Task.Delay(1); //lze použít i timer
                //Thread.Sleep(1);
                sp.BreakState = false;
                // sp.Write(jedna, 0, 1);
                //await Task.Delay(1);
                //Thread.Sleep(1);
                sp.Write(toSend, 0, toSend.Length);
                /* /*await Task.Delay(1); //co to dìlá?
                 sp.BreakState = true;
                 if (test == true)
                 {
                     ts = timer.Elapsed;
                     test = false;
                 }
                 if (timer.Elapsed > ts + TimeSpan.FromMilliseconds(4))
                 {
                     Console.WriteLine("teï");
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
        public void start()
        {
            loop = true;
            //Stopwatch timer = new Stopwatch();
            
           
            //byte[] nula = { 0x00 };
            //byte[] jedna = { 0xFF };
            //Console.WriteLine("Test DMX");
            //Console.ReadKey();
            //Thread.Sleep(5000);
            //Console.WriteLine("Dobøe");
            
            sp.PortName = "COM5";
            sp.BaudRate = 250000;
            sp.Parity = Parity.None;
            sp.DataBits = 8;
            sp.StopBits = StopBits.Two;
            sp.Handshake = Handshake.None;
            sp.ReadTimeout = 500;
            sp.WriteTimeout = 500;
            sp.Open(); //pøidat try catch pokud se neotevøe
        while (loop == true)
        {
            send(getTextBoxes());
        }
            //timer.Start();
            //TimeSpan ts;
            //ts = timer.Elapsed;
            //TimeSpan ts1 = new TimeSpan();
            //ts1 = TimeSpan.FromMilliseconds(1);
            //bool test = true;
            //bool loop = true;

        }
        public void stop()
        {
            loop = false;          
            sp.Close();
        }
    }
}
