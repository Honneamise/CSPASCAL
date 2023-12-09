using System.Data.Common;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace Pascal;


public class Lexer
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        //reserved keywords
        ["PROGRAM"]     = TokenType.PROGRAM,
        ["INTEGER"]     = TokenType.INTEGER,
        ["REAL"]        = TokenType.REAL,
        ["DIV"]         = TokenType.INTEGER_DIV,
        ["VAR"]         = TokenType.VAR,
        ["PROCEDURE"]   = TokenType.PROCEDURE,
        ["BEGIN"]       = TokenType.BEGIN,
        ["END"]         = TokenType.END,

        //single char tokens
        ["+"] = TokenType.PLUS,
        ["-"] = TokenType.MINUS,
        ["*"] = TokenType.MUL,
        ["/"] = TokenType.FLOAT_DIV,
        ["("] = TokenType.LPAREN,
        [")"] = TokenType.RPAREN,
        [";"] = TokenType.SEMI,
        ["."] = TokenType.DOT,
        [":"] = TokenType.COLON,
        [","] = TokenType.COMMA,

        //misc
        [":="] = TokenType.ASSIGN
     
    };

    private readonly string text;
    private int pos;
    public char CurrentChar {  get; private set; }
    public int Line { get; private set; }
    public int Col { get; private set; }

    public Lexer(string txt)
    {
        text = txt;
        pos = 0;
        CurrentChar = text[pos];
        Line = 1;
        Col = 1;
    }

    public void Error()
    {
        string s = $"[Lexer error] '{CurrentChar}' line: {Line} column: {Col}";

        throw new LexerError(msg:s);
    }

    public void Advance()
    {
        if(CurrentChar == '\n') 
        {
            Line++;
            Col = 0;
        }
        
        pos++;

        if(pos >= text.Length)
        {
            CurrentChar = default;
        }
        else
        {
            CurrentChar = text[pos];
            Col++;
        }
    }

    public char Peek() 
    {
        int peekPos = pos + 1;

        return peekPos < text.Length ? text[peekPos] : default;
    }

    public void SkipWhiteSpaces()
    {
        while (CurrentChar != default && char.IsWhiteSpace(CurrentChar))
        {
            Advance();
        }
    }

    public void SkipComment()
    {
        while (CurrentChar != '}')
        {
            Advance();
        }

        Advance();
    }

    public Token Number()
    {
        string str = "";

        while (CurrentChar != default && char.IsDigit(CurrentChar))
        {
            str += CurrentChar;
            Advance();
        }

        if (CurrentChar == '.')
        {
            str += CurrentChar;
            Advance();

            while (CurrentChar != default && char.IsDigit(CurrentChar))
            {
                str += CurrentChar;
                Advance();
            }
            
            return new (TokenType.REAL_CONST, str, Line, Col);

        }
        else
        {
            return new (TokenType.INTEGER_CONST, str, Line, Col);
        }
    }

    public Token Id()
    {
        string str = "";

        while (CurrentChar != default && char.IsLetterOrDigit(CurrentChar))
        {
            str += CurrentChar;
            Advance();
        }

        if(Keywords.TryGetValue(str.ToUpper(), out var type))
        {
            return new (type, str.ToUpper(), Line, Col);
        }

        return new (TokenType.ID, str, Line, Col);
    }

    public Token NextToken()
    {
        while (CurrentChar != default)
        {
            if (char.IsWhiteSpace(CurrentChar))
            {
                SkipWhiteSpaces();
            }

            if (char.IsDigit(CurrentChar))
            {
                return Number();
            }

            if (char.IsLetter(CurrentChar))
            {
                return Id();
            }

            if(CurrentChar.Equals('{'))
            {
                Advance();
                SkipComment();
                continue;
            }

            if (CurrentChar.Equals(':') && Peek()=='=')
            {
                Token t = new (TokenType.ASSIGN, ":=", Line, Col);
                Advance();
                Advance();
                return t;
            }


            if (Keywords.TryGetValue(CurrentChar.ToString(), out TokenType type))
            {
                Token t = new (type, CurrentChar.ToString(), Line, Col);
                Advance();
                return t;
            }
            
            Error(); 
            
        }

        return new Token(TokenType.EOF, "", Line, Col);
    }
}