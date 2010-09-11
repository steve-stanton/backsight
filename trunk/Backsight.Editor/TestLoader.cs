using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backsight.Editor
{
    class TestLoader : ILoader
    {
        #region ILoader Members

        public T Find<T>(string s) where T : Feature
        {
            return null;
        }

        #endregion
    }
}
