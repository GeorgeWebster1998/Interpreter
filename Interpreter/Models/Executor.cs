using System;
using System.Collections;
using static Interpreter.Models.LookupTable;

namespace Interpreter.Models
{

	//This class executes the expression using a modified version of the Shunting
	//Yard algorithm by Rudy Lapeer. 
	public class Executor
	{
		LookupTable lt;
		Stack Operators;
		Stack Numbers;
		double operand1, operand2;
		Tokens operatorID;

		//Inits a fresh executor
		public Executor(ref LookupTable lt)
		{
			this.lt = lt;
			Operators = new Stack();
			Numbers = new Stack();
			operand1 = operand2 = 0;
			operatorID = Tokens.EMPTY;
		}

		/// <summary>
		/// This function calculates the result of an operation by taking
		/// two numbers of the numbers stack and an operater of the stack.
		/// The result of the caclulation is then pushed back into the 
		/// numbers stack as a running total.
		/// </summary>
		public void Calculate()
		{
			//operand1 only has a tuple as only an = operation 
			//requires the left operator to be set
			(string, double) op1 = ("", 0);

			operatorID = (Tokens)Operators.Pop();

			//This section checks to see if the object in the numbers stack is a string
			//this will mean it is a variable and sets the op2 variable as as so.
			if (Numbers.Peek() is string)
			{
				string var = (string)Numbers.Pop();
				operand2 = lt.GetVarValue((string)var);
			}
			else
			{
				operand2 = Convert.ToDouble(Numbers.Pop());
			}
			if (Numbers.Peek() is string)
			{
				string var = (string)Numbers.Pop();
				op1 = ((string)var, lt.GetVarValue((string)var));
				operand1 = lt.GetVarValue((string)var);
			}
			else
			{
				operand1 = Convert.ToDouble(Numbers.Pop());
			}


			//This section is just a switch case to decide which operation
			//should be performed to the two operands.
			double result = 0;
			switch (operatorID)
			{
				case Tokens.Plus:
					result = operand1 + operand2;
					Numbers.Push(result);
					break;

				case Tokens.Minus:
					result = operand1 - operand2;
					Numbers.Push(result);
					break;

				case Tokens.Exponent:
					result = Math.Pow(operand1, operand2);
					Numbers.Push(result);
					break;

				case Tokens.Multiply:
					result = operand1 * operand2;
					Numbers.Push(result);
					break;

				case Tokens.Divide:
					result = operand1 / operand2;
					Numbers.Push(result);
					break;

				case Tokens.Equal:
					lt.UpdateVariable(key: op1.Item1, operand2);
					break;
			}
		}

		/// <summary>
		/// This is the modified Shunting Yard algorithm. First it reads the symbols/tokens
		/// and pushes them to the correct stack. It will then call calulate on certain operators,
		/// this follows order of presidence/BIDMAS
		/// </summary>
		/// <returns></returns>
		public double ShuntYard()
		{
			int count = 0;

			while (count < lt.symbols.Length)
			{
				switch (lt.GetSymbol(count).Type)
				{
					case Tokens.Integer:
						Numbers.Push(lt.GetSymbol(count++).Value);
						break;

					case Tokens.Double:
						Numbers.Push(lt.GetSymbol(count++).Value);
						break;

					case Tokens.Variable:
						Numbers.Push(lt.GetSymbol(count++).Value);
						break;

					case Tokens.Equal:
						Operators.Push(lt.GetSymbol(count++).Type);
						break;

					case Tokens.Plus:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para && (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.GetSymbol(count++).Type);
						break;

					case LookupTable.Tokens.Minus:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para && (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.GetSymbol(count++).Type);
						break;

					case LookupTable.Tokens.Exponent:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para
							&& (Tokens)Operators.Peek() != Tokens.Plus && (Tokens)Operators.Peek() != Tokens.Minus
							&& (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.GetSymbol(count++).Type);
						break;


					case LookupTable.Tokens.Multiply:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para
							&& (Tokens)Operators.Peek() != Tokens.Plus && (Tokens)Operators.Peek() != Tokens.Minus
							&& (Tokens)Operators.Peek() != Tokens.Equal && (Tokens)Operators.Peek() != Tokens.Exponent)
						{
							Calculate();
						}
						Operators.Push(lt.GetSymbol(count++).Type);
						break;

					case LookupTable.Tokens.Divide:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para
							&& (Tokens)Operators.Peek() != Tokens.Plus && (Tokens)Operators.Peek() != Tokens.Minus
							&& (Tokens)Operators.Peek() != Tokens.Equal && (Tokens)Operators.Peek() != Tokens.Exponent)
						{
							Calculate();
						}
						Operators.Push(lt.GetSymbol(count++).Type);
						break;

					case LookupTable.Tokens.Left_Para:
						Operators.Push(lt.GetSymbol(count++).Type);
						break;

					case LookupTable.Tokens.Right_Para:
						while (Operators.Count > 0 && (LookupTable.Tokens)Operators.Peek() != LookupTable.Tokens.Left_Para)
						{
							Calculate();
						}
						if ((LookupTable.Tokens)Operators.Peek() == LookupTable.Tokens.Left_Para)
						{
							Operators.Pop();
						}
						count++;
						break;

					default:
						count++;
						break;
				}
			}
			//This is a fail safe to make sure that all operators are used.
			while (Operators.Count > 0)
			{
				Calculate();
			}

			//This if exists as an assignment of a variable will not return back
			//to the numbers stack. 
			if (Numbers.Count == 1)
			{
				return Convert.ToDouble(Numbers.Pop());
			}
			else
			{
				return 0;
			}
		}
	}
}