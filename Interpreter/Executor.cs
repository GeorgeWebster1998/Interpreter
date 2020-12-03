using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
				op2 = ((string)var, lt.getVarValue((string)var));
				//op2IsVar = true;
				operand2 = lt.getVarValue((string)var);
			}
			else
			{
				operand2 = Convert.ToDouble(Numbers.Pop());
			}
			if (Numbers.Peek() is string)
			{
				string var = (string)Numbers.Pop();
				op1 = ((string)var, lt.getVarValue((string)var));
				operand1 = lt.getVarValue((string)var);
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
					lt.updateVariable(key: op1.Item1, operand2);
					break;
			}
		}

		public Object ShuntYard()
		{
			int count = 0;

			while (count < lt.symbols.Length)
			{
				switch (lt.getSymbol(count).type)
				{
					case Tokens.Integer:
						Numbers.Push(lt.getSymbol(count++).value);
						break;

					case Tokens.Float:
						Numbers.Push(lt.getSymbol(count++).value);
						break;

					case Tokens.Variable:
						Numbers.Push(lt.getSymbol(count++).value);
						break;

					case Tokens.Equal:
						Operators.Push(lt.getSymbol(count++).type);
						break;

					case Tokens.Plus:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para && (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.getSymbol(count++).type);
						break;

					case LookupTable.Tokens.Minus:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para && (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.getSymbol(count++).type);
						break;

					case LookupTable.Tokens.Exponent:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para
							&& (Tokens)Operators.Peek() != Tokens.Plus && (Tokens)Operators.Peek() != Tokens.Minus
							&& (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.getSymbol(count++).type);
						break;


					case LookupTable.Tokens.Multiply:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para
							&& (Tokens)Operators.Peek() != Tokens.Plus && (Tokens)Operators.Peek() != Tokens.Minus
							&& (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.getSymbol(count++).type);
						break;

					case LookupTable.Tokens.Divide:
						while (Operators.Count > 0 && (Tokens)Operators.Peek() != Tokens.Left_Para
							&& (Tokens)Operators.Peek() != Tokens.Plus && (Tokens)Operators.Peek() != Tokens.Minus
							&& (Tokens)Operators.Peek() != Tokens.Equal)
						{
							Calculate();
						}
						Operators.Push(lt.getSymbol(count++).type);
						break;

					case LookupTable.Tokens.Left_Para:
						Operators.Push(lt.getSymbol(count++).type);
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
				var list = lt.variables.ToList();

				foreach (KeyValuePair<string, LookupTable.Var> temp in list)
				{
					Console.WriteLine("{0} -> {1}", temp.Key, temp.Value.value);
				}

				return "Variable assignment";
			}
		}


	}
}
