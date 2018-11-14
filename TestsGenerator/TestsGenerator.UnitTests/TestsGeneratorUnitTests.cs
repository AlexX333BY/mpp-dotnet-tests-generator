using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsGenerator.IO;

namespace TestsGenerator.UnitTests
{
    [TestClass]
    public class TestsGeneratorUnitTests
    {
        private CompilationUnitSyntax class1Root, class2Root;

        [TestInitialize]
        public void TestInit()
        {
            string testFilesDirectory = "../../";
            string testFilePath = testFilesDirectory + "TestFile.cs";
            string testClass1FilePath = testFilesDirectory + "TestClass1Test.cs";
            string testClass2FilePath = testFilesDirectory + "TestClass2Test.cs";

            TestsGeneratorConfig config = new TestsGeneratorConfig
            {
                ReadPaths = new List<string>
                {
                    testFilePath
                },
                Writer = new FileWriter()
                {
                    Directory = testFilesDirectory
                }
            };

            new TestsGenerator(config).Generate();

            class1Root = CSharpSyntaxTree.ParseText(File.ReadAllText(testClass1FilePath)).GetCompilationUnitRoot();
            class2Root = CSharpSyntaxTree.ParseText(File.ReadAllText(testClass2FilePath)).GetCompilationUnitRoot();
        }

        [TestMethod]
        public void UsingTests()
        {
            Assert.IsTrue(class1Root.Usings.Any((usingEntry) => usingEntry.Name.ToString() == "Moq"));
            Assert.IsTrue(class1Root.Usings.Any((usingEntry) => usingEntry.Name.ToString() == "System"));
            Assert.IsTrue(class1Root.Usings.Any((usingEntry) => usingEntry.Name.ToString() == "TestClassNamespace.Class1"));
            Assert.IsTrue(class2Root.Usings.Any((usingEntry) => usingEntry.Name.ToString() == "TestClassNamespace.Class2"));
        }

        [TestMethod]
        public void NamespaceTest()
        {
            IEnumerable<NamespaceDeclarationSyntax> namespaces;

            namespaces = class1Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual("TestClassNamespace.Class1.Test", namespaces.First().Name.ToString());

            namespaces = class2Root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
            Assert.AreEqual(1, namespaces.Count());
            Assert.AreEqual("TestClassNamespace.Class2.Test", namespaces.First().Name.ToString());
        }

        [TestMethod]
        public void ClassTest()
        {
            IEnumerable<ClassDeclarationSyntax> classes;

            classes = class1Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            Assert.AreEqual(1, classes.Count());
            Assert.AreEqual("TestClass1Test", classes.First().Identifier.ToString());

            classes = class2Root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            Assert.AreEqual(1, classes.Count());
            Assert.AreEqual("TestClass2Test", classes.First().Identifier.ToString());
        }
    }
}
