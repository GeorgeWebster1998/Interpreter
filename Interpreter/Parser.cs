using System;

public class Parser
{
	int LookAhead;
	int currentToken;
	bool ret;
	LookupTable lt;
	ParsedTrie trie; 

	public Parser(LookupTable lt)
	{
		this.LookAhead = -1;
		this.currentToken = 0;
		this.ret = true;
		this.lt = lt;
		trie = new ParsedTrie();
	}

	/*	BNF
	 *	<expression> ::= <term> <expression_prime>
	 *	<expression_prime> ::= + <term> <expression_prime>
	 *	<expression_prime> ::= - <term> <expression_prime>
	 *	<term> ::= <factor> <term_prime>
	 *	<term_prime> ::= ^ <factor> <term_prime>
	 *	<term_prime> ::= * <factor> <term_prime>
	 *	<term_prime> := / <factor> <term_prime>
	 *	<factor> ::= number | (<expr>)
	 */

	public bool Parse()
	{
		/*Check that lt is parsed correctly 
		*
		*for (int i = 0; i < this.lt.tokens.Length; i++)
		*{
		*	if (lt.tokens[i] != LookupTable.Tokens.EMPTY)
		*	{
		*		Console.WriteLine("Token {0} = ID,Value {1} -> {2}", i, lt.tokens[i], lt.symbols[i]);
		*	}
		*}
		*/

		Expression(0);
		return ret;

	}

	bool Match_Token(int token)
	{
		this.LookAhead = (int)lt.tokens[currentToken];
		return (LookAhead.Equals(token));
	}

	void Advance_LookAhead(int level)
	{
		this.LookAhead = (int)lt.tokens[++currentToken];
	}

	void Expression(int level)
	{
		//Console.WriteLine("Expression() Called at level {0}", level);
		
		Term(level+1);
		Expression_Prime(level+1);
	}

	void Expression_Prime (int level)
	{
		//Console.WriteLine("Expression_Prime() Called at level {0}", level);
		if (Match_Token((int)(LookupTable.Tokens.Plus)))
		{
			Advance_LookAhead(level + 1);
			Term(level + 1);
			Expression_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Minus)))
		{
			Advance_LookAhead(level + 1);
			Term(level + 1);
			Expression_Prime(level + 1);
		}
	}

	void Term(int level) 
	{
		//Console.WriteLine("Term() Called at level {0}", level);
		Factor(level + 1);
		Term_Prime(level + 1);
	}


	void Term_Prime(int level)
	{
		//Console.WriteLine("Term_Prime() Called at level {0}", level);
		if (Match_Token((int)(LookupTable.Tokens.Multiply)))
		{
			Advance_LookAhead(level + 1);
			Factor(level + 1);
			Term_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Divide)))
		{
			Advance_LookAhead(level + 1);
			Factor(level + 1);
			Term_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Exponent)))
		{
			Advance_LookAhead(level + 1);
			Factor(level + 1);
			Term_Prime(level + 1);
		}
	}

	void Factor(int level)
	{
		//Console.WriteLine("Factor() Called at level {0}", level);
		if (Match_Token((int)LookupTable.Tokens.Number))
		{
			Advance_LookAhead(level + 1);
		}
		else if (Match_Token((int)LookupTable.Tokens.Left_Para))
		{
			Advance_LookAhead(level + 1);
			Expression(level + 1);
			if (Match_Token((int)LookupTable.Tokens.Right_Para))
			{
				Advance_LookAhead(level + 1);
			}
			else ret = false;
		}
		else ret = false;
	}








}
