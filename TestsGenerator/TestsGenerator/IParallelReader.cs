using System.Collections.Generic;

namespace TestsGenerator
{
    public interface IParallelReader
    {
        IDictionary<string, string> ReadText(IEnumerable<string> paths);

        IDictionary<string, string> SuccessfullyReadText
        { get; }

        int ThreadsCount
        { get; set; }
    }
}
