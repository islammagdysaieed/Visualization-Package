using System;
using System.Collections;

namespace Visualization
{
	public class LinkedList : IEnumerable, ICollection
	{
		/// <summary>
		/// We have here 3 states:
		/// <list>
		/// <item>
		/// State 1
		/// count = 0. last = first = null. No items at all
		/// </item>
		/// <item>
		/// State 2
		/// count = 1. last = first = Same Node. One item only.
		/// </item>
		/// <item>
		/// State 3
		/// count > 1. first != last. More than one item.
		/// </item>
		/// </list>
		/// As we must provide a separate implementation for each such state.
		/// </summary>
		

		private Node first;
		private Node last;
		private int count;
		public LinkedList()
		{
			first = last = null;
			count = 0;
		}
		public int Count
		{
			get { return count; }
		}
		// Helper Functions:
		public void PushFront(object data) 
		{
			first = new Node(data, first);
			if( last == null ) //State 1
				last = first;
			//other states have the same implementation.
			count++;
		}
		public void PushBack(object data) 
		{
			if(last == null) //State 1
				first = last = new Node(data);
			else
			{
				last.Next = new Node(data);
				last = last.Next;
			}
			count++;
		}
		public object Top()
		{
			if(first == null) return null;
			return first.Data;
		}
		public object Bottom()
		{
			if(last == null) return null;
			return last.Data;
		}
		public object PopFront()
		{
			object retval;
			if(first == null)	return null;
			retval = first.Data;
			if(first == last)	first = last = null;
			else				first = first.Next;
			return retval;
		}
		public bool Remove(object item)
		{
			Node top = first;
			Node beforeTop;
			if(top == null) return false;	//State 1
			if(top.Data == item)
			{
				if(first == last) //State 2
					first = last = null;
				else first = first.Next;
				return true;
			}
			beforeTop = top;
			top = top.Next;
			while(top != null)
			{
				if(top.Data == item)
				{
					beforeTop.Next = top.Next;
					return true;
				}
				beforeTop = top;
				top = top.Next;
			}
			return false;
		}
		public bool IsEmpty()
		{
			return count == 0;
		}
		
		public override string ToString()
		{
			string retval = string.Format("Count = {0}.\n",count);
			Node top = first;
			while(top != null)
			{
				retval += top.Data.ToString() + "\n";
				top = top.Next;
			}
			return retval;
		}

		
		public IEnumerator GetEnumerator()
		{
			return new ListEnumerator(this);
		}


		class ListEnumerator : IEnumerator
		{
			LinkedList theList;
			bool beforefirst; 
			Node node;
			public ListEnumerator(LinkedList thelist)
			{
				this.theList = thelist;
				beforefirst = true;
				node = null; 
			}

			public object Current
			{
				get 
				{
					if (beforefirst) throw new InvalidOperationException("The Iterator position is before the first element");
					if (node==null ) throw new InvalidOperationException("The Iterator position is after the last element");
					return node.Data;
				} 
			}
		
			public bool MoveNext()
			{
				if (beforefirst) 
				{
					beforefirst = false;
					node = theList.first;
					return (node != null);
				}
				if (node == null) return false;
				node = node.Next;
				return (node != null);
			}

			public void Reset()
			{
				beforefirst = true;
				node = null; 
			}
		}
		public bool IsSynchronized
		{
			get
			{ return false; }
		}
		public void CopyTo(Array array, int index)
		{
			foreach(object data in this)
				array.SetValue(data, index++);
		}
		public object SyncRoot
		{
			get
			{ return null; }
		}
	}
	public class Node
	{
		public object Data;
		public Node Next;
		public Node(object data) : this(data, null) {}
		public Node(object data, Node next)
		{
			Data = data;
			Next = next;
		}
	}
}
 