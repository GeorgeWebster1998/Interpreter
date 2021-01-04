using System;
using static LookupTable;

public class Parser
{
	int LookAhead;
	int currentToken;
	string ret;
	LookupTable lt;
	ParsedTrie trie;

	public Parser(ref LookupTable lt)
	{
		this.LookAhead = -1;
		this.currentToken = 0;
		this.ret = "p";
		this.lt = lt;
		trie = new ParsedTrie();
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
		lt.SetParsedTrie(trie);
		return ret;

	}

	bool Match_Token(int token)
	{
		this.LookAhead = (int)lt.symbols[currentToken].Type;
		return (LookAhead.Equals(token));

	}

	void Advance_LookAhead()
	{
		this.LookAhead = (int)lt.symbols[++currentToken].Type;
	}

	void Statement(int level)
	{

		if (lt.GetSymbol(1).Type == Tokens.Equal)
		{
			trie.AddNewNode(level, Tokens.EMPTY, "<<Statement>>");
			if (!(Variable(level + 1, true))){
				ret = "Can't assign data to " + lt.symbols[currentToken].Type;
				return;
			}

			if (Match_Token((int) Tokens.Equal))
			{
				trie.AddNewNode(level + 1, "<<Statement>>", Tokens.Equal);
				Advance_LookAhead();
			}

			Expression(level + 1);
		}
		else
		{
			Expression(level);
		}
	}

	void Expression(int level)
	{
		//Console.WriteLine("Expression() Called at level {0}", level);
		if (level == 1)
		{
			trie.AddNewNode(level, "<<Statement>>", "<<Expression>>");
		}
		else if (level == 0)
		{
			trie.AddNewNode(level, Tokens.EMPTY, "<<Expression>>");
		}
		else
		{
			trie.AddNewNode(level, "<<Factor>>", "<<Expression>>");
		}
		Term(level + 1, true);
		Expression_Prime(level + 1);
	}

	void Expression_Prime(int level)
	{
		//Console.WriteLine("Expression_Prime() Called at level {0}", level);
		
		if (Match_Token((int)(Tokens.Plus)))
		{
			trie.AddNewNode(level, "<<Expression>>", "<<Expression_Prime>>");
			trie.AddNewNode(level + 1, "<<Expression_Prime>>", Tokens.Plus);
			Advance_LookAhead();
			Term(level + 1, false);
			Expression_Prime(level + 1);
		}
		else if (Match_Token((int)(Tokens.Minus)))
		{
			trie.AddNewNode(level, "<<Expression>>", "<<Expression_Prime>>");
			trie.AddNewNode(level + 1, "<<Expression_Prime>>", Tokens.Minus);
			Advance_LookAhead();
			Term(level + 1, false);
			Expression_Prime(level + 1);
		}
	}

	void Term(int level, bool isCalledFromExp)
	{
		if (isCalledFromExp)
		{
			trie.AddNewNode(level, "<<Expression>>", "<<Term>>");
		}
		else
		{
			trie.AddNewNode(level, "<<Expression_Prime>>", "<<Term>>");
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
			trie.AddNewNode(level, "<<Term>>", "<<Term_Prime>>");
			trie.AddNewNode(level + 1, "<<Term_Prime>>", Tokens.Multiply);
			Advance_LookAhead();
			Factor(level + 1, false);
			Term_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Divide)))
		{
			trie.AddNewNode(level, "<<Term>>", "<<Term_Prime>>");
			trie.AddNewNode(level + 1, "<<Term_Prime>>", Tokens.Divide);
			Advance_LookAhead();
			Factor(level + 1, false);
			Term_Prime(level + 1);
		}
		else if (Match_Token((int)(LookupTable.Tokens.Exponent)))
		{
			trie.AddNewNode(level, "<<Term>>", "<<Term_Prime>>");
			trie.AddNewNode(level + 1, "<<Term_Prime>>", Tokens.Exponent);
			Advance_LookAhead();
			Factor(level + 1, false);
			Term_Prime(level + 1);
		}
	}

	void Factor(int level, bool isCalledFromTerm)
	{
		if (isCalledFromTerm)
		{
			trie.AddNewNode(level, "<<Term>>", "<<Factor>>");
		}
		else
		{
			trie.AddNewNode(level, "<<Term_Prime>>", "<<Factor>>");
		}

		if (Match_Token((int)LookupTable.Tokens.Integer)){
			trie.AddNewNode(level + 1, "<<Factor>>", lt.GetSymbol(currentToken).Value);
			Advance_LookAhead();
		}
		else if (Match_Token((int)LookupTable.Tokens.Double))
		{
			trie.AddNewNode(level + 1, "<<Factor>>", lt.GetSymbol(currentToken).Value);
			Advance_LookAhead();
		}
		else if (Match_Token((int)LookupTable.Tokens.Variable))
		{
			Variable(level, false);
		}
		else if (Match_Token((int)LookupTable.Tokens.Left_Para))
		{
			Advance_LookAhead();
			Expression(level + 1);
			if (Match_Token((int)LookupTable.Tokens.Right_Para))
			{
				Advance_LookAhead();
			}
			else ret = "Missing closing bracket";
			return;
		}
		else ret = "Missing factor";
		return;
	}

	bool Variable(int level, bool isStatement)
	{
		if (Match_Token((int)LookupTable.Tokens.Variable))
		{
			if (isStatement)
			{
				trie.AddNewNode(level, "<<Statement>>", (string)lt.GetSymbol(currentToken).Value);
				lt.AddToVariables((string)lt.GetSymbol(currentToken).Value, new Var((double)0)); 
				Advance_LookAhead();
				return true;
			}
			else
			{
				if (lt.VariableExist((string)lt.GetSymbol(currentToken).Value))
				{
					string varName = (string)lt.GetSymbol(currentToken).Value;
					trie.AddNewNode(level+1, "<<Factor>>", (string)varName + " -> " + lt.GetVarValue(varName));
					Advance_LookAhead();
					return true;
				}
				else
				{
					ret = "Variable " + lt.GetSymbol(currentToken).Value + " not initialised";
				}
				Advance_LookAhead();
			}
		}
		return false;
	}
}
