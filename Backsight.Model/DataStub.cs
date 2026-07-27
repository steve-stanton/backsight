namespace Backsight.Model;

abstract class DataStub
{
    /// <summary>
    /// The unique ID for an object (0 indicates a null). Values less than zero
    /// are not currently expected.
    /// </summary>
    private readonly int m_Id;

    protected DataStub(int id)
    {
        m_Id = id;
    }

    public int Id => m_Id;
}