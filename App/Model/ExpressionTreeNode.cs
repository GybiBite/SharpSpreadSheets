using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets.Model
{
    public class ExpressionTreeNode(IToken token, ExpressionTreeNode? left = null, ExpressionTreeNode? right = null)
    {
        public IToken Token { get; set; } = token;

        public ExpressionTreeNode? Left { get; set; } = left;

        public ExpressionTreeNode? Right { get; set; } = right;
    }
}