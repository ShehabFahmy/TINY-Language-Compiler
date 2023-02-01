using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Comment()); // just call comment() to ignore it
            root.Children.Add(Function_Statments());
            root.Children.Add(Main_Function());

            return root;
        }

        Node Expression()
        {
            Node expression = new Node("expression");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.StringValue)
                {
                    expression.Children.Add(match(Token_Class.StringValue));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    expression.Children.Add(Equation());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Number ||
                    TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    if (InputPointer + 1 < TokenStream.Count)
                    {
                        if (TokenStream[InputPointer + 1].token_type == Token_Class.Plus ||
                            TokenStream[InputPointer + 1].token_type == Token_Class.Minus ||
                            TokenStream[InputPointer + 1].token_type == Token_Class.Multiply ||
                            TokenStream[InputPointer + 1].token_type == Token_Class.Divide)
                        {
                            expression.Children.Add(Equation());
                        }
                        else
                        {
                            expression.Children.Add(Term());
                        }
                    }
                    else
                    {
                        expression.Children.Add(Term());
                    }
                }
            }
            else
            {
                return null;
            }
            return expression;
        }
        Node Term()
        {
            Node term = new Node("term");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Number)
                {
                    term.Children.Add(match(Token_Class.Number));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    if ((InputPointer + 1 < TokenStream.Count) && TokenStream[InputPointer + 1].token_type == Token_Class.LParanthesis)
                    {
                        term.Children.Add(Function_Call());
                    }
                    else
                    {
                        term.Children.Add(match(Token_Class.Identifier));
                    }
                }
            }
            else
            {
                return null;
            }
            return term;
        }
        Node Function_Call()
        {
            Node function_call = new Node("function_call");
            function_call.Children.Add(match(Token_Class.Identifier));
            function_call.Children.Add(match(Token_Class.LParanthesis));
            function_call.Children.Add(Arguments());
            function_call.Children.Add(match(Token_Class.RParanthesis));
            return function_call;
        }
        Node Arguments()
        {
            Node arguments = new Node("arguments");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Number ||
                TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    arguments.Children.Add(Term());
                    arguments.Children.Add(ArgumentsR());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return arguments;
        }
        Node ArgumentsR()
        {
            Node argumentsR = new Node("argumentsR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    argumentsR.Children.Add(match(Token_Class.Comma));
                    argumentsR.Children.Add(Term());
                    argumentsR.Children.Add(ArgumentsR());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return argumentsR;
        }
        Node Equation()
        {
            Node equation = new Node("equation");
            equation.Children.Add(Factor());
            equation.Children.Add(EquationR());
            return equation;
        }
        Node Factor()
        {
            Node factor = new Node("factor");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Number ||
                TokenStream[InputPointer].token_type == Token_Class.Identifier)
                {
                    factor.Children.Add(Term());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.LParanthesis)
                {
                    factor.Children.Add(match(Token_Class.LParanthesis));
                    factor.Children.Add(Equation());
                    factor.Children.Add(match(Token_Class.RParanthesis));
                }
            }
            else
            {
                return null;
            }
            return factor;
        }
        Node EquationR()
        {
            Node equationR = new Node("equationR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Plus ||
                TokenStream[InputPointer].token_type == Token_Class.Minus ||
                TokenStream[InputPointer].token_type == Token_Class.Multiply ||
                TokenStream[InputPointer].token_type == Token_Class.Divide)
                {
                    equationR.Children.Add(Arithmatic_Operator());
                    equationR.Children.Add(Factor());
                    equationR.Children.Add(EquationR());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return equationR;
        }
        Node Arithmatic_Operator()
        {
            Node arithmatic_operator = new Node("arithmatic_operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Plus)
                {
                    arithmatic_operator.Children.Add(match(Token_Class.Plus));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Minus)
                {
                    arithmatic_operator.Children.Add(match(Token_Class.Minus));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Multiply)
                {
                    arithmatic_operator.Children.Add(match(Token_Class.Multiply));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Divide)
                {
                    arithmatic_operator.Children.Add(match(Token_Class.Divide));
                }
            }
            else
            {
                return null;
            }
            return arithmatic_operator;
        }
        Node Comment()
        {
            Node comment = new Node("comment");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    comment.Children.Add(match(Token_Class.Comment));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return comment;
        }
        Node Assignment_Statement()
        {
            Node assignment_statement = new Node("assignment_statement");
            assignment_statement.Children.Add(match(Token_Class.Identifier));
            assignment_statement.Children.Add(match(Token_Class.Assign));
            assignment_statement.Children.Add(Expression());
            assignment_statement.Children.Add(match(Token_Class.Semicolon));
            return assignment_statement;
        }
        Node Datatype()
        {
            Node datatype = new Node("datatype");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Int)
                {
                    datatype.Children.Add(match(Token_Class.Int));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Float)
                {
                    datatype.Children.Add(match(Token_Class.Float));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    datatype.Children.Add(match(Token_Class.String));
                }
            }
            else
            {
                return null;
            }
            return datatype;
        }
        Node Declaration_Statement()
        {
            Node declaration_statement = new Node("declaration_statement");
            declaration_statement.Children.Add(Datatype());
            declaration_statement.Children.Add(AssignOrId());
            declaration_statement.Children.Add(Declaration_StatementR());
            declaration_statement.Children.Add(match(Token_Class.Semicolon));
            return declaration_statement;
        }
        Node AssignOrId()
        {
            Node assignOrId = new Node("assignOrId");
            assignOrId.Children.Add(match(Token_Class.Identifier));
            assignOrId.Children.Add(AssignOrIdR());
            return assignOrId;
        }
        Node AssignOrIdR()
        {
            Node assignOrIdR = new Node("assignOrIdR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Assign)
                {
                    assignOrIdR.Children.Add(match(Token_Class.Assign));
                    assignOrIdR.Children.Add(Expression());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return assignOrIdR;
        }
        Node Declaration_StatementR()
        {
            Node declaration_statementR = new Node("declaration_statementR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    declaration_statementR.Children.Add(match(Token_Class.Comma));
                    declaration_statementR.Children.Add(AssignOrId());
                    declaration_statementR.Children.Add(Declaration_StatementR());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return declaration_statementR;
        }
        Node Write_Statement()
        {
            Node write_statement = new Node("write_statement");
            write_statement.Children.Add(match(Token_Class.Write));
            write_statement.Children.Add(Write_StatementR());
            write_statement.Children.Add(match(Token_Class.Semicolon));
            return write_statement;
        }
        Node Write_StatementR()
        {
            Node write_statementR = new Node("write_statementR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.StringValue ||
                TokenStream[InputPointer].token_type == Token_Class.Number ||
                TokenStream[InputPointer].token_type == Token_Class.Identifier ||
                TokenStream[InputPointer].token_type == Token_Class.LParanthesis ||
                (InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer + 1].token_type == Token_Class.Plus ||
                TokenStream[InputPointer + 1].token_type == Token_Class.Minus ||
                TokenStream[InputPointer + 1].token_type == Token_Class.Multiply ||
                TokenStream[InputPointer + 1].token_type == Token_Class.Divide)))
                {
                    write_statementR.Children.Add(Expression());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Endl)
                {
                    write_statementR.Children.Add(match(Token_Class.Endl));
                }
            }
            else
            {
                return null;
            }
            return write_statementR;
        }
        Node Read_Statement()
        {
            Node read_statement = new Node("read_statement");
            read_statement.Children.Add(match(Token_Class.Read));
            read_statement.Children.Add(match(Token_Class.Identifier));
            read_statement.Children.Add(match(Token_Class.Semicolon));
            return read_statement;
        }
        Node Return_Statement()
        {
            Node return_statement = new Node("return_statement");
            return_statement.Children.Add(match(Token_Class.Return));
            return_statement.Children.Add(Expression());
            return_statement.Children.Add(match(Token_Class.Semicolon));
            return return_statement;
        }
        Node Condition_Operator()
        {
            Node condition_operator = new Node("condition_operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.LessThan)
                {
                    condition_operator.Children.Add(match(Token_Class.LessThan));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.GreaterThan)
                {
                    condition_operator.Children.Add(match(Token_Class.GreaterThan));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Equal)
                {
                    condition_operator.Children.Add(match(Token_Class.Equal));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.NotEqual)
                {
                    condition_operator.Children.Add(match(Token_Class.NotEqual));
                }
            }
            else
            {
                return null;
            }
            return condition_operator;
        }
        Node Condition()
        {
            Node condition = new Node("condition");
            condition.Children.Add(match(Token_Class.Identifier));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());
            return condition;
        }
        Node Boolean_Operator()
        {
            Node boolean_operator = new Node("boolean_operator");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.And)
                {
                    boolean_operator.Children.Add(match(Token_Class.And));
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Or)
                {
                    boolean_operator.Children.Add(match(Token_Class.Or));
                }
            }
            else
            {
                return null;
            }
            return boolean_operator;
        }
        Node Condition_Statement()
        {
            Node condition_statement = new Node("condition_statement");
            condition_statement.Children.Add(Condition());
            condition_statement.Children.Add(Condition_StatementR());
            return condition_statement;
        }
        Node Condition_StatementR()
        {
            Node condition_statementR = new Node("condition_statementR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.And ||
                TokenStream[InputPointer].token_type == Token_Class.Or)
                {
                    condition_statementR.Children.Add(Boolean_Operator());
                    condition_statementR.Children.Add(Condition());
                    condition_statementR.Children.Add(Condition_StatementR());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return condition_statementR;
        }
        Node If_Statement()
        {
            Node if_statement = new Node("if_statement");
            if_statement.Children.Add(match(Token_Class.If));
            if_statement.Children.Add(Condition_Statement());
            if_statement.Children.Add(match(Token_Class.Then));
            if_statement.Children.Add(Statements());
            if_statement.Children.Add(Elif());
            if_statement.Children.Add(Els());
            if_statement.Children.Add(match(Token_Class.End));
            return if_statement;
        }
        Node Statements()
        {
            Node statements = new Node("statements");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comment || (TokenStream[InputPointer].token_type == Token_Class.Identifier && InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.Assign) ||
                TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String ||
                    TokenStream[InputPointer].token_type == Token_Class.Write || TokenStream[InputPointer].token_type == Token_Class.Read || TokenStream[InputPointer].token_type == Token_Class.Return ||
                    TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    statements.Children.Add(Stats());
                    statements.Children.Add(Statements());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return statements;
        }
        Node Stats()
        {
            Node stats = new Node("stats");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    stats.Children.Add(Comment()); // comment this line to ignore it
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Identifier &&
                    (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].token_type == Token_Class.Assign))
                {
                    stats.Children.Add(Assignment_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Int ||
                    TokenStream[InputPointer].token_type == Token_Class.Float ||
                    TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    stats.Children.Add(Declaration_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    stats.Children.Add(Write_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    stats.Children.Add(Read_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Return)
                {
                    stats.Children.Add(Return_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.If)
                {
                    stats.Children.Add(If_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    stats.Children.Add(Repeat_Statement());
                }
            }
            else
            {
                return null;
            }
            return stats;
        }
        Node Elif()
        {
            Node elif = new Node("elif");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.ElseIf)
                {
                    elif.Children.Add(Else_If_Statement());
                    elif.Children.Add(Elif());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return elif;
        }
        Node Els()
        {
            Node els = new Node("els");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Else)
                {
                    els.Children.Add(Else_Statement());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return els;
        }
        Node Else_If_Statement()
        {
            Node elseif = new Node("elseif");
            elseif.Children.Add(match(Token_Class.ElseIf));
            elseif.Children.Add(Condition_Statement());
            elseif.Children.Add(match(Token_Class.Then));
            elseif.Children.Add(Statements());
            return elseif;
        }
        Node Else_Statement()
        {
            Node elses = new Node("else");
            elses.Children.Add(match(Token_Class.Else));
            elses.Children.Add(Statements());
            return elses;
        }
        Node Repeat_Statement()
        {
            Node repeat = new Node("repeat");
            repeat.Children.Add(match(Token_Class.Repeat));
            repeat.Children.Add(Statements());
            repeat.Children.Add(match(Token_Class.Until));
            repeat.Children.Add(Condition_Statement());
            return repeat;
        }
        Node Parameter()
        {
            Node parameter = new Node("parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Token_Class.Identifier));
            return parameter;
        }
        Node Function_Declaration()
        {
            Node function_declaration = new Node("function_declaration");
            function_declaration.Children.Add(Datatype());
            function_declaration.Children.Add(match(Token_Class.Identifier));
            function_declaration.Children.Add(match(Token_Class.LParanthesis));
            function_declaration.Children.Add(Parameter_List());
            function_declaration.Children.Add(match(Token_Class.RParanthesis));
            return function_declaration;
        }
        Node Parameter_List()
        {
            Node parameter_list = new Node("parameter_list");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Int ||
                TokenStream[InputPointer].token_type == Token_Class.Float ||
                TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    parameter_list.Children.Add(Parameter());
                    parameter_list.Children.Add(Parameter_ListR());
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            return parameter_list;
        }
        Node Parameter_ListR()
        {
            Node parameter_listR = new Node("parameter_listR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comma)
                {
                    parameter_listR.Children.Add(match(Token_Class.Comma));
                    parameter_listR.Children.Add(Parameter());
                    parameter_listR.Children.Add(Parameter_ListR());
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
            return parameter_listR;
        }
        Node Function_Body()
        {
            Node function_body = new Node("Function_Body");
            function_body.Children.Add(match(Token_Class.OpenBrace));
            function_body.Children.Add(StatementsR());
            function_body.Children.Add(Return_Statement());
            function_body.Children.Add(match(Token_Class.CloseBrace));
            return function_body;
        }
        Node StatementsR()
        {
            Node statementsR = new Node("statementsR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comment || (TokenStream[InputPointer].token_type == Token_Class.Identifier) ||
                TokenStream[InputPointer].token_type == Token_Class.Int || TokenStream[InputPointer].token_type == Token_Class.Float || TokenStream[InputPointer].token_type == Token_Class.String ||
                    TokenStream[InputPointer].token_type == Token_Class.Write || TokenStream[InputPointer].token_type == Token_Class.Read ||
                    TokenStream[InputPointer].token_type == Token_Class.If || TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    statementsR.Children.Add(StatsR());
                    statementsR.Children.Add(StatementsR());
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
            return statementsR;
        }
        Node StatsR()
        {
            Node statsR = new Node("statsR");
            if (InputPointer < TokenStream.Count)
            {
                if (TokenStream[InputPointer].token_type == Token_Class.Comment)
                {
                    statsR.Children.Add(Comment()); // comment this line to ignore it
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Identifier &&
                    ((InputPointer + 1 < TokenStream.Count) && TokenStream[InputPointer + 1].token_type == Token_Class.Assign))
                {
                    statsR.Children.Add(Assignment_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Int ||
                    TokenStream[InputPointer].token_type == Token_Class.Float ||
                    TokenStream[InputPointer].token_type == Token_Class.String)
                {
                    statsR.Children.Add(Declaration_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Write)
                {
                    statsR.Children.Add(Write_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Read)
                {
                    statsR.Children.Add(Read_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.If)
                {
                    statsR.Children.Add(If_Statement());
                }
                else if (TokenStream[InputPointer].token_type == Token_Class.Repeat)
                {
                    statsR.Children.Add(Repeat_Statement());
                }
            }
            else
            {
                return null;
            }
            return statsR;
        }
        Node Function_Statment()
        {
            Node function_statment = new Node("function_statment");
            function_statment.Children.Add(Function_Declaration());
            function_statment.Children.Add(Function_Body());
            return function_statment;
        }
        Node Main_Function()
        {
            Node main_function = new Node("main_function");
            main_function.Children.Add(Datatype());
            main_function.Children.Add(match(Token_Class.Main));
            main_function.Children.Add(match(Token_Class.LParanthesis));
            main_function.Children.Add(match(Token_Class.RParanthesis));
            main_function.Children.Add(Function_Body());
            return main_function;
        }
        Node Function_Statments()
        {
            Node function_statments = new Node("function_statments");
            if (InputPointer < TokenStream.Count)
            {
                if ((TokenStream[InputPointer].token_type == Token_Class.Int ||
                    TokenStream[InputPointer].token_type == Token_Class.Float ||
                    TokenStream[InputPointer].token_type == Token_Class.String) &&
                    TokenStream[InputPointer + 1].token_type != Token_Class.Main)
                {
                    function_statments.Children.Add(Function_Statment());
                    function_statments.Children.Add(Function_Statments());
                }
            }
            else
            {
                return null;
            }
            return function_statments;
        }





        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}