namespace Pascal;


public enum TokenType
{
    INTEGER,
    INTEGER_CONST,
    REAL,
    REAL_CONST,

    PLUS,
    MINUS,
    MUL,
    INTEGER_DIV,
    FLOAT_DIV,

    PROGRAM,
    PROCEDURE,
    VAR,
    BEGIN,
    END,
    ASSIGN,
    ID,

    LPAREN,
    RPAREN,
    SEMI,
    COLON,
    COMMA,
    DOT,

    EOF
};


public class Token(TokenType type, dynamic value)
{
    public TokenType Type { get; } = type;
    public dynamic Value { get; } = value;

    public override string ToString() => $"<{Type}:{Value}>";
}
