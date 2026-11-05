namespace Backsight.Model.Operations;

/// <summary>
/// Tagging interface for editing operations that may be recalled using the
/// Map Editor's command recall facility (the <c>Edit - Recall</c> command).
/// </summary>
/// <remarks>
/// This tag identifies those editing operation that may be recalled, but the operation
/// class does not have to do anything about it. The recall function only needs the
/// value provided by the <see cref="Operation.EditId"/> property. This gives the
/// Map Editor what it needs to relay the request to the correct handler. 
/// <para/>
/// The handler will usually involve an instance of the <c>CommandTool</c> class
/// in the <c>Backsight.Map.Editor</c> project. The responsibility for accommodating
/// command recall rests there.
/// </remarks>
public interface IRecallable;