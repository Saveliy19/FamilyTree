using BLL.Managers.Interfaces;
using DAL.Entities;
using Spectre.Console;

namespace Presentation.Commands
{
    internal class ShowAllPersonsCommand : ICommand
    {
        private readonly ITreeManager _treeManager;

        public ShowAllPersonsCommand(ITreeManager treeManager) { _treeManager = treeManager; }

        public void Execute()
        {
            List<Person> people = _treeManager.GetAllPersons();

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Имя");
            table.AddColumn("Пол");
            table.AddColumn("Дата рождения");
            table.AddColumn("Супруг(а)");
            table.AddColumn("Родители");
            table.AddColumn("Дети");

            foreach (var person in people)
            {
                string spouse = person.Spouse != null ? person.Spouse.Id.ToString() : "Нет";

                string parents = person.Parents != null && person.Parents.Any()
                    ? string.Join(", ", person.Parents.Select(p => p.Id))
                    : "Нет";

                string children = person.Children != null && person.Children.Any()
                    ? string.Join(", ", person.Children.Select(c => c.Id))
                    : "Нет";

                table.AddRow(
                    person.Id.ToString(),
                    person.Name,
                    person.Sex,
                    person.Birthdate.ToString("dd.MM.yyyy"),
                    spouse,
                    parents,
                    children
                );
            }

            AnsiConsole.Write(table);
        }
    }
}
