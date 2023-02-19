using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TransformMethods;

class Program
{
    static void Main()
    {
        string code = File.ReadAllText("Sample.cs");
        TransformMethodToUppercaseAsync(code).Wait();
    }

    static async Task TransformMethodToUppercaseAsync(string code)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        SyntaxNode root = await tree.GetRootAsync();

        var methods = root.DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .Where(m => char.IsLower(m.Identifier.ValueText.First()))
            .Where(m => m.Modifiers.Select(t => t.Value).Contains("public")).ToList();
     
        root = root.ReplaceNodes(methods, (oldMethod, newMethod) =>
        {
            string newName = char.ToUpperInvariant(oldMethod.Identifier.ValueText[0]) + oldMethod.Identifier.ValueText.Substring(1);
            return newMethod.WithIdentifier(SyntaxFactory.Identifier(newName));
        });

        Console.WriteLine();
        Console.WriteLine(root.ToString());
    }
}
