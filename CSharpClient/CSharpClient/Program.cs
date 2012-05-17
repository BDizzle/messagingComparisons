using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TRMS.MessagingComparisons.Protos;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using MsgPack;
using System.ComponentModel;
using System.Threading;

namespace CSharpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Messaging Comparisons Test Client");

            Console.WriteLine();

            Console.WriteLine("Menu:");
            Console.WriteLine("\t1: Test Serialization");
            Console.WriteLine("\t2: Test Data Transfer");

            var entry = Console.ReadLine();

            if (entry.Contains('1'))
                TestSerialization();
            if (entry.Contains('2'))
                TestTransfer();

            Console.ReadLine();
        }

        private static void TestTransfer()
        {
            var watch = new Stopwatch();

            Console.WriteLine("Data Transfer Tests:");
            Console.WriteLine();
            Console.WriteLine("clrzmq:");

            var random = new Random();
            var data = new byte[1024];

            //First byte needs to be 0x00 for the subscription filter to work
            data[0] = 0x00;

            //Fill the array with some random data
            for (int i = 1; i < data.Length; i++)
                data[i] = (byte)random.Next(255);

            {
                Console.WriteLine("\tTest 1: Publish 1024 Byte payload 1,000,000 times, localhost TCP");

                var receiver = new BackgroundWorker();
                receiver.DoWork += (sender, e) =>
                    {
                        var receivedMessages = 0;

                        using (var context = new ZMQ.Context())
                        {
                            using (var socket = context.Socket(ZMQ.SocketType.SUB))
                            {
                                socket.Subscribe(new byte[] { 0x00 });

                                socket.Connect("tcp://127.0.0.1:12345");

                                for (int i = 0; i < (int)e.Argument; i++)
                                {
                                    var receivedData = socket.Recv();

                                    receivedMessages++;
                                }
                            }
                        }

                        e.Result = receivedMessages;
                    };

                var received = 0;

                receiver.RunWorkerCompleted += (sender, e) => { received = (int)e.Result; };

                var tries = 1000000;

                receiver.RunWorkerAsync(tries);

                watch.Start();

                using (var context = new ZMQ.Context())
                {
                    using (var socket = context.Socket(ZMQ.SocketType.PUB))
                    {
                        socket.Bind("tcp://127.0.0.1:12345");

                        for (int i = 0; i < tries; i++)
                            socket.Send(data);
                    }
                }

                EndTest(watch, tries, "Published command");

                Thread.Sleep(1);
                Console.WriteLine("\t\tReceived count: {0}", received);
            }

            {
                Console.WriteLine("\tTest 2: Req/Rep 1024 Byte payload 1,000,000 times, localhost TCP");

                var receiver = new BackgroundWorker();
                receiver.DoWork += (sender, e) =>
                {
                    var receivedMessages = 0;

                    using (var context = new ZMQ.Context())
                    {
                        using (var socket = context.Socket(ZMQ.SocketType.REP))
                        {
                            socket.Connect("tcp://127.0.0.1:12345");

                            for (int i = 0; i < (int)e.Argument; i++)
                            {
                                var receivedData = socket.Recv();

                                receivedMessages++;

                                socket.Send("Ok", Encoding.UTF8);
                            }
                        }
                    }

                    e.Result = receivedMessages;
                };

                var received = 0;

                receiver.RunWorkerCompleted += (sender, e) => { received = (int)e.Result; };

                var tries = 1000000;

                receiver.RunWorkerAsync(tries);

                watch.Start();

                using (var context = new ZMQ.Context())
                {
                    using (var socket = context.Socket(ZMQ.SocketType.REQ))
                    {
                        socket.Bind("tcp://127.0.0.1:12345");

                        for (int i = 0; i < tries; i++)
                        {
                            socket.Send(data);

                            var response = socket.Recv();
                        }
                    }
                }

                EndTest(watch, tries, "Sent command");

                Thread.Sleep(1);
                Console.WriteLine("\t\tReceived count: {0}", received);
            }

            {
                Console.WriteLine("\tTest 3: Router/Dealer 1024 Byte payload 1,000,000 times, localhost TCP");

                var received = 0;

                var receiver = new BackgroundWorker();

                receiver.DoWork += (sender, e) =>
                {
                    using (var context = new ZMQ.Context())
                    {
                        using (var client = context.Socket(ZMQ.SocketType.ROUTER))
                        {
                            client.Connect("tcp://127.0.0.1:23456");

                            var pollItem = client.CreatePollItem(ZMQ.IOMultiPlex.POLLIN);

                            pollItem.PollInHandler += (socket, revents) =>
                                {
                                    var receivedData = socket.Recv();

                                    if(receivedData != null)
                                        received++;

                                    //Respond
                                    socket.Send("OK", Encoding.UTF8);
                                };

                            var items = new[] {pollItem};

                            while (true)
                            {
                                context.Poll(items, 100);
                            }                       
                        }
                    }

                };

                var tries = 1000000;

                receiver.RunWorkerAsync();

                watch.Start();

                using (var context = new ZMQ.Context())
                {
                    using (var server = context.Socket(ZMQ.SocketType.DEALER))
                    {
                        server.Bind("tcp://127.0.0.1:23456");

                        for (int i = 0; i < tries; i++)
                        {
                            server.Send(data);
                        }
                    }
                }

                var elapsedMSToSend = watch.ElapsedMilliseconds;

                Console.WriteLine("\t\tSent {0} commands in {1:0.0000} ms", tries, elapsedMSToSend);

                while (received < tries)
                    Thread.Sleep(10);

                Console.WriteLine("\t\tReceived count: {0}", received);

                watch.Stop();
                Console.WriteLine("\t\tAll responses processed in {0:0.0000} ms", watch.ElapsedMilliseconds);
            }
        }

        private static void TestSerialization()
        {
            var watch = new Stopwatch();
            byte[] data = null;


            Console.WriteLine("Serialization Tests:");
            Console.WriteLine();
            Console.WriteLine("Protobuf:");

            {
                StartTest(watch, "Test 1: Create and Serialize");

                for (int i = 0; i < 1000000; i++)
                {
                    data = CreateCommandBuilder().Build().ToByteArray();
                }

                EndTest(watch, data, 1000000, "Created and serialized command");
            }

            {
                StartTest(watch, "Test 2: Serialize Existing");

                var builder = CreateCommandBuilder();

                for (int i = 0; i < 1000000; i++)
                {
                    data = builder.Build().ToByteArray();
                }

                EndTest(watch, data, 1000000, "Serialized command");
            }

            Console.WriteLine();
            Console.WriteLine("FastJSON:");

            {
                StartTest(watch, "Test 1: Create and Serialize");

                for (int i = 0; i < 1000000; i++)
                {
                    var command = CreateJSONCommand();

                    data = System.Text.Encoding.ASCII.GetBytes(fastJSON.JSON.Instance.ToJSON(command));
                }

                EndTest(watch, data, 1000000, "Created and serialized command");
            }

            {
                StartTest(watch, "Test 2: Serialize Existing");

                var jsonCommand = CreateJSONCommand();

                for (int i = 0; i < 1000000; i++)
                {
                    data = System.Text.Encoding.ASCII.GetBytes(fastJSON.JSON.Instance.ToJSON(jsonCommand));
                }

                EndTest(watch, data, 1000000, "Serialized command");
            }

            Console.WriteLine();
            Console.WriteLine("JSON.Net");

            {
                StartTest(watch, "Test 1: Create and Serialize");

                for (int i = 0; i < 1000000; i++)
                {
                    var command = CreateJSONNetCommand();

                    data = System.Text.Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(command));
                }

                EndTest(watch, data, 1000000, "Created and serialized command");
            }

            {
                StartTest(watch, "Test 2: Serialize Existing");

                var command = CreateJSONNetCommand();

                for (int i = 0; i < 1000000; i++)
                {
                    data = System.Text.Encoding.ASCII.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(command));
                }

                EndTest(watch, data, 1000000, "Serialized command");
            }

            Console.WriteLine();
            Console.WriteLine("MS XML Serializer");

            {
                StartTest(watch, "Test 1: Create and Serialize");

                var serializer = new XmlSerializer(typeof(MSXML.Command));

                for (int i = 0; i < 1000000; i++)
                {
                    var command = CreateXMLCommand();

                    var stream = new MemoryStream();

                    serializer.Serialize(stream, command);

                    data = stream.ToArray();
                }

                EndTest(watch, data, 1000000, "Created and serialized command");
            }

            {
                StartTest(watch, "Test 2: Serialize Existing");

                var serializer = new XmlSerializer(typeof(MSXML.Command));

                var command = CreateXMLCommand();

                for (int i = 0; i < 1000000; i++)
                {
                    var stream = new MemoryStream();

                    serializer.Serialize(stream, command);

                    data = stream.ToArray();
                }

                EndTest(watch, data, 1000000, "Serialized command");
            }

            Console.WriteLine();

            Console.WriteLine("MessagePack");

            {
                StartTest(watch, "Test 1: Create and Serialize");

                for (int i = 0; i < 1000000; i++)
                {
                    var command = CreateMessagePackCommand();

                    data = new CompiledPacker(true).Pack<MessagePack.Command>(command);
                }

                EndTest(watch, data, 1000000, "Created and serialized command");
            }

            {
                StartTest(watch, "Test 2: Serialize Existing");

                var command = CreateMessagePackCommand();

                for (int i = 0; i < 1000000; i++)
                {
                    data = new CompiledPacker(true).Pack<MessagePack.Command>(command);
                }

                EndTest(watch, data, 1000000, "Serialized command");
            }

            Console.WriteLine("Thrift");

            {
                StartTest(watch, "Test 1: Create and Serialize");

                for (int i = 0; i < 1000000; i++)
                {
                    var command = CreateThriftCommand();

                    var stream = new MemoryStream();

                    var protocol = new Thrift.Protocol.TBinaryProtocol(new Thrift.Transport.TStreamTransport(stream, stream));

                    command.Write(protocol);

                    data = stream.ToArray();
                }

                EndTest(watch, data, 1000000, "Created and serialized command");
            }

            {
                StartTest(watch, "Test 2: Serialize Existing");

                var command = CreateThriftCommand();

                for (int i = 0; i < 1000000; i++)
                {
                    var stream = new MemoryStream();

                    var protocol = new Thrift.Protocol.TBinaryProtocol(new Thrift.Transport.TStreamTransport(stream, stream));

                    command.Write(protocol);

                    data = stream.ToArray();
                }

                EndTest(watch, data, 1000000, "Serialized command");
            }

        }


        private static void StartTest(Stopwatch watch, string description)
        {
            Console.WriteLine("\t{0}", description);

            watch.Start();
        }

        private static void EndTest(Stopwatch watch, int tries, string description)
        {
            watch.Stop();
            Console.WriteLine("\t\tDone: {0} {1} times", description, tries);
            Console.WriteLine("\t\t\tElapsed: {0}ms", watch.ElapsedMilliseconds);

            var ElapsedMSPerIteration = (watch.ElapsedMilliseconds / (double)tries);

            if (ElapsedMSPerIteration > 1000)
                Console.WriteLine("\t\t\tTime Each: {0:0.000} Seconds", ElapsedMSPerIteration / 1000);
            else if (ElapsedMSPerIteration > 1)
                Console.WriteLine("\t\t\tTime Each: {0:0.000} Milliseconds", ElapsedMSPerIteration);
            else
                Console.WriteLine("\t\t\tTime Each: {0:0.000} Microseconds", ElapsedMSPerIteration * 1000);
        }

        private static void EndTest(Stopwatch watch, byte[] data, int tries, string description)
        {
            EndTest(watch, tries, description);

            Console.WriteLine("\t\t\tCommand serialized to {0} Bytes", data.Length);
        }

        private static TRMS.MessagingComparisons.Protos.Command.Builder CreateCommandBuilder()
        {
            var builder = TRMS.MessagingComparisons.Protos.Command.CreateBuilder();

            builder.CommandName = "CueDecode";
            builder.Region = 27;
            builder.VideoFileName = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            builder.AudioFileName0 = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            builder.AudioFileName1 = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            builder.AudioFileName2 = "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav";
            builder.AudioFileName3 = "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav";
            builder.VbiFileName = string.Empty;
            builder.UseTDIR = false;
            builder.InitialFrame = 1773249712u;
            builder.InitialRate = 0.75d;
            builder.Loop = false;

            return builder;
        }

        private static JSON.Command CreateJSONCommand()
        {
            var command = new JSON.Command(
                "CueDecode",
                27,
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                string.Empty,
                false, 
                1773249712u,
                0.75d,
                false);

            return command;
        }

        private static JSON_Net.Command CreateJSONNetCommand()
        {
            var command = new JSON_Net.Command(
                "CueDecode",
                27,
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                string.Empty,
                false,
                1773249712u,
                0.75d,
                false);

            return command;
        }

        private static MSXML.Command CreateXMLCommand()
        {
            var command = new MSXML.Command(
                "CueDecode",
                27,
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                string.Empty,
                false,
                1773249712u,
                0.75d,
                false);

            return command;
        }

        private static MessagePack.Command CreateMessagePackCommand()
        {
            var command = new MessagePack.Command(
                "CueDecode",
                27,
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav",
                string.Empty,
                false,
                1773249712u,
                0.75d,
                false);

            return command;
        }

        private static TRMS.MessagingComparisons.Thrift.Command CreateThriftCommand()
        {
            var command = new TRMS.MessagingComparisons.Thrift.Command();

            command.CommandName = "CueDecode";
            command.Region = 27;
            command.VideoFileName = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            command.AudioFileName0 = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            command.AudioFileName1 = "E:\\Zeplay\\Video\\Video_12-25-1995-12-00-00_Channel0.avi";
            command.AudioFileName2 = "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav";
            command.AudioFileName3 = "E:\\Zeplay\\Audio\\Audio_12-25-1995-12-00-00-Channel0.pair2.wav";
            command.VbiFileName = string.Empty;
            command.UseTDIR = false;
            command.InitialFrame = 1773249712u;
            command.InitialRate = 0.75d;
            command.Loop = false;

            return command;
        }
    }
}
