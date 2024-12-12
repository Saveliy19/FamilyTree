using BLL.Managers.Interfaces;
using DAL.Entities;
using DAL.Repositories.Interfaces;
using Spectre.Console;

namespace BLL.Managers
{
    public class TreeManager: ITreeManager
    {
        private IPersonRepository _personRepository;

        public TreeManager(IPersonRepository personRepository) { _personRepository = personRepository; }

        public void AddPerson(Person person)
        {
            _personRepository.Create(person);
        }

        public void ClearTree()
        {
            _personRepository.DeleteAll();
        }

        public int GetPersonsAgeDifference(Person person1, Person person2)
        {
            person1 = _personRepository.Get(person1.Id);
            person2 = _personRepository.Get(person2.Id);

            Person ancestor, descendant; // предок и потомок

            if (person1.Birthdate > person2.Birthdate)
            {
                descendant = person1; 
                ancestor = person2;
            }
            else 
            {
                descendant = person2;
                ancestor = person1;
            }

            int ageDifference = descendant.Birthdate.Year - ancestor.Birthdate.Year;
            if (ancestor.Birthdate.AddYears(ageDifference) > descendant.Birthdate) ageDifference--;

            return ageDifference;
        }

        public Dictionary<string, List<Person>> GetPersonsCloseRelatives(Person person)
        {
            return _personRepository.FindCloseRelatives(person);
        }

        public void UpdatePerson(Person person)
        {
            if (person.Parents != null) 
            {
                foreach (Person parent in person.Parents) 
                {
                    parent.Children = new List<Person>() { person };
                    _personRepository.Update(parent);
                }
            }

            if (person.Children != null) 
            {
                foreach(Person child in person.Children)
                {
                    child.Parents = new List<Person> { person };
                    _personRepository.Update(child);
                }
            }

            if (person.Spouse != null)
            {
                person.Spouse.Spouse = person;
                _personRepository.Update(person.Spouse);
            }

            _personRepository.Update(person);
        }

        public void AddDescendants(TreeNode node, int personId)
        {
            var person = _personRepository.Get(personId);

            if (person.Children == null || !person.Children.Any()) return;

            foreach (var ch in person.Children)
            {
                var children = _personRepository.Get(ch.Id);
                var childNode = node.AddNode($"[green]{children.Name}[/] ([yellow]{children.Id}[/])");
                AddDescendants(childNode, children.Id);
            }
        }

        public void AddAncestors(TreeNode node, int personId)
        {
            var person = _personRepository.Get(personId);

            if (person.Parents == null || !person.Parents.Any()) return;

            foreach (var p in person.Parents)
            {
                var parent = _personRepository.Get(p.Id);
                var parentNode = node.AddNode($"[blue]{parent.Name}[/] ([yellow]{parent.Id}[/])");
                AddAncestors(parentNode, parent.Id);
            }
        }

        public Tree LoadTree(int rootId)
        {
            var rootPerson = _personRepository.Get(rootId);
            var tree = new Tree($"[bold]{rootPerson.Name}[/] ([yellow]{rootPerson.Id}[/])");

            if (rootPerson.Spouse != null)
            {
                var spouse = _personRepository.Get(rootPerson.Spouse.Id);
                tree.AddNode($"[bold underline]Супруг(а)[/]: [magenta]{spouse.Name}[/] ([yellow]{spouse.Id}[/])");
            }


            AddAncestors(tree.AddNode("[bold underline]Предки[/]"), rootId);

            AddDescendants(tree.AddNode("[bold underline]Потомки[/]"), rootId);
            return tree;
        }

    }
}
