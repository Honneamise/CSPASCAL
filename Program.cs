
using Pascal;

namespace CSPASCAL;

public class Program
{
    const string src = @"

PROGRAM Main;

VAR x, y: REAL;


    PROCEDURE Alpha(a : INTEGER);
        VAR y : INTEGER;
        VAR b : REAL;

    BEGIN
        x := b + x + y;
    END;

BEGIN { Main }

END.  { Main }";

    private static void Main()
    {
        Init.Initialize();
        
        Console.WriteLine("********************");
        Console.Write(src + "\n");

        Lexer lexer = new(src);

        Parser parser = new(lexer);

        Ast ast = parser.GenerateAst();

        ScopedSymbolTable? _ = SemanticAnalyzer.Analyze(ast);

    }
}