using System;
using System.Text;
using static LookupTable;

public class Lexer
{
	char[] input; //Input parsed through reference
	LookupTable lt; //Lookup Table parsed thorugh reference
	int MAX_TOKENS;

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
					lt.tokens[token_i++] = Tokens.Plus;
					break;
					}

				case ('-'): //Used to add the minus to the token array
					{
						lt.tokens[token_i++] = Tokens.Minus;
						break;
					}

				case ('^'): //Used to add the multiplication to the token array
					{
						lt.tokens[token_i++] = Tokens.Exponent;
						break;
					}

				case ('*'): //Used to add the multiplication to the token array
					{
						lt.tokens[token_i++] = Tokens.Multiply;
						break;
					}

				case ('/'): //Used to add the division to the token array
					{
						lt.tokens[token_i++] = Tokens.Divide;
						break;
					}
				
				case ('('): //Used to add the left bracket to the token array
					{
						lt.tokens[token_i++] = Tokens.Left_Para;
						break;
					}
				
				case (')'): //Used to add the right bracket to the token array
					{
						lt.tokens[token_i++] = Tokens.Right_Para;
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
							lt.tokens[token_i] = Tokens.Number;
							lt.symbols[token_i++] = int.Parse(new string(number));
							--i;
							break;
						}
					}

			}

		}
		int fill_array=token_i;
		while (fill_array < lt.tokens.Length)
		{
			lt.tokens[fill_array++] = Tokens.EMPTY;
		}


		return token_i;
	}













}
