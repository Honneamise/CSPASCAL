
using Pascal;

namespace CSPASCAL;

public class Program
{
    private static void Main()
    {
        Init.Initialize(true,true);

        string src = File.ReadAllText("input.txt");

        Console.WriteLine("\n*****SOURCE CODE*****\n");
        Console.WriteLine(src);
        
        Lexer lexer = new(src);

        Parser parser = new(lexer);

        Ast ast = parser.GenerateAst();

        Console.WriteLine("\n*****SEMANTIC ANALYZER*****\n");

        SemanticAnalyzer analyzer = new(ast);

        analyzer.Analyze();

        Console.WriteLine("\n*****INTERPRETER*****\n");

        Interpreter intp = new(ast);

        intp.Execute();
    }
}