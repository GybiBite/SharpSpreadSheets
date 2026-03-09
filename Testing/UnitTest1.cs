using SharpSpreadSheets.Logic; // Adjust if your namespace is different
using SharpSpreadSheets.Model.Tokens;
using Xunit;

namespace Testing
{
    public class ParserTests
    {
        [Fact]
        public void GetFormula_SimpleInfix_ReturnsCorrectPostfixStack()
        {
            // Arrange
            //Util util = new Util();
            string formula = "5 + 10 * 2";

            // Act
            // Note: In C#, the built-in Stack<T> returns items in LIFO order. 
            // The "bottom" of the stack in the handout corresponds to the last items 
            // you would see if you converted the stack to a list.
            Stack<IToken> resultStack = Util.GetFormula(formula);

            // Convert to an array for easy indexing: [Top -> Bottom]
            // Postfix: 5 10 2 * + 
            // Stack (Top to Bottom): +, *, 2, 10, 5
            IToken[] tokens = resultStack.ToArray();

            // Assert
            Assert.Equal(5, tokens.Length);

            // Check the operators (The top of the stack)
            Assert.IsType<OperatorToken>(tokens[0]);
            Assert.Equal('+', ((OperatorToken)tokens[0]).GetOperatorToken());

            Assert.IsType<OperatorToken>(tokens[1]);
            Assert.Equal('*', ((OperatorToken)tokens[1]).GetOperatorToken());

            // Check the literals
            Assert.Equal(2, ((LiteralToken)tokens[2]).GetValue());
            Assert.Equal(10, ((LiteralToken)tokens[3]).GetValue());
            Assert.Equal(5, ((LiteralToken)tokens[4]).GetValue());
        }
    }
}