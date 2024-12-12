using DAL.Entities;

namespace DAL.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        // создать человека
        public void Create(Person person);

        // обновить данные человека (женился / развелся / родил детей)
        public void Update(Person person);

        // получить человека по айди
        public Person Get(int id);

        // очистить дерево
        public void DeleteAll();

        // найти близких родственников
        public Dictionary<string, List<Person>> FindCloseRelatives(Person person);

        // найти всех людей в дереве

        public List<Person> GetAll();
    }
}
