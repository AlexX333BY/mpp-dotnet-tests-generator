using System;
using System.Collections.Generic;
using System.IO;

namespace TestsGenerator.IO
{
    public class FileWriter : IWriter
    {
        protected string directory;

        public void WriteText(KeyValuePair<string, string> filenameTextPair)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            File.WriteAllText(Directory + Path.DirectorySeparatorChar + filenameTextPair.Key, filenameTextPair.Value);
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
            Directory = string.Empty;
        }
    }
}
