using BLL.Managers.Interfaces;
using DAL.Entities;
using Spectre.Console;

namespace Presentation.Commands
{
    public class CreatePersonCommand : ICommand
    {
        private readonly ITreeManager _treeManager;

        public CreatePersonCommand(ITreeManager treeManager) { _treeManager = treeManager; }
        public void Execute()
        {
            var name = AnsiConsole.Ask<string>("Введите [bold green]ФИО человека[/]: ");
            var sex = AnsiConsole.Ask<string>("Введите [bold green]пол (Male/Female)[/]: ");
            var birthdate = AnsiConsole.Ask<string>("Введите [bold green]дату рождения в формате dd-mm-yyyy[/]: ");

            _treeManager.AddPerson(new Person()
            {
                Name = name,
                Sex = sex,
                Birthdate = DateTime.Parse(birthdate),
            });

            Console.Clear();
            AnsiConsole.MarkupLine($"[bold green]{name} успешно добавлен![/]");
        }
    }
}
