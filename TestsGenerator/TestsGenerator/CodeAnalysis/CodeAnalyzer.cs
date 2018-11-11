using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using TestsGenerator.DataStructures;

namespace TestsGenerator.CodeAnalysis
{
    public class CodeAnalyzer : ICodeAnalyzer
    {
        public TestFileInfo Analyze(string code)
        {
            TestFileInfo testFileInfo = new TestFileInfo();
            CompilationUnitSyntax root = CSharpSyntaxTree.ParseText(code).GetCompilationUnitRoot();

            foreach (UsingDirectiveSyntax usingEntry in root.Usings)
            {
                testFileInfo.Usings.Add(usingEntry.Name.ToString());
            }

            foreach (ClassDeclarationSyntax classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                testFileInfo.Classes.Add(CreateClassInfo(classDeclaration));
            }

            return testFileInfo;
        }

        protected TestTypeInfo CreateClassInfo(ClassDeclarationSyntax classDeclaration)
        {
            TestTypeInfo typeInfo = new TestTypeInfo(classDeclaration.Identifier.ValueText, ((NamespaceDeclarationSyntax)classDeclaration.Parent).Name.ToString());

            foreach (MethodDeclarationSyntax methodDeclaration in classDeclaration.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(methodDeclaration => methodDeclaration.Modifiers.Any(modifier => modifier.ValueText == "public")))
            {
                typeInfo.Methods.Add(CreateMethodInfo(methodDeclaration));
            }

            return typeInfo;
        }

        protected TestMethodInfo CreateMethodInfo(MethodDeclarationSyntax methodDeclaration)
        {
            return new TestMethodInfo(methodDeclaration.Identifier.ValueText);
        }
    }
}
