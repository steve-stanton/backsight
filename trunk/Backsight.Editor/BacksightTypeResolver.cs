using System;
using System.Web.Script.Serialization;

namespace Backsight.Editor
{
    class BacksightTypeResolver : JavaScriptTypeResolver
    {
        public override Type ResolveType(string id)
        {
            throw new NotImplementedException();
        }

        public override string ResolveTypeId(Type type)
        {
            return type.Name;
        }
    }
}
