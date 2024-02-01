using System.Drawing;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Series;

namespace PKGRAPH.Models
{

    public class GraphModel
    {
        public string Expression { get; private set; }
        public LineSeries Lines { get; private set; }


        [JsonIgnore] public string FullExpression => "y = " + Expression;

        public GraphModel(string expression,LineSeries lines  )
        {
            Expression = expression;   
            Lines = lines;
        }

        public override string ToString()
        {
            return FullExpression;
        }
    }
}