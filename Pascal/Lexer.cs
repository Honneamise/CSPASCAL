namespace Pascal;


public class Lexer
{
    private static readonly Dictionary<string, Token> Keywords = new()
    {
        ["program"] = new Token(TokenType.PROGRAM, "program"),
        ["var"] = new Token(TokenType.VAR, "var"),
        ["procedure"] = new Token(TokenType.PROCEDURE, "procedure"),
        ["begin"] = new Token(TokenType.BEGIN, "begin"),
        ["end"] = new Token(TokenType.END, "end"),
        ["integer"] = new Token(TokenType.INTEGER, "integer"),
        ["real"] = new Token(TokenType.REAL, "real"),
        ["div"] = new Token(TokenType.INTEGER_DIV, "div"),
    };

    private readonly string text;
    private int pos;
    private char currentChar;
    public int Line { get; set; }

    public Lexer(string txt)
    {
        text = txt;
        Line = 1;
        pos = 0;
        currentChar = text[pos];
    }

    public void Reset()
    {
        Line = 1;
        pos = 0;
        currentChar = text[pos];
    }

    public void Advance()
    {
        pos++;

        if (pos < text.Length)
        {
            if (text[pos] == '\n') { Line++; }

            currentChar = text[pos];
        }
        else
        { 
            currentChar = default; 
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
            
            return new Token(TokenType.REAL_CONST, str);

        }
        else
        {
            return new Token(TokenType.INTEGER_CONST, str);
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

        return Keywords.TryGetValue(str, out Token? value) ? value : new Token(TokenType.ID, str);
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

            switch (currentChar)
            {
                case '{':
                    Advance();
                    SkipComment();
                    break;

                case ':':
                    if(Peek()=='=')
                    {
                        Advance();
                        Advance();
                        return new Token(TokenType.ASSIGN, ":=");
                    }
                    else
                    {
                        Advance();
                        return new Token(TokenType.COLON, ':');
                    }

                case ',':
                    Advance();
                    return new Token(TokenType.COMMA, ',');

                case ';':
                    Advance();
                    return new Token(TokenType.SEMI, ';');

                case '.':
                    Advance();
                    return new Token(TokenType.DOT, '.');

                case '+':
                    Advance();
                    return new Token(TokenType.PLUS, '+');

                case '-':
                    Advance();
                    return new Token(TokenType.MINUS, '-');

                case '*':
                    Advance();
                    return new Token(TokenType.MUL, '*');

                case '/' :
                    Advance();
                    return new Token(TokenType.FLOAT_DIV, '/');

                case '(':
                    Advance();
                    return new Token(TokenType.LPAREN, '(');

                case ')':
                    Advance();
                    return new Token(TokenType.RPAREN, ')');

                default:
                    throw new Exception($"[{Line}]Invalid character found:{currentChar}");
            }
        }

        return new Token(TokenType.EOF, 0);
    }
}