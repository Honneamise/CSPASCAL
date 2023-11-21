using System.Xml.Linq;

namespace Pascal;


public class Parser(Lexer lexer)
{
    private readonly Lexer lexer = lexer;
    private Token currentToken = lexer.NextToken();

    public void Consume(TokenType type)
    {
        //currentToken = (currentToken.Type == type) ? lexer.NextToken() : 

        if (currentToken.Type == type)
        { 
            currentToken = lexer.NextToken();
        }
        else
        {
            throw new Exception($"[{lexer.Line}]Invalid token found expected {type} found {currentToken}");
        }
    
    }

    public Ast Factor()
    {
        Token token = currentToken;

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

            case TokenType.ID:
                return Variable();

            default: 
                throw new Exception($"[{lexer.Line}]Invalid token type found");
        }
    }

    public Ast Term()
    {
        Ast node = Factor();

        while (currentToken.Type is TokenType.MUL or TokenType.INTEGER_DIV or TokenType.FLOAT_DIV)
        {
            Token token = currentToken;

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
                    throw new Exception($"[{lexer.Line}]Invalid token type found");
            }

            node = new AstBinOp(node, token.Type, Factor());
        }

        return node;
    }

    public Ast Expr()
    {
        Ast node = Term();

        while (currentToken.Type is TokenType.PLUS or TokenType.MINUS)
        {
            Token token = currentToken;

            switch (token.Type)
            {
                case TokenType.PLUS:
                    Consume(TokenType.PLUS);
                    break;

                case TokenType.MINUS:
                    Consume(TokenType.MINUS);
                    break;

                default:
                    throw new Exception($"[{lexer.Line}]Invalid token type found");
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
        return currentToken.Type switch
        {
            TokenType.BEGIN => CompoundStatement(),
            TokenType.ID => AssignStatement(),
            _ => new AstEmpty()
        };
    }

    public List<Ast> StatementList()
    {
        List<Ast> list = [ Statement() ];

        while (currentToken.Type is TokenType.SEMI)
        {
            Consume(TokenType.SEMI);

            list.Add(Statement());
        }

        if (currentToken.Type is TokenType.ID)
        {
            throw new Exception($"[{lexer.Line}]Unexpected token ID found");
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
        List<AstVar> varNodes = [new AstVar(currentToken.Value)];

        Consume(TokenType.ID);

        while (currentToken.Type == TokenType.COMMA)
        {
            Consume(TokenType.COMMA);

            varNodes.Add(new AstVar(currentToken.Value));

            Consume(TokenType.ID);
        }

        Consume(TokenType.COLON);

        AstType typeNode = TypeSpec();

        List<AstVarDecl> varDeclarations = [];

        foreach (AstVar node in varNodes)
        {
            varDeclarations.Add(new AstVarDecl(node, typeNode));
        }

        return varDeclarations;
    }

    AstType TypeSpec()
    {
        Token token = currentToken;

        switch (currentToken.Type)
        {
            case TokenType.INTEGER:
                Consume(TokenType.INTEGER);
                break;

            case TokenType.REAL:
                Consume(TokenType.REAL);
                break;

            default:
                throw new Exception($"[{lexer.Line}]Invalid data type");
        }

        return new AstType(token.Value);
    }

    public List<AstParam> FormalParameters()
    {
        List<AstParam> list = [];

        List<Token> tokens = [currentToken];

        Consume(TokenType.ID);

        while(currentToken.Type == TokenType.COMMA) 
        {
            Consume(TokenType.COMMA);

            tokens.Add(currentToken);

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
        List<AstParam> list = [];

        if(currentToken.Type == TokenType.ID)
        {
            List<AstParam> parameters = FormalParameters();

            while(currentToken.Type == TokenType.SEMI)
            {
                Consume(TokenType.SEMI);
                
                list = new(list.Concat(FormalParameters()));
            }
        }

        return list;
    }

    public List<Ast> Declarations()
    {
        List<Ast> declarations = [];

        while (true)
        {
            if (currentToken.Type == TokenType.VAR)//variables
            {
                Consume(TokenType.VAR);

                while (currentToken.Type == TokenType.ID)
                {
                    List<AstVarDecl> list = VariableDeclaration();

                    declarations = new(declarations.Concat(list));

                    Consume(TokenType.SEMI);
                }
            }
            else if (currentToken.Type == TokenType.PROCEDURE)//procedures
            {
                Consume(TokenType.PROCEDURE);

                string name = currentToken.Value;

                Consume(TokenType.ID);

                List<AstParam> parameters = [];

                if(currentToken.Type == TokenType.LPAREN) 
                {
                    Consume(TokenType.LPAREN);

                    parameters = FormalParameterList();

                    Consume(TokenType.RPAREN);
                }

                Consume(TokenType.SEMI);

                AstBlock block = Block();

                AstProcedureDecl node = new(name, parameters, block);

                declarations.Add(node);

                Consume(TokenType.SEMI);
            }
            else
            {
                break;
            }
        }
        return declarations;
    }

    public AstBlock Block()
    {
        List<Ast> declarations = Declarations();

        AstCompound compound = CompoundStatement();

        return new AstBlock(declarations, compound);
    }

    public AstVar Variable()
    {
        AstVar node = new(currentToken.Value);

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

        if (currentToken.Type is not TokenType.EOF)
        {
            throw new Exception($"[{lexer.Line}]Parsing error");
        }

        return programNode;
    }

    public Ast GenerateAst()
    {
        lexer.Reset();
        
        currentToken = lexer.NextToken();
        
        return Program();
    }
}