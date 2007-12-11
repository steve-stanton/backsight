using System;

namespace Backsight.Editor.Forms
{
    /// <summary>
    /// Methods that must be implemented by dialogs that can be hosted by
    /// the <see cref="IntersectUI"/> class.
    /// </summary>
    /// <remarks>Was CdDialog. Still not sure if it's better to derive from an
    /// IntersectForm class</remarks>
    interface IIntersectDialog : IDisposable
    {
    }
}
