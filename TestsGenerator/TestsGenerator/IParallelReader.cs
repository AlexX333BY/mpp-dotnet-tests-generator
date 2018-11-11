using System.Collections.Generic;

namespace TestsGenerator
{
    public interface IParallelReader
    {
        void ReadText();

        IDictionary<string, string> SuccessfullyReadText
        { get; }
    }
}
