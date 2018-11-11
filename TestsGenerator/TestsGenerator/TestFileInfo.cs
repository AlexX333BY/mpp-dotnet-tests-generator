using System.Collections.Generic;

namespace TestsGenerator
{
    public class TestFileInfo
    {
        public List<string> Usings
        { get; protected set; }

        public List<TestClassInfo> Classes
        { get; protected set; }
    }
}
