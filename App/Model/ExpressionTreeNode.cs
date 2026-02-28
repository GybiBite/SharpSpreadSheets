namespace SharpSpreadSheets.Model
{
    using SharpSpreadSheets.Model.Tokens;

    public class ExpressionTreeNode
    {
        public IToken Token { get; set; }
        public ExpressionTreeNode Left { get; set; }
        public ExpressionTreeNode Right { get; set; }

        public ExpressionTreeNode(IToken token, ExpressionTreeNode left = null, ExpressionTreeNode right = null)
        {
            Token = token;
            Left = left;
            Right = right;
        }
    }
}