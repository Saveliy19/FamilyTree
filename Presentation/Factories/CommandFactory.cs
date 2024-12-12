using BLL.Managers.Interfaces;
using Presentation.Commands;

namespace Presentation.Factories
{
    public class CommandFactory
    {
        private readonly ITreeManager _treeManager;

        public CommandFactory(ITreeManager treeManager)
        {
            _treeManager = treeManager;
        }

        public ICommand GetCommand(string choice)
        {
            return choice switch
            {
                "1" => new ClearTreeCommand(_treeManager),
                "2" => new CreatePersonCommand(_treeManager),
                "3" => new SetRelationCommand(_treeManager),
                "4" => new AgeDifferenceCommand(_treeManager),
                "5" => new CloseRelativesCommand(_treeManager),
                "6" => new GetTreeCommand(_treeManager),
                "7" => new ShowAllPersonsCommand(_treeManager),
                _ => null
            };
        }
    }
}
