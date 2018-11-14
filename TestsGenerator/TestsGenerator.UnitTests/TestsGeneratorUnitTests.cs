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
            Assert.IsTrue(class1Root.Usings.Any((usingEntry) => usingEntry.Name.ToString() == "Microsoft.VisualStudio.TestTools.UnitTesting"));
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
            Assert.IsTrue(methods.Any((method) => method.Identifier.ToString() == "TestInitialize"));
            Assert.IsTrue(methods.Any((method) => method.Identifier.ToString() == "GetInterfaceTest"));
            Assert.IsTrue(methods.Any((method) => method.Identifier.ToString() == "SetInterfaceTest"));
            Assert.IsFalse(methods.Any((method) => method.Identifier.ToString() == "GetProtectedInterfaceTest"));
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
            Assert.IsTrue(class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "TestInitialize").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Any((statement) => statement.ToFullString().Contains("new Mock")));
        }

        [TestMethod]
        public void ClassInitTest()
        {
            Assert.IsTrue(class2Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "TestInitialize").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Any((statement) => statement.ToFullString().Contains("new TestClass2")));
        }

        [TestMethod]
        public void ActualTest()
        {
            Assert.IsTrue(class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Any((statement) => statement.Declaration.Variables.Any((variable) => variable.Identifier.ToString() == "actual")));
        }

        [TestMethod]
        public void ExpectedTest()
        {
            Assert.IsTrue(class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<LocalDeclarationStatementSyntax>()
                .Any((statement) => statement.Declaration.Variables.Any((variable) => variable.Identifier.ToString() == "expected")));
        }

        [TestMethod]
        public void AreEqualTest()
        {
            Assert.IsTrue(class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Any((statement) => statement.ToString().Contains("Assert.AreEqual(expected, actual)")));
        }

        [TestMethod]
        public void FailTest()
        {
            Assert.IsTrue(class1Root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .FirstOrDefault((method) => method.Identifier.ToString() == "GetIntTest").Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Any((statement) => statement.ToString().Contains("Assert.Fail")));
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
