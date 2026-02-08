namespace SHSDP.Models;

public sealed class GHClassroom
{
    public int Id { get; set; }
    public String Name { get; set; }
    
    public override String ToString() => $"Id: {Id}, Name: {Name}";
}