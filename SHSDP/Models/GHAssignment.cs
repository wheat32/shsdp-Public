namespace SHSDP.Models;

public sealed class GHAssignment
{
    public int Id { get; set; }
    public String Title { get; set; }
    
    public override String ToString() => $"Id: {Id}, Title: {Title}";
}