
using Pascal;

namespace CSPASCAL;

public class Program
{
    const string src = @"
program Main;
   var x, y: real;

   procedure Alpha(a : integer);
      var y : integer;
   begin
      x := b + x + y; { ERROR here! }
   end;

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