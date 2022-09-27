using System;
using System.Net.Http;
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
        private HttpListener _listener;

        public void startServer(){
            //start our server on the specified IP and port
            string url = $"http://{Ipaddress}:{Port}";
            _listener.Prefixes.Add(url);
            _listener.Start();
            //check if our server is listening
            if (_listener.IsListening){
                Console.WriteLine($"Server is listening at {url}");
            } else {
                Console.WriteLine($"Server failed to start at {url}");
            }
        }

        public void stop(){
            //stop our listener
            _listener.Stop();
        }

        //per https://zetcode.com/csharp/httplistener/ 
        public void registerAgent(){
            //accept POST requests to register an agent
            HttpListenerContext ctx = _listener.GetContext();
            using HttpListenerResponse resp = ctx.Response();
        }

        public Listener(string name, int port, string ipaddress){
            this.Name = name;
            this.Port = port;
            this.Ipaddress = ipaddress;
            this.path = $"data/listeners/{this.Name}/";
            this.keyPath = $"{this.path}key";
            this.filePath = $"{this.path}files/";
            this.agentsPath = $"{this.path}agents/";
            //will need to generate a key in this location
            File.Create(this.keyPath);
            //Create the paths defined above if they don't already exist
            Directory.CreateDirectory(this.path);
            Directory.CreateDirectory(this.filePath);
            Directory.CreateDirectory(this.agentsPath);

            //this.key = generateKey();
            _listener = new HttpListener();
        }
    }

    public class Server{
        static void Main(string[] args){
            Listener test = new Listener("test", 61, "192.168.1.1");
            test.startServer();
        }
    }
}
