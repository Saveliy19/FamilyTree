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
            if (person.Birthdate > DateTime.Now) throw new Exception("Человек не мог родиться позже текущей даты!");
            _personRepository.Create(person);
        }

        public void ClearTree()
        {
            _personRepository.DeleteAll();
        }

        public int GetPersonsAgeDifference(Person person1, Person person2)
        {
            try
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
            catch (Exception) { throw; }
            
        }

        public Dictionary<string, List<Person>> GetPersonsCloseRelatives(Person person)
        {
            try
            {
                return _personRepository.FindCloseRelatives(person);
            }
            catch (Exception) { throw; }
        }

        public void UpdatePerson(Person person)
        {
            try
            {
                var existingPerson = _personRepository.Get(person.Id);
                var personBirthYear = existingPerson.Birthdate.Year;
                if (person.Parents != null)
                    foreach (Person parent in person.Parents)
                    {
                        existingPerson = _personRepository.Get(parent.Id);
                        if (personBirthYear < existingPerson.Birthdate.Year) throw new Exception("Предок не может быть моложе потомка!");
                    }                        

                if (person.Children != null)
                    foreach (Person children in person.Children)
                    {
                        existingPerson = _personRepository.Get(children.Id);
                        if (personBirthYear > existingPerson.Birthdate.Year) throw new Exception("Предок не может быть моложе потомка!");
                    }
                        

                if (person.Spouse != null)
                    existingPerson = _personRepository.Get(person.Spouse.Id);

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
                    foreach (Person child in person.Children)
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
            catch (Exception) { throw; }            
        }

        private void AddDescendants(TreeNode node, int personId)
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

        private void AddAncestors(TreeNode node, int personId)
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
            try
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
            catch (Exception) { throw; }
        }

        public List<Person> GetAllPersons()
        {
            return _personRepository.GetAll();
        }

    }
}
