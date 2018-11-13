using System.Collections.Generic;

namespace TestsGenerator.DataStructures
{
    public class ConstructorInfo
    {
        public List<TypeInfo> Arguments
        { get; protected set; }

        public ConstructorInfo()
        {
            Arguments = new List<TypeInfo>();
        }
    }
}
