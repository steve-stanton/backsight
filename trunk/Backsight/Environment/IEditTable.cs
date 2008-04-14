using System;

namespace Backsight.Environment
{
    /// <written by="Steve Stanton" on="14-APR-2008"/>
    /// <summary>
    /// A mutable version of <c>ITable</c>
    /// </summary>
    public interface IEditTable : ITable, IEditControl
    {
        /// <summary>
        /// The name of the database table (possibly decorated with schema name)
        /// </summary>
        new string TableName { get; set; }
    }
}
