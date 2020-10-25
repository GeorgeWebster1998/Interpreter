using System;

public class LookupTable
{
	/*
	 * This class is used for storing tokens
	 * and symbols so that both the lexer and
	 * parser can access them.
	 */

	public enum Tokens : int
	{
		EMPTY = -1,
		Plus = 0,
		Minus = 1,
		Multiply = 2,
		Divide = 3,
		Exponent = 4,
		Left_Para = 7,
		Right_Para = 8,
		Number = 9,
		Varaible = 10
	}
	//DICTIONARY

	public Tokens[] tokens; 
	public int[] symbols;
	
	public LookupTable(int MAX_TOKENS)
	{
		tokens = new Tokens[MAX_TOKENS];
		symbols = new int[MAX_TOKENS];

		for (int i = 0; i < MAX_TOKENS; i++)
		{
			symbols[i] = -1; //This is kept at -1 for symbols and used when storing numbers.
		}
	}


}
