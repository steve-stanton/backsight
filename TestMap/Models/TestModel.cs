namespace TestMap.Models;


public class TestModel
{
    public TestModel() : this("Steve")
    {
    }

    public TestModel(string name)
    {
        Name = name;
    }
    
    public string Name
    {
        get;
    }
}