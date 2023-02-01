using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TINY_Compiler;
using TINY_Compiler;

public enum Token_Class
{
    /*Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Integer,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Idenifier, Constant,*/
    Int, Float, String, Read, Write, Repeat, Until, If, ElseIf, Else, Then, Return,
    Endl, End, Main, Identifier, Number, StringValue, Comment, LParanthesis, RParanthesis, Comma,
    Plus, Minus, Multiply, Divide, Assign, Semicolon, LessThan, GreaterThan, NotEqual,
    Equal, And, Or, OpenBrace, CloseBrace
}
namespace TINY_Compiler
{
    public class Token
    {
        public string lex;
        public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> DataTypes = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            DataTypes.Add("int", Token_Class.Int);
            DataTypes.Add("float", Token_Class.Float);
            DataTypes.Add("string", Token_Class.String);

            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.ElseIf);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.Endl);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("main", Token_Class.Main);

            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("{", Token_Class.OpenBrace);
            Operators.Add("}", Token_Class.CloseBrace);

            Operators.Add("=", Token_Class.Equal);
            Operators.Add("<", Token_Class.LessThan);
            Operators.Add(">", Token_Class.GreaterThan);
            Operators.Add("<>", Token_Class.NotEqual);
            Operators.Add("+", Token_Class.Plus);
            Operators.Add("-", Token_Class.Minus);
            Operators.Add("*", Token_Class.Multiply);
            Operators.Add("/", Token_Class.Divide);
            Operators.Add("&&", Token_Class.And);
            Operators.Add("||", Token_Class.Or);


        }

        public void StartScanning(string SourceCode)
        {
            for (int i = 0; i < SourceCode.Length; i++)
            {
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (CurrentChar >= 'A' && CurrentChar <= 'z') // Identifier
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if ((SourceCode[j] >= 'A' && SourceCode[j] <= 'z') ||
                            (SourceCode[j] >= '0' && SourceCode[j] <= '9'))
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '"') // String
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] != '"')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                            break;
                        }
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '/') // Comment
                {
                    j++;
                    if (SourceCode[j] == '*')
                    {
                        while (j < SourceCode.Length)
                        {
                            if (j < SourceCode.Length - 1 && SourceCode[j] == '*' && SourceCode[j + 1] == '/')
                            {
                                CurrentLexeme += SourceCode[j];
                                j++;
                                CurrentLexeme += SourceCode[j];
                                j++;
                                break;
                            }
                            else
                            {
                                CurrentLexeme += SourceCode[j];
                                j++;
                            }
                        }
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/') // Arithemtic Operator
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '+' || SourceCode[j] == '-' ||
                            SourceCode[j] == '*' || SourceCode[j] == '/')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '&' || CurrentChar == '|') // Boolean Operator
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '&' || SourceCode[j] == '|')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = j - 1;
                }
                else if (CurrentChar == '>' || CurrentChar == '<' || CurrentChar == '=') // Condition Operator
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == '>' || SourceCode[j] == '<' || SourceCode[j] == '=')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = j - 1;
                }
                else if ((CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '.') // Number
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if ((SourceCode[j] >= '0' && SourceCode[j] <= '9') || SourceCode[j] == '.')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            if (SourceCode[j] >= 'A' && SourceCode[j] <= 'z')
                            {
                                CurrentLexeme += SourceCode[j];
                                j++;
                            }
                            else
                                break;
                        }
                    }
                    i = j - 1;
                }

                else if (CurrentChar == ';' || CurrentChar == ',' || CurrentChar == '(' || CurrentChar == ')'
                    || CurrentChar == '{' || CurrentChar == '}')
                {
                    i = j;
                }
                else if (CurrentChar == ':')
                {
                    j++;
                    while (j < SourceCode.Length)
                    {
                        if (SourceCode[j] == ':' || SourceCode[j] == '=')
                        {
                            CurrentLexeme += SourceCode[j];
                            j++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i = j - 1;
                }
                FindTokenClass(CurrentLexeme);
            }

            TINY_Compiler.TokenStream = Tokens;
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (ReservedWords.ContainsKey(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }

            //Is it an Operator?
            else if (Operators.ContainsKey(Lex))
            {
                //Is it an Arithemtic Operator
                if (isArithmeticOperator(Lex))
                {
                    if (Lex == "+")
                    {
                        Tok.token_type = Token_Class.Plus;
                    }
                    else if (Lex == "-")
                    {
                        Tok.token_type = Token_Class.Minus;
                    }
                    else if (Lex == "*")
                    {
                        Tok.token_type = Token_Class.Multiply;
                    }
                    else if (Lex == "/")
                    {
                        Tok.token_type = Token_Class.Divide;
                    }
                }
                //Is it a Boolean Operator
                else if (isBooleanOperator(Lex))
                {
                    if (Lex == "&&")
                    {
                        Tok.token_type = Token_Class.And;
                    }
                    else if (Lex == "||")
                    {
                        Tok.token_type = Token_Class.Or;
                    }
                }
                //Is it a Condition Operator
                else if (isConditionOperator(Lex))
                {
                    if (Lex == "<")
                    {
                        Tok.token_type = Token_Class.LessThan;
                    }
                    else if (Lex == ">")
                    {
                        Tok.token_type = Token_Class.GreaterThan;
                    }
                    else if (Lex == "=")
                    {
                        Tok.token_type = Token_Class.Equal;
                    }
                    else if (Lex == "<>")
                    {
                        Tok.token_type = Token_Class.NotEqual;
                    }
                }
                else
                {
                    Tok.token_type = Operators[Lex];
                }
                Tokens.Add(Tok);
            }

            //Is it a DataType
            else if (isDataType(Lex))
            {
                if (Lex == "int")
                {
                    Tok.token_type = Token_Class.Int;
                }
                else if (Lex == "string")
                {
                    Tok.token_type = Token_Class.String;
                }
                else if (Lex == "float")
                {
                    Tok.token_type = Token_Class.Float;
                }
                Tokens.Add(Tok);

            }

            //Is it an Identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }

            //Is it a String
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.StringValue;
                Tokens.Add(Tok);
            }

            //Is it a Number
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }

            //Is it a Comment
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }

            //Is it an undefined?
            else
            {
                Errors.Error_List.Add(Lex);
            }

        }


        bool isDataType(string lex)
        {
            bool isValid = false;
            if (lex == "int" || lex == "string" || lex == "float")
            {
                isValid = true;
            }
            return isValid;
        }

        bool isIdentifier(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^[a-zA-Z]([a-zA-Z]|[0-9])*$", RegexOptions.Compiled);
            isValid = rx.IsMatch(lex);
            return isValid;
        }
        bool isString(string lex)
        {
            bool isValid = false;
            var rx = new Regex("^\"[^\"]*\"$", RegexOptions.Compiled);
            isValid = rx.IsMatch(lex);
            return isValid;
        }
        bool isArithmeticOperator(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^(\+|-|\*|\/)$", RegexOptions.Compiled);
            isValid = rx.IsMatch(lex);
            return isValid;
        }
        bool isBooleanOperator(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^(&&|\|\|)$", RegexOptions.Compiled);
            isValid = rx.IsMatch(lex);
            return isValid;
        }
        bool isConditionOperator(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^(<>|<|>|=)$", RegexOptions.Compiled);
            isValid = rx.IsMatch(lex);
            return isValid;
        }
        bool isComment(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^/\*.*\*/$", RegexOptions.Compiled);
            isValid = rx.IsMatch(lex);
            return isValid;
        }
        bool isNumber(string lex)
        {
            bool isValid = false;
            var rx = new Regex(@"^[0-9]+(\.[0-9]+)?$", RegexOptions.Compiled);
            isValid = rx.IsMatch(lex);
            return isValid;
        }
    }
}