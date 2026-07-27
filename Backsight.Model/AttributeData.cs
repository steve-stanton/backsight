using System.Diagnostics;
using Backsight.Database;
using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// Miscellaneous attributes associated with spatial features.
/// </summary>
static class AttributeData
{
    /// <summary>
    /// Attaches miscellaneous attribute data to features
    /// </summary>
    /// <param name="features">The features to process (those that don't have a feature ID
    /// will be ignored)</param>
    /// <returns>The number of rows that were found (-1 if no database tables have
    /// been associated with Backsight)</returns>
    internal static int Load(Feature[] features)
    {
        var fids = new List<FeatureId>(features.Length);
        foreach (Feature f in features)
        {
            FeatureId fid = f.FeatureId;
            if (fid is not null)
                fids.Add(fid);
        }

        if (fids.Count == 0)
            return 0;
        else
            return Load(fids.ToArray());
    }

    /// <summary>
    /// Attaches miscellaneous attribute data to the features that have been loaded.
    /// </summary>
    /// <param name="fids">The feature IDs to look for</param>
    /// <returns>The number of rows that were found (-1 if no database tables have
    /// been associated with Backsight)</returns>
    /// <remarks>
    /// The current Backsight implementation deals primarily with the
    /// definition of the geometry for spatial features. While it is intended to
    /// provide basic attribute data entry, the overall design calls for a very
    /// loose binding.
    /// <para/>
    /// To cover this design goal, there are no references to miscellaneous attributes
    /// in any editing operation. This makes it possible to manipulate the attributes
    /// using external systems, with minimal concern for the impact it could have
    /// on Backsight (the only consequence of inadvertant attribute changes is
    /// that instances of <see cref="RowTextGeometry"/> could be orphaned by
    /// removing the associated attributes).
    /// <para/>
    /// While this simplifies the overall architecture, it is advisable to
    /// make any attribute data easily available to the user, since that may well
    /// guide the user regarding the relevance of spatial edits.
    /// <para/>
    /// This method will be called after the spatial features for a project have been
    /// loaded from the database. It takes a very simple-minded approach, by attempting
    /// to match features with every table associated with Backsight via the
    /// Environment Editor application (hopefully there aren't TOO many).
    /// This could potentially be overly time-consuming as part of the loading logic.
    /// While some of this could be addressed by lazy loading, or perhaps some more
    /// definitive layer-&gt;table associations, there is no proof that there is actually
    /// an issue that needs solving. Without that proof, it is considered inappropriate
    /// to code anything more complicated.
    /// </remarks>
    internal static int Load(FeatureId[] fids)
    {
        // Cross-reference the supplied IDs to their formatted key
        var keyIds = new Dictionary<string, FeatureId>(fids.Length);
        foreach (FeatureId fid in fids)
        {
            string key = fid.FormattedKey;
            FeatureId existingId;

            if (keyIds.TryGetValue(key, out existingId))
            {
                if (!object.ReferenceEquals(existingId, fid))
                    throw new Exception("More than one ID object for: "+key);
            }
            else
            {
                keyIds.Add(key, fid);
            }
        }

        return Load(keyIds);
    }

    /// <summary>
    /// Attaches miscellaneous attribute data to the features that have been loaded.
    /// </summary>
    /// <param name="keyIds">Index of the IDs to look for (indexed by formatted key)</param>
    /// <returns>The number of rows that were found (-1 if no database tables have
    /// been associated with Backsight)</returns>
    static int Load(Dictionary<string, FeatureId> keyIds)
    {
        // Locate information about the tables associated with Backsight
        ITable[] tables = EnvironmentRepository.Current.Tables.ToArray();
        if (tables.Length == 0)
            return -1;

        // Copy the required keys into a temp table
        Trace.WriteLine($"Locating attributes for {keyIds.Count} feature IDs in {tables.Length} tables");

        int nFound = 0;
        var repo = EnvironmentRepository.Current;
        var keys = keyIds.Keys.ToArray();
        
        foreach (ITable t in tables)
        {
            foreach (var record in repo.FindAttributeRecords(t, keys))
            {
                if (keyIds.TryGetValue(record.Id, out var fid))
                {
                    // Don't create a row if the ID is already associated with the
                    // table (this is meant to cover situations where the edit has actively
                    // formed the attributes, and is calling this method only to cover the
                    // fact that further attributes may be involved).

                    if (!fid.RefersToTable(t))
                    {
                        // Creating the row will update the supplied FeatureId to refer to it
                        var r = new Row(fid, record);
                        nFound++;
                    }
                }
                else
                {
                    throw new ApplicationException($"Cannot find '{record.Id}' in dictionary");
                }
            }
        }

        return nFound;
    }

    /*
     *
     * This doesn't belong in the model. Probably better as part of a ViewModel on a
     * replacement for the AttributeDataForm class.
     * 
    /// <summary>
    /// Displays database attributes so that they can be edited by the user.
    /// </summary>
    /// <param name="r">The row of interest</param>
    /// <returns>True if any changes were saved to the database</returns>
    internal static bool Update(Row r)
    {
        // If the row is associated with any RowText, ensure it is removed from
        // the spatial index NOW (if we wait until the edit has been completed,
        // it's possible we won't be able to update the index properly)
        TextFeature[] text = r.Id.GetRowText();
        EditingIndex index = CadastralMapModel.Current.EditingIndex;
        bool isChanged = false;

        try
        {
            // Remove the text from the spatial index (but see comment below)
            foreach (TextFeature tf in text)
                index.RemoveFeature(tf);

            // Display the attribute entry dialog
            var dial = new AttributeDataForm(r.Record);
            isChanged = dial.ShowDialog() == DialogResult.OK;
            dial.Dispose();

            if (isChanged)
                EnvironmentRepository.Current.UpdateRecord(r.Record);
        }

        finally
        {
            // Ensure text has been re-indexed... actually, this is likely to be
            // redundant, because nothing here has actually altered the stored
            // width and height of the text (if the attributes have become more
            // verbose, they'll just be scrunched up a bit tighter). The text
            // metrics probably should be reworked (kind of like AutoSize for
            // Windows labels), but I'm not sure whether this demands a formal
            // editing operation.

            foreach (TextFeature tf in text)
                index.AddFeature(tf);

            // Re-display the text if any changes have been saved
            if (isChanged)
            {
                EditingController.Current.ActiveMap.Redraw();
            }
        }

        return isChanged;
    }
    */
}