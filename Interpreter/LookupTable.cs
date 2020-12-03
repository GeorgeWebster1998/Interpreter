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

	public void AddSymbol(int index, Tokens type, Object value)
	{
		symbols[index] = new Symbol(type, value);
	}

	public void ResetSymbols(int len)
	{
		symbols = new Symbol[len];
	}

	public Symbol GetSymbol(int index)
	{
		return symbols[index];
	}

	public void UpdateVariable(string key, double value)
	{
		variables[key] = new Var(false, value);
	}

	public double GetVarValue(string key)
	{
		return (double)variables[key].Value;
	}

	public void AddToVariables(string key, Var value)
	{
		variables.TryAdd(key, value);
	}

	public void ClearVariables()
	{
		variables.Clear();
	}

	public bool VariableExist(string key)
	{
		return variables.ContainsKey(key);
	}

	public struct Symbol
	{
		public Symbol(Tokens type, Object value)
		{
			this.Type = type;
			this.Value = value;
		}

		public Tokens Type { get; }
		public Object Value { get; }
	}
	public struct Var
	{
		public Var(bool isPtr, Object value)
		{
			this.IsPtr = isPtr;
			this.Value = value;
		}
		public bool IsPtr { get; set; }
		public Object Value { get; set; }

		public override string ToString() { return IsPtr.ToString() + Value.ToString(); }
	}

}