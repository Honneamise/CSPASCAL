namespace Pascal;


public enum TokenType
{
    //keywords
    PROGRAM,
    INTEGER,
    REAL,
    INTEGER_DIV,
    VAR,
    PROCEDURE,
    BEGIN,
    END,

    PLUS,
    MINUS,
    MUL,
    FLOAT_DIV,
    LPAREN,
    RPAREN,
    SEMI,
    DOT,
    COLON,
    COMMA,

    //misc
    ASSIGN,
    EOF,

    ID,
    INTEGER_CONST,
    REAL_CONST 
};


public class Token
{
    public TokenType Type { get; }
    public string Value { get; }
    public int Line { get; set; }
    public int Col { get; set; }

    public Token(TokenType type, string value, int line = -1, int col = -1)
    {
        Type = type;
        Value = value;
        Line = line;
        Col = col;
    }


    public override string ToString()
    {
        return $"<{Type}:{Value} {Line}:{Col}>";
    }
}
