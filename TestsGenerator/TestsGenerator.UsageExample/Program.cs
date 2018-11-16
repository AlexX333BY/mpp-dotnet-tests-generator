using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestsGenerator.IO;

namespace TestsGenerator.UsageExample
{
    class Program
    {
        static void Main(string[] args)
        {
            TestsGeneratorConfig config = new TestsGeneratorConfig
            {
                ReadPaths = new List<string>
                {
                    "../../SimpleTestFile.cs",
                    "../../ExtendedTestFile.cs"
                },
                Writer = new FileWriter()
                {
                    Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                },
                ReadThreadCount = 2,
                WriteThreadCount = 2
            };

            Task generationTask = new TestsGenerator(config).GenerateAsync();
            generationTask.Start();
            generationTask.Wait();
            Console.WriteLine("Generation completed");
            Console.ReadKey();
        }
    }
}
