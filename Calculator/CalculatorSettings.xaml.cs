using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    /// <summary>
    /// CalculatorSettings.xaml 的交互逻辑
    /// </summary>
    public partial class CalculatorSettings : Window
    {
        private Interpreter interpreter;
        private double intPrecision;
        private double diffDelta;
        public CalculatorSettings(Interpreter _interpreter)
        {
            InitializeComponent();
            interpreter = _interpreter;
            Slider_Integral.Value = Math.Log10(interpreter.intPrecision / Settings.IntegralPrecision) + 3;
            Slider_Diff.Value = Math.Log10(interpreter.diffDelta / Settings.DiffDelta) + 5;
            TextBox_Integral.Text = (Settings.IntegralPrecision * Math.Pow(10, Slider_Integral.Value - 3)).ToString("f0");
            TextBox_Diff.Text = (Settings.DiffDelta * Math.Pow(10, Slider_Diff.Value - 5)).ToString("f15");
        }

        private bool ignoreSliderValueChange = false;
        private void Slider_Integral_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ignoreSliderValueChange)
            {
                return;
            }
            intPrecision = Settings.IntegralPrecision * Math.Pow(10, Slider_Integral.Value - 3);
            ignoreTextChange = true;
            TextBox_Integral.Text = intPrecision.ToString("f0");
            ignoreTextChange = false;
        }

        private void Slider_Diff_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ignoreSliderValueChange)
            {
                return;
            }
            diffDelta = Settings.DiffDelta * Math.Pow(10, Slider_Diff.Value - 5);
            ignoreTextChange = true;
            TextBox_Diff.Text = diffDelta.ToString("f15");
            ignoreTextChange = false;
        }

        private bool ignoreTextChange = false;

        private void TextBox_Integral_KeyDown(object sender, KeyEventArgs e)
        {
            if(ignoreTextChange)
            {
                return;
            }
            if(e.Key == Key.Enter)
            {
                try
                {
                    double i = double.Parse(TextBox_Integral.Text);
                    ignoreSliderValueChange = true;
                    Slider_Integral.Value = Math.Log10(i / Settings.IntegralPrecision) + 3;
                    ignoreSliderValueChange = false;
                    intPrecision = Settings.IntegralPrecision * Math.Pow(10, Slider_Integral.Value - 3);
                    TextBox_Integral.Text = intPrecision.ToString("f0");
                }
                catch
                { }
            }
        }

        private void TextBox_Diff_KeyDown(object sender, KeyEventArgs e)
        {
            if (ignoreTextChange)
            {
                return;
            }
            if (e.Key == Key.Enter)
            {
                try
                {
                    double i = double.Parse(TextBox_Diff.Text);
                    ignoreSliderValueChange = true;
                    Slider_Diff.Value = Math.Log10(i / Settings.DiffDelta) + 5;
                    ignoreSliderValueChange = false;
                    diffDelta = Settings.DiffDelta * Math.Pow(10, Slider_Diff.Value - 5);
                    TextBox_Diff.Text = diffDelta.ToString("f15");
                }
                catch
                { }
            }
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            interpreter.intPrecision = intPrecision;
            interpreter.diffDelta = diffDelta;
            this.Hide();
        }

        private void Button_Discard_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
