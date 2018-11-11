using System.Collections.Generic;

namespace TestsGenerator.IO
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
