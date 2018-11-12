using System.Collections.Generic;

namespace TestsGenerator.IO
{
    public interface IWriter
    {
        void WriteText(KeyValuePair<string, string> pathTextPair);
    }
}
