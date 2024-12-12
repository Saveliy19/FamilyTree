using BLL.Managers;
using BLL.Managers.Interfaces;
using DAL.Entities;
using DAL.Repositories;
using DAL.Repositories.Interfaces;

class Programm
{
    private static  IPersonRepository _personRepository = new PersonRepository("./../../../../DAL/Data/tree.json");
    private static ITreeManager _treeManager = new TreeManager(_personRepository);
    static void Main(string[] args)
    {
        while (true) 
        {
            Console.WriteLine("Выберите действие");
            Console.WriteLine("1. Очистить дерево");
            Console.WriteLine("2. Добавить человека в дерево");
            Console.WriteLine("3. Установить связь двух людей");
            Console.WriteLine("4. Вычислить возраст одного человека при рождении другого");
            Console.WriteLine("5. Вывести ближайших родственников человека");
            Console.WriteLine("6. Вывести текущее дерево");
            Console.WriteLine("e. Выход");

            string choice = Console.ReadLine();

            switch (choice) 
            {
                case "1":
                    _treeManager.ClearTree();
                    Console.WriteLine();
                    Console.WriteLine("Дерево очищено!");
                    Console.WriteLine();
                    Console.WriteLine();
                    break;

                case "2":
                    Console.Write("Введите ФИО человека: ");
                    string name = Console.ReadLine();

                    Console.Write("Введите пол (Male/Female): ");
                    string sex = Console.ReadLine();

                    Console.Write("Введите дату рождения в формате dd-mm-yyyy: ");
                    string birthdate = Console.ReadLine();

                    _treeManager.AddPerson(new DAL.Entities.Person()
                    {
                        Name = name,
                        Sex = sex,
                        Birthdate = DateTime.Parse(birthdate),
                    });

                    Console.Clear();
                    Console.WriteLine($"{name} успешно добавлен!");
                    Console.WriteLine();

                    break;

                case "3":
                    Console.Write("Введите id человека, для которого нужно установить родственную связь: ");
                    string id = Console.ReadLine();

                    Person person = new Person() { Id = int.Parse(id) };

                    Console.Write("Установить супруга? y/n: ");
                    string res = Console.ReadLine();
                    if (res != "n") 
                    {
                        Console.Write("Введите Id супруга: ");
                        id = Console.ReadLine();
                        person.Spouse = new Person() { Id = int.Parse(id)};
                    }
                                    
                    Console.Write("Добавить родителей? y/n: ");
                    res = Console.ReadLine();
                    var parents = new List<Person>();
                    if (res != "n")
                    {
                        for (int i=0; i<2; i++)
                        {
                            Console.Write("Введите Id родителя: ");
                            id = Console.ReadLine();
                            parents.Add(new Person() { Id = int.Parse(id) });
                        }             
                        person.Parents = parents;
                    }

                    Console.Write("Добавить ребенка? y/n: ");
                    res = Console.ReadLine();
                    var children = new List<Person>();
                    if (res != "n")
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Console.Write("Введите Id ребенка: ");
                            id = Console.ReadLine();
                            children.Add(new Person() { Id = int.Parse(id) });
                        }
                        person.Children = children;
                    }
                    _treeManager.UpdatePerson(person);
                    break;

                case "4":
                    Console.WriteLine("Введите Id первого человека: ");
                    string id1 = Console.ReadLine();
                    Console.WriteLine("Введите Id второго человека: ");
                    string id2 = Console.ReadLine();
                    int ageDifference = _treeManager.GetPersonsAgeDifference(new DAL.Entities.Person() { Id = int.Parse(id1) }, new DAL.Entities.Person() { Id = int.Parse(id2) });
                    Console.WriteLine();
                    Console.WriteLine($"Разница в возрасте: {ageDifference}");
                    Console.WriteLine();
                    break;

                case "5":
                    Console.Write("Введите Id человека: ");
                    id = Console.ReadLine();
                    var relatives = _treeManager.GetPersonsCloseRelatives(new DAL.Entities.Person() { Id = int.Parse(id) });

                    Console.WriteLine();
                    Console.WriteLine($"Ближайшие родственники человека с Id = {id}:");
                    Console.WriteLine();

                    foreach (var relation in relatives)
                    {
                        Console.WriteLine(relation.Key);
                        Console.WriteLine();
                        foreach (var rel in relation.Value)
                        {
                            Console.WriteLine($"{rel.Id} - {rel.Name}");
                            Console.WriteLine(rel.Sex);
                            Console.WriteLine(rel.Birthdate);
                            Console.WriteLine();
                        }
                        Console.WriteLine("------------------------------");
                        Console.WriteLine();

                    }
                    break;

                case "6":
                    break;

                case "e":
                    Console.WriteLine("Выход из программы...");
                    Environment.Exit(0);
                    break;

                default:
                    Console.Clear();
                    Console.WriteLine("Неверный выбор!\n Пожалуйста, выберите одно из предложенных действий!");
                    Console.WriteLine();
                    break;
            }
        }
    }
}
