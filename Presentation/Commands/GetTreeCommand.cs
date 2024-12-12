using BLL.Managers.Interfaces;
using Spectre.Console;

namespace Presentation.Commands
{
    public class GetTreeCommand: ICommand
    {
        private readonly ITreeManager _treeManager;

        public GetTreeCommand(ITreeManager treeManager) { _treeManager = treeManager; }

        public void Execute() 
        {
            Console.Clear();
            Console.Write("Введите айди человека, для которого необходимо вывести древо: ");
            string id = Console.ReadLine();
            var tree = _treeManager.LoadTree(int.Parse(id));
            AnsiConsole.Write(tree);
        }
    }
}
