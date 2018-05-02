using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";
		Transport t1;

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			// TO DO Your own code
			//initialiser variabler 
			string filePath = string.Empty;
			string fileName = string.Empty;
			long fileSizeLong = 0;  
			byte[] tempBuf = new byte[BUFSIZE];

			//string fileSizeStr = string.Empty;
			Console.WriteLine("Inside file_server");
			//Fejler her!!
			t1 = new Transport(BUFSIZE, APP);
			Console.WriteLine("Server started");

			t1.receive (ref tempBuf); 
			filePath = Encoding.ASCII.GetString(tempBuf);
			fileName = LIB.extractFileName (filePath);

			Console.WriteLine($"Server looking for file {fileName}");

			fileSizeLong = LIB.check_File_Exists (fileName);
			sendFile (filePath, fileSizeLong, t1);
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String filePath, long fileSize, Transport transport)
		{
			// TO DO Your own code
			// Transport.send(byte[] buf, int size)

			Byte[] bufferServer = new Byte[BUFSIZE]; 
			string fileName = String.Empty;

			fileName = LIB.extractFileName (filePath);
			Console.WriteLine (fileName);

			FileStream Fs = new FileStream (filePath, FileMode.Open, FileAccess.Read);

			int bytesRead = Fs.Read(bufferServer, 0, BUFSIZE); //Der bliver læst fra fileName, puttes ind i bufferserveren og må max læse 1000 bytes(BUFSIZE)
			transport.send(bufferServer, bytesRead);

			//Whileloop fortsætter, så længe der er bytes at sende (fra fil)
			while(bytesRead > 0)
			{
				bytesRead = Fs.Read(bufferServer, 0, BUFSIZE);
				transport.send(bufferServer, bytesRead);
			}

			Console.WriteLine ("File sent!");
			Fs.Close ();
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			while (true) {
				new file_server ();
			}
		}
	}
}