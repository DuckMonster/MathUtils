using System.Collections;
using System.Collections.Generic;

namespace MathUtils.Collections {
	public class DataTree<T> : IEnumerable
	{
		class TreeEnumerator : IEnumerator
		{
			DataTree<T> list;
			Node node = null;

			public TreeEnumerator(DataTree<T> list)
			{
				this.list = list;
			}

			public bool MoveNext()
			{
				if (node == null)
					node = list.GetFirstNode();
				else
					node = node.NextNode;

				return node != null;
			}

			public void Reset() { node = null; }

			public object Current
			{
				get { return node.Value; }
			}
		}

		class Node
		{
			DataTree<T> tree;

			Node parent;
			public Node Parent
			{
				get { return parent; }
			}

			T value;
			public T Value
			{
				get { return value; }
			}

			float weight;
			public float Weight
			{
				get { return weight; }
			}

			Node left, right;
			public Node Left
			{
				get { return left; }
			}

			public Node Right
			{
				get { return right; }
			}

			public Node NextNode
			{
				get
				{
					if (right != null)
					{
						Node next = right;
						while (next.left != null)
							next = next.left;

						return next;
					}
					else
					{
						Node next = this;
						while (next.parent != null && next.parent.weight < next.weight)
							next = next.parent;

						return next.parent;
					}
				}
			}

			public Node(DataTree<T> tree, T value, float w)
			{
				this.tree = tree;
				this.value = value;
				this.weight = w;
			}
			public Node(DataTree<T> tree, Node n)
			{
				this.tree = tree;
				this.value = n.Value;
				this.weight = n.weight;
			}

			public Node FindValue(T value)
			{
				if (this.value.Equals(value)) return this;

				if (Left != null) return Left.FindValue(value);
				if (Right != null) return Right.FindValue(value);

				return null;
			}
			public IEnumerable<Node> FindValues(T value)
			{
				if (this.value.Equals(value)) yield return this;

				if (Left != null) foreach (Node n in Left.FindValues(value)) yield return n;
				if (Right != null) foreach (Node n in Right.FindValues(value)) yield return n;
			}

			public void Remove()
			{
				Node newChild = null;
				Node looseNode = null;

				if (right != null)
				{
					newChild = Right;
					looseNode = Left;
				}
				else if (left != null)
				{
					newChild = Left;
				}

				if (parent != null)
				{
					if (parent.weight < weight)
						parent.right = newChild;
					else
						parent.left = newChild;
				}
				else tree.headNode = newChild;

				if (newChild != null)
					newChild.parent = parent;

				if (looseNode != null) newChild.AddItem(looseNode);
			}

			public void AddItem(Node node)
			{
				if (node.weight <= weight)
				{
					if (left == null)
					{
						left = node;
						node.parent = this;
					}
					else
						left.AddItem(node);
				}
				else
				{
					if (right == null)
					{
						right = node;
						node.parent = this;
					}
					else
						right.AddItem(node);
				}
			}
		}

		Node headNode;
		int numberOfItems;

		public int Count
		{
			get
			{
				int nmbr = 0;
				foreach (T n in this)
					nmbr++;

				return nmbr;
			}
		}

		public DataTree()
		{
		}
		public DataTree(DataTree<T> tree)
		{
			if (tree.headNode == null) return;

			foreach (Node n in tree)
				AddNodeCopy(n);
	    }

		void AddNodeCopy(Node n)
		{
			AddItem(n.Value, n.Weight);
	    }

		public void AddItem(T item, float weight)
		{
			if (headNode == null)
				headNode = new Node(this, item, weight);
			else
				headNode.AddItem(new Node(this, item, weight));

			numberOfItems++;
		}
		public bool RemoveItem(T item)
		{
			if (headNode == null) return false;

			bool removed = false;
			foreach (Node n in headNode.FindValues(item))
			{
				n.Remove();
				removed = true;

				numberOfItems--;
			}

			return removed;
		}
		public bool Contains(T item)
		{
			if (headNode == null) return false;
			return headNode.FindValue(item) != null;
		}

		Node GetFirstNode()
		{
			Node next = headNode;
			while (next != null && next.Left != null) next = next.Left;

			return next;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)new TreeEnumerator(this);
		}
	}
}