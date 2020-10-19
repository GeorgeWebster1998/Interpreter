using System;
using System.Text;

public class Lexer
{
	char[] input; //Input parsed through reference
	LookupTable lt; //Lookup Table parsed thorugh reference
	int MAX_TOKENS;

	//TOKEN KEY
	static int T_PLUS = 1;
	static int T_MINUS = 2;
	static int T_MULTIPLY = 3;
	static int T_DIVIDE = 4;
	static int T_LPAR = 5;
	static int T_RPAR = 6;
	static int T_NR = 7;

	public Lexer(ref char[] input, ref int MAX_TOKENS, ref LookupTable lt)
	{
		this.input=input;
		this.MAX_TOKENS = MAX_TOKENS;
		this.lt = lt;
	}

	public int Process()
	{
		int token_i = 0; //Token Counter
		
		for (int i = 0; input[i]!='\n'; ++i) //For loop to go through input
		{
			if (token_i == MAX_TOKENS) //If token count is the same size as max tokens stop the lexer 
				break;

			switch (input[i])
			{
				case (' '): break; //Used to ignore spaces from the input

				case ('+'): //Used to add the plus to the token array
					{
					lt.tokens[token_i++] = T_PLUS;
					break;
					}

				case ('-'): //Used to add the minus to the token array
					{
						lt.tokens[token_i++] = T_MINUS;
						break;
					}

				case ('*'): //Used to add the multiplication to the token array
					{
						lt.tokens[token_i++] = T_MULTIPLY;
						break;
					}

				case ('/'): //Used to add the division to the token array
					{
						lt.tokens[token_i++] = T_DIVIDE;
						break;
					}
				
				case ('('): //Used to add the left bracket to the token array
					{
						lt.tokens[token_i++] = T_LPAR;
						break;
					}
				
				case (')'): //Used to add the right bracket to the token array
					{
						lt.tokens[token_i++] = T_RPAR;
						break;
					}


				default: //If no symbol above is found
					{
						if (!Char.IsLetterOrDigit(input[i]))
						{
							break;
						}
						else if (Char.IsLetter(input[i])) 
						{
							/*
							 * If the current char is a letter we find out what the following number is
							 * then converting the char to a byte for storage as a token and storing the 
							 * number in the symbol table
							 */

							int number_counter = 0;
							char[] number = new char[input.Length];
							if(input[i+1] == '=')
							{
								i += 2 ;
								while (Char.IsDigit(input[i]))
								{
									number[number_counter++] = input[i];
									++i;
								}
								ASCIIEncoding ascii = new ASCIIEncoding();

								lt.tokens[token_i] = (byte)input[i-2];
								lt.symbols[token_i++] = int.Parse(new string(number));
								--i;
							}
							break;
						}
						else
						{
							/*
							 * This is very similar to the above but just counts out numbers by themselves.
							 * So the token for number is stored in the token table and the number is stored
							 * in symbol table.
							 */
							 
							int number_counter = 0;
							char[] number = new char[input.Length];
							while (Char.IsDigit(input[i]))
							{
								number[number_counter++] = input[i];
								++i;
							}
							lt.tokens[token_i] = T_NR;
							lt.symbols[token_i++] = int.Parse(new string(number));
							--i;
							break;
						}
					}

			}

		}
		return token_i;
	}













}
