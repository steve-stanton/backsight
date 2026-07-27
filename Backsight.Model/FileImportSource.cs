namespace Backsight.Model;

/// <written by="Steve Stanton" on="08-JUN-2007" />
/// <summary>
/// Something that imports spatial data from a file.
/// </summary>
abstract class FileImportSource
{
    /// <summary>
    /// The file specification of the import source.
    /// </summary>
    readonly string m_FileName;

    /// <summary>
    /// Creates a new <c>FileImportSource</c> that refers to the specified file.
    /// </summary>
    /// <param name="fileName">The file specification for the import source.</param>
    protected FileImportSource(string fileName)
    {
        m_FileName = fileName;
    }

    /// <summary>
    /// Loads the file on behalf of the supplied editing operation.
    /// </summary>
    /// <param name="creator">The editing operation performing the import</param>
    /// <returns>The loaded features</returns>
    internal Feature[] Load(Operation creator)
    {
        if (creator==null)
            throw new ArgumentNullException();

        return LoadFeatures(m_FileName, creator);
    }

    /// <summary>
    /// Creates features that correspond to the data in a file.
    /// </summary>
    /// <param name="fileName">The file containing data to import.</param>
    /// <param name="creator">The editing operation creating the features.</param>
    /// <returns>The features that correspond to the content of the file</returns>
    internal abstract Feature[] LoadFeatures(string fileName, Operation creator);

    /// <summary>
    /// The file specification of the import source.
    /// </summary>
    internal string FileName => m_FileName;
}