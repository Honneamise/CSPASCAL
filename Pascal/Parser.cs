

using System.Collections.Generic;

namespace Pascal;


public class Parser
{
    public Lexer Lexer { get; }
    public Token CurrentToken { get; set; }  

    public Parser(Lexer lexer)
    {
        Lexer = lexer;

        CurrentToken = lexer.NextToken();
    }

    public void Error(ErrorCode code)
    {
        string s = $"[Parser error : {code}] {CurrentToken}";

        throw new ParserError(code, CurrentToken, s);
    }

    public void Consume(TokenType type)
    {
        if (CurrentToken.Type == type)
        { 
            CurrentToken = Lexer.NextToken();
        }
        else
        {
            Error(ErrorCode.UNEXPECTED_TOKEN);
        }
    
    }

    public Ast Factor()
    {
        Token token = CurrentToken;

        switch (token.Type)
        {
            case TokenType.PLUS:
                Consume(TokenType.PLUS);
                return new AstUnaryOp(TokenType.PLUS, Factor());

            case TokenType.MINUS:
                Consume(TokenType.MINUS);
                return new AstUnaryOp(TokenType.MINUS, Factor());

            case TokenType.INTEGER_CONST:
                Consume(TokenType.INTEGER_CONST);
                return new AstNum(token.Value);

            case TokenType.REAL_CONST:
                Consume(TokenType.REAL_CONST);
                return new AstNum(token.Value);

            case TokenType.LPAREN:
                Consume(TokenType.LPAREN);
                Ast node = Expr();
                Consume(TokenType.RPAREN);
                return node;

            default:
                return Variable();
        }
    }

    public Ast Term()
    {
        Ast node = Factor();

        while (CurrentToken.Type is TokenType.MUL or TokenType.INTEGER_DIV or TokenType.FLOAT_DIV)
        {
            Token token = CurrentToken;

            switch (token.Type)
            {
                case TokenType.MUL:
                    Consume(TokenType.MUL);
                    break;

                case TokenType.INTEGER_DIV:
                    Consume(TokenType.INTEGER_DIV);
                    break;

                case TokenType.FLOAT_DIV:
                    Consume(TokenType.FLOAT_DIV);
                    break;

                default:
                    break;

            }

            node = new AstBinOp(node, token.Type, Factor());
        }

        return node;
    }

    public Ast Expr()
    {
        Ast node = Term();

        while (CurrentToken.Type is TokenType.PLUS or TokenType.MINUS)
        {
            Token token = CurrentToken;

            switch (token.Type)
            {
                case TokenType.PLUS:
                    Consume(TokenType.PLUS);
                    break;

                case TokenType.MINUS:
                    Consume(TokenType.MINUS);
                    break;

                default:
                    break;
            }

            node = new AstBinOp(node, token.Type, Term());
        }

        return node;
    }

    public AstAssign AssignStatement()
    {
        AstVar left = Variable();

        Consume(TokenType.ASSIGN);

        Ast right = Expr();

        return new AstAssign(left, right); ;
    }

    public Ast Statement()
    {
        return CurrentToken.Type switch
        {
            TokenType.BEGIN => CompoundStatement(),
            TokenType.ID => AssignStatement(),
            _ => new AstEmpty()
        };
    }

    public List<Ast> StatementList()
    {
        
        List<Ast> list = new(){ Statement() };

        while (CurrentToken.Type is TokenType.SEMI)
        {
            Consume(TokenType.SEMI);

            list.Add(Statement());
        }

        return list;
    }

    public AstCompound CompoundStatement()
    {
        Consume(TokenType.BEGIN);

        List<Ast> nodes = StatementList();

        Consume(TokenType.END);

        AstCompound root = new();

        foreach (Ast node in nodes)
        {
            root.Nodes.Add(node);
        }

        return root;
    }

    public List<AstVarDecl> VariableDeclaration()
    {
        List<AstVar> varNodes = new() { new AstVar(CurrentToken.Value) };
        
        Consume(TokenType.ID);

        while (CurrentToken.Type == TokenType.COMMA)
        {
            Consume(TokenType.COMMA);

            varNodes.Add(new AstVar(CurrentToken.Value));

            Consume(TokenType.ID);
        }

        Consume(TokenType.COLON);

        AstType typeNode = TypeSpec();

        List<AstVarDecl> varDeclarations = new();

        foreach (AstVar node in varNodes)
        {
            varDeclarations.Add(new AstVarDecl(node, typeNode));
        }

        return varDeclarations;
    }

    AstType TypeSpec()
    {
        Token token = CurrentToken;

        switch (CurrentToken.Type)
        {
            case TokenType.INTEGER:
                Consume(TokenType.INTEGER);
                break;

            case TokenType.REAL:
                Consume(TokenType.REAL);
                break;

            default:
                break;
        }

        return new AstType(token.Value);
    }

    public List<AstParam> FormalParameters()
    {
        List<AstParam> list = new();

        List<Token> tokens =  new() { CurrentToken };

        Consume(TokenType.ID);

        while(CurrentToken.Type == TokenType.COMMA) 
        {
            Consume(TokenType.COMMA);

            tokens.Add(CurrentToken);

            Consume(TokenType.ID);
        }

        Consume(TokenType.COLON);

        AstType type = TypeSpec();

        foreach(Token token in tokens) 
        {
            AstVar v = new(token.Value);

            AstParam p = new(v, type);

            list.Add(p);
        }

        return list;
    }

    public List<AstParam> FormalParameterList() 
    {
        List<AstParam> list = new();

        if(CurrentToken.Type == TokenType.ID)
        {
            List<AstParam> parameters = FormalParameters();

            while(CurrentToken.Type == TokenType.SEMI)
            {
                Consume(TokenType.SEMI);
                
                parameters = new(parameters.Concat(FormalParameters()));
            }

            list = parameters;
        }

        return list;
    }

    public List<Ast> Declarations()
    {
        List<Ast> declarations = new();

        if (CurrentToken.Type == TokenType.VAR)
        {
            Consume(TokenType.VAR);

            while (CurrentToken.Type == TokenType.ID)
            {
                List<AstVarDecl> list = VariableDeclaration();

                declarations = new(declarations.Concat(list));

                Consume(TokenType.SEMI);
            }
        }

        while (CurrentToken.Type == TokenType.PROCEDURE)
        {
            AstProcedureDecl decl = ProcedureDeclaration();

            declarations.Add(decl);
        }   

        return declarations;    
    }

    public AstProcedureDecl ProcedureDeclaration()
    {
        Consume(TokenType.PROCEDURE);

        string name = CurrentToken.Value;

        Consume(TokenType.ID);

        List<AstParam> parameters = new();

        if (CurrentToken.Type == TokenType.LPAREN)
        {
            Consume(TokenType.LPAREN);

            parameters = FormalParameterList();

            Consume(TokenType.RPAREN);
        }

        Consume(TokenType.SEMI);

        AstBlock block = Block();

        AstProcedureDecl decl = new(name, parameters, block);

        Consume(TokenType.SEMI);

        return decl;
    }

    public AstBlock Block()
    {
        List<Ast> declarations = Declarations();

        AstCompound compound = CompoundStatement();

        return new AstBlock(declarations, compound);
    }

    public AstVar Variable()
    {
        AstVar node = new(CurrentToken.Value);

        Consume(TokenType.ID);

        return node;
    }

    public AstProgram Program()
    {
        Consume(TokenType.PROGRAM);

        AstVar varNode = Variable();

        string progName = varNode.Name;

        Consume(TokenType.SEMI);

        AstBlock blockNode = Block();

        AstProgram programNode = new(progName, blockNode);

        Consume(TokenType.DOT);

        return programNode;
    }

    public Ast GenerateAst()
    {
        return Program();
    }
}