namespace Pascal;


public static class SemanticAnalyzer
{
    private static ScopedSymbolTable? currentScope = null;

    private static void Visit(AstVar node)
    {
        if (currentScope == null) { throw new Exception("Invalid scoper"); }
        
        string varName = node.Name;
        
        _ = currentScope.Lookup(varName) ?? throw new Exception($"{varName}:undeclared variable");
    }

    private static void Visit(AstBinOp node)
    {
        Visit(node.Left);
        Visit(node.Right);
    }

    private static void Visit(AstAssign node)
    {
        Visit(node.Left);
        Visit(node.Right);
    }
    
    private static void Visit(AstCompound node)
    {
        foreach (Ast n in node.Nodes)
        {
            Visit(n);
        }
    }

    private static void Visit(AstProcedureDecl node)
    {
        if (currentScope == null) { throw new Exception("Invalid scoper"); }

        string procName = node.Name;

        SymbolProcedure procSymbol = new(procName, []);

        currentScope.Define(procSymbol);

        ScopedSymbolTable procScope = new(procName, currentScope.Level + 1, currentScope);

        currentScope = procScope;

        foreach(AstParam param in node.Params)
        {
            Symbol? paramType = currentScope.Lookup(param.Type.Name) ?? throw new Exception($"{param.Type.Name}:invalid data type");
            
            string paramName = param.Var.Name;

            SymbolVar varSymbol = new(paramName, paramType);    

            currentScope.Define(varSymbol);

            procSymbol.Parameters.Add(varSymbol);
        }

        Visit(node.Block);

        Console.WriteLine(procScope.ToString());

        currentScope = currentScope.Enclosing;
    }

    private static void Visit(AstVarDecl node)
    {
        if (currentScope == null) { throw new Exception("Invalid scoper"); }

        string typeName = node.TypeNode.Name;

        Symbol typeSymbol = currentScope.Lookup(typeName) ?? throw new Exception($"{typeName}:invalid datatype");

        string varName = node.VarNode.Name;

        Symbol varSymbol = new SymbolVar(varName, typeSymbol);

        if (currentScope.Lookup(varName) != null) { throw new Exception($"{varName}:duplicated variable name"); }

        currentScope.Define(varSymbol);
    }

    private static void Visit(AstBlock node)
    {
        foreach (Ast ast in node.Declarations)
        {
            Visit(ast);
        }

        Visit(node.Compound);
    }

    private static void Visit(AstProgram node)
    {
        ScopedSymbolTable globalScope = new("GLOBAL", 1, currentScope);

        currentScope = globalScope;

        Visit(node.Block);

        Console.WriteLine(globalScope.ToString());

        currentScope = currentScope.Enclosing;
    }

    private static void Visit(Ast node)
    {
        switch (node)
        {
            case AstEmpty: break;
            case AstNum: break;
            case AstVar: Visit((AstVar)node); break;
            case AstUnaryOp: break;
            case AstBinOp: Visit((AstBinOp)node); break;
            case AstAssign: Visit((AstAssign)node); break;
            case AstCompound: Visit((AstCompound)node); break;
            case AstType: break;
            case AstProcedureDecl: Visit((AstProcedureDecl)node); break;
            case AstVarDecl: Visit((AstVarDecl)node); break;
            case AstBlock: Visit((AstBlock)node); break;
            case AstProgram: Visit((AstProgram)node); break;

            default:
                throw new Exception("Invalid node type found");
        }
    }

    public static ScopedSymbolTable? Analyze(Ast ast)
    {
        Visit(ast);

        return currentScope;
    }
}
