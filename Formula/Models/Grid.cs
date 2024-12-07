namespace Formula.Models;

public interface IGrid
{
    object GetValueForCell(string cellReference);
}

public class Grid : IGrid
{
    public object GetValueForCell(string cellReference)
    {
        return 2;
    }
}