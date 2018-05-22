//using System;
//using System.IO.Ports;
//using System.Text;
//using Library;
//using System.Collections.Generic;
//
//
///// <summary>
///// Link.
using System;
using System.Collections.Generic;
using System.IO.Ports;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			/*#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/ttySn0",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/ttySn1",115200,Parity.None,8,StopBits.One);
				}
			#else*/
			serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			//#endif
			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
			// TO DO Your own code
			List<byte> dataList = new List<byte>(1)
			{
				DELIMITER // start delimiter
			};

			for (var i = 0; i < size; i++) 
			{
				if (buf[i] == 'A') {        // hvis  ascii A erstat med B og C
					dataList.Add ((byte)'B');
					dataList.Add ((byte)'C');
				} else if (buf[i] == 'B')   // hvis ascii B erstat med B og D
				{ 
					dataList.Add ((byte)'B');
					dataList.Add ((byte)'D');
				} else
					dataList.Add (buf[i]);
			}

			dataList.Add (DELIMITER); // stop delimiter

			serialPort.Write (dataList.ToArray (), 0, dataList.Count);
		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{
			int index = 0;
			for(;;)
			{
				if (serialPort.ReadByte() == DELIMITER)
					break;                                         //ikke mere data, break!
			}
			byte receivedByte = (byte)serialPort.ReadByte();

			while(receivedByte != DELIMITER)                         //sålænge delimiter ikke er någet
			{
				if (receivedByte == (byte)'B') {
					var nextdata = serialPort.ReadByte ();          
					if (nextdata == (byte)'C')
						buf[index++] = (byte)'A';                   //hopper videre fra B til A
					else if (nextdata == (byte)'D')
						buf[index++] = (byte)'B';                  //videre til B
					else
						return 0;
					//error
				} else
					buf [index++] = receivedByte;               

				receivedByte = (byte)serialPort.ReadByte ();
			}

			return index;
		}
	}
}
