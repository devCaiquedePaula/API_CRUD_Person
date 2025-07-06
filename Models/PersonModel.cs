namespace PersonCRUD.Models;

public class PersonModel
{
    public PersonModel(string name)
    {
        Name = name;
        Id = Guid.NewGuid();
    }

    public Guid Id { get; init; }
    public string Name { get; private set; }

    public void ChangeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        Name = name;
    }

    public void SetInactive()
    {
        Name =  Name + " - " + "Desativado " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
}