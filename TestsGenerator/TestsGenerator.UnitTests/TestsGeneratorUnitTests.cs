using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            Task generationTask = new TestsGenerator(config).GetGenerateTask();
            generationTask.Start();
            generationTask.Wait();

            class1Root = CSharpSyntaxTree.ParseText(File.ReadAllText(testClass1FilePath)).GetCompilationUnitRoot();
            class2Root = CSharpSyntaxTree.ParseText(File.ReadAllText(testClass2FilePath)).GetCompilationUnitRoot();
        }

        [TestMethod]
        public void ExceptionThrowingTest()
        {
            TestsGeneratorConfig config = new TestsGeneratorConfig
            {
                ReadPaths = new List<string>
                {
                    "NonExistingFile.cs"
                }
            };

            Task generationTask = new TestsGenerator(config).GetGenerateTask();
            generationTask.Start();

            Assert.ThrowsException<AggregateException>(() => generationTask.Wait());
        }

        [TestMethod]
        public void UsingTests()
        {
            Assert.AreEqual(1, class1Root.Usings.Where((usingEntry) => usingEntry.Name.ToString() == "Microsoft.VisualStudio.TestTools.UnitTesting").Count());
            Assert.AreEqual(1, class1Root.Usings.Where((usingEntry) => usingEntry.Name.ToString() == "Moq").Count());
            Assert.AreEqual(1, class1Root.Usings.Where((usingEntry) => usingEntry.Name.ToString() == "System").Count());
            Assert.AreEqual(1, class1Root.Usings.Where((usingEntry) => usingEntry.Name.ToString() == "TestClassNamespace.Class1").Count());
            Assert.AreEqual(1, class2Root.Usings.Where((usingEntry) => usingEntry.Name.ToString() == "TestClassNamespace.Class2").Count());
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

        [TestMethod]
        public void ClassAttributeTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where((classDeclaration) => classDeclaration.AttributeLists.Any((attributeList) => attributeList.Attributes
                .Any((attribute) => attribute.Name.ToString() == "TestClass"))).Count());
        }

        [TestMethod]
        public void MethodsTest()
        {
            IEnumerable<MethodDeclarationSyntax> methods = class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            Assert.AreEqual(3, methods.Count());
            Assert.AreEqual(1, methods.Where((method) => method.Identifier.ToString() == "TestInitialize").Count());
            Assert.AreEqual(1, methods.Where((method) => method.Identifier.ToString() == "GetInterfaceTest").Count());
            Assert.AreEqual(1, methods.Where((method) => method.Identifier.ToString() == "SetInterfaceTest").Count());
            Assert.AreEqual(0, methods.Where((method) => method.Identifier.ToString() == "GetProtectedInterfaceTest").Count());
        }

        [TestMethod]
        public void MethodAttributeTest()
        {
            IEnumerable<MethodDeclarationSyntax> methods = class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>();

            Assert.AreEqual(2, methods.Where((methodDeclaration) => methodDeclaration.AttributeLists
                .Any((attributeList) => attributeList.Attributes.Any((attribute) => attribute.Name.ToString() == "TestMethod")))
                .Count());
            Assert.AreEqual(1, methods.Where((methodDeclaration) => methodDeclaration.AttributeLists
                .Any((attributeList) => attributeList.Attributes.Any((attribute) => attribute.Name.ToString() == "TestInitialize")))
                .Count());
        }

        [TestMethod]
        public void MockTest()
        {
            Assert.AreEqual(1, class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "TestInitialize").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Where((statement) => statement.ToFullString().Contains("new Mock")).Count());
        }

        [TestMethod]
        public void ClassInitTest()
        {
            Assert.AreEqual(1, class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "TestInitialize").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Where((statement) => statement.ToFullString().Contains("new TestClass2")).Count());
        }

        [TestMethod]
        public void ActualTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Where((statement) => statement.Declaration.Variables.Any((variable) => variable.Identifier.ToString() == "actual")).Count());
        }

        [TestMethod]
        public void ExpectedTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Where((statement) => statement.Declaration.Variables.Any((variable) => variable.Identifier.ToString() == "expected")).Count());
        }

        [TestMethod]
        public void AreEqualTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Where((statement) => statement.ToString().Contains("Assert.AreEqual(expected, actual)")).Count());
        }

        [TestMethod]
        public void FailTest()
        {
            Assert.AreEqual(1, class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Where((statement) => statement.ToString().Contains("Assert.Fail")).Count());
        }

        [TestMethod]
        public void ArgumentsInitializationTest()
        {
            IEnumerable<LocalDeclarationStatementSyntax> declarations = class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "DoSomethingTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>();

            Assert.AreEqual(2, declarations.Count());
            Assert.AreEqual(1, declarations.Where((declaration) => declaration.Declaration.Variables
                .Any((variable) => variable.Identifier.ToString() == "param1")).Count());
            Assert.AreEqual(1, declarations.Where((declaration) => declaration.Declaration.Variables
                .Any((variable) => variable.Identifier.ToString() == "param2")).Count());
        }
    }
}
