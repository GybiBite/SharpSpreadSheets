using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSpreadSheets.Model;
using SharpSpreadSheets.Model.Tokens;
using Xunit;

namespace Testing
{
    public class ExpressionTreeNodeTests
    {
        // Test that a node is created correctly with just a token
        [Fact]
        public void ExpressionTreeNode_Constructor_TokenOnly_LeftAndRightAreNull()
        {
            LiteralToken token = new LiteralToken(5);
            ExpressionTreeNode node = new ExpressionTreeNode(token);

            Assert.Equal(token, node.Token);
            Assert.Null(node.Left);
            Assert.Null(node.Right);
        }

        // Test that a node is created correctly with left and right children
        [Fact]
        public void ExpressionTreeNode_Constructor_WithChildren_SetsCorrectly()
        {
            LiteralToken leftToken = new LiteralToken(3);
            LiteralToken rightToken = new LiteralToken(4);
            LiteralToken opToken = new LiteralToken('+');

            ExpressionTreeNode left = new ExpressionTreeNode(leftToken);
            ExpressionTreeNode right = new ExpressionTreeNode(rightToken);
            ExpressionTreeNode node = new ExpressionTreeNode(opToken, left, right);

            Assert.Equal(opToken, node.Token);
            Assert.Equal(left, node.Left);
            Assert.Equal(right, node.Right);
        }

        // Test that a node's token can be updated
        [Fact]
        public void ExpressionTreeNode_SetToken_UpdatesCorrectly()
        {
            LiteralToken token = new LiteralToken(5);
            ExpressionTreeNode node = new ExpressionTreeNode(token);

            LiteralToken newToken = new LiteralToken(10);
            node.Token = newToken;

            Assert.Equal(newToken, node.Token);
        }

        // Test that a leaf node has no children
        [Fact]
        public void ExpressionTreeNode_LeafNode_HasNoChildren()
        {
            LiteralToken token = new LiteralToken(42);
            ExpressionTreeNode node = new ExpressionTreeNode(token);

            Assert.Null(node.Left);
            Assert.Null(node.Right);
        }
    }
}
