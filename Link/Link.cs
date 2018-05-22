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
			List<byte> byteList = new List<byte>(1)
			{
				DELIMITER // start delimiter
			};

			for (var i = 0; i < size; i++) 
			{
				if (buf[i] == 0x41) { // hvis elementet er et ascii A
					byteList.Add (0x42);
					byteList.Add (0x43);
				} else if (buf[i] == 0x42) { // hvis elementet er et ascii B
					byteList.Add (0x42);
					byteList.Add (0x44);
				} else
					byteList.Add (buf[i]);
			}

			byteList.Add (DELIMITER); // stop delimiter

			serialPort.Write (byteList.ToArray (), 0, byteList.Count);
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
					break;
			}
			byte received = (byte)serialPort.ReadByte();

			while(received != DELIMITER)
			{
				if (received == (byte)'B') {
					var nextbyte = serialPort.ReadByte ();
					if (nextbyte == (byte)'C')
						buf[index++] = (byte)'A';
					else if (nextbyte == (byte)'D')
						buf[index++] = (byte)'B';
					else
						return 0;
					//error
				} else
					buf [index++] = received;

				received = (byte)serialPort.ReadByte ();
			}

			return index;
		}
	}
}
