using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using protokolDMX512;

namespace DMX512_analyzator //Singleton?
{
	internal static class Environment
	{
		//Protocol device1 = new Protocol();
		//int cislo;
		//Můžu tady mít array instancí Protocol
		

		public static void testMetoda()
		{
			//public Protocol[] protocolArray = new Protocol[256]; //ke každý public proměnný pak můžu dodělat gettery a settery
			//protocolArray[1]=test;
			Protocol[] protocolArray = new Protocol[256];
			protocolArray[0] = new Protocol();
			
		}
		/*public static Protocol[] get()
		{
			return protocolArray;
		}*/
		
	}
}
