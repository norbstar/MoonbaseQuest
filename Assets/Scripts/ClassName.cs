[System.AttributeUsage(System.AttributeTargets.Class)]
public class ClassName : System.Attribute
{
    private string _name;

    public ClassName(string name)
    {
        _name = name;        
    }

    public string GetName()
    {
        return _name;
    }
}