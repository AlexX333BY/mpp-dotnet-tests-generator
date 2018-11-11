using System.Collections.Generic;

namespace TestsGenerator.DataStructures
{
    public class TestFileInfo
    {
        public List<string> Usings
        { get; protected set; }

        public List<TestClassInfo> Classes
        { get; protected set; }
    }
}
