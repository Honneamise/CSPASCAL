
using Pascal;

namespace CSPASCAL;

public class Program
{
    const string src = @"
    PROGRAM Main;
    VAR x, y: REAL;

        PROCEDURE Alpha(a : INTEGER);
        VAR y : INTEGER;
        BEGIN
          x := a + x + y;
        END;

    BEGIN { Main }
    x := 2;
    y := 3;
    END.  { Main }";

    private static void Main()
    {
        Init.Initialize();
        

        Console.WriteLine("********************");
        Console.Write(src + "\n");

        //Lexer lexer = new(src);

        //Parser parser = new(lexer);

        //Ast ast = parser.GenerateAst();

        //ScopedSymbolTable symtab = SemanticAnalyzer.Analyze(ast);

        //Console.WriteLine(symtab.ToString());

        //Interpreter intp = new(symtab);

        //intp.Execute(ast);

        //Console.WriteLine(intp.ToString());

        SymbolProcedure s = new("name", []);

        Console.WriteLine(s);

        s.Parameters.Add(new("test", new Symbol("INTEGER")));

        Console.WriteLine(s);

    }
}