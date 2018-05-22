using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";
		private Transport t1;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
	    	// TO DO Your own code
			int fileSize = 0;
			byte[] filePathBuf;
			string filePath;
			t1 = new Transport(BUFSIZE, APP);

			//Receives filepath as a string. 
			filePath = @"C:/root/Desktop/ServerFiles/Kitten1.jpg";//args [0];
			string fileName = LIB.extractFileName (filePath);

			//Converts to bytes
			filePathBuf = Encoding.ASCII.GetBytes(filePath);
			fileSize = filePath.Length;

			//Sends filepath to server
			Console.WriteLine ("Requesting file...");
			t1.send (filePathBuf, fileSize);

			//Receives file from server
			Console.WriteLine ("Receiving file...");
			receiveFile (fileName, t1);
	    }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, Transport transport)
		{
			string fileNameS = "Kitten.jpg";
			byte[] receiveBuf = new byte[BUFSIZE];

			string fileDirectory;
			fileDirectory = "/root/Desktop/ServerFiles/";
			Directory.CreateDirectory (fileDirectory);

			FileStream Fs = new FileStream (fileDirectory + fileNameS, FileMode.OpenOrCreate, FileAccess.Write);
			Console.WriteLine ("Reading file " + fileName + "...");

			//int bytesRead = transport.receive (ref receiveBuf);

//			while (bytesRead >= 0)
//			{
//				Fs.Write (receiveBuf, 0, bytesRead);
//				bytesRead = transport.receive (ref receiveBuf);
//				Console.WriteLine ($"{bytesRead}");
//			}
//			
			byte[] sizebuf = new byte[BUFSIZE];
			int size = transport.receive (ref sizebuf);
			sizebuf [size] = 0;
			string str = ((new UTF8Encoding ()).GetString (sizebuf)).Substring (0, size);
			long fileSizeLong = long.Parse (str);
			Console.WriteLine (fileSizeLong);
			Console.WriteLine (str);

			int bytesRead = 0;

			do {
				int i = 0;
				i = transport.receive (ref receiveBuf);
				//Console.WriteLine ($"{bytesRead}");
				Fs.Write (receiveBuf, 0, i);
				bytesRead += i;
				//Console.WriteLine ($"{bytesRead}");
			} while((long)bytesRead < (long)fileSizeLong);

			Console.WriteLine("File received");

			Fs.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new file_client(args);
		}
	}
}