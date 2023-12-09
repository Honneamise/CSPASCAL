
using Pascal;

namespace CSPASCAL;

public class Program
{
    private static void Main()
    {
        Init.Initialize(true);

        string src = File.ReadAllText("input.txt");

        Console.Write(src + "\n");

        Lexer lexer = new(src);

        Parser parser = new(lexer);

        Ast ast = parser.GenerateAst();

        Console.WriteLine("********************");

        SemanticAnalyzer analyzer = new(ast);

        analyzer.Analyze();

        Interpreter intp = new(ast);

        intp.Execute();
    }
}