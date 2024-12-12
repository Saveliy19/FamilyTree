using DAL.Entities;
using DAL.Repositories.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DAL.Repositories
{
    public class PersonRepository: IPersonRepository
    {
        private string _dataPath;

        public PersonRepository(string dataPath) { _dataPath = dataPath; }

        public void Create(Person person)
        {

            JObject json = JObject.Parse(File.ReadAllText(_dataPath));

            int newId = json["people"].Children<JProperty>().Count() + 1;

            var childrenList = new JArray();
            if (person.Children is not null)
                foreach (var child in person.Children) { childrenList.Add(child.Id); }

            var parentsList = new JArray();
            if (person.Parents is not null)
                foreach (var parent in person.Parents) { parentsList.Add(parent.Id); }

            var newPerson = new JObject
            {
                { "name", person.Name },
                { "birthdate", person.Birthdate.ToString("dd.MM.yyyy") },
                { "sex", person.Sex },
                { "spouse", person.Spouse != null ? (JToken)person.Spouse.Id : null },
                { "parents", parentsList},
                { "children", childrenList}
            };

            json["people"][newId.ToString()] = newPerson;

            File.WriteAllText(_dataPath, json.ToString());

        }

        public void DeleteAll()
        {
            JObject json = JObject.Parse(File.ReadAllText(_dataPath));

            json["people"] = new JObject();

            File.WriteAllText(_dataPath, json.ToString());
        }

        public Dictionary<string, List<Person>> FindCloseRelatives(Person person)
        {
            var relatives = new Dictionary<string, List<Person>>();

            // Получаем данные о текущем человеке из JSON
            var currentPerson = Get(person.Id);

            // Поиск супруга
            if (currentPerson.Spouse != null)
            {
                var spouse = Get(currentPerson.Spouse.Id);
                relatives.Add("Супруг(а)", new List<Person> { spouse });
            }

            // Поиск родителей
            if (currentPerson.Parents != null && currentPerson.Parents.Any())
            {
                var parents = currentPerson.Parents
                    .Select(parent => Get(parent.Id))
                    .ToList();
                relatives.Add("Родители", parents);
            }

            // Поиск детей
            if (currentPerson.Children != null && currentPerson.Children.Any())
            {
                var children = currentPerson.Children
                    .Select(child => Get(child.Id))
                    .ToList();
                relatives.Add("Дети", children);
            }

            return relatives;
        }



        public Person Get(int personId)
        {
            // Чтение JSON из файла
            JObject json = JObject.Parse(File.ReadAllText(_dataPath));

            // Поиск человека по ID
            var jsonPerson = json["people"][$"{personId}"];

            if (jsonPerson != null)
            {
                var person = new Person
                {
                    Id = personId,
                    Name = jsonPerson["name"]?.ToString(),
                    Sex = jsonPerson["sex"]?.ToString(),
                    Birthdate = DateTime.Parse(jsonPerson["birthdate"].ToString()),

                    // Супруг
                    Spouse = jsonPerson["spouse"] != null && !string.IsNullOrEmpty(jsonPerson["spouse"].ToString())
                        ? new Person { Id = int.Parse(jsonPerson["spouse"].ToString()) }
                        : null,

                    // Родители
                    Parents = jsonPerson["parents"] != null && jsonPerson["parents"].HasValues
                        ? jsonPerson["parents"].Select(p => new Person { Id = int.Parse(p.ToString()) }).ToList()
                        : new List<Person>(),

                    // Дети
                    Children = jsonPerson["children"] != null && jsonPerson["children"].HasValues
                        ? jsonPerson["children"].Select(c => new Person { Id = int.Parse(c.ToString()) }).ToList()
                        : new List<Person>()
                };

                return person;
            }
            else
            {
                throw new Exception($"Человек с ID {personId} не найден.");
            }
        }


        public void Update(Person person)
        {
            JObject json = JObject.Parse(File.ReadAllText(_dataPath));

            var personToUpdate = json["people"][$"{person.Id}"];
            if (personToUpdate == null) throw new Exception($"Человек с ID {person.Id} не найден.");

            if (person.Spouse != null) personToUpdate["spouse"] = JToken.FromObject(person.Spouse.Id);

            var existingParents = personToUpdate["parents"] as JArray ?? new JArray();
            if (person.Parents != null)
            {
                foreach (var parent in person.Parents)
                {
                    if (!existingParents.Any(p => (int)p == parent.Id))
                    {
                        existingParents.Add(parent.Id);
                    }
                }
            }
            personToUpdate["parents"] = existingParents;

            var existingChildren = personToUpdate["children"] as JArray ?? new JArray();
            if (person.Children != null)
            {
                foreach (var child in person.Children)
                {
                    if (!existingChildren.Any(c => (int)c == child.Id))
                    {
                        existingChildren.Add(child.Id);
                    }
                }
            }
            personToUpdate["children"] = existingChildren;
            File.WriteAllText(_dataPath, json.ToString());
        }



    }
}
