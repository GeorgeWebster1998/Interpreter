using System;

public class Lexer
{
	string input;
	int MAX_TOKENS;
	int current;
	int ahead;

	struct LookupTable
	{
		public int[] symbols;
		public int[] values;

		public LookupTable(int MAX_TOKENS) {
			symbols = new int[MAX_TOKENS];
			values = new int[MAX_TOKENS];
		}
	}

	public Lexer(ref string input, int MAX_TOKENS)
	{
		this.input=input;
		this.MAX_TOKENS = MAX_TOKENS;
		this.current = 0;
		this.ahead = -1;
		Process();
	}

	public int Process()
	{
		LookupTable lt = new LookupTable(MAX_TOKENS);

		for (this.current = 0; this.current < this.input.Length; this.current++)
		{
			switch (input[current])
			{
				case (' '): break;

				case ('*'):
					Console.WriteLine("match found for * ");
					break;

				case ('/'):
					Console.WriteLine("match found for / ");
					break;

				case ('+'):
					Console.WriteLine("match found for + ");
					break;

				case ('-'):
					Console.WriteLine("match found for - ");
					break;

				case ('('):
					Console.WriteLine("match found for ( ");
					break;

				case (')'):
					Console.WriteLine("match found for ) ");
					break;

				default:
					if (Char.IsDigit(input[current]) == false)
					{
						//Console.WriteLine("unkown symbol {0}", currentChar);
					}
					else{
						Console.WriteLine("is digit {0}", input[current]);
					}
					break;

			}

		}









		return 1;
	}













}
