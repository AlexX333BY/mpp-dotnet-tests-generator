using System.Collections.Generic;

namespace TestsGenerator
{
    public interface IParallelWriter
    {
        void WriteText(IDictionary<string, string> pathContentPairs);

        int ThreadsCount
        { get; set; }
    }
}
