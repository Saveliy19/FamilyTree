using BLL.Managers.Interfaces;
using Spectre.Console;

namespace Presentation.Commands
{
    public class ClearTreeCommand: ICommand
    {
        private readonly ITreeManager _treeManager;

        public ClearTreeCommand(ITreeManager treeManager)
        {
            _treeManager = treeManager;
        }

        public void Execute()
        {
            _treeManager.ClearTree();
            AnsiConsole.MarkupLine($"[bold green]Дерево очищено![/]");
        }
    }
}
