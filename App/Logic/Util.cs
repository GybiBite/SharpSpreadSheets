using System;
using System.Collections.Generic;
// using System.Collections.Generic.Stack<T>;
using SharpSpreadSheets.Model.Tokens;

namespace SharpSpreadSheets.Logic
{

    /*
     * Utility functions.
     * You should place these methods in the appropriate classes.
     * @author Donald Chinn
     */
    public static class Util
    {
        // Assuming these constants are defined elsewhere in your project, 
        // but added here to provide context for the translation.
        public const char Plus = '+';
        public const char Minus = '-';
        public const char Mult = '*';
        public const char Div = '/';
        public const char LeftParen = '(';
        public const int BadCell = -1;

        /**
         * Return a string associated with a token
         * @param expTreeToken  an ExpressionTreeToken
         * @return a String associated with expTreeToken
         */
        public static string PrintExpressionTreeToken(IToken expTreeToken)
        {
            string returnString = "";

            if (expTreeToken is OperatorToken)
            {
                returnString = ((OperatorToken)expTreeToken).getOperatorToken() + " ";
            }
            else if (expTreeToken is CellToken)
            {
                returnString = PrintCellToken((CellToken)expTreeToken) + " ";
            }
            else if (expTreeToken is LiteralToken)
            {
                returnString = ((LiteralToken)expTreeToken).getValue() + " ";
            }
            else
            {
                // This case should NEVER happen
                Console.WriteLine("Error in printExpressionTreeToken.");
                Environment.Exit(0);
            }
            return returnString;
        }

        /**
         * Return true if the char ch is an operator of a formula.
         * Current operators are: +, -, *, /, (.
         * @param ch  a char
         * @return  whether ch is an operator
         */
        public static bool IsOperator(char ch)
        {
            return ch == Plus ||
                    ch == Minus ||
                    ch == Mult ||
                    ch == Div ||
                    ch == LeftParen;
        }

        /**
         * Given an operator, return its priority.
         *
         * priorities:
         * +, - : 0
         * *, / : 1
         * (    : 2
         *
         * @param ch  a char
         * @return  the priority of the operator
         */
        public static int OperatorPriority(char ch)
        {
            if (!IsOperator(ch))
            {
                // This case should NEVER happen
                Console.WriteLine("Error in operatorPriority.");
                Environment.Exit(0);
            }
            switch (ch)
            {
                case Plus:
                    return 0;
                case Minus:
                    return 0;
                case Mult:
                    return 1;
                case Div:
                    return 1;
                case LeftParen:
                    return 2;

                default:
                    // This case should NEVER happen
                    Console.WriteLine("Error in operatorPriority.");
                    Environment.Exit(0);
                    return -1; // C# requires a return path here
            }
        }

        /*
         * Return the priority of this OperatorToken.
         * Note: In the original Java, this referenced 'this.operatorToken', 
         * implying it belongs inside the OperatorToken class itself.
         *
         * priorities:
         * +, - : 0
         * *, / : 1
         * (    : 2
         *
         * @return  the priority of operatorToken
         */
        public static int Priority(char operatorToken)
        {
            switch (operatorToken)
            {
                case Plus:
                    return 0;
                case Minus:
                    return 0;
                case Mult:
                    return 1;
                case Div:
                    return 1;
                case LeftParen:
                    return 2;

                default:
                    // This case should NEVER happen
                    Console.WriteLine("Error in priority.");
                    Environment.Exit(0);
                    return -1;
            }
        }


        /**
         * getCellToken
         * * @param inputString  the input string
         * @param startIndex  the index of the first char to process
         * @param cellToken  a cellToken (essentially a return value)
         * @return  index corresponding to the position in the string just after the cell reference
         */
        public static int GetCellToken(string inputString, int startIndex, CellToken cellToken)
        {
            char ch;
            int column = 0;
            int row = 0;
            int index = startIndex;

            // handle a bad startIndex
            if (startIndex < 0 || startIndex >= inputString.Length)
            {
                cellToken.setColumn(BadCell);
                cellToken.setRow(BadCell);
                return index;
            }

            // get rid of leading whitespace characters
            while (index < inputString.Length)
            {
                ch = inputString[index];
                if (!char.IsWhiteSpace(ch))
                {
                    break;
                }
                index++;
            }
            // Fix from original Java code: inputString.length missing ()
            if (index == inputString.Length)
            {
                // reached the end of the string before finding a capital letter
                cellToken.setColumn(BadCell);
                cellToken.setRow(BadCell);
                return index;
            }

            // ASSERT: index now points to the first non-whitespace character

            ch = inputString[index];
            // process CAPITAL alphabetic characters to calculate the column
            if (!char.IsUpper(ch))
            {
                cellToken.setColumn(BadCell);
                cellToken.setRow(BadCell);
                return index;
            }
            else
            {
                column = ch - 'A';
                index++;
            }

            while (index < inputString.Length)
            {
                ch = inputString[index];
                if (char.IsUpper(ch))
                {
                    column = (column + 1) * 26 + (ch - 'A');
                    index++;
                }
                else
                {
                    break;
                }
            }
            if (index == inputString.Length)
            {
                // reached the end of the string before fully parsing the cell reference
                cellToken.setColumn(BadCell);
                cellToken.setRow(BadCell);
                return index;
            }

            // ASSERT: We have processed leading whitespace and the
            // capital letters of the cell reference

            // read numeric characters to calculate the row
            if (char.IsAsciiDigit(ch)) // Or char.IsDigit(ch)
            {
                row = ch - '0';
                index++;
            }
            else
            {
                cellToken.setColumn(BadCell);
                cellToken.setRow(BadCell);
                return index;
            }

            while (index < inputString.Length)
            {
                ch = inputString[index];
                if (char.IsAsciiDigit(ch))
                {
                    row = row * 10 + (ch - '0');
                    index++;
                }
                else
                {
                    break;
                }
            }

            // successfully parsed a cell reference
            cellToken.setColumn(column);
            cellToken.setRow(row);
            return index;
        }

