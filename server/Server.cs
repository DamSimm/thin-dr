using System;

namespace server
{
    public class Server{
        static void Main(string[] args){
            Console.Clear();
            try {
                //take command line arguments of Name, Port, IPaddress
                Listener test = new Listener(args[0], Int32.Parse(args[1]), args[2]);
                UserInput input = new UserInput(test);
                test.StartServer();
                //load already registered clients
                test.LoadRegisteredClients();
                //start the async listener
                //meant to not be awaited so that it runs in the background
                test.ListenAndRespond();
                //start our menu and take server input
                //acts as a block for the async functions
                input.MenuInput();
                //stop the server
                test.Terminate();
            } catch (System.IndexOutOfRangeException) {
                Console.WriteLine("Please provide the name of the listener, port to listen on, and the IP to bind to.");
                Environment.Exit(0);
            }
        }
    }

}