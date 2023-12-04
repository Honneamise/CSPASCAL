
using Pascal;

namespace CSPASCAL;

public class Program
{
    private static void Main()
    {
        Init.Initialize();

        string src = File.ReadAllText("input.txt");

        Console.Write(src + "\n");

        Lexer lexer = new(src);

        Parser parser = new(lexer);

        Ast ast = parser.GenerateAst();

        Console.WriteLine("********************");

        ScopedSymbolTable? _ = SemanticAnalyzer.Analyze(ast);

    }
}