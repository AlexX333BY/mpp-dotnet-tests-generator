using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using TestsGenerator.CodeAnalysis;
using TestsGenerator.DataStructures;

namespace TestsGenerator.TemplateGenerators
{
    public class TemplateGenerator : ITemplateGenerator
    {
        protected readonly ICodeAnalyzer codeAnalyzer;

        public IEnumerable<PathContentPair> Generate(string source)
        {
            if (source == null)
            {
                throw new ArgumentException("Source shouldn't be null");
            }

            List<PathContentPair> resultList = new List<PathContentPair>();
            
            TestFileInfo fileInfo = codeAnalyzer.Analyze(source);
            List<UsingDirectiveSyntax> commonUsings = fileInfo.Usings.Select((usingStr) => UsingDirective(IdentifierName(usingStr))).ToList();

            foreach (TestClassInfo typeInfo in fileInfo.Classes)
            {
                resultList.Add(new PathContentPair(typeInfo.Name + "Test.cs", CompilationUnit()
                    .WithUsings(
                        List<UsingDirectiveSyntax>(
                            CreateClassUsings(typeInfo, commonUsings)))
                    .WithMembers(
                        SingletonList<MemberDeclarationSyntax>(
                            CreateTestClassWithNamespaceDeclaration(typeInfo)))
                    .NormalizeWhitespace().ToFullString()));
            }

            return resultList;
        }

        protected UsingDirectiveSyntax[] CreateClassUsings(TestClassInfo typeInfo, List<UsingDirectiveSyntax> fileUsings)
        {
            return new List<UsingDirectiveSyntax>(fileUsings)
            {
                UsingDirective(IdentifierName(typeInfo.Namespace))
            }.ToArray();
        }

        protected MemberDeclarationSyntax CreateTestClassWithNamespaceDeclaration(TestClassInfo typeInfo)
        {
            return NamespaceDeclaration(
                IdentifierName(typeInfo.Namespace + ".Test"))
            .WithMembers(
                SingletonList<MemberDeclarationSyntax>(
                    CreateClassDeclaration(typeInfo)));
        }

        protected ClassDeclarationSyntax CreateClassDeclaration(TestClassInfo typeInfo)
        {
            IEnumerable<MemberDeclarationSyntax> methods = typeInfo.Methods.Select((methodInfo) => CreateTestMethodDeclaration(methodInfo));
            MemberDeclarationSyntax[] members = new MemberDeclarationSyntax[methods.Count() + 2];

            return ClassDeclaration(typeInfo.Name + "Test")
            .WithAttributeLists(
                SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                IdentifierName("TestClass"))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithMembers(
                List<MemberDeclarationSyntax>(
                    new MemberDeclarationSyntax[]{
                        CreateClassInstanceFieldDeclaration(typeInfo),
                        CreateTestInitializeMethodDeclaration(typeInfo)
                    }.Concat(typeInfo.Methods.Select((methodInfo) => CreateTestMethodDeclaration(methodInfo)))));
        }

        protected MethodDeclarationSyntax CreateTestMethodDeclaration(TestMethodInfo methodInfo)
        {
            return MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier(methodInfo.Name + "Test"))
            .WithAttributeLists(
                SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                IdentifierName("TestMethod"))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithBody(
                Block(
                    SingletonList<StatementSyntax>(
                        ExpressionStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    IdentifierName("Assert"),
                                    IdentifierName("Fail")))
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList<ArgumentSyntax>(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal("autogenerated"))))))))));
        }

        protected string CreateVariableName(string parameterName, bool isTestInstance = false)
        {
            return char.ToLower(parameterName[0]) + (parameterName.Length == 1 ? string.Empty : parameterName.Substring(1)) + (isTestInstance ? "TestInstance" : string.Empty);
        }

        protected FieldDeclarationSyntax CreateClassInstanceFieldDeclaration(TestClassInfo classInfo)
        {
            return FieldDeclaration(
                VariableDeclaration(
                    IdentifierName(classInfo.Name))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                            Identifier(CreateVariableName(classInfo.Name, true))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PrivateKeyword)));
        }

        protected MethodDeclarationSyntax CreateTestInitializeMethodDeclaration(TestClassInfo classInfo)
        {
            return MethodDeclaration(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier("TestInitialize"))
            .WithAttributeLists(
                SingletonList<AttributeListSyntax>(
                    AttributeList(
                        SingletonSeparatedList<AttributeSyntax>(
                            Attribute(
                                IdentifierName("TestInitialize"))))))
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword)))
            .WithBody(
                Block(
                    classInfo.Constructor.Parameters.Select((parameter) => CreateParameterInitializeExpression(parameter))
                        .Concat(new StatementSyntax[] {CreateTestClassInitializeExpression(classInfo)})
                    ));
        }

        protected LocalDeclarationStatementSyntax CreateParameterInitializeExpression(ParameterInfo parameterInfo)
        {
            ExpressionSyntax initializer;
            if (parameterInfo.Type.IsInterface)
            {
                initializer = ObjectCreationExpression(
                    GenericName(
                        Identifier("Mock"))
                    .WithTypeArgumentList(
                        TypeArgumentList(
                            SingletonSeparatedList<TypeSyntax>(
                                IdentifierName(parameterInfo.Type.Typename)))))
                .WithArgumentList(
                    ArgumentList());
            }
            else
            {
                initializer = DefaultExpression(IdentifierName(parameterInfo.Type.Typename));
            }

            return LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName(parameterInfo.Type.Typename))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                            Identifier(CreateVariableName(parameterInfo.Name)))
                        .WithInitializer(
                            EqualsValueClause(
                                initializer)))));
        }

        protected ExpressionStatementSyntax CreateTestClassInitializeExpression(TestClassInfo classInfo)
        {
            return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(CreateVariableName(classInfo.Name, true)),
                    ObjectCreationExpression(
                        IdentifierName(classInfo.Name))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                CreateObjectCreationArguments(classInfo.Constructor))))));
        }

        protected List<SyntaxNodeOrToken> CreateObjectCreationArguments(ConstructorInfo constructorInfo)
        {
            SyntaxToken commaToken = Token(SyntaxKind.CommaToken);
            List<SyntaxNodeOrToken> arguments = new List<SyntaxNodeOrToken>();

            if (constructorInfo.Parameters.Count > 0)
            {
                arguments.Add(CreateObjectCreationArgument(constructorInfo.Parameters[0]));
            }

            for (int i = 1; i < constructorInfo.Parameters.Count; ++i)
            {
                arguments.Add(commaToken);
                arguments.Add(CreateObjectCreationArgument(constructorInfo.Parameters[i]));
            }

            return arguments;
        }

        protected SyntaxNodeOrToken CreateObjectCreationArgument(ParameterInfo parameterInfo)
        {
            if (parameterInfo.Type.IsInterface)
            {
                return Argument(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(CreateVariableName(parameterInfo.Name)),
                        IdentifierName("Object")));
            }
            else
            {
                return Argument(IdentifierName(CreateVariableName(parameterInfo.Name)));
            }
        }

        public TemplateGenerator(ICodeAnalyzer codeAnalyzer)
        {
            this.codeAnalyzer = codeAnalyzer ?? throw new ArgumentException("Code analyzer shouldn't be null");
        }
    }
}
