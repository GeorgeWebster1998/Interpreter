using System;
using System.Collections;
using System.Collections.Generic;
using static LookupTable;

namespace InterpreterCore
{

	public class Executor
	{
		LookupTable lt;
		Stack Operators;
		Stack Numbers;
		double operand1, operand2;
		Tokens operatorID;

		public Executor(ref LookupTable lt)
		{
			this.lt = lt;
			Operators = new Stack();
			Numbers = new Stack();
			operand1 = operand2 = 0;
			operatorID = Tokens.EMPTY;
		}

		public void Calculate()
		{
			(string, double) op2 = ("", 0);
			(string, double) op1 = ("", 0);

			operatorID = (Tokens)Operators.Pop();

			if (Numbers.Peek() is string)
			{
				string var = (string)Numbers.Pop();
				op2 = ((string)var, lt.GetVarValue((string)var));
				//op2IsVar = true;
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

		public Object ShuntYard()
		{
			int count = 0;

			while (count < lt.symbols.Length)
			{
				switch (lt.GetSymbol(count).Type)
				{
					case Tokens.Integer:
						Numbers.Push(lt.GetSymbol(count++).Value);
						break;

					case Tokens.Float:
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
							&& (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.GetSymbol(count++).Type);
						break;

					case LookupTable.Tokens.Divide:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para
							&& (Tokens)Operators.Peek() != Tokens.Plus && (Tokens)Operators.Peek() != Tokens.Minus
							&& (Tokens)Operators.Peek() != Tokens.Equal)
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
			while (Operators.Count > 0)
			{
				Calculate();
			}


			if (Numbers.Count == 1)
			{
				return Convert.ToDouble(Numbers.Pop());
			}
			else
			{
				return "Variable assignment";
			}
		}


	}
}
