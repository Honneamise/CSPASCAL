
using Pascal;

namespace CSPASCAL;

public class Program
{
    private static void Main()
    {
        Init.Initialize(true,true);

        string src = File.ReadAllText("input.txt");
        
        Console.WriteLine(src);
        
        Console.WriteLine("********************");

        Lexer lexer = new(src);

        Parser parser = new(lexer);

        Ast ast = parser.GenerateAst();

        SemanticAnalyzer analyzer = new(ast);

        analyzer.Analyze();

        Console.WriteLine("********************");

        Interpreter intp = new(ast);

        intp.Execute();
    }
}