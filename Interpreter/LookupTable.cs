using System;

public class LookupTable
{
	/*
	 * This class is used for storing tokens
	 * and symbols so that both the lexer and
	 * parser can access them.
	 */

	public int[] tokens; 
	public int[] symbols;
	
	public LookupTable(int MAX_TOKENS)
	{
		tokens = new int[MAX_TOKENS];
		symbols = new int[MAX_TOKENS];

		for (int ltCount = 0; ltCount < MAX_TOKENS; ltCount++)
		{
			tokens[ltCount] = 0; //This is set to zero for all entries as it will be used later.
			symbols[ltCount] = -1; //This is kept at -1 for symbols and used when storing numbers.
		}
	}
}
