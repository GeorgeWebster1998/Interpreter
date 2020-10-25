using System;
using System.Collections;

public class ParsedTrie
{
	ParsedTrieNode root;

	public ParsedTrie()
	{
		root = new ParsedTrieNode(0, LookupTable.Tokens.EMPTY, null);
	}

	public void addChild(int depth, int? parent, int? value)
	{
		for (int i = 0; i < depth; i++)
		{

		}


	}





}



public class ParsedTrieNode
{
	int depth;
	LookupTable.Tokens parent;
	int? value;
	ArrayList children;

	public ParsedTrieNode(int depth, LookupTable.Tokens parent, int? value)
	{
		this.depth = depth;
		this.parent = parent;
		this.value = value;
	}
}

