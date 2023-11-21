namespace Pascal;


public static class SemanticAnalyzer
{
    private static readonly ScopedSymbolTable symtab = new("global",1);

    private static void Visit(AstVar node)
    {
        string varName = node.Name;
        _ = symtab.Lookup(varName) ?? throw new Exception($"{varName}:undeclared variable");
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

    private static void Visit(AstVarDecl node)
    {
        string typeName = node.TypeNode.Type;
        Symbol typeSymbol = symtab.Lookup(typeName) ?? throw new Exception($"{typeName}:invalid datatype");

        string varName = node.VarNode.Name;
        Symbol varSymbol = new SymbolVar(varName, typeSymbol);

        if (symtab.Lookup(varName) != null) { throw new Exception($"{varName}:duplicated variable name"); }

        symtab.Define(varSymbol);
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
        Visit(node.Block);
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
            case AstProcedureDecl: break;
            case AstType: break;
            case AstVarDecl: Visit((AstVarDecl)node); break;
            case AstBlock: Visit((AstBlock)node); break;
            case AstProgram: Visit((AstProgram)node); break;

            default:
                throw new Exception("Invalid node type found");
        }
    }

    public static ScopedSymbolTable Analyze(Ast ast)
    {
        Visit(ast);

        return symtab;
    }
}
