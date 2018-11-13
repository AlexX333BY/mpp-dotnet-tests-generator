using System;
using System.IO;
using TestsGenerator.DataStructures;

namespace TestsGenerator.IO
{
    public class FileWriter : IWriter
    {
        protected string directory;

        public void WriteText(PathContentPair pathContentPair)
        {
            if (pathContentPair == null)
            {
                throw new ArgumentException("PathContent pair shouldn't be null");
            }
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            File.WriteAllText(Directory + Path.DirectorySeparatorChar + pathContentPair.Path, pathContentPair.Content);
        }

        public string Directory
        {
            get => directory;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("Directory shouldn't be null");
                }
                directory = Path.GetFullPath(value);
            }
        }

        public FileWriter()
        {
            directory = string.Empty;
        }
    }
}
