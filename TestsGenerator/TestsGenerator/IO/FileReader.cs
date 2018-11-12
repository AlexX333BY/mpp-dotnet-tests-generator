using System;
using System.IO;

namespace TestsGenerator.IO
{
    public class FileReader : IReader
    {
        public string ReadText(string path)
        {
            if (path == null)
            {
                throw new ArgumentException("Path shouldn't be null");
            }

            return File.ReadAllText(path);
        }
    }
}
