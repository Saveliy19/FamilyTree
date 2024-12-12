using BLL.Managers.Interfaces;
using DAL.Entities;
using Spectre.Console;

namespace Presentation.Commands
{
    public class CloseRelativesCommand : ICommand
    {
        private readonly ITreeManager _treeManager;

        public CloseRelativesCommand(ITreeManager treeManager)
        {
            _treeManager = treeManager;
        }

        public void Execute()
        {
            try
            {
                Console.Clear();
                Console.Write("Введите Id человека: ");
                string id = Console.ReadLine();

                var relatives = _treeManager.GetPersonsCloseRelatives(new Person() { Id = int.Parse(id) });

                if (relatives.Count == 0)
                {
                    AnsiConsole.MarkupLine($"[red]Родственники для человека с Id = {id} не найдены![/]");
                    return;
                }

                AnsiConsole.MarkupLine($"[bold yellow]Ближайшие родственники человека с Id = {id}[/]");
                AnsiConsole.Write(new Rule("[blue]Родственники[/]"));

                foreach (var relation in relatives)
                {
                    var table = new Table()
                        .Title($"[green]{relation.Key}[/]")
                        .AddColumn("[blue]ID[/]")
                        .AddColumn("[blue]Имя[/]")
                        .AddColumn("[blue]Пол[/]")
                        .AddColumn("[blue]Дата рождения[/]");

                    foreach (var rel in relation.Value)
                    {
                        table.AddRow(
                            $"[yellow]{rel.Id}[/]",
                            $"[white]{rel.Name}[/]",
                            $"[white]{rel.Sex}[/]",
                            $"[white]{rel.Birthdate:dd-MM-yyyy}[/]"
                        );
                    }

                    AnsiConsole.Write(table);
                    AnsiConsole.Write(new Rule().RuleStyle("gray").Centered());
                }
            }
            catch (Exception ex) { AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]"); }

        }
    }
}
