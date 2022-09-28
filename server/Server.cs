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

    //This class is inspired from this blog:
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

        public void StartServer(){
            //start our server on the specified IP and port
            string url = $"http://{Ipaddress}:{Port}/";
            _listener.Prefixes.Add(url);
            _listener.Start();
            //check if our server is listening
            if (_listener.IsListening){
                Console.WriteLine($"Server is listening at {url}");
            } else {
                Console.WriteLine($"Server failed to start at {url}");
            }
        }

        public void Terminate(){
            //stop our listener
            _listener.Stop();
        }

        private void Listen(IAsyncResult result){
            //per https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-6.0
            HttpListener listener = (HttpListener) result.AsyncState;
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = listener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer,0,buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        public void AsyncRespond(){
            //per https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-6.0
            //loop 5 times
            for (int x = 0; x < 5; x++){
                IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(Listen),_listener);
                // Applications can do some work here while waiting for the
                // request. If no work can be done until you have processed a request,
                // use a wait handle to prevent this thread from terminating
                // while the asynchronous operation completes.
                Console.WriteLine("Waiting for request to be processed asyncronously.");
                result.AsyncWaitHandle.WaitOne();
                Console.WriteLine("Request processed asyncronously.");
            }
        }

        //per https://zetcode.com/csharp/httplistener/ 
        public void RegisterAgent(){
            //accept POST requests to register an agent
            //HttpListenerContext ctx = _listener.GetContext();
            //using HttpListenerResponse resp = ctx.Response();
        }

    }

    public class Server{
        static void Main(string[] args){
            Listener test = new Listener("test", 61, "127.0.0.1");
            test.StartServer();
            test.AsyncRespond();
            test.Terminate();
        }
    }
}