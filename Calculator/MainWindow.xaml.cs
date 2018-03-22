using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Calculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int lastLength = 0;
        Parser parser = new Parser();
        Interpreter interpreter;
        CalculatorSettings calculatorSettings;

        private bool isCalculating = false;
        public MainWindow()
        {
            InitializeComponent();

            interpreter = new Interpreter(this);
            calculatorSettings = new CalculatorSettings(interpreter);
        }

        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {
            if(isCalculating)
            {
                return;
            }
            isCalculating = true;
            OutputBox.Text = "";
            TextBox_Status.Text = "";
            interpreter.StartCalc(parser.Parse(InputBox.Text, InputBox.Text.Length, out int stoppedIndex));
            InputBox.SelectionStart = stoppedIndex + 1;
            TextBox_ErrorOuput.Text = parser.parser_error;
        }

        private bool IgnoreChange = false;
        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox_ErrorOuput.Text = "";
            if(IgnoreChange)
            {
                return;
            }
            int lastMatchedIndex = 0;
            parser.Parse(InputBox.Text, InputBox.SelectionStart, out lastMatchedIndex);
            SetListbox(lastMatchedIndex + 1);
            TextBox_ErrorOuput.Text += parser.parser_error;
        }

        #region Click Methods
        private int lastSelectionStart;
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            lastSelectionStart = InputBox.SelectionStart;
            if(e.Key == Key.Enter)
            {
                if (AutoCompleteBox.Items.Count != 0)
                {
                    if (AutoCompleteBox.Focus())
                    {
                        AutoCompleteBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    CalcButton_Click(sender, e);
                }
            }

        }

        private void Button_Symbols_Click(object sender, RoutedEventArgs e)
        {
            IgnoreChange = true;
            int index = InputBox.SelectionStart;
            string symbolString = Symbols.ButtonNameToSymbol[((Button)sender).Name];
            InputBox.Focus();
            InputBox.Text = InputBox.Text.Insert(index, symbolString);
            InputBox.SelectionStart = index + symbolString.Length;
            IgnoreChange = false;
        }

        #endregion
        private int lastMatched;
        private int lastComplete;
        private void SetListbox(int index)
        {
            lastComplete = index - 1;
            AutoCompleteBox.Items.Clear();
            List<string> list = Symbols.LongestMatch(InputBox.Text, index, InputBox.SelectionStart, out lastMatched);
            if (lastMatched == InputBox.SelectionStart - 1)
            {
                foreach (string s in list)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = s;
                    AutoCompleteBox.Items.Add(item);
                }
            }
        }

        private void AutoCompleteBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                IgnoreChange = true;
                int index = InputBox.SelectionStart;
                string symbolString = (string)((ListBoxItem)((ListBox)sender).SelectedItem).Content;
                InputBox.Text = InputBox.Text.Substring(0, lastComplete + 1) + InputBox.Text.Substring(index);

                if (lastComplete + 1 == InputBox.Text.Length)
                {
                    InputBox.Text += symbolString;
                }
                else
                {
                    InputBox.Text = InputBox.Text.Insert(lastComplete + 1, symbolString);
                }
                InputBox.Focus();
                InputBox.SelectionStart = index + symbolString.Length;
                IgnoreChange = false;
                parser.Parse(InputBox.Text, InputBox.SelectionStart, out int lastMatchedIndex);
                SetListbox(lastMatchedIndex + 1);

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            calculatorSettings.Show();
        }

        public void UpdateProgressBar(byte act)
        {
            this.Dispatcher.Invoke(new Action(() => { if (act == 1) ProgressBar_Calc.Value++; else ProgressBar_Calc.Value = 0; }));
        }

        public void UpdateStatusText(string text)
        {
            this.Dispatcher.Invoke(new Action(() => { TextBox_Status.Text += text; }));
        }

        private void FinishCalc(double result)
        {
            OutputBox.Text = result.ToString();
            TextBox_ErrorOuput.Text += interpreter.errorMessage;
            isCalculating = false;
        }

        public void WorkDone(double result)
        {
            this.Dispatcher.Invoke(new Action(() => { FinishCalc(result); }));
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            interpreter.StopCalc();
            isCalculating = false;
        }
    }
}
