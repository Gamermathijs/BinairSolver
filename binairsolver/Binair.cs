namespace binairsolver;

public class Binair
{
    public readonly List<Row> Rows = new();
    
    public bool Solved = false;

    public Boolean SetValue(int x, int y, String value)
    {
        if (Size <= x || Size <= y)
        {
            return false;
        }

        if (Rows[x].Values[y] != " ")
        {
            return false;
        }
        
        if(value == " ")
        {
            return false;
        }
        
        Rows[x].Values[y] = value;

        return true;
    }

    public int Size;
}

public class Row
{
    public readonly List<String> Values = new();
}