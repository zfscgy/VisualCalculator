using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace Calculator
{
    public class Parser
    {
        public string parser_error;
        int maxLength = 100;
        public Element[] Parse(string exprString, int endIndex, out int lastMatchedIndex)
        {
            parser_error = "";
            exprString = exprString.Insert(endIndex,"~~~~~~");
            Element[] Sequence = new Element[maxLength];
            int seqIndex = 0;
            Element var = new Element(Token.Variable);
            lastMatchedIndex = -1;
            for (int i = 0; i < exprString.Length; i++)
            {
                if (exprString[i] == ' ') { }
                else if (char.IsNumber(exprString[i]))
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
                        parser_error += "数字格式错误！\n";
                        Sequence[seqIndex++] = new Element(Token.Null);
                        return Sequence;
                    }
                    Sequence[seqIndex++] = new Element(Token.Number, value);
                    i = j - 1;
                }
                else if (char.IsLower(exprString[i]))
                {
                    List<string> Candiates = Symbols.LongestMatch(exprString, i, endIndex, out int lastMatchedIndex_1);
                    if (Candiates.Count == 1 && Candiates[0].Length == lastMatchedIndex_1 - lastMatchedIndex)
                    {
                        Sequence[seqIndex++] = new Element(Symbols.SymbolToToken[Candiates[0]]);
                        if(Sequence[seqIndex - 1].token == Token.Variable)
                        {
                            Sequence[seqIndex - 1] = var;
                        }
                        i = lastMatchedIndex_1;
                    }
                    else
                    {
                        parser_error += "未找到该标识符！\n";
                        Sequence[seqIndex++] = new Element(Token.Null);
                        return Sequence;
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
                        case ',': Sequence[seqIndex++] = new Element(Token.Comma); break;
                        case '~': Sequence[seqIndex++] = new Element(Token.Null); break;
                        default: Sequence[seqIndex++] = new Element(Token.Null);
                            parser_error += "错误符号！\n"; return Sequence;
                    }
                }
                lastMatchedIndex = i;
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
        Int,
        Diff,
        Sigma,

        Pi,
        E,

        Lparen,
        Rparen,
        Comma,

        Variable,
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

    public class InterpreteException : Exception
    {
        public InterpreteException() { }
        public InterpreteException(string message) : base(message) { }
        public InterpreteException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class Interpreter
    {
        public double intPrecision = Settings.IntegralPrecision;
        public double diffDelta = Settings.DiffDelta;
        public string errorMessage;
        private MainWindow mainWindow;
        private Thread calcThread;
        private double result;
        private bool isRunning = false;
        Element[] Sequence;
        int index;
        long currentProgressUnit;
        public Interpreter(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
        }

        double Prime()
        {
            double value;
            if (Sequence[index].token == Token.Null)
            {
                return 0;
            }
            if (Sequence[index].token == Token.Lparen)
            {
                index++;
                value = Expr();
                if (Sequence[index].token != Token.Rparen)
                {
                    errorMessage += "在左括号后缺少一个右括号！\n";
                    return 0;
                }
                index++;
            }
            else if (Sequence[index].token == Token.Sin)
            {
                index++;
                value = Math.Sin(Expr());
                if (Sequence[index].token != Token.Rparen)
                {
                    errorMessage += "在Sin后缺少右括号！\n";
                    return 0;
                }
                index++;
            }
            else if (Sequence[index].token == Token.Cos)
            {
                index++;
                value = Math.Cos(Expr());
                if(Sequence[index].token != Token.Rparen)
                {
                    errorMessage += "在Cos后缺少右括号！\n";
                    return 0;
                }
                index++;
            }
            else if (Sequence[index].token == Token.Int)
            {
                value = Integral();
            }
            else if (Sequence[index].token == Token.Diff)
            {
                value = Diff();
            }
            else
            {
                value = Sequence[index].val;
                index++;
            }
            return value;
        }
        double Power()
        {
            double val = Prime();
            while (Sequence[index].token == Token.Pow)
            {
                Token op = Sequence[index].token;
                index++;
                val = Math.Pow(val, Prime());
            }
            return val;
        }
        double Product()
        {
            double val = Power();
            while (Sequence[index].token == Token.Mul || Sequence[index].token == Token.Div)
            {
                Token op = Sequence[index].token;
                index++;
                if (op == Token.Mul)
                {
                    val *= Power();
                }
                else
                {
                    val /= Power();
                }
            }
            return val;
        }
        double Expr()
        {
            double val = Product();
            while (Sequence[index].token == Token.Add || Sequence[index].token == Token.Sub)
            {
                Token op = Sequence[index].token;
                index++;
                if (op == Token.Add)
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
        double Integral()
        {
            mainWindow.UpdateProgressBar(0);
            mainWindow.UpdateStatusText(string.Format("开始计算积分，\n分割次数: {0}\n", intPrecision.ToString("f0")));

            double lowerBound;
            double upperBound;
            currentProgressUnit = (long) (intPrecision / 100);
            Element varX = new Element();
            index++;
            lowerBound = Expr();
            if (Sequence[index].token != Token.Comma)
            {
                errorMessage += "在积分内缺少一个逗号！\n";
                return 0;
            }
            index++;
            upperBound = Expr();
            if (Sequence[index].token != Token.Comma)
            {
                errorMessage += "在积分内缺少一个逗号！\n";
                return 0;
            }
            index++;
            for (int i = index; Sequence[i].token != Token.Null; i++)
            {
                if (Sequence[i].token == Token.Variable)
                {
                    varX = Sequence[i];
                    break;
                }
            }
            int currentIndex = index;
            double incremental = (upperBound - lowerBound) / intPrecision;
            double integral = 0;
            int round = 0;
            for (varX.val = lowerBound; varX.val < upperBound; varX.val += incremental)
            {
                integral += Expr() * incremental;
                index = currentIndex;
                round++;
                if(round % currentProgressUnit == 0)
                {
                    mainWindow.UpdateProgressBar(1);
                }
            }
            Expr();
            if (Sequence[index].token != Token.Rparen)
            {
                errorMessage += "在积分号后缺少一个右括号！\n";
                return integral;
            }
            index++;
            mainWindow.UpdateStatusText("积分计算完毕！\n");
            return integral;
        }
        double Diff()
        {
            mainWindow.UpdateStatusText(string.Format("开始求导, 增量: {0}\n", diffDelta.ToString("f15")));
            double var;
            Element varX = new Element();
            for (int i = index; Sequence[i].token != Token.Null; i++)
            {
                if (Sequence[i].token == Token.Variable)
                {
                    varX = Sequence[i];
                    break;
                }
            }
            index++;
            int funcIndex = index;
            Expr();
            if(Sequence[index].token != Token.Comma)
            {
                errorMessage += "在微分号里缺少一个逗号！\n";
            }
            index++;
            varX.val = Expr();
            varX.val -= 0.5 * diffDelta;
            if(Sequence[index].token != Token.Rparen)
            {
                errorMessage += "在微分号后缺少一个右括号！\n";
            }
            int endIndex = index;
            index = funcIndex;
            double val_0 = Expr();
            varX.val += diffDelta;
            index = funcIndex;
            double val_1 = Expr();
            index = endIndex;
            index++;
            return (val_1 - val_0) / diffDelta;

        }
        private void FinishInterpreteProcess(double _result)
        {
            result = _result;
        }
        public void Interprete(object _Sequence)
        {
            isRunning = true;
            errorMessage = "";
            index = 0;
            Sequence = (Element[])_Sequence;
            double result = Expr();
            mainWindow.WorkDone(result);
            isRunning = false;
        }

        public void StartCalc(object _Sequence)
        {
            if (!isRunning)
            {
                calcThread = new Thread(Interprete);
                calcThread.Start(_Sequence);
                return;
            }
        }
        public void StopCalc()
        {
            if (calcThread.IsAlive)
            {
                calcThread.Abort();
                mainWindow.UpdateStatusText("计算终止！\n");
            }
        }
    }
}
