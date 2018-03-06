using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public class Parser
    {
        int maxLength = 100;
        public Element[] Parse(string exprString)
        {
            Element[] Sequence = new Element[maxLength];
            int seqIndex = 0;
            for(int i = 0; i< exprString.Length; i++)
            {
                if (exprString[i] <= '9' && exprString[i] >= '0')
                {
                    int j = i;
                    double value = 0;
                    while (j < exprString.Length && ((exprString[j] <= '9' && exprString[j] >= '0') || exprString[j] == '.'))
                    {
                        j++;
                    }
                    try
                    {
                        value = Double.Parse(exprString.Substring(i, j - i));
                    }
                    catch (FormatException fe)
                    {
                        Sequence[seqIndex++] = new Element(Token.Null);
                        return Sequence;
                    }
                    Sequence[seqIndex++] = new Element(Token.Number, value);
                    i = j - 1;
                }
                else if (exprString[i] <= 'z' && exprString[i] >= 'a')
                {
                    if(exprString[i] == 'p')
                    {
                        i++;
                        if(exprString[i] == i)
                        {
                            Sequence[seqIndex++] = new Element(Token.Pi,Math.PI);
                            i++;
                        }
                        else
                        {
                            return Sequence;
                        }
                    }
                    else if(exprString[i] == 'e')
                    {
                        Sequence[seqIndex++] = new Element(Token.E, Math.E);
                        i++;
                    }
                }
                else
                {
                    switch (exprString[i])
                    {
                        case '+': Sequence[seqIndex++] = new Element(Token.Add); break;
                        case '-': Sequence[seqIndex++] = new Element(Token.Sub); break;
                        case '*': Sequence[seqIndex++] = new Element(Token.Mul); break;
                        case '/': Sequence[seqIndex++] = new Element(Token.Div); break;
                        case '^': Sequence[seqIndex++] = new Element(Token.Pow); break;
                        case '(': Sequence[seqIndex++] = new Element(Token.Lparen); break;
                        case ')': Sequence[seqIndex++] = new Element(Token.Rparen); break;
                        default: Sequence[seqIndex++] = new Element(Token.Null); return Sequence;
                    }
                }
            }
            Sequence[seqIndex++] = new Element(Token.Null);
            return Sequence;

        }
    }


    public enum Token
    {
        Number,
        Add,
        Sub,
        Mul,
        Div,
        Pow,
        Exp,
        Cos,
        Sin,
        Tan,

        Pi,
        E,

        Lparen,
        Rparen,

        Terminate,
        Null,
    }

    public class Element
    {
        public Token token;
        public double val;
        public Element(Token _token = Token.Null, double _val = 0)
        {
            token = _token;
            val = _val;
        }
        public bool isNumber()
        {
            return token == Token.Number || token == Token.Pi || token == Token.E;
        }
    }

    public class InterpreteException:Exception
    {
        public InterpreteException() { }
        public InterpreteException(string message):base(message) { }
        public InterpreteException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class Interpreter
    {
        Element[] Sequence;
        int index;
        double Prime()
        {
            double value;
            if(Sequence[index].token == Token.Lparen)
            {
                index++;
                value = Expr();
                if(Sequence[index].token != Token.Rparen)
                {
                    throw new InterpreteException("Parens not match!");
                }
            }
            else
            {
                value = Sequence[index].val;
            }
            index++;
            return value;
        }
        double Product()
        {
            double val = Prime();
            while(Sequence[index].token == Token.Mul || Sequence[index].token == Token.Div)
            {
                Token op = Sequence[index].token;
                index++;
                if(op == Token.Mul)
                {
                    val *= Prime();
                }
                else
                {
                    val /= Prime();
                }
            }
            return val;
        }
        double Expr()
        {
            double val = Product();
            while(Sequence[index].token == Token.Add || Sequence[index].token == Token.Sub)
            {
                Token op = Sequence[index].token;
                index++;
                if(op == Token.Add)
                {
                    val += Product();
                }
                else
                {
                    val -= Product();
                }
            }
            return val;
        }

        public double Interprete(Element[] _Sequence)
        {
            index = 0;
            Sequence = _Sequence;
            return Expr();
        }
    }
}
