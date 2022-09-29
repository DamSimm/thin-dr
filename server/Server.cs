using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class Server{
        static void Main(string[] args){
            try {
                //take command line arguments of Name, Port, IPaddress
                Listener test = new Listener(args[0], Int32.Parse(args[1]), args[2]);
                UserInput input = new UserInput();
                test.StartServer();
                //start the async listener
                test.Listen();
                //start our menu and take server input
                input.MenuInput();
                test.Terminate();
            } catch (System.IndexOutOfRangeException) {
                Console.WriteLine("Please provide the name of the listener, port to listen on, and the IP to bind to.");
                Environment.Exit(0);
            }
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
            //Constructor for the Listener object. 
            //Listener is the main class of the server
            //and handles connections with clients
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
            //The main method of this class
            //per https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistener?view=net-6.0
            //and https://gist.github.com/define-private-public/d05bc52dd0bed1c4699d49e2737e80e7
            while(true){
                //Get our context asynchronously
                HttpListenerContext context = await _listener.GetContextAsync();
                HttpListenerRequest request = context.Request;

                //read client data
                Stream body = request.InputStream;
                System.Text.Encoding encoding = request.ContentEncoding;
                StreamReader reader = new System.IO.StreamReader(body, encoding);
                string test = reader.ReadToEnd();

                // Construct a response.
                HttpListenerResponse response = context.Response;
                string responseString = RespondToClient(test);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;

                //write our response string to the client using the outputstream
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer,0,buffer.Length);
            }
            // You must close the output stream.
            //Console.WriteLine("Listener Closed");
            //output.Close();
        }

        public string RespondToClient(string text){
            //will construct a response string to send to the client based on requests
            Console.WriteLine(text);
            return $"<HTML><BODY> Hello {text}</BODY></HTML>";
            //if the request is looking for a new command
                //check if there is a qued command for them
            //else if this is a new client
                //RegisterAgent
            //else
                //say no
        }

        //per https://zetcode.com/csharp/httplistener/ 
        public void RegisterAgent(){
            //accept POST requests to register an agent
            //HttpListenerContext ctx = _listener.GetContext();
            //using HttpListenerResponse resp = ctx.Response();
            //check the key that is given via POST
            //if it is valid
                //store the client's information in a file
            //if it isn't
                //return a 404 or just dont respond
        }
    }
}