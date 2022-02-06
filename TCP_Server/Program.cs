using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using TCP_Server.Models;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
class MyAtributes : Attribute
{
    public string Name { get; set; }
}

class Command
{
    public const string Post = "POST";
    public const string Put = "PUT";
    public const string Delete = "DELETE";
    public const string Get = "GET";
    public CarTb Value { get; set; }
    public string HttpMethod { get; set; }
}
class Program
{


    //SERVER


    static public CarDbContext MyDb { get; set; } = new CarDbContext();

    public static Command CommandClient { get; set; }
    static void Main(string[] args)
    {
        Console.WriteLine("I AM SERVER");




        Console.WriteLine("END");
        //MyDb.CarTbs.Add(new CarTb { Vendor = "Audi", Model = "A4", Year = 2018 });
        //MyDb.CarTbs.Add(new CarTb { Vendor = "Audi", Model = "A4", Year = 2018 });
        //MyDb.CarTbs.Add(new CarTb { Vendor = "BMW", Model = "X5", Year = 2019 });
        //MyDb.CarTbs.Add(new CarTb { Vendor = "Mercedes", Model = "C300", Year = 2017 });
        //MyDb.CarTbs.Add(new CarTb { Vendor = "Audi", Model = "A6", Year = 2018 });
        //MyDb.CarTbs.Add(new CarTb { Vendor = "BMW", Model = "X6", Year = 2019 });
        //MyDb.CarTbs.Add(new CarTb { Vendor = "Mercedes", Model = "C350", Year = 2017 });
        //MyDb.CarTbs.Add(new CarTb { Vendor = "Ford", Model = "Mondeo", Year = 2012 });

        //MyDb.SaveChanges();
        //Console.WriteLine("ADDED");

        //MyDb.CarTbs.Select(car => new CarTb { Id = car.Id, Vendor = car.Vendor, Model = car.Model, Year = car.Year });


        var ip = IPAddress.Loopback;
        var port = 1111;
        var server = new TcpListener(ip, port);
        server.Start(100);

        while (true)
        {


            var client = server.AcceptTcpClient();
            Console.WriteLine(client.Client.RemoteEndPoint + "Connected");
            while (true)
            {
                try
                {

                    bw = new BinaryWriter(client.GetStream());
                    br = new BinaryReader(client.GetStream());

                    var ClientText = br.ReadString();
                    if (ClientText == "Exit")
                        break;

                    CommandClient = JsonSerializer.Deserialize<Command>(ClientText);

                    var assembly = Assembly.GetExecutingAssembly();

                    var methods = assembly.GetTypes()
                                  .SelectMany(t => t.GetMethods())
                                  .Where(m => m.GetCustomAttributes(typeof(MyAtributes), false).Length > 0)
                                  .ToArray();

                    var InvokeThisMethod = methods.Where(x => x.GetCustomAttributes(false).OfType<MyAtributes>().First().Name == CommandClient.HttpMethod).FirstOrDefault();

                    if (InvokeThisMethod != null)
                        InvokeThisMethod.Invoke(null, null);

                }
                catch (Exception)
                {
                    Console.WriteLine(client.Client.RemoteEndPoint + " Disconnected");
                    break;
                }
            }

        }

    }
    [MyAtributes(Name = "DELETE")]
    public static void DeleteMethod()
    {
        var deletecar = MyDb.CarTbs.Where(x => x.Id == CommandClient.Value.Id).FirstOrDefault();
        if (deletecar is not null)
        {
            MyDb.CarTbs.Remove(deletecar);
            bw.Write(true);
            MyDb.SaveChanges();
        }
        else
            bw.Write(false);
        Console.WriteLine("Delete Method");
    }

    [MyAtributes(Name = "PUT")]
    public static void PutMethod()
    {
        var UpdateCar = MyDb.CarTbs.Where(x => x.Id == CommandClient.Value.Id).FirstOrDefault();
        if (UpdateCar == null)
            bw.Write(false);
        else
        {
            UpdateCar.Vendor = CommandClient.Value.Vendor;
            UpdateCar.Year = CommandClient.Value.Year;
            UpdateCar.Model = CommandClient.Value.Model;

            bw.Write(true);
            MyDb.SaveChanges();
        }
        Console.WriteLine("PUT Method");
    }

    public static BinaryWriter bw { get; set; }
    public static BinaryReader br { get; set; }
    [MyAtributes(Name = "POST")]
    public static void PostMethod()
    {
        if (MyDb.CarTbs.Where(x => x.Id == CommandClient.Value.Id).FirstOrDefault() is null)
        {
            MyDb.CarTbs.Add(CommandClient.Value);
            bw.Write(true);
            MyDb.SaveChanges();
        }
        else
            bw.Write(false);
        Console.WriteLine("Post Method");
    }


    [MyAtributes(Name = "GET")]
    public static void GetMethod()
    {
        var jsontext = JsonSerializer.Serialize(MyDb.CarTbs.ToList());
        bw.Write(jsontext);
        Console.WriteLine("Get Method");
    }








}

