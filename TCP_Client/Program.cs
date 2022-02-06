using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using TCP_Server.Models;

class Program
{
    class Command
    {
        public const string Post = "POST";
        public const string Put = "PUT";
        public const string Delete = "DELETE";
        public const string Get = "GET";
        public CarTb Value { get; set; }
        public string HttpMethod { get; set; }
    }
    //Client
    static void Main(string[] args)
    {
        Console.WriteLine("I AM CLIENT");
        var ip = IPAddress.Loopback;
        var port = 1111;
        TcpClient client = new TcpClient();
        Console.ReadKey();
        client.Connect(ip, port);
        Console.Clear();
        BinaryReader br = new BinaryReader(client.GetStream());
        BinaryWriter bw = new BinaryWriter(client.GetStream());

        while (true)
        {
            var str = Console.ReadLine();

            var command = new Command();
            if (str == "Exit")
            {
                bw.Write(str);
                break;
            }
            command.HttpMethod = str.Split('-')[0].ToUpper();
            switch (command.HttpMethod)
            {
                case Command.Get:

                    bw.Write(JsonSerializer.Serialize(command));

                    var CarList = JsonSerializer.Deserialize<List<CarTb>>(br.ReadString());
                    foreach (var car in CarList)
                    {
                        Console.WriteLine($"\tId -> {car.Id}");
                        Console.WriteLine($"\tVendor -> {car.Vendor}");
                        Console.WriteLine($"\tModel -> {car.Model}");
                        Console.WriteLine($"\tYear -> {car.Year}");
                        Console.WriteLine("************************");
                        Console.WriteLine();
                    }
                    break;
                case Command.Post:
                    command.Value = new CarTb { Id = int.Parse(str.Split('-')[1]), Vendor = str.Split('-')[2], Model = str.Split('-')[3], Year = int.Parse(str.Split('-')[4]) };
                    bw.Write(JsonSerializer.Serialize(command));
                    if (br.ReadBoolean())
                        Console.WriteLine("Added");
                    else
                        Console.WriteLine("Id Already Have");
                    break;
                case Command.Put:
                    command.Value = new CarTb { Id = int.Parse(str.Split('-')[1]), Vendor = str.Split('-')[2], Model = str.Split('-')[3], Year = int.Parse(str.Split('-')[4]) };
                    bw.Write(JsonSerializer.Serialize(command));
                    if (br.ReadBoolean())
                        Console.WriteLine("Updated");
                    else
                        Console.WriteLine("Not Found");
                    break;
                case Command.Delete:
                    command.Value = new CarTb { Id = int.Parse(str.Split('-')[1]) };
                    bw.Write(JsonSerializer.Serialize(command));

                    if (br.ReadBoolean())
                        Console.WriteLine("Succesfull");
                    else
                        Console.WriteLine("Not Found");
                    break;
            }
        }
    }
}

