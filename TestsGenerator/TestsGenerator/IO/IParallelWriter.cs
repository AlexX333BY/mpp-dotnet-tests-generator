using System.Collections.Generic;

namespace TestsGenerator.IO
{
    public interface IParallelWriter
    {
        void WriteText(IDictionary<string, string> pathContentPairs);

        int ThreadsCount
        { get; set; }
    }
}
