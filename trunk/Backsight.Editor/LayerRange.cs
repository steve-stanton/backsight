/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;

using Backsight.Environment;
using System.Diagnostics;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="19-APR-2007" />
    /// <summary>
    /// A range of map layers.
    /// </summary>
    class LayerRange
    {
        #region Static

        /// <summary>
        /// All defined map layers, sorted by (Theme, ThemeSequence). This should be initialized
        /// by the controller during application startup.
        /// </summary>
        static ILayer[] s_Layers;

        /// <summary>
        /// Initializes the map layers known to this class. This should be called only once
        /// when the application is starting up (before any instances of <c>LayerRange</c>
        /// are created).
        /// </summary>
        /// <param name="ec"></param>
        internal static void Initialize(IEnvironmentContainer ec)
        {
            if (s_Layers!=null)
                throw new InvalidOperationException("Layer range data has already been initialized");

            // Make a copy of the array returned by the container, since I want to make
            // sure nothing else messes with the array order.

            ILayer[] layers = ec.Layers;
            s_Layers = new ILayer[layers.Length];
            Array.Copy(layers, s_Layers, s_Layers.Length);
            Array.Sort<ILayer>(s_Layers, delegate(ILayer a, ILayer b)
            {
                int aThemeId = a.Theme.Id;
                int bThemeId = b.Theme.Id;
                int order = aThemeId.CompareTo(bThemeId);
                if (order==0)
                    order = a.ThemeSequence.CompareTo(b.ThemeSequence);

                return order;
            });
        }

        /// <summary>
        /// Creates a new <c>LayerRange</c> that corresponds to the supplied map layer (and
        /// layers derived from it). If the supplied layer is a facade, it's backing data
        /// will also be defined.
        /// </summary>
        /// <param name="baseLayer">The base layer for the range</param>
        /// <returns>The corresponding <c>LayerRange</c> instance</returns>
        internal static LayerRange CreateRange(ILayer baseLayer)
        {
            int baseIndex = IndexOf(baseLayer);
            if (baseIndex<0)
                throw new Exception("Cannot determine layer range for: "+baseLayer.Name);

            // Attach data to the supplied layer if it's a facade
            if (baseLayer is LayerFacade)
            {
                LayerFacade baseFacade = (LayerFacade)baseLayer;
                baseFacade.Data = s_Layers[baseIndex];
            }

            // Return simple range if the layer doesn't belong to a theme
            if (baseLayer.ThemeSequence==0)
            {
                Debug.Assert(baseLayer.Theme==null);
                return new LayerRange(baseIndex, baseIndex);
            }

            // Determine the array index of the most derived layer in the theme
            ITheme t = baseLayer.Theme;
            Debug.Assert(t!=null);
            int themeId = t.Id;
            Debug.Assert(themeId>0);
            int tailIndex = baseIndex;

            for(int i=baseIndex+1; i<s_Layers.Length && s_Layers[i].Theme.Id==themeId; i++)
                tailIndex = i;

            return new LayerRange(baseIndex, tailIndex);
        }

        /// <summary>
        /// Obtains the array index of the supplied map layer
        /// </summary>
        /// <param name="layer">The map layer of interest</param>
        /// <returns>Index into the <c>s_Layers</c> array (or -1 if the array doesn't contain
        /// any elements that have a matching ID)</returns>
        private static int IndexOf(ILayer layer)
        {
            Debug.Assert(s_Layers!=null);

            int id = layer.Id;

            for (int i=0; i<s_Layers.Length; i++)
            {
                if (s_Layers[i].Id == id)
                    return i;
            }

            return -1;
        }

        #endregion

        #region Class data

        /// <summary>
        /// Array index of the first layer in the range (points into the <c>s_Layers</c> array).
        /// </summary>
        private readonly int m_Min;

        /// <summary>
        /// Array index of the last layer in the range (points into the <c>s_Layers</c> array).
        /// </summary>
        private readonly int m_Max;

        #endregion

        #region Constructors

        private LayerRange(int baseIndex, int tailIndex)
        {
            if (s_Layers==null)
                throw new InvalidOperationException("Layer range data hasn't been initialized");

            if (baseIndex<0 || baseIndex>(s_Layers.Length-1))
                throw new ArgumentOutOfRangeException("Invalid base index for layer range");

            if (tailIndex<0 || tailIndex>(s_Layers.Length-1))
                throw new ArgumentOutOfRangeException("Invalid tail index for layer range");

            m_Min = baseIndex;
            m_Max = tailIndex;
        }

        #endregion
    }
}
