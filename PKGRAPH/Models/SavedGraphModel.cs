using System.Drawing;
using Newtonsoft.Json;

namespace PKGRAPH.Models
{

    public class SavedGraphModel
    {
        public string Expression { get; private set; }
        public Color Color { get; private set; }


        [JsonIgnore] public string FullExpression => "y=" + Expression;

        public SavedGraphModel(string expression,Color color  )
        {
            Expression = expression;   
            Color = color;
        }

        public override string ToString()
        {
            return FullExpression;
        }
    }
}