using BLL.Managers.Interfaces;
using DAL.Entities;
using Spectre.Console;

namespace Presentation.Commands
{
    public class SetRelationCommand: ICommand
    {
        private readonly ITreeManager _treeManager;

        public SetRelationCommand(ITreeManager treeManager) { _treeManager = treeManager; }

        public void Execute()
        {
            var id = AnsiConsole.Ask<int>("Введите [bold green]id человека[/], для которого нужно установить родственные связи: ");
            var person = new Person() { Id = id };

            var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"Что вы хотите установить для [bold green]человека с id {id}[/]?")
                .AddChoices("Супруга", "Родителей", "Детей", "Завершить"));

            switch (choice)
            {
                case "Супруга":
                    SetSpouse(person);
                    break;

                case "Родителей":
                    SetParents(person);
                    break;

                case "Детей":
                    SetChildren(person);
                    break;

                case "Завершить":
                    AnsiConsole.MarkupLine("[bold green]Процесс завершен.[/]");
                    return;
            }

            _treeManager.UpdatePerson(person);

            AnsiConsole.MarkupLine($"[bold green]Родственные связи для человека с id {person.Id} успешно обновлены![/]");
        }

        private void SetSpouse(Person person)
        {
            var res = AnsiConsole.Ask<string>("Установить [bold yellow]супруга[/]? (y/n): ");
            if (res.ToLower() != "n")
            {
                var spouseId = AnsiConsole.Ask<int>("Введите [bold yellow]Id супруга[/]: ");
                person.Spouse = new Person() { Id = spouseId };
            }
        }

        private void SetParents(Person person)
        {
            var parents = new List<Person>();
            var counter = 0;

            string addMoreParent = "y";

            do
            {
                if (counter > 1) break;
                var parentId = AnsiConsole.Ask<int>("Введите [bold yellow]Id родителя[/]: ");
                parents.Add(new Person() { Id = parentId });
                counter++;
                addMoreParent = AnsiConsole.Ask<string>("Хотите добавить еще одного родителя? (y/n): ").ToLower();
            }
            while (addMoreParent == "y");
            person.Parents = parents;
        }

        private void SetChildren(Person person)
        {
            var children = new List<Person>();
            string addMoreChildren = "y";

            do
            {
                var childId = AnsiConsole.Ask<int>("Введите [bold yellow]Id ребенка[/]: ");
                children.Add(new Person() { Id = childId });

                addMoreChildren = AnsiConsole.Ask<string>("Хотите добавить еще ребенка? (y/n): ").ToLower();
            }
            while (addMoreChildren == "y");
            person.Children = children;
        }

    }
}
