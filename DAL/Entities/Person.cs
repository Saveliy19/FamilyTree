namespace DAL.Entities
{
    public class Person: Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }

        public string Sex { get; set; }

        public Person Spouse { get; set; }

        public List<Person> Parents { get; set; }

        public List<Person> Children { get; set;} 
        
    }
}
