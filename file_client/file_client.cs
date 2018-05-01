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
			string filePath = "";
			long fileSize = 0;

			TcpClient ClientSocket = new TcpClient ();
			try
			{
				ClientSocket.Connect(args[0], PORT);
				Console.WriteLine("Connected to {0}, port {1}", args[0], PORT);
			}
			catch
			{
				Console.WriteLine ("No connection");
			}
			NetworkStream serverStream = ClientSocket.GetStream();

			//Saving requested file in variable filePath
			filePath = args[1];
			Console.WriteLine ("Requesting file...");
			LIB.writeTextTCP (serverStream, filePath);

			fileSize = LIB.getFileSizeTCP (serverStream);

			//If file does not exist, keep asking for existing file
			while (fileSize == 0) {
				Console.WriteLine ("!File not found!");
				Console.WriteLine ("Enter new file: ");
				filePath = Console.ReadLine ();

				Console.WriteLine ("Requesting file...");
				LIB.writeTextTCP (serverStream, filePath);
				fileSize = LIB.getFileSizeTCP (serverStream);
			}
			Console.WriteLine ("Receiving file...");
			receiveFile (filePath, serverStream);
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
			// TO DO Your own code
			string fileName = string.Empty;
			string fileDirectory = string.Empty;
			long fileSize = 0;
			int bytesRead = 0;
			byte[] buffer = new byte[BUFSIZE];

			//Create directory for file
			fileDirectory = "/root/Desktop/ServerFiles/";
			Directory.CreateDirectory (fileDirectory);

			fileName = LIB.extractFileName(filePath);
			FileStream Fs = new FileStream (fileDirectory + fileName, FileMode.OpenOrCreate, FileAccess.Write);
			Console.WriteLine ("Reading file " + fileName + "...");

			//Writes into file as long as it receives bytes, bytesRead>0
			do
			{
				bytesRead = io.Read (buffer, 0, BUFSIZE);
				Fs.Write(buffer, 0, bytesRead);
				Console.WriteLine("Read bytes: " + bytesRead.ToString());
			}while (bytesRead > 0);

			//Lukker fil
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