using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using NCalc;
using OxyPlot;
using OxyPlot.Series;
using System.Drawing;


namespace PKGRAPH.Models
{
    public class ExpressionModel
    {
        private const char XChar = 'x';
        public const string IncorrectExpressionMessage = "Incorrect expression";

        private Range _plotRange = new Range(-500, 500, 50000);        

        public bool IsValidated = true;
        public string ExpressionString { get; set; }
        public string ExpressionString2 { get; set; }
        public string FullExpressionString => "y=" + ExpressionString;

        public double[] XValues { get; private set; }
        public double[] YValues { get; private set; }
        public LineSeries Lines;

        public event Action<ExpressionModel, bool> OnValidationCheck;
        public string Error => string.Empty;


        public ExpressionModel(string expressionStr, Color Color)
        {
            try
            {
                ExpressionString = expressionStr;
                CorrectExpressionIfNeed();
                Lines = new LineSeries();
                Lines.Color = new OxyColor();

                Lines.Color = OxyPlot.OxyColor.FromArgb(Color.A, Color.R, Color.G, Color.B);
                XValues = _plotRange.Generate().ToArray();


                for (int i = 0; i < XValues.Length; i++)
                {
                    double? result = GetY(XValues[i]);

                    if (result == null)
                    {
                        FireOnValidationCheck(false);
                    }
                    else
                        Lines.Points.Add(new DataPoint(XValues[i], (double)result));
                    //YValues[i] = Math.Clamp(result.Value, _plotRange.Min, _plotRange.Max);
                }
            }
            catch
            {
                FireOnValidationCheck(false);
            }
        }

        private void CorrectExpressionIfNeed()
        {
            ExpressionString2 = ExpressionString;
            ExpressionString2 = ExpressionString2.Replace("E", "e");
            ExpressionString2 = ExpressionString2.Replace("PI", "p");
            for (int i = 1; i < ExpressionString2.Length; i++)
            {
                if ((ExpressionString2[i] == XChar || ExpressionString2[i] == 'e' || ExpressionString2[i] == 'p') && IsAsciiDigitOrX(ExpressionString2[i - 1]))
                {
                    ExpressionString2 = ExpressionString2.Insert(i, "*");
                }

                if ((ExpressionString2[i] == XChar || ExpressionString2[i] == 'e' || ExpressionString2[i] == 'p') && i < ExpressionString2.Length - 1
                                                 && IsAsciiDigitOrX(ExpressionString2[Math.Min(i + 1, ExpressionString2.Length - 1)]))
                {
                    ExpressionString2 = ExpressionString2.Insert(i + 1, "*");
                }

                if ((ExpressionString2[i] == XChar || ExpressionString2[i] == 'e' || ExpressionString2[i] == 'p') && ExpressionString2[i - 1] == '-')
                {
                    ExpressionString2 = ExpressionString2.Insert(i, "1*");
                }

            }
            ExpressionString2 = ExpressionString2.Replace("e", Math.E.ToString(CultureInfo.InvariantCulture));
            ExpressionString2 = ExpressionString2.Replace("p", Math.PI.ToString(CultureInfo.InvariantCulture));
        }


        private static bool IsAsciiDigitOrX(char symbol)
        {
            return symbol == XChar || symbol == 'e' || symbol == 'p' || char.IsDigit(symbol);
        }

        private void FireOnValidationCheck(bool validationResult)
        {
            IsValidated = validationResult;
            OnValidationCheck?.Invoke(this, validationResult);
        }

        private double? GetY(double x)
        {
            string expressionString = ExpressionString2;

            if (string.IsNullOrEmpty(expressionString))
            {
                return null;
            }



            for (int i = 0; i < expressionString.Length; i++)
            {
                if (expressionString[i] == XChar)
                {
                    expressionString = expressionString
                        .Remove(i, 1)
                        .Insert(i, x.ToString(CultureInfo.InvariantCulture));
                }
            }

            Expression expression = new Expression(expressionString);

            if (expression.HasErrors())
            {
                return null;
            }

            return Convert.ToDouble(expression.Evaluate());
        }
    }
}
