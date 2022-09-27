using System;
using System.Net;
using System.IO;

namespace server
{
    //we probably dont need this interface
    public interface IListen{
        string Name {get; set;}
        int Port {get; set;}
        string Ipaddress {get; set;}
    }

    //This class is translated from this blog:
    //https://0xrick.github.io/misc/c2/#about-c2-servers--agents
    public class Listener : IListen{
        private string path {get; set;}
        private string keyPath {get; set;}
        private string filePath {get; set;}
        private string agentsPath {get; set;}
        public string Name {get; set;}
        public int Port {get; set;}
        public string Ipaddress {get; set;}
        public string key {get; set;}

        public Listener(string name, int port, string ipaddress){
            this.Name = name;
            this.Port = port;
            this.Ipaddress = ipaddress;
            this.path = $"data/listeners/{this.Name}/";
            this.keyPath = $"{this.path}key";
            this.filePath = $"{this.path}files/";
            this.agentsPath = $"{this.path}agents/";
            //Create the paths defined above if they don't already exist
            Directory.CreateDirectory(this.path);
            //will need to generate a key in this location
            File.Create(this.keyPath);
            Directory.CreateDirectory(this.filePath);
            Directory.CreateDirectory(this.agentsPath);

            //this.key = generateKey();

        }
    }

    public class Server{
        static void Main(string[] args){
            Listener test = new Listener("test", 61, "192.168.1.1");
            Console.WriteLine(test.Port);
        }
    }
}
