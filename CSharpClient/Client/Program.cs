using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using ZMQ;
using System.Diagnostics;
using TRMS.MessagingComparisons.Protos;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            const string DEFAULT_SERVER = "127.0.0.1";

            Console.WriteLine("Messaging Comparisons Client");
            Console.WriteLine("C# on .NET 4.0");
            Console.WriteLine();

            Console.WriteLine("Enter Server Address and then press Enter [{0}]:", DEFAULT_SERVER);
            var server = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(server))
                server = DEFAULT_SERVER;

            Console.WriteLine("Using Server \"{0}\"...", server);

            Console.WriteLine();

            var workingDirectory = Directory.GetCurrentDirectory();

            var clrzmq = Directory.GetFiles(workingDirectory, "clrzmq.dll");

            var version = Assembly.LoadFrom(clrzmq[0]).GetName().Version;

            Console.WriteLine("Using clrzmq {0}", version);

            Console.WriteLine();

            SendCommandsWithProtoBufAndZeroMQ(server);

            Console.ReadLine();
        }

        private static void SendCommandsWithProtoBufAndZeroMQ(string server)
        {
            const int commandCount = 1000;
            var watch = new Stopwatch();

            Console.WriteLine("Test 1: Send {0} \"Commands\" using ProtoBuf and ZeroMQ", commandCount);

            Console.WriteLine("\tCreate ZeroMQ Context...");
            using (var context = new Context())
            {
                Console.WriteLine("\tCreate ZeroMQ Req Socket...");

                using (var socket = context.Socket(SocketType.REQ))
                {
                    while (true)
                    {
                        Console.WriteLine("\t\tPress escape to exit, any other key to send request...");
                        var key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.Escape)
                            break;
                        else
                            SendCommand(server, socket);
                    }

                    Console.WriteLine("\tClosing Socket...");
                }

                Console.WriteLine("\tClosing Context...");
            }
        }

        private static void SendCommand(string server, Socket socket)
        {
            var remoteAddress = string.Format("tcp://{0}:3456", server);

            Console.WriteLine("\tConnect socket to remote address \"{0}\"", remoteAddress);
            socket.Connect(remoteAddress);

            Console.WriteLine("\tConnected...");
            Console.WriteLine("\tCreate Command Builder...");

            var builder = new Command.Builder();

            Console.WriteLine("\tCreate Command...");
            builder.CommandName = "CueDecode";
            builder.Region = 27;
            builder.VideoFileName = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            builder.AudioFileName0 = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            builder.AudioFileName1 = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            builder.AudioFileName2 = "E:\\Zeplay\\Audio\\Audio_12_25-1995-12-00-00_Channel0.pair2.wav";
            builder.AudioFileName3 = "E:\\Zeplay\\Audio\\Audio_12_25-1995-12-00-00_Channel0.pair2.wav";
            builder.VbiFileName = string.Empty;
            builder.UseTDIR = false;
            builder.InitialFrame = 1773249712u;
            builder.InitialRate = 0.75;
            builder.Loop = true;

            var request = builder.Build();
            var requestBytes = request.ToByteArray();
            Console.WriteLine("\t\tSerialized Comand request to {0} bytes", requestBytes.Length);
            Console.WriteLine("\t\tRequest:\n{0}", request.ToString());

            Console.WriteLine("\tSending request...");
            socket.Send(requestBytes);

            Console.WriteLine("\tWaiting for response...");

            var received = socket.Recv();
            Console.WriteLine("\tDone, received {0} bytes", received.Length);

            var response = CommandResponse.ParseFrom(received);

            Console.WriteLine("\tParsed {0} from received data", response.GetType().Name);
            Console.WriteLine("\t\tResponse:\n{0}", response.ToString());
            Console.WriteLine();

            if (response.HasHr)
                Console.WriteLine("\t\tHR:{0:x}", response.Hr);

            if (response.HasDescription)
                Console.WriteLine("\t\tDescription: {0}", response.Description);

            Console.WriteLine();
        }
    }
}
