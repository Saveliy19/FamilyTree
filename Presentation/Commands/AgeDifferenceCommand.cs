using BLL.Managers.Interfaces;
using DAL.Entities;
using Spectre.Console;

namespace Presentation.Commands
{
    public class AgeDifferenceCommand : ICommand
    {
        private readonly ITreeManager _treeManager;

        public AgeDifferenceCommand(ITreeManager treeManager) { _treeManager = treeManager; }
        public void Execute()
        {
            Console.Clear();
            var id1 = AnsiConsole.Ask<int>("Введите [bold green]Id первого человека[/]: ");
            var id2 = AnsiConsole.Ask<int>("Введите [bold green]Id второго человека[/]: ");

            int ageDifference = _treeManager.GetPersonsAgeDifference(new Person() { Id = id1 }, new DAL.Entities.Person() { Id = id2 });

            AnsiConsole.MarkupLine($"[bold yellow]Разница в возрасте в годах: {ageDifference}[/]");
        }
    }
}
