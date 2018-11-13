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

        protected TestClassInfo CreateClassInfo(ClassDeclarationSyntax classDeclaration)
        {
            TestClassInfo typeInfo = new TestClassInfo(classDeclaration.Identifier.ValueText, ((NamespaceDeclarationSyntax)classDeclaration.Parent).Name.ToString());

            foreach (MethodDeclarationSyntax methodDeclaration in classDeclaration.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(methodDeclaration => methodDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword))))
            {
                typeInfo.Methods.Add(CreateMethodInfo(methodDeclaration));
            }

            return typeInfo;
        }

        protected TestMethodInfo CreateMethodInfo(MethodDeclarationSyntax methodDeclaration)
        {
            return new TestMethodInfo(methodDeclaration.Identifier.ValueText, new DataStructures.TypeInfo(methodDeclaration.ReturnType.ToString()));
        }

        protected ConstructorInfo GetMaxedConstructor(ClassDeclarationSyntax classDeclaration)
        {
            // TODO

            return null;
        }
    }
}
