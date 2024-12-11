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
            foreach (var child in person.Children) { childrenList.Add(child.Id); }

            var parentsList = new JArray();
            foreach (var parent in person.Parents) { parentsList.Add(parent.Id); }

            var newPerson = new JObject
            {
                { "name", person.Name },
                { "birthdate", person.Birthdate },
                { "sex", person.Sex },
                { "spouse", person.Spouse.Id},
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

        public Dictionary<string, List<Person>> FindCloseRelatives(Person person, int level)
        {
            var relatives = new Dictionary<string, List<Person>>();

            JObject json = JObject.Parse(File.ReadAllText(_dataPath));

            var lookingPerson = json["people"].FirstOrDefault(p => p.Path == $"people.{person.Id.ToString()}");

            if (lookingPerson != null) 
            {
                // ищем супруга
                var relativeJson = json["people"].FirstOrDefault(p => p.Path == $"people.{lookingPerson["spouce"].ToString()}");
                var relative = new Person()
                {
                    Id = int.Parse(lookingPerson["spouse"].ToString()),
                    Name = relativeJson["name"].ToString(),
                    Sex = relativeJson["sex"].ToString(),
                    Birthdate = DateTime.Parse(relativeJson["birthdate"].ToString()),
                };

                relatives.Add("Spouce", new List<Person> { relative });

                var persons = new List<Person>();
                // ищем родителей
                foreach (var parentId in lookingPerson["parents"])
                {
                    if (parentId != null)
                    {
                        relativeJson = json["people"].FirstOrDefault(p => p.Path == $"people.{parentId.ToString()}");
                        relative = new Person()
                        {
                            Id = int.Parse(parentId.ToString()),
                            Name = relativeJson["name"].ToString(),
                            Sex = relativeJson["sex"].ToString(),
                            Birthdate = DateTime.Parse(relativeJson["birthdate"].ToString()),
                        };

                        persons.Add(relative);
                    }
                }

                relatives.Add("Parents", persons);

                persons = new List<Person>();
                // ищем детей
                foreach (var childId in lookingPerson["children"])
                {
                    if (childId != null)
                    {
                        relativeJson = json["people"].FirstOrDefault(p => p.Path == $"people.{childId.ToString()}");
                        relative = new Person()
                        {
                            Id = int.Parse(childId.ToString()),
                            Name = relativeJson["name"].ToString(),
                            Sex = relativeJson["sex"].ToString(),
                            Birthdate = DateTime.Parse(relativeJson["birthdate"].ToString()),
                        };

                        persons.Add(relative);
                    }
                }

                relatives.Add("Children", persons);


                return relatives;
            }
            else { throw new Exception(); }

            
        }

        public Person Get(int personId)
        {
            JObject json = JObject.Parse(File.ReadAllText(_dataPath));

            var jsonPerson = json["people"].FirstOrDefault(p => p.Path == $"people.{personId.ToString()}");

            if (jsonPerson != null)
            {
                var person = new Person()
                {
                    Id = personId,
                    Name = jsonPerson["name"].ToString(),
                    Sex = jsonPerson["sex"].ToString(),
                    Birthdate = DateTime.Parse(jsonPerson["birthdate"]?.ToString()),

                    Spouse = jsonPerson["spouse"] != null
                    ? new Person { Id = (int)jsonPerson["spouse"] }
                    : null,

                    Parents = jsonPerson["parents"] != null
                    ? jsonPerson["parents"].Select(p => new Person { Id = (int)p }).ToList()
                    : new List<Person>(),

                    Children = jsonPerson["children"] != null
                    ? jsonPerson["children"].Select(c => new Person { Id = (int)c }).ToList()
                    : new List<Person>()
                };

                return person;
            }
            else { throw new Exception(); }
        }

        public void Update(Person person)
        {

            JObject json = JObject.Parse(File.ReadAllText(_dataPath));

            var personToUpdate = json["people"].FirstOrDefault(p => p.Path == $"people.{person.Id.ToString()}" );

            if (personToUpdate != null)
            {
                personToUpdate["name"] = person.Name;
                personToUpdate["spouse"] = person.Spouse.Id;

                var childrenList = new JArray();
                foreach (var child in person.Children) { childrenList.Add(child.Id); }

                var parentsList = new JArray();
                foreach (var parent in person.Parents) { parentsList.Add(parent.Id); }

                File.WriteAllText(_dataPath, json.ToString());
            }
            else { throw new Exception(); }
        }
    }
}
