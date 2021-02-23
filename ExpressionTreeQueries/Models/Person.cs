namespace ExpressionTreeQueries.Models
{
    public class Person
    {
        public Person(
            string name, 
            int age, 
            Gender gender, 
            int shoeSize
        )
        {
            Name = name;
            Age = age;
            Gender = gender;
            ShoeSize = shoeSize;
        }

        [Searchable]
        public string Name { get; set; }

        [Searchable]
        public int Age { get; set; }

        [Searchable]
        public long Id { get; set; }

        public Gender Gender { get; set; }

        [Searchable]
        public int ShoeSize { get; set; }
    }

    public enum Gender
    {
        Male,
        Female
    }

    public class Searchable : System.Attribute
    { }

}
