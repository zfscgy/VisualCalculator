using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
    public static class Settings
    {
        public static double IntegralPrecision = 100000;
        public static double DiffDelta = 0.00001;
    }
    public static class ButtonNames
    {
        public static string button_add_name = "Button_Add";
        public static string button_sub_name = "Button_Sub";
        public static string button_mul_name = "Button_Mul";
        public static string button_div_name = "Button_Div";
        public static string button_pow_name = "Button_Pow";
        public static string button_sqrt_name = "Button_Sqrt";
        public static string button_ln_name = "Button_Ln";
        public static string button_log_name = "Button_Log";
        public static string button_sh_name = "Button_Sh";
        public static string button_ch_name = "Button_Ch";
        public static string button_sin_name = "Button_Sin";
        public static string button_cos_name = "Button_Cos";
        public static string button_tan_name = "Button_Tan";
        public static string button_arcsin_name = "Button_ArcSin";
        public static string button_arccos_name = "Button_ArcCos";
        public static string button_arctan_name = "Button_ArcTan";
        public static string button_int_name = "Button_Int";
        public static string button_diff_name = "Button_Diff";
        public static string button_sum_name = "Button_Sum";
        public static string button_pi_name = "Button_Pi";
        public static string button_e_name = "Button_E";
    }
    public static class Symbols
    {
        public static string s_add = "+";
        public static string s_sub = "-";
        public static string s_mul = "*";
        public static string s_div = "/";
        public static string s_pow = "^";
        public static string s_sqrt = "sqrt(";
        public static string s_sh = "sh(";
        public static string s_ch = "ch(";
        public static string s_ln = "ln(";
        public static string s_log = "log(";
        public static string s_sin = "sin(";
        public static string s_cos = "cos(";
        public static string s_tan = "tan(";
        public static string s_arcsin = "arcsin(";
        public static string s_arccos = "arccos(";
        public static string s_arctan = "arctan(";
        public static string s_int = "int(";
        public static string s_diff = "diff(";
        public static string s_sum = "sum(";
        public static string s_pi = "pi";
        public static string s_e = "e";
        public static string s_x = "x";
        public static string[] AllSymbols =
        {
            s_add,
            s_sub,
            s_mul,
            s_div,
            s_pow,
            s_sqrt,
            s_sh,
            s_ch,
            s_ln,
            s_log,
            s_sin,
            s_cos,
            s_tan,
            s_arcsin,
            s_arccos,
            s_arctan,
            s_int,
            s_diff,
            s_sum,
            s_pi,
            s_e,
            s_x,
        };
        public static Dictionary<string, string> ButtonNameToSymbol
            = new Dictionary<string, string>
            {
                { ButtonNames.button_add_name, Symbols.s_add },
                { ButtonNames.button_sub_name, Symbols.s_sub },
                { ButtonNames.button_mul_name, Symbols.s_mul },
                { ButtonNames.button_div_name, Symbols.s_div },
                { ButtonNames.button_pow_name, Symbols.s_pow },
                { ButtonNames.button_sqrt_name, Symbols.s_sqrt },
                { ButtonNames.button_sh_name, Symbols.s_sh },
                { ButtonNames.button_ch_name, Symbols.s_ch },
                { ButtonNames.button_ln_name, Symbols.s_ln },
                { ButtonNames.button_log_name, Symbols.s_log },
                { ButtonNames.button_sin_name, Symbols.s_sin },
                { ButtonNames.button_cos_name, Symbols.s_cos },
                { ButtonNames.button_tan_name, Symbols.s_tan },
                { ButtonNames.button_arcsin_name, Symbols.s_arcsin },
                { ButtonNames.button_arccos_name, Symbols.s_arccos },
                { ButtonNames.button_arctan_name, Symbols.s_arctan },
                { ButtonNames.button_int_name, Symbols.s_int },
                { ButtonNames.button_diff_name, Symbols.s_diff },
                { ButtonNames.button_sum_name, Symbols.s_sum },
                { ButtonNames.button_pi_name, Symbols.s_pi },
                { ButtonNames.button_e_name, Symbols.s_e },
            };
        public static Dictionary<string, Token> SymbolToToken
            = new Dictionary<string, Token>
            {
                { Symbols.s_add, Token.Add },
                { Symbols.s_sub, Token.Sub },
                { Symbols.s_mul, Token.Mul },
                { Symbols.s_div, Token.Div },
                { Symbols.s_pow, Token.Pow },
                { Symbols.s_sqrt,Token.Sqrt },
                { Symbols.s_sh, Token.Sh },
                { Symbols.s_ch, Token.Ch },
                { Symbols.s_ln, Token.Ln },
                { Symbols.s_log, Token.Log },
                { Symbols.s_sin, Token.Sin },
                { Symbols.s_cos, Token.Cos },
                { Symbols.s_tan, Token.Tan },
                { Symbols.s_arcsin, Token.Arcsin },
                { Symbols.s_arccos, Token.Arccos },
                { Symbols.s_arctan, Token.Arctan },
                { Symbols.s_int, Token.Int },
                { Symbols.s_diff,Token.Diff },
                { Symbols.s_sum, Token.Sum },
                { Symbols.s_pi, Token.Pi },
                { Symbols.s_e, Token.E },
                { Symbols.s_x, Token.Variable },
            };
        public static List<String> LongestMatch(string s, int start, int end, out int lastMatched)
        {
            byte[] currentMatched = new byte[AllSymbols.Length];
            List<string> Matched = new List<string>();
            int i;
            for(i = 0; i < currentMatched.Length; i ++)
            {
                currentMatched[i] = 1;
            }
            for (i = start; i < end; i++)
            {
                bool hasAnyMatch = false;
                for (int j = 0; j < currentMatched.Length; j++)
                {

                    if (currentMatched[j] == 1)
                    {
                        if (i - start == AllSymbols[j].Length)
                        {
                            currentMatched[j] = 1;
                        }
                        else if (s[i] == AllSymbols[j][i - start])
                        {
                            hasAnyMatch = true;
                            currentMatched[j] = 1;
                        }
                        else
                        {
                            currentMatched[j] = 0;
                        }
                    }
                }
                if (!hasAnyMatch)
                {
                    break;
                }
            }
            lastMatched = i - 1;
            for(i = 0;i < AllSymbols.Length; i++)
            {
                if(currentMatched[i] == 1)
                {
                    Matched.Add(AllSymbols[i]);
                }
            }
            return Matched;
        }
    }
}
