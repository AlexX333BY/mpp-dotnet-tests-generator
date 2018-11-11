using System.Collections.Generic;

namespace TestsGenerator
{
    public interface IParallelReader
    {
        IEnumerable<string> ReadText(IEnumerable<string> paths);

        IEnumerable<string> SuccessfullyReadText
        { get; }

        int ThreadsCount
        { get; set; }
    }
}
