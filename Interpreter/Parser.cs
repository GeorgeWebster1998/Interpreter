using System;
using static LookupTable;

public class Parser
{
	int LookAhead;
	int currentToken;
	string ret;
	LookupTable lt;
	ParsedTrie ParsedTrie;
	ParsedTrie AST;


	public Parser(ref LookupTable lt)
	{
		this.LookAhead = -1;
		this.currentToken = 0;
		this.ret = "Parsed";
		this.lt = lt;
		ParsedTrie = new ParsedTrie();
	}

	/*	BNF
	 *	<statement> ::= <variable> = <expression>
	 *	<expression> ::= <term> <expression_prime>
	 *	<expression_prime> ::= + <term> <expression_prime>
	 *	<expression_prime> ::= - <term> <expression_prime>
	 *	<term> ::= <factor> <term_prime>
	 *	<term_prime> ::= ^ <factor> <term_prime>
	 *	<term_prime> ::= * <factor> <term_prime>
	 *	<term_prime> := / <factor> <term_prime>
	 *	<factor> ::= number | <variable> | (<expr>)
	 *  <variable> ::= id
	 */

	public string Parse()
	{
		/*Check that lt is parsed correctly 
		*/
		Statement(0);
		//ParsedTrie.PrintWidthFirst();
		return ret;

	}

	bool Match_Token(int token)
	{
		this.LookAhead = (int)lt.symbols[currentToken].type;
		return (LookAhead.Equals(token));

	}

	void Advance_LookAhead()
	{
		this.LookAhead = (int)lt.symbols[++currentToken].type;
	}

	void Statement(int level)
	{
		//trie.AddNewNode(level, LookupTable.Tokens.EMPTY, "<<Statement>>");
		Variable(level + 1, true);

		if (Match_Token((int)LookupTable.Tokens.Equal))
		{
		//	trie.AddNewNode(level + 1, "<<Statement>>", LookupTable.Tokens.Equal);
			Advance_LookAhead();
		}

		Expression(level + 1);
	}

	void Expression(int level)
	{
		//Console.WriteLine("Expression() Called at level {0}", level);
		if (level == 0)
		{
			ParsedTrie.AddNewNode(level, LookupTable.Tokens.EMPTY, "<<Expression>>");
		}
		else
		{
			ParsedTrie.AddNewNode(level, "<<Factor>>", "<<Expression>>");
		}
		Term(level + 1, true);
		Expression_Prime(level + 1);
	}

	void Expression_Prime(int level)
	{
		//Console.WriteLine("Expression_Prime() Called at level {0}", level);
		
		if (Match_Token((int)(LookupTable.Tokens.Plus)))
		{
			ParsedTrie.AddNewNode(level, "<<Expression>>", "<<Expression_Prime>>");
			ParsedTrie.AddNewNode(level + 1, "<<Expression_Prime>>", lt.symbols[currentToken]);
			Advance_LookAhead();
			Term(level + 1, false);
			Expression_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Minus)))
		{
			ParsedTrie.AddNewNode(level, "<<Expression>>", "<<Expression_Prime>>");
			ParsedTrie.AddNewNode(level + 1, "<<Expression_Prime>>", lt.symbols[currentToken]);
			Advance_LookAhead();
			Term(level + 1, false);
			Expression_Prime(level + 1);
		}
	}

	void Term(int level, bool isCalledFromExp)
	{
		if (isCalledFromExp)
		{
			ParsedTrie.AddNewNode(level, "<<Expression>>", "<<Term>>");
		}
		else
		{
			ParsedTrie.AddNewNode(level, "<<Expression_Prime>>", "<<Term>>");
		}
		//Console.WriteLine("Term() Called at level {0}", level);
		Factor(level + 1, true);
		Term_Prime(level + 1);
	}


	void Term_Prime(int level)
	{
		//Console.WriteLine("Term_Prime() Called at level {0}", level);
		if (Match_Token((int)(LookupTable.Tokens.Multiply)))
		{
			ParsedTrie.AddNewNode(level, "<<Term>>", "<<Term_Prime>>");
			ParsedTrie.AddNewNode(level + 1, "<<Term_Prime>>", lt.symbols[currentToken]);
			Advance_LookAhead();
			Factor(level + 1, false);
			Term_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Divide)))
		{
			ParsedTrie.AddNewNode(level, "<<Term>>", "<<Term_Prime>>");
			ParsedTrie.AddNewNode(level + 1, "<<Term_Prime>>", lt.symbols[currentToken]);
			Advance_LookAhead();
			Factor(level + 1, false);
			Term_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Exponent)))
		{
			ParsedTrie.AddNewNode(level, "<<Term>>", "<<Term_Prime>>");
			ParsedTrie.AddNewNode(level + 1, "<<Term_Prime>>", lt.symbols[currentToken]);
			Advance_LookAhead();
			Factor(level + 1, false);
			Term_Prime(level + 1);
		}
	}

	void Factor(int level, bool isCalledFromTerm)
	{
		if (isCalledFromTerm)
		{
			ParsedTrie.AddNewNode(level, "<<Term>>", "<<Factor>>");
		}
		else
		{
			ParsedTrie.AddNewNode(level, "<<Term_Prime>>", "<<Factor>>");
		}

		if (Match_Token((int)LookupTable.Tokens.Integer) || (Match_Token((int)LookupTable.Tokens.Float)))
		{
			//Console.WriteLine("factor -> {0}", lt.symbols[currentToken].value.ToString());
			ParsedTrie.AddNewNode(level + 1, "<<Factor>>", lt.symbols[currentToken]);
			Advance_LookAhead();
		}
		if (Match_Token((int)LookupTable.Tokens.Variable))
		{
			Variable(level + 1, false);
		}
		else if (Match_Token((int)LookupTable.Tokens.Left_Para))
		{
			Advance_LookAhead();
			Expression(level + 1);
			if (Match_Token((int)LookupTable.Tokens.Right_Para))
			{
				Advance_LookAhead();
			}
			else ret = "ERROR: Missing closing bracket";
		}
		else ret = "ERROR: Missing factor";
	}

	void Variable(int level, bool statement)
	{
		if (Match_Token((int)LookupTable.Tokens.Variable))
		{
			if (statement)
			{
				//trie.AddNewNode(level + 1, "<<Statement>>", lt.getSymbol(currentToken).value);
				lt.addToVariables((string)lt.getSymbol(currentToken).value, new Var(false, 0)); ;
				Advance_LookAhead();
			}
			else
			{
				if (lt.variableExist((string)lt.getSymbol(currentToken).value))
				{
					//trie.AddNewNode(level + 1, "<<Factor>>", lt.getSymbol(currentToken).value);
				}
				else
				{
					ret = "ERROR: Variable " + lt.getSymbol(currentToken).value + " not initialised";
				}
				Advance_LookAhead();
			}
		}
	}
}
