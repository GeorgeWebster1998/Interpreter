using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static LookupTable;
using static LookupTable.Symbol;

public class ParsedTrie
{
	ParsedTrieNode root;
	public int deepest_node;
	public double final_result;
	public ParsedTrieNode ABST;

	public ParsedTrie()
	{
		root = new ParsedTrieNode(0, Tokens.EMPTY, null);
		final_result = 0;
		ABST = new ParsedTrieNode(0, Tokens.EMPTY, null);
	}

	public void AddNewNode(int depth, Object parentValue, Object value)
	{
		if (root.IsLeaf()) { root.Children.Add(new ParsedTrieNode(depth, parentValue, value)); return; }

		ArrayList toVisit = new ArrayList(root.Children);
		ArrayList Visited = new ArrayList();
		while (toVisit.Count != 0)
		{
			ParsedTrieNode node = (ParsedTrieNode)toVisit[0];
			if (node.Value == parentValue && node.Depth == depth - 1)
			{
				node.Children.Add(new ParsedTrieNode(depth, parentValue, value));
				if (depth > deepest_node)
				{
					deepest_node = depth;
				}
				return;
			}
			else
			{
				if (node.IsLeaf() == false)
				{
					foreach (ParsedTrieNode toAdd in node.Children)
					{
						if (!Visited.Contains(toAdd))
							toVisit.Insert(0, toAdd);
					}
				}
				toVisit.Remove(node);
				Visited.Add(node);
			}
		}
	}

	public void SetABST()
	{
		List<List<ParsedTrieNode>> list = new List<List<ParsedTrieNode>>();
		for (int i = 0; i <= deepest_node; i++)
		{
			list.Add(new List<ParsedTrieNode>());
		}

		ArrayList toVisit = new ArrayList(root.Children);
		ArrayList Visited = new ArrayList();

		while (toVisit.Count != 0)
		{
			ParsedTrieNode node = (ParsedTrieNode)toVisit[0];

			if (!(node.ToString().Contains("<<")))
			{
				list[node.Depth].Add(node);
			}
			
			if (node.IsLeaf() == false)
			{
				foreach (ParsedTrieNode toAdd in node.Children)
				{
					if (!Visited.Contains(toAdd))
						toVisit.Add(toAdd);
				}
			}
			toVisit.Remove(node);
			Visited.Add(node);
		}

		int emptyCount = 0;
		int depth = 0;
		ParsedTrie newTrie = new ParsedTrie();
		ParsedTrieNode lastItem = newTrie.root;
		ArrayList Operators = new ArrayList{ Tokens.Plus, Tokens.Minus, Tokens.Multiply, Tokens.Divide, Tokens.Exponent, Tokens.Equal };

		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].Count == 0)
			{
				emptyCount++;
			}

			for (int k=0; k < list[j].Count; k++)
			{
				if (emptyCount > 0)
				{
					depth = (j+1) - emptyCount;
					emptyCount = 0;
				}

				ParsedTrieNode toAdd = new ParsedTrieNode(depth, lastItem, list[j][k].Value);

				lastItem.AddChild(toAdd);

				if (Operators.Contains(list[j][k].Value))
				{
					lastItem = toAdd;
				}

				depth++;
			}

		}

		ABST = newTrie.root;
	}

	public void PrintWidthFirst()
	{
		ArrayList toVisit = new ArrayList(root.Children);
		ArrayList Visited = new ArrayList();

		Console.WriteLine("#########");

		while (toVisit.Count != 0)
		{
			ParsedTrieNode node = (ParsedTrieNode)toVisit[0];
			Console.WriteLine(node);
			if (node.IsLeaf() == false)
			{
				foreach (ParsedTrieNode toAdd in node.Children)
				{
					if (!Visited.Contains(toAdd))
						toVisit.Add(toAdd);
				}
			}
			toVisit.Remove(node);
			Visited.Add(node);
		}

		Console.WriteLine("#########");
	}

	public void PrintDepthFirst()
	{
		ArrayList toVisit = new ArrayList(root.Children);
		ArrayList Visited = new ArrayList();
		ArrayList AddList = new ArrayList();

		Console.WriteLine("#########");

		while (toVisit.Count != 0)
		{
			ParsedTrieNode node = (ParsedTrieNode)toVisit[0];
			Console.WriteLine(node);
			if (node.IsLeaf() == false)
			{
				foreach (ParsedTrieNode toAdd in node.Children)
				{
					if (!Visited.Contains(toAdd))
						AddList.Add(toAdd);
				}
			}
			toVisit.Remove(node);
			toVisit.InsertRange(0, AddList);
			Visited.Add(node);
			AddList.Clear();
		}

		Console.WriteLine("#########");

	}

	public ParsedTrie ToAbstractSyntaxTree()
	{
		ArrayList toVisit = new ArrayList(root.Children);
		ArrayList Visited = new ArrayList();
		ParsedTrie AST = new ParsedTrie();
		Tokens Parent = Tokens.EMPTY;

		while (toVisit.Count != 0)
		{
			ParsedTrieNode node = (ParsedTrieNode)toVisit[0];

			if (!(node.Value is string))
			{
				AST.AddNewNode(node.Depth, Parent, node.Value);

				if (node.Value is Tokens)
				{
					Parent = (Tokens)node.Value;
				}
			}

			if (node.IsLeaf() == false)
			{
				foreach (ParsedTrieNode toAdd in node.Children)
				{
					if (!Visited.Contains(toAdd))
						toVisit.Add(toAdd);
				}
			}
			toVisit.Remove(node);
			Visited.Add(node);
		}

		return AST;
	}

	public class ParsedTrieNode
	{
		public int Depth { get; set; }
		public Object Parent { get; set; }
		public Object Value { get; set; }
		public ArrayList Children { get; set; }

		public ParsedTrieNode(int depth, Object parent, Object value)
		{
			this.Depth = depth;
			this.Parent = parent;
			this.Value = value;
			this.Children = new ArrayList();
		}

		public bool IsLeaf() { return this.Children.Count == 0; }

		public ParsedTrieNode FindChild(Object value)
		{
			foreach (ParsedTrieNode child in Children)
			{
				if (child.Value == value)
				{
					return child;
				}
			}
			return null;
		}

		public void AddChild(ParsedTrieNode node)
		{
			this.Children.Add(node);
		}


		/*
		public bool RemoveChild(Object value)
		{
			foreach (ParsedTrieNode child in children)
			{
				if (child.value == value)
				{
					children.Remove(child);
					return true;
				}
			}
			return false;
		}
		*/

		public override string ToString()
		{
			/*
			string ret = "";
			for (int i = Depth; i > 0; i--)
			{
				ret += "-";
			}
			*/

			return this.Value.ToString();
		}

		public bool IsString()
		{
			return this.Value is string;
		}

	}
}