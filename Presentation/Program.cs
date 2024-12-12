using BLL.Managers;
using BLL.Managers.Interfaces;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Presentation.Factories;
using Spectre.Console;

class Program
{
    private static IPersonRepository _personRepository = new PersonRepository("./../../../../DAL/Data/tree.json");
    private static ITreeManager _treeManager = new TreeManager(_personRepository);
    private static CommandFactory _commandFactory = new CommandFactory(_treeManager);

    static void Main(string[] args)
    {
        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold cyan]Выберите действие:[/]")
                    .AddChoices(
                        "1. Очистить дерево",
                        "2. Добавить человека в дерево",
                        "3. Установить связь двух людей",
                        "4. Вычислить возраст одного человека при рождении другого",
                        "5. Вывести ближайших родственников человека",
                        "6. Вывести текущее дерево",
                        "7. Вывод всех людей в базе",
                        "e. Выход"));

            string commandChoice = choice.Trim().Split('.')[0];

            if (commandChoice == "e")
            {
                AnsiConsole.MarkupLine("[bold green]Выход из программы...[/]");
                break;
            }          

            var command = _commandFactory.GetCommand(commandChoice);

            if (command == null) AnsiConsole.MarkupLine("[bold red]Неверный выбор![/]");

            else command.Execute();

            AnsiConsole.MarkupLine("");
        }
    }
}
