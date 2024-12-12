using DAL.Entities;
using Spectre.Console;

namespace BLL.Managers.Interfaces
{
    public interface ITreeManager
    {
        // Создать сущность “Человек” и добавить в древо;
        public void AddPerson(Person person);

        // Установить отношения (Указать, кто кому приходится родителем, ребенком или супругом);
        public void UpdatePerson(Person person);

        // Вывести ближайших родственников (родителей и детей);
        public Dictionary< string, List<Person> > GetPersonsCloseRelatives(Person person);

        // Вычислить возраст предка при рождении потомка;
        public int GetPersonsAgeDifference(Person person1, Person person2);

        // Создать новое древо(очищение предыдущего построенного дерева).
        public void ClearTree();


        // Показать получившееся древо;
        public Tree LoadTree(int rootId);

        public List<Person> GetAllPersons();
    }
}
