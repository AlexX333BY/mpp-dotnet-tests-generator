using System;
using System.IO;

namespace TestsGenerator.IO
{
    public class FileWriter : IWriter
    {
        public void WriteText(string path, string text)
        {
            if ((path == null) || (text == null))
            {
                throw new ArgumentException("Arguments shouldn't be null");
            }

            string directoryName = Path.GetDirectoryName(Path.GetFullPath(path));
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            File.WriteAllText(path, text);
        }
    }
}
