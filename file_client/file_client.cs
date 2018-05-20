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
			long fileSize = 0;
			byte[] filePathBuf;
			string filePath;
			//string fileName;
			t1 = new Transport(BUFSIZE, APP);

			//Receives filepath as a string. 
			filePath = args [0];
			string fileName = LIB.extractFileName (filePath);

			//Converts to bytes
			filePathBuf = Encoding.ASCII.GetBytes(filePath);

			//Get filesize
			fileSize = LIB.check_File_Exists (filePath);
			int fileSizeInt = (int)fileSize;

			//Sends filepath to server
			Console.WriteLine ("Requesting file...");
			t1.send (filePathBuf, fileSizeInt);

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
			byte[] buf = new byte[BUFSIZE];
			//transport.receive (ref buf);
			//string wuhu = Encoding.ASCII.GetString(buf);
			//Console.WriteLine ($"Står her axby = {wuhu}");

			int fileSize; 
			byte[] receiveBuf = new byte[BUFSIZE];
			string fileDirectory;
			//string fileName;
			//Create directory for file
			fileDirectory = "/root/Desktop/ServerFiles/";
			Directory.CreateDirectory (fileDirectory);

			//fileName = LIB.extractFileName(filePath);
			FileStream Fs = new FileStream (fileDirectory + fileName, FileMode.OpenOrCreate, FileAccess.Write);
			Console.WriteLine ("Reading file " + fileName + "...");

			do
			{
				fileSize = transport.receive(ref receiveBuf);
				Fs.Write(receiveBuf, 0, fileSize);

			}while(fileSize > 0);

			//Closes file after writing into it. 
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