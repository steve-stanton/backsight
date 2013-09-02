using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Backsight.Editor.Operations;

namespace Backsight.Editor
{
    class ForwardSplit
    {
        readonly ForwardFeatureRef m_LineRef;
        readonly string m_SplitBeforeId;
        readonly string m_SplitAfterId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardFeatureRef"/> class.
        /// </summary>
        /// <param name="referenceFrom">The object that makes the forward-reference (not null).</param>
        /// <param name="field">The ID of the persistent field.</param>
        /// <param name="parentLineId">The internal ID that has been persisted for the field (relating to a feature
        /// that has not been created yet).</param>
        /// <param name="splitBeforeId">The ID of the line section prior to the split (null if no split)</param>
        /// <param name="splitAfterId">The ID of the line section after the the split (null if no split)</param>
        /// <exception cref="ArgumentNullException">If <paramref name="referenceFrom"/> is not defined.</exception>
        internal ForwardSplit(ForwardFeatureRef lineRef, string splitBeforeId, string splitAfterId)
        {
            m_LineRef = lineRef;
            m_SplitBeforeId = splitBeforeId;
            m_SplitAfterId = splitAfterId;
        }

        internal void Resolve(CadastralMapModel model)
        {
            IFeatureRef fr = m_LineRef.ReferenceFrom;
            Debug.Assert(fr is Operation);

            Feature f = model.Find<Feature>(m_LineRef.InternalId);
            if (f == null)
                throw new ApplicationException("Cannot locate forward reference " + m_LineRef.InternalId);

            // Only IntersectDirectionAndLineOperation has forward splits, so follow that logic
            LineFeature line = (LineFeature)f;
            var dff = new DeserializationFactory(fr as Operation);
            dff.AddLineSplit(line, DataField.SplitBefore, m_SplitBeforeId);
            dff.AddLineSplit(line, DataField.SplitAfter, m_SplitAfterId);

            IntersectOperation xop = (IntersectOperation)fr;
            LineFeature lineBefore, lineAfter;
            dff.MakeSections(line, DataField.SplitBefore, xop.IntersectionPoint, DataField.SplitAfter,
                                out lineBefore, out lineAfter);
        }
    }
}
