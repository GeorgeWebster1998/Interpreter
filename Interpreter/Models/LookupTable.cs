using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		Double = 10,
		Variable = 11,
		Pi = 12,
		Tan = 13 

	}

	public Symbol[] symbols;
	public Dictionary<String, object> variables;
	//public const 
	public ParsedTrie pt;
	public int MAX_TOKENS;

	public LookupTable(int MAX_TOKENS)
	{
		this.MAX_TOKENS = MAX_TOKENS;
		symbols = new Symbol[MAX_TOKENS];
		variables = new Dictionary<string, object>();
	}

	public void InitSymbols(int MAX_TOKENS)
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
		variables[key] = value;
	}

	public double GetVarValue(string key)
	{
		return Convert.ToDouble(variables[key]);
	}

	public void AddToVariables(string key, object value)
	{
		variables.Add(key, value);
	}

	public void ClearVariables()
	{
		variables.Clear();
	}

	public bool VariableExist(string key)
	{
		return variables.ContainsKey(key);
	}

	public void SetParsedTrie(ParsedTrie pt)
	{
		this.pt = pt;
	}

	public struct Symbol
	{
		public Symbol(Tokens type, object value)
		{
			this.Type = type;
			this.Value = value;
		}

		public Tokens Type { get; }
		public object Value { get; }

		public override string ToString()
		{
			return this.Type.ToString();
		}
	}
}