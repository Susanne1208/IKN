using System;
using Linklaget;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
	/// <summary>
	/// Transport.
	/// </summary>
	public class Transport
	{
		/// <summary>
		/// The link.
		/// </summary>
		private Link link;
		/// <summary>
		/// The 1' complements checksum.
		/// </summary>
		private Checksum checksum;
		/// <summary>
		/// The buffer.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The seq no.
		/// </summary>
		private byte seqNo;
		/// <summary>
		/// The old_seq no.
		/// </summary>
		private byte old_seqNo;
		/// <summary>
		/// The error count.
		/// </summary>
		private int errorCount;
		/// <summary>
		/// The DEFAULT_SEQNO.
		/// </summary>
		private const int DEFAULT_SEQNO = 2;
		/// <summary>
		/// The data received. True = received data in receiveAck, False = not received data in receiveAck
		/// </summary>
		private bool dataReceived;
		/// <summary>
		/// The number of data the recveived.
		/// </summary>
		private int recvSize = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE, string APP)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
			seqNo = 0;
			old_seqNo = DEFAULT_SEQNO;
			errorCount = 0;
			dataReceived = false;
		}

		/// <summary>
		/// Receives the ack.
		/// </summary>
		/// <returns>
		/// The ack.
		/// </returns>
		private bool receiveAck()
		{
			recvSize = link.receive(ref buffer);
			dataReceived = true;

			if (recvSize == (int)TransSize.ACKSIZE) {
				dataReceived = false;
				if (!checksum.checkChecksum (buffer, (int)TransSize.ACKSIZE) ||
					buffer [(int)TransCHKSUM.SEQNO] != seqNo ||
					buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
				{
					return false;
				}
				seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
			}

			return true;
		}

		private void nextSeqNo()
		{
			seqNo = (byte)((seqNo + 1) % 2);
		}
		/// <summary>
		/// Sends the ack.
		/// </summary>
		/// <param name='ackType'>
		/// Ack type.
		/// </param>
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);

			if(++errorCount == 2) // Simulate noise
			{
				ackBuf[1]++; // Important: Only spoil a checksum-field (ackBuf[0] or ackBuf[1])
				Console.WriteLine($"Noise! byte #1 is spoiled in ACK-package #{errorCount}");
			}

			link.send(ackBuf, (int)TransSize.ACKSIZE);
		}

		/// <summary>
		/// Send the specified buffer and size.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send(byte[] buf, int size)
		{
			// TO DO Your own code
			do 
			{
				//Codes to default segment header format from assignment
				buffer[(int)TransCHKSUM.SEQNO] = seqNo; //Bytes
				buffer[(int)TransCHKSUM.TYPE] = (int)TransType.DATA; //0-1000 bytes
				Array.Copy (buf, 0, buffer, (int)TransCHKSUM.DATA, size); //Copies a range of elements in one Array to another Array
				checksum.calcChecksum (ref buffer, size+4);

				//Error handling
				if (++errorCount == 2) 
				{
					//checking the buffer
					buffer [1]++;
					Console.WriteLine ($"Noise! - byte #1 is spoiled in transmission #{errorCount}");
				}

				//Calls send in Link Layer and send det buffer 
				link.send (buffer, size+4);
			} 

			while (!receiveAck()); //while there is something to send
			nextSeqNo ();
			old_seqNo = DEFAULT_SEQNO;
		}

		/// <summary>
		/// Receive the specified buffer.
		/// </summary>
		/// <param name='buffer'>
		/// Buffer.
		/// </param>
		public int receive (ref byte[] buf)
		{
			// TO DO Your own code
			bool state = false;
			int bytesReceived = 0;

			do 
			{
				bytesReceived = link.receive (ref buffer); //Calls linkLayer receive function for received bytes
				var checksumCheck = checksum.checkChecksum (buffer, bytesReceived); 
				state = checksumCheck && buffer [(int)TransCHKSUM.SEQNO] != old_seqNo;
				sendAck (checksumCheck);
			
			} while(!state); //while state is true

			//old_seqNo = buffer [2];
			old_seqNo = buffer [(int)TransCHKSUM.SEQNO];//old seqNo get reassigned
			Array.Copy (buffer, (int)TransCHKSUM.DATA, buf, 0, buffer.Length - 4); //Copies a range of elements in one Array to another Array
			return bytesReceived-4; // - headersize (only wants DATA)
		}
	}
}
