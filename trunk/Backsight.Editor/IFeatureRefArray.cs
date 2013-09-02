using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backsight.Editor
{
    /// <summary>
    /// Something that makes a reference to an array of spatial features.
    /// </summary>
    /// <remarks>This interface was created to handle forward-references that might be
    /// encountered when loading data that originates from the old CEdit system.</remarks>
    interface IFeatureRefArray
    {
        /// <summary>
        /// Ensures that a persistent field has been associated with a spatial feature.
        /// </summary>
        /// <param name="field">A tag associated with the item</param>
        /// <param name="featureRefs">The features involved (not null).</param>
        void ApplyFeatureRefArray(DataField field, ForwardRefArrayItem[] featureRefs);
    }
}
