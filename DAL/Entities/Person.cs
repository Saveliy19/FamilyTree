namespace DAL.Entities
{
    public class Person: Entity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }

        public string Sex { get; set; } = string.Empty;

        public Person Spouse { get; set; } = new();

        public List<Person> Parents { get; set; } = new();

        public List<Person> Children { get; set;} = new();       
        
    }
}
