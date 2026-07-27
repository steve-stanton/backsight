using System.Diagnostics;
using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// Basic information for a spatial feature, used during deserialization from the database.
/// </summary>
class FeatureStub : IFeature, IPersistent
{
    /// <summary>
    /// The editing operation that created the feature (not null).
    /// </summary>
    readonly Operation m_Creator;

    /// <summary>
    /// The internal ID of the feature (holds the 1-based creation sequence
    /// of this feature within the project that created it).
    /// </summary>
    /// <remarks>The sequence value could be 0 if not yet defined (not sure if that still applies).</remarks>
    readonly InternalIdValue m_InternalId;

    /// <summary>
    /// The type of real-world object that the feature corresponds to.
    /// </summary>
    readonly IEntity m_What;

    /// <summary>
    /// The ID of the feature (may be shared by multiple features).
    /// </summary>
    readonly FeatureId? m_Id;

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStub"/> class
    /// using the data read from persistent storage.
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    internal FeatureStub(EditDeserializer editDeserializer)
    {
        m_Creator = editDeserializer.CurrentEdit;
        Debug.Assert(m_Creator != null);
        ReadData(editDeserializer, out m_InternalId, out m_What, out m_Id);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStub"/> class with the
    /// next available internal ID.
    /// </summary>
    /// <param name="creator">The editing operation that created the feature.</param>
    /// <param name="ent">The entity type for the feature (not null)</param>
    /// <param name="fid">The (optional) user-perceived ID for the feature.</param>
    /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
    /// <paramref name="creator"/> is null.</exception>
    internal FeatureStub(Operation creator, IEntity ent, FeatureId fid)
        : this(creator, creator.Session.AllocateNextId(), ent, fid)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStub"/> class
    /// </summary>
    /// <param name="creator">The editing operation that created the feature.</param>
    /// <param name="id">The internal ID of the feature within
    /// the project that created it.</param>
    /// <param name="ent">The entity type for the feature (not null)</param>
    /// <param name="fid">The (optional) user-perceived ID for the feature.</param>
    /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
    /// <paramref name="creator"/> is null.</exception>
    internal FeatureStub(Operation creator, InternalIdValue id, IEntity ent, FeatureId? fid)
    {
        if (creator == null || ent == null)
            throw new ArgumentNullException();

        m_Creator = creator;
        m_InternalId = id;
        m_What = ent;
        m_Id = fid;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureStub"/> class
    /// that contains a copy of the properties of a feature.
    /// </summary>
    /// <param name="f">The feature containing the properties to copy (not null).</param>
    /// <exception cref="ArgumentNullException">If the supplied feature is null.</exception>
    internal FeatureStub(IFeature f)
    {
        if (f == null)
            throw new ArgumentNullException();

        m_Creator = f.Creator;
        m_InternalId = f.InternalId;
        m_What = f.EntityType;
        m_Id = f.FeatureId;
    }

    /// <summary>
    /// The editing operation that created the feature (not null).
    /// </summary>
    public Operation Creator => m_Creator;

    /// <summary>
    /// The internal ID of this feature (holds the 1-based creation sequence
    /// of this feature within the project that created it).
    /// </summary>
    public InternalIdValue InternalId => m_InternalId;

    /// <summary>
    /// The type of real-world object that the feature corresponds to (not null).
    /// </summary>
    public IEntity EntityType => m_What;

    /// <summary>
    /// The ID of the feature (may be shared by multiple features).
    /// </summary>
    public FeatureId FeatureId => m_Id;

    /// <summary>
    /// Writes the content of this instance to a persistent storage area.
    /// </summary>
    /// <param name="editSerializer">The mechanism for storing content.</param>
    public void WriteData(EditSerializer editSerializer)
    {
        editSerializer.WriteInternalId(DataField.Id, m_InternalId);
        editSerializer.WriteEntity(DataField.Entity, m_What);
        editSerializer.WriteFeatureId(m_Id);
    }

    /// <summary>
    /// Reads data that was previously written using <see cref="WriteData"/>
    /// </summary>
    /// <param name="editDeserializer">The mechanism for reading back content.</param>
    /// <param name="id">The internal of the feature within the project that created it.</param>
    /// <param name="entity">The type of real-world object that the feature corresponds to.</param>
    /// <param name="fid">The ID of the feature (may be null).</param>
    static void ReadData(EditDeserializer editDeserializer, out InternalIdValue id, out IEntity entity, out FeatureId? fid)
    {
        id = editDeserializer.ReadInternalId(DataField.Id);
        entity = editDeserializer.ReadEntity(DataField.Entity);
        fid = editDeserializer.ReadFeatureId();
    }
}