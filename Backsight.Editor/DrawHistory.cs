namespace Backsight.Editor;

class DrawHistory
{
    /// <summary>
    /// Info about previous draw extents
    /// </summary>
    private List<DrawInfo> m_Extents = [];

    /// <summary>
    /// Index of the draw that's currently on screen ((-1 if there is no draw info).
    /// Refers to an element in <c>m_Extents</c>
    /// </summary>
    private int m_CurrentExtentIndex = -1;

    internal bool IsNextEnabled => (m_CurrentExtentIndex+1) < m_Extents.Count;

    /// <summary>
    /// Appends a new draw extent
    /// </summary>
    internal void AddDraw(DrawInfo info)
    {
        // If we currently have 32 extents, drop the head.
        if (m_Extents.Count==32)
            m_Extents.RemoveAt(0);

        // Remember a new extent, add make it the current one
        m_Extents.Add(info);
        m_CurrentExtentIndex = m_Extents.Count-1;
    }

    internal DrawInfo? GetCurrentDraw()
    {
        if (m_CurrentExtentIndex < 0 || m_CurrentExtentIndex >= m_Extents.Count)
            return null;

        return  m_Extents[m_CurrentExtentIndex];
    }

    internal bool IsPreviousEnabled => m_CurrentExtentIndex>0;

    internal bool SetPrevious()
    {
        if (m_CurrentExtentIndex > 0)
        {
            m_CurrentExtentIndex--;
            return true;
        }

        return false;
    }

    internal bool SetNext()
    {
        if (m_CurrentExtentIndex < m_Extents.Count-1)
        {
            m_CurrentExtentIndex++;
            return true;
        }

        return false;
    }
    
    /// <summary>
    /// Gets rid of all draw extents that may have been stored. 
    /// </summary>
    internal void RemoveAllDraws()
    {
        m_Extents = new List<DrawInfo>();
        m_CurrentExtentIndex = -1;
    }
}