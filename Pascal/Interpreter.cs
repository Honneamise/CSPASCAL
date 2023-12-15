using System.Collections.Generic;
using System.Xml.Linq;

namespace Pascal;

public class Interpreter
{
    public Ast Ast { get; }
    CallStack Stack { get; }

    public Interpreter(Ast ast)
    {
        Ast = ast;
        Stack = new();
    }

    public void Log(string msg)
    {
        if (Init.LogScope)
        {
            Console.WriteLine(msg);
        }
    }

    private float Visit(AstProgram node)
    {
        string name = node.Name;

        Log("--Entering Program: " + name);

        ActivationRecord ar = new(name, ArType.PROGRAM, 1);

        Stack.Push(ar);

        Log(Stack.ToString());

        Visit(node.Block);

        Log("---Leaving Program: " + name);
        Log(Stack.ToString());

        _ = Stack.Pop();

        return 0.0f;
    }

    private float Visit(AstBlock node)
    {
        foreach (Ast n in node.Declarations)
        {
            Visit(n);
        }

        Visit(node.Compound);

        return 0.0f;
    }

    private float Visit(AstBinOp node)
    {
        float lval = Visit(node.Left);
        float rval = Visit(node.Right);

        switch (node.Op.Type)
        {
            case TokenType.PLUS: return (lval + rval);

            case TokenType.MINUS: return (lval - rval);

            case TokenType.MUL: return (lval* rval);

            case TokenType.INTEGER_DIV: return ((int)lval / (int)rval);
            
            case TokenType.FLOAT_DIV: return (lval / rval);

            default: return 0.0f;
        }
    }

    private float Visit(AstUnaryOp node)
    {
        switch(node.Op.Type)
        {
            case TokenType.PLUS: return +Visit(node.Expr);

            case TokenType.MINUS: return -Visit(node.Expr);

            default: return 0.0f;
        };
    }

    private float Visit(AstCompound node)
    {
        foreach (Ast n in node.Nodes)
        {
            _ = Visit(n);
        }

        return 0.0f;
    }

    private float Visit(AstAssign node)
    {
        string name = node.Left.Name;
        float value = Visit(node.Right);

        ActivationRecord ar = Stack.Peek();

        ar.Set(name, value);

        return 0.0f;
    }

    private float Visit(AstVar node)
    {
        string name = node.Name;

        ActivationRecord ar = Stack.Peek();

        return ar.Get(name);
    }

    private float Visit(AstProcedureCall node)
    {
        if (node.Symbol != null)
        {
            ActivationRecord ar = new(node.Name, ArType.PROCEDURE, node.Symbol.Level + 1);

            IEnumerable<(SymbolVar paramSymbol, Ast argumentNode)> list = node.Symbol.Parameters.Zip(node.ActualParameters);

            foreach (var (paramSymbol, argumentNode) in list)
            {
                ar.Set(paramSymbol.Name, Visit(argumentNode));
            }

            Stack.Push(ar);

            Log("--Entering Procedure: " + node.Name);
            Log(Stack.ToString());

            if (node.Symbol.Block != null)
            {
                Visit(node.Symbol.Block);
            }

            Log("--Leaving Procedure: " + node.Name);
            Log(Stack.ToString());

            Stack.Pop();
        }

        return 0.0f;
    }

    private float Visit(Ast node)
    {
        switch(node)
        {
            case AstProgram: return Visit((AstProgram)node);
            case AstBlock: return Visit((AstBlock)node);
            case AstVarDecl: return 0.0f;
            case AstType: return 0.0f;
            case AstBinOp: return Visit((AstBinOp)node);
            case AstNum: return ((AstNum)node).FloatValue;
            case AstUnaryOp: return Visit((AstUnaryOp)node);
            case AstCompound: return Visit((AstCompound)node);
            case AstAssign: return Visit((AstAssign)node);
            case AstVar: return Visit((AstVar)node);
            case AstEmpty: return 0.0f;
            case AstProcedureDecl: return 0.0f;
            case AstProcedureCall: return Visit((AstProcedureCall)node);

            default: return 0.0f;
        };
    }

    public void Execute()
    {
        Visit(Ast);
    }

}
