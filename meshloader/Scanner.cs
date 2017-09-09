using System;
using System.Collections;
using System.IO;

namespace Visualization
{
	public enum TokenType
	{
		EQ = 1,
		COMMA,
		NUMBER,
		INVALID_TOKEN,
		ENDOFFILE,
		UNEXPECTED_EOF,
		EMPTY_QUOTES,
		ID,
		QUOTED_ID,
		TITLE,
		VARIABLES,
		ZONE,
		//	T,
		FEPOINT,
		FEBLOCK,
		FECONNECT,
		TRIANGLE,
		QUADRILATERAL,
		TETRAHEDRON,
		BRICK,
		POINT,
		BLOCK,
		I,
		J,
		K,
		N,
		E,
		F,
		ET,
		D
	}
	/// <summary>
	/// Summary description for Scanner.
	/// </summary>
	public class Scanner
	{
		private enum State
		{
			stINIT = 1,
			stQUOTED,
			stPLUS,
			stNUM,
			stIDKEY,
		}
		//////////////////////////////////////////////
		private static Hashtable Keywords;
		static Scanner()
		{
			Keywords = new Hashtable( 23 );
			Keywords.Add("title", TokenType.TITLE);
			Keywords.Add("variables", TokenType.VARIABLES);
			Keywords.Add("zone", TokenType.ZONE);
			//Keywords.Add("t", T);
			Keywords.Add("fepoint", TokenType.FEPOINT);
			Keywords.Add("feblock", TokenType.FEBLOCK);
			Keywords.Add("feconnect", TokenType.FECONNECT);
			Keywords.Add("triangle", TokenType.TRIANGLE);
			Keywords.Add("quadrilateral", TokenType.QUADRILATERAL);
			Keywords.Add("tetrahedron", TokenType.TETRAHEDRON);
			Keywords.Add("brick", TokenType.BRICK);
			Keywords.Add("point", TokenType.POINT);
			Keywords.Add("block", TokenType.BLOCK);
			Keywords.Add("i", TokenType.I);
			Keywords.Add("j", TokenType.J);
			Keywords.Add("k", TokenType.K);
			Keywords.Add("n", TokenType.N);
			Keywords.Add("e", TokenType.E);
			Keywords.Add("f", TokenType.F);
			Keywords.Add("et", TokenType.ET);
			Keywords.Add("d", TokenType.D);
		}
		
		private Stream			_fin;
		private State			_currentState;

		private TokenType	CheckKeyword()
		{
			string copy = Text.ToLower();
			object retval = Keywords[copy];
			if(retval != null) return (TokenType) retval;
			else return TokenType.ID;
		}
		static bool		IsIDStarter(char c)
		{
			if(char.IsLetter(c))
				return true;
			if(c == '_')
				return true;
			return false;
		}
		static bool		IsIDChar(char c)
		{
			if(char.IsLetter(c))
				return true;
			if(c == '_')
				return true;
			if(char.IsDigit(c))
				return true;
			return false;
		}

		public string		Text;
		public uint			Number;

		public static bool IsID(TokenType token)
		{
			return token >= TokenType.ID && token <= TokenType.D;
		}

		public Scanner()
		{

		}
		public Scanner(Stream inputStream)
		{
			Reset(inputStream);
		}
	
		public void Reset(Stream inputStream)
		{
			_fin = inputStream;
			Number = 0;
			_currentState = State.stINIT;
			Text = "";
		}
		public TokenType	Tokenize()
		{
			char c;
			while(true)
			{
				switch(_currentState)
				{
					case State.stINIT:
						c = (char) _fin.ReadByte();
						if(char.IsWhiteSpace(c))	continue;
						if(char.IsDigit(c))
						{
							Text = new string(c, 1);
							_currentState = State.stNUM;
						}
						else if(IsIDStarter(c))
						{
							Text = new string(c, 1);
							_currentState = State.stIDKEY;
						}
						else 
						{
							switch(c)
							{
								case '=':
									return TokenType.EQ;
								case ',':
									return TokenType.COMMA;
								case '\"':
									Text = "";
									_currentState = State.stQUOTED;
									break;
								case '+':
									_currentState = State.stPLUS;
									break;
								case (char)0xFFFF:
									return TokenType.ENDOFFILE;
								default:
									Text = new string(c, 1);
									return TokenType.INVALID_TOKEN;
							}
						}
						break;
						//END OF State.stINIT 
					case State.stQUOTED:
						c = (char) _fin.ReadByte();
						if(c == '\"')
						{
							_currentState = State.stINIT;
							if( Text.Length > 0 )
								return TokenType.QUOTED_ID;
							else
								return TokenType.EMPTY_QUOTES;
						}
						else if(c == (char)0xFFFF)
						{
							_currentState = State.stINIT;
							return TokenType.UNEXPECTED_EOF;//After DQuotes and zero or more chars
						}
						else Text += c;
						break;
						//END OF State.stQUOTED
					case State.stPLUS:
						c = (char) _fin.ReadByte();
						if(c != '+')
						{
							if(char.IsDigit(c))
							{
								Text += c;
								_currentState = State.stNUM;
							}
							else if(c == (char)0xFFFF)
							{
								_currentState = State.stINIT;
								return TokenType.UNEXPECTED_EOF;//After pluses
							}
							else
							{
								_currentState = State.stINIT;
								_fin.Position--;
								return TokenType.INVALID_TOKEN;//BAD PLUSES
							}
						}
						break;
						//END OF State.stPLUS
					case State.stNUM:
						c = (char) _fin.ReadByte();
						if(char.IsDigit(c))
							Text += c;
						else
						{
							_fin.Position--;
							Number = uint.Parse(Text);
							_currentState = State.stINIT;
							return TokenType.NUMBER;
						}
						break;
						//END OF State.stNUM
					case State.stIDKEY:
						c = (char) _fin.ReadByte();
						if(IsIDChar(c))
							Text += c;
						else
						{
							_fin.Position--;
							_currentState = State.stINIT;
							return CheckKeyword();
						}
						break;
						//END OF State.stIDKEY
					default: //We should not enter here!!
						throw new Exception("Bad State.state identifier in scanner implementation.\r\nCheck your design");
				}
			}
		}
	}
}