        /**
         * Given a CellToken, print it out as it appears on the
         * spreadsheet (e.g., "A3")
         * @param cellToken  a CellToken
         * @return  the cellToken's coordinates
         */
        public static string PrintCellToken(CellToken cellToken)
        {
            char ch;
            string returnString = "";
            int col;
            int largest = 26;  // minimum col number with number_of_digits digits
            int number_of_digits = 2;

            col = cellToken.getColumn();

            // compute the biggest power of 26 that is less than or equal to col
            // We don't check for overflow of largest here.
            while (largest <= col)
            {
                largest = largest * 26;
                number_of_digits++;
            }
            largest = largest / 26;
            number_of_digits--;

            // append the column label, one character at a time
            while (number_of_digits > 1)
            {
                ch = (char)(col / largest - 1 + 'A');
                returnString += ch;
                col = col % largest;
                largest = largest / 26;
                number_of_digits--;
            }

            // handle last digit
            ch = (char)(col + 'A');
            returnString += ch;

            // append the row as an integer
            returnString += cellToken.getRow();

            return returnString;
        }

        /**
         * getFormula
         * * Given a string that represents a formula that is an infix
         * expression, return a stack of Tokens so that the expression,
         * when read from the bottom of the stack to the top of the stack,
         * is a postfix expression.
         */
        public static Stack<IToken> GetFormula(string formula)
        {
            Stack<IToken> returnStack = new Stack<IToken>();  // stack of Tokens (representing a postfix expression)
            bool error = false;
            char ch = ' ';

            int literalValue = 0;

            int index = 0;  // index into formula
            Stack<IToken> operatorStack = new Stack<IToken>();  // stack of operators

            while (index < formula.Length)
            {
                // get rid of leading whitespace characters
                while (index < formula.Length)
                {
                    ch = formula[index];
                    if (!char.IsWhiteSpace(ch))
                    {
                        break;
                    }
                    index++;
                }

                if (index == formula.Length)
                {
                    error = true;
                    break;
                }

                // ASSERT: ch now contains the first character of the next token.
                if (IsOperator(ch))
                {
                    // We found an operator token
                    switch (ch)
                    {
                        case Plus:
                        case Minus:
                        case Mult:
                        case Div:
                        case LeftParen:
                            // push operatorTokens onto the output stack until
                            // we reach an operator on the operator stack that has
                            // lower priority than the current one.
                            OperatorToken stackOperator;
                            while (operatorStack.Count > 0)
                            {
                                stackOperator = (OperatorToken)operatorStack.Peek();
                                if (stackOperator.priority() >= OperatorPriority(ch) &&
                                    stackOperator.getOperatorToken() != LeftParen)
                                {
                                    // output the operator to the return stack    
                                    operatorStack.Pop();
                                    returnStack.Push(stackOperator);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;

                        default:
                            // This case should NEVER happen
                            Console.WriteLine("Error in getFormula.");
                            Environment.Exit(0);
                            break;
                    }
                    // push the operator on the operator stack
                    operatorStack.Push(new OperatorToken(ch));

                    index++;

                }
                else if (ch == ')')
                {    // maybe define OperatorToken.RightParen ?
                    OperatorToken stackOperator;
                    stackOperator = (OperatorToken)operatorStack.Pop();
                    // This code does not handle operatorStack underflow.
                    while (stackOperator.getOperatorToken() != LeftParen)
                    {
                        // pop operators off the stack until a LeftParen appears and
                        // place the operators on the output stack
                        returnStack.Push(stackOperator);
                        stackOperator = (OperatorToken)operatorStack.Pop();
                    }

                    index++;
                }
                else if (char.IsAsciiDigit(ch))
                {
                    // We found a literal token
                    literalValue = ch - '0';
                    index++;
                    while (index < formula.Length)
                    {
                        ch = formula[index];
                        if (char.IsAsciiDigit(ch))
                        {
                            literalValue = literalValue * 10 + (ch - '0');
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    // place the literal on the output stack
                    returnStack.Push(new LiteralToken(literalValue));

                }
                else if (char.IsUpper(ch))
                {
                    // We found a cell reference token
                    CellToken cellToken = new CellToken();
                    index = GetCellToken(formula, index, cellToken);
                    if (cellToken.getRow() == BadCell)
                    {
                        error = true;
                        break;
                    }
                    else
                    {
                        // place the cell reference on the output stack
                        returnStack.Push(cellToken);
                    }

                }
                else
                {
                    error = true;
                    break;
                }
            }

            // pop all remaining operators off the operator stack
            while (operatorStack.Count > 0)
            {
                returnStack.Push(operatorStack.Pop());
            }

            if (error)
            {
                // a parse error; return the empty stack
                returnStack.Clear();
            }

            return returnStack;
        }
    }
}