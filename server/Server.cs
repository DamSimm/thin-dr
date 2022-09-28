using System;
using System.Net;
using System.IO;
using System.Threading.Tasks;

namespace server
{
    public class Server{
        static void Main(string[] args){
            //take command line arguments of Name, Port, IPaddress
            Listener test = new Listener(args[0], Int32.Parse(args[1]), args[2]);
            test.StartServer();
            var handle = test.Listen();
            handle.Wait();
            test.Terminate();
        }
    }

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

            //Create the paths defined above if they don't already exist
            Directory.CreateDirectory(this.path);
            Directory.CreateDirectory(this.filePath);
            Directory.CreateDirectory(this.agentsPath);

            //will need to generate a key in this location
            File.Create(this.keyPath);

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

        public async Task Listen(){
            //per https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-6.0
            //and https://gist.github.com/define-private-public/d05bc52dd0bed1c4699d49e2737e80e7
            while(true){
                //Get our context asynchronously
                HttpListenerContext context = await _listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                // Construct a response.
                string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;

                //write our response string to the client using the outputstream
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer,0,buffer.Length);

                // You must close the output stream.
                output.Close();
            }
        }

        public void AsyncRespond(){
            //per https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-6.0
            //loop 5 times
            while(true){
                /*
                IAsyncResult result = _listener.BeginGetContext(new AsyncCallback(Listen),_listener);
                // Applications can do some work here while waiting for the
                // request. If no work can be done until you have processed a request,
                // use a wait handle to prevent this thread from terminating
                // while the asynchronous operation completes.
                result.StartAsync();
                Console.WriteLine("Waiting for request to be processed asyncronously.");
                //Console.WriteLine("Request processed asyncronously.");
                */
            }
        }

        //per https://zetcode.com/csharp/httplistener/ 
        public void RegisterAgent(){
            //accept POST requests to register an agent
            //HttpListenerContext ctx = _listener.GetContext();
            //using HttpListenerResponse resp = ctx.Response();
        }

    }
}