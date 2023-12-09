namespace Pascal;

public enum ErrorCode
{
    INVALID_DATATYPE,
    UNEXPECTED_TOKEN,
    ID_NOT_FOUND,
    DUPLICATE_ID
}

public class Error : Exception
{
    public ErrorCode? Code { get; }
    public Token? Token { get; }

    public Error(ErrorCode? code = null, Token? token = null, string? msg = null) : base(msg)
    {
        Code = code;
        Token = token;  
    }
}

public class LexerError : Error
{
    public LexerError(ErrorCode? code = null, Token? token = null, string? msg = null) : base(code, token, msg)
    { }
}

public class ParserError : Error
{
    public ParserError(ErrorCode? code = null, Token? token = null, string? msg = null) : base(code, token, msg)
    { }
}

public class SemanticError : Error
{
    public SemanticError(ErrorCode? code = null, Token? token = null, string? msg = null) : base(code, token, msg)
    { }
}