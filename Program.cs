
using Pascal;

namespace CSPASCAL;

public class Program
{
    const string src = @"
    program Main;
   var x, y : real;

   procedure AlphaA(a : integer);
      var y : integer;
   begin { AlphaA }

   end;  { AlphaA }

   procedure AlphaB(a : integer);
      var b : integer;
   begin { AlphaB }

   end;  { AlphaB }

begin { Main }

end.  { Main }";

    private static void Main()
    {
        Init.Initialize();
        

        Console.WriteLine("********************");
        Console.Write(src + "\n");

        Lexer lexer = new(src);

        Parser parser = new(lexer);

        Ast ast = parser.GenerateAst();

        ScopedSymbolTable? _ = SemanticAnalyzer.Analyze(ast);

        //Interpreter intp = new(symtab);

        //intp.Execute(ast);

        //Console.WriteLine(intp.ToString());


    }
}