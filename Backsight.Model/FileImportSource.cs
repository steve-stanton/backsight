using Backsight.Model.Operations;

namespace Backsight.Model;

/// <written by="Steve Stanton" on="08-JUN-2007" />
/// <summary>
/// Something that imports spatial data from a file.
/// </summary>
public abstract class FileImportSource
{
    /// <summary>
    /// The file specification of the import source.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Creates a new <c>FileImportSource</c> that refers to the specified file.
    /// </summary>
    /// <param name="fileName">The file specification for the import source.</param>
    protected FileImportSource(string fileName)
    {
        FileName = fileName;
    }

    /// <summary>
    /// Loads the file on behalf of the supplied editing operation.
    /// </summary>
    /// <param name="creator">The editing operation performing the import</param>
    /// <returns>The loaded features</returns>
    internal Feature[] Load(ImportOperation creator)
    {
        if (creator==null)
            throw new ArgumentNullException();

        return LoadFeatures(creator);
    }

    /// <summary>
    /// Creates features that correspond to the data in a file.
    /// </summary>
    /// <param name="creator">The editing operation creating the features.</param>
    /// <returns>The features that correspond to the content of the file</returns>
    protected abstract Feature[] LoadFeatures(ImportOperation creator);
}