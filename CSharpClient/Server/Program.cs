using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ZMQ;
using TRMS.MessagingComparisons.Protos;
using System.Threading;

namespace Server
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Messaging Comparisons Server");
            Console.WriteLine("C# on .NET 4.0");
            Console.WriteLine();

            ServiceCommandRequests();

            Console.ReadLine();
        }

        private static void ServiceCommandRequests()
        {
            const int port = 3456;

            var localAddress = string.Format("tcp://127.0.0.1:{0}", port);

            Console.WriteLine("Starting thread to monitor port {0} for Command requests...");

            var worker = new BackgroundWorker();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            worker.RunWorkerAsync(localAddress);

            Thread.Sleep(1000);
        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Console.WriteLine("\tThread started...");
            Console.WriteLine("\tCreating ZeroMQ Context...");

            var localAddress = e.Argument.ToString();

            using (var context = new Context())
            {
                Console.WriteLine("\tCreating ZeroMQ REP Socket...");

                using (var socket = context.Socket(SocketType.REP))
                {
                    Console.WriteLine("\tBinding socket to local address: \"{0}\"", localAddress);

                    socket.Bind(localAddress);

                    while (e.Cancel == false)
                    {
                        Console.WriteLine("\tWaiting for message...");

                        var request = socket.Recv();

                        Console.WriteLine("\t\tGot {0} byte request...", request.Length);

                        var command = Command.ParseFrom(request);

                        Console.WriteLine("\t\tCommand parsed:\n{0}", command.ToString());

                        Console.WriteLine();
                        Console.WriteLine("\t\tCreating CommandResponse...");

                        var builder = new CommandResponse.Builder();

                        builder.Hr = 0;
                        builder.Description = "OK";

                        var response = builder.Build();

                        Console.WriteLine("\t\tResponse created:\n{0}", response.ToString());
                        Console.WriteLine();

                        var responseBytes = response.ToByteArray();

                        Console.WriteLine("\t\tSending Response ({0} bytes)...", responseBytes.Length);

                        socket.Send(responseBytes);

                        Console.WriteLine("\t\tResponse Sent.");
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
