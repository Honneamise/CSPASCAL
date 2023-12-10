namespace Pascal;


public class SemanticAnalyzer
{
    Ast ast;
    ScopedSymbolTable? currentScope;

    public SemanticAnalyzer(Ast a)
    {
        ast = a;
        currentScope = null;
    }

    public void Error(ErrorCode code, Token token)
    {
        string s = $"[Semantic error : {code}] {token}";

        throw new SemanticError(code, token, s);
    }

    public void Log(string msg)
    {
        if (Init.LogScope)
        {
            Console.WriteLine(msg);
        }
    }

    private void Visit(AstVar node)
    {
        if (currentScope != null)
        {
            string varName = node.Name;

            Symbol? varSymbol = currentScope.Lookup(varName);
        
            if(varSymbol==null)
            {
                Error(ErrorCode.ID_NOT_FOUND, node.Token);
            }
        }
    }

    private void Visit(AstVarDecl node)
    {
        if (currentScope != null)
        {
            string typeName = node.TypeNode.Name;

            Symbol? typeSymbol = currentScope.Lookup(typeName);

            if (typeSymbol != null)
            {
                string varName = node.VarNode.Name;

                Symbol varSymbol = new SymbolVar(varName, typeSymbol);

                if (currentScope.Lookup(varName, true) != null)
                {
                    Error(ErrorCode.DUPLICATE_ID, node.VarNode.Token);
                }

                currentScope.Define(varSymbol);
            }
        }
    }

    private void Visit(AstBinOp node)
    {
        Visit(node.Left);
        Visit(node.Right);
    }

    private void Visit(AstAssign node)
    {
        Visit(node.Right);
        Visit(node.Left);
    }
    
    private void Visit(AstCompound node)
    {
        foreach (Ast n in node.Nodes)
        {
            Visit(n);
        }
    }

    private void Visit(AstProcedureCall node)
    {
        foreach(Ast n in node.Nodes)
        {
            Visit(n);
        }
    }

    private void Visit(AstProcedureDecl node)
    {
        if (currentScope != null)
        {
            Log("---Entering scope: " + node.Name);

            string procName = node.Name;

            SymbolProcedure procSymbol = new(procName, new());

            currentScope.Define(procSymbol);

            ScopedSymbolTable procScope = new(procName, currentScope.Level + 1, currentScope);

            currentScope = procScope;

            foreach (AstParam param in node.Parameters)
            {
                Symbol? paramType = currentScope.Lookup(param.Type.Name) ?? throw new Exception($"{param.Type.Name}:invalid data type");

                string paramName = param.Var.Name;

                SymbolVar varSymbol = new(paramName, paramType);

                currentScope.Define(varSymbol);

                procSymbol.Parameters.Add(varSymbol);
            }

            Visit(node.Block);

            Log(procScope.ToString());

            currentScope = currentScope.Enclosing;

            Log("---Leaving scope: " + node.Name);
        }
    }

    private void Visit(AstBlock node)
    {
        foreach (Ast ast in node.Declarations)
        {
            Visit(ast);
        }

        Visit(node.Compound);
    }

    private void Visit(AstProgram node)
    {
        Log("---Entering scope: GLOBAL");

        ScopedSymbolTable globalScope = new("GLOBAL", 1, currentScope);

        currentScope = globalScope;

        Visit(node.Block);

        Log(globalScope.ToString());

        currentScope = currentScope.Enclosing;

        Log("---Leaving scope: GLOBAL");
    }

    private void Visit(Ast node)
    {
        switch (node)
        {
            case AstEmpty: break;
            case AstNum: break;
            case AstVar: Visit((AstVar)node); break;
            case AstVarDecl: Visit((AstVarDecl)node); break;
            case AstUnaryOp: break;
            case AstBinOp: Visit((AstBinOp)node); break;
            case AstAssign: Visit((AstAssign)node); break;
            case AstCompound: Visit((AstCompound)node); break;
            case AstType: break;
            case AstProcedureCall: Visit((AstProcedureCall)node); break;
            case AstProcedureDecl: Visit((AstProcedureDecl)node); break;
            case AstBlock: Visit((AstBlock)node); break;
            case AstProgram: Visit((AstProgram)node); break;

            default:
                break;
        }
    }

    public void Analyze()
    {
        Visit(ast);
    }
}
