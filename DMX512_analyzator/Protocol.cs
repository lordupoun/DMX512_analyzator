using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMX512_analyzator
{
    public abstract class Protocol
    {
        public bool Started { get; set; }
        //private String selectedPort = "";
        public String Port { get; set; }

        //public pole dat "packet", i s jednotlivejma getterama a setterama
    }
}
