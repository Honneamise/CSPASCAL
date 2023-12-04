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
    private char currentChar;
    public int Line { get; set; }
    public int Col { get; set; }

    public Lexer(string txt)
    {
        text = txt;
        pos = 0;
        currentChar = text[pos];
        Line = 1;
        Col = 1;
    }

    public void Error()
    {
        string s = $"[Lexer error] '{currentChar}' line: {Line} column: {Col}";

        throw new LexerError(msg:s);
    }

    public void Advance()
    {
        if(currentChar == '\n') 
        {
            Line++;
            Col = 0;
        }
        
        pos++;

        if(pos >= text.Length)
        {
            currentChar = default;
        }
        else
        {
            currentChar = text[pos];
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
        while (currentChar != default && char.IsWhiteSpace(currentChar))
        {
            Advance();
        }
    }

    public void SkipComment()
    {
        while (currentChar != '}')
        {
            Advance();
        }

        Advance();
    }

    public Token Number()
    {
        string str = "";

        while (currentChar != default && char.IsDigit(currentChar))
        {
            str += currentChar;
            Advance();
        }

        if (currentChar == '.')
        {
            str += currentChar;
            Advance();

            while (currentChar != default && char.IsDigit(currentChar))
            {
                str += currentChar;
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

        while (currentChar != default && char.IsLetterOrDigit(currentChar))
        {
            str += currentChar;
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
        while (currentChar != default)
        {
            if (char.IsWhiteSpace(currentChar))
            {
                SkipWhiteSpaces();
            }

            if (char.IsDigit(currentChar))
            {
                return Number();
            }

            if (char.IsLetter(currentChar))
            {
                return Id();
            }

            if(currentChar.Equals('{'))
            {
                Advance();
                SkipComment();
                continue;
            }

            if (currentChar.Equals(':') && Peek()=='=')
            {
                Token t = new (TokenType.ASSIGN, ":=", Line, Col);
                Advance();
                Advance();
                return t;
            }


            if (Keywords.TryGetValue(currentChar.ToString(), out TokenType type))
            {
                Token t = new (type, currentChar.ToString(), Line, Col);
                Advance();
                return t;
            }
            
            Error(); 
            
        }

        return new Token(TokenType.EOF, "", Line, Col);
    }
}