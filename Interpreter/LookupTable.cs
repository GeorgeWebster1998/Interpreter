using System;
using System.Collections.Generic;
using System.Text;

public class LookupTable
{
	public enum Tokens : int
	{
		EMPTY = -1,
		Plus = 0,
		Minus = 1,
		Multiply = 2,
		Divide = 3,
		Exponent = 4,
		Equal = 5,
		Left_Para = 7,
		Right_Para = 8,
		Integer = 9,
		Float = 10,
		Variable = 11
	}

	public Symbol[] symbols;
	public Dictionary<String, Var> variables = new Dictionary<string, Var>();

	public LookupTable(int MAX_TOKENS)
	{
		symbols = new Symbol[MAX_TOKENS];
	}

	public void addSymbol(int index, Tokens type, Object value)
	{
		symbols[index] = new Symbol(type, value);
	}

	public void resetSymbols(int len)
	{
		symbols = new Symbol[len];
	}

	public Symbol getSymbol(int index)
	{
		return symbols[index];
	}

	public void updateVariable(string key, double value)
	{
		variables[key] = new Var(false, value);
	}

	public double getVarValue(string key)
	{
		return (double)variables[key].value;
	}

	public void addToVariables(string key, Var value)
	{
		variables.TryAdd(key, value);
	}

	public void clearVariables()
	{
		variables.Clear();
	}

	public bool variableExist(string key)
	{
		return variables.ContainsKey(key);
	}

	public struct Symbol
	{
		public Symbol(Tokens type, Object value)
		{
			this.type = type;
			this.value = value;
		}

		public Tokens type { get; }
		public Object value { get; }
	}
	public struct Var
	{
		public Var(bool isPtr, Object value)
		{
			this.isPtr = isPtr;
			this.value = value;
		}
		public bool isPtr { get; set; }
		public Object value { get; set; }

		public override string ToString() { return isPtr.ToString() + value.ToString(); }
	}

}