using System;
using System.IO;

namespace Visualization
{
	/// <summary>
	/// Summary description for FormattedStream.
	/// </summary>
	public class FormattedStream
	{
		private enum ReadDoubleState
		{
			stStart,
			stMantissaInt,
			stMantissaPoint,
			stESeen,
			stESign,
			stExponent,
			stError,
			stEnd
		}
		private enum ReadULongState
		{
			stStart,
			stDecimal,
			stZeroSeen,
			stHexadecimalStart,
			stHexadecimal,
			stError,
			stEnd
		}
		private Stream _stream;
		/*public static FormattedStream operator >>(FormattedStream s, double d)
		{

		}*/
		public FormattedStream()
		{
			_stream = null;
		}
		public FormattedStream(Stream stream)
		{
			Reset(stream);
		}
		public void Reset(Stream stream)
		{
			_stream = stream;
		}
		public string GetLine()
		{
			return GetLine('\n');
		}
		public string GetLine(char delim)
		{
			long len;
			char c;
			string retval = "";
			len = _stream.Length;
			c = (char) _stream.ReadByte();
			while( c != delim && _stream.Position != len )
			{
				retval += c;
				c = (char) _stream.ReadByte();
			}
			return retval;
		}
		/// <summary>
		/// Reads a string from the stream until it sees the specified delimiter. Unlike
		/// GetLine, it leaves the delimiter in the stream.
		/// </summary>
		/// <param name="delim">
		/// The delimiter character
		/// </param>
		/// <returns>
		/// The string up to (not including) the delimiter.
		/// </returns>
		public string Gets(char delim)
		{
			string retval = GetLine(delim);
			if(_stream.Position != _stream.Length)
				_stream.Position--;
			return retval;
		}
		/// <summary>
		/// The function ignores any white spaces and then tries to read a double value
		/// from the stream. If any error occurs, the function restores the stream to
		/// its state before the reading operation and throws an exception. It stops
		/// before the character after the double value.
		/// </summary>
		/// <param name="d">
		/// The variable on which to read the value.
		/// </param>
		/// <returns>
		/// Returns the object back again to allow chaining.
		/// </returns>
		public FormattedStream Read(out double value)
		{
			string val = "";
			char c;
			long originalPosition = _stream.Position;
			long len = _stream.Length;
			ReadDoubleState state = ReadDoubleState.stStart;
			while(true)
			{
				c = (char) _stream.ReadByte();
				switch(state)
				{
					case ReadDoubleState.stStart:
						if( char.IsWhiteSpace(c) )
							break;
						if( c == '+' )
							break;
						if( c == '-' )
							val += c;
						else if( char.IsDigit(c) )
						{
							val += c;
							state = ReadDoubleState.stMantissaInt;
						}
						else if( c == '.' )
						{
							val += c;
							state = ReadDoubleState.stMantissaPoint;
						}
						else
							state = ReadDoubleState.stError;
						break;
					case ReadDoubleState.stMantissaInt:
						if( char.IsDigit( c ) )
							val += c;
						else if( c == '.' )
						{
							val += c;
							state = ReadDoubleState.stMantissaPoint;
						}
						else if( char.ToUpper(c) == 'E' )
						{
							val += c;
							state = ReadDoubleState.stESeen;
						}
						else
						{
							_stream.Position--;
							state = ReadDoubleState.stEnd;
						}
						break;
					case ReadDoubleState.stMantissaPoint:
						if( char.IsDigit( c ) )
							val += c;
						else if( char.ToUpper(c) == 'E' )
						{
							val += c;
							state = ReadDoubleState.stESeen;
						}
						else
						{
							_stream.Position--;
							state = ReadDoubleState.stEnd;
						}
						break;
					case ReadDoubleState.stESeen:
						if( c == '+' || c == '-' )
						{
							val += c;
							state = ReadDoubleState.stESign;
						}
						else if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadDoubleState.stExponent;
						}
						else
						{
							val.Remove(val.Length - 1, 1);
							_stream.Position -= 2;
							state = ReadDoubleState.stEnd;
						}
						break;
					case ReadDoubleState.stESign:
						if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadDoubleState.stExponent;
						}
						else
						{
							val.Remove(val.Length - 2, 2);
							_stream.Position -= 3;
							state = ReadDoubleState.stEnd;
						}
						break;
					case ReadDoubleState.stExponent:
						if( char.IsDigit( c ) )
							val += c;
						else
						{
							_stream.Position --;
							state = ReadDoubleState.stEnd;
						}
						break;
					case ReadDoubleState.stError:
						_stream.Position = originalPosition;
						throw new IOException("No double precision value exists in the stream.");
					case ReadDoubleState.stEnd:
						value = double.Parse( val );
						return this;
				}
			}
		}
		public static bool IsHexDigit(char c)
		{
			if( char.IsDigit(c) )
				return true;
			c = char.ToUpper(c);
			return c <= 'F' && c >= 'A';
		}
		public FormattedStream Read(out uint value)
		{
			//("+")* ([0-9]+|0x[0-F]+)
			string val = "";
			char c;
			long originalPosition = _stream.Position;
			long len = _stream.Length;
			ReadULongState state = ReadULongState.stStart;
			while(true)
			{
				c = (char) _stream.ReadByte();
				switch(state)
				{
					case ReadULongState.stStart:
						if( char.IsWhiteSpace(c) )
							break;
						if( c == '+' )
							break;
						else if( c == '0' )
						{
							val += c;
							state = ReadULongState.stZeroSeen;
						}
						else if( char.IsDigit(c) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else
							state = ReadULongState.stError;
						break;
					case ReadULongState.stDecimal:
						if( char.IsDigit( c ) )
							val += c;
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stZeroSeen:
						if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else if( char.ToUpper(c) == 'X' )
						{
							val += c;
							state = ReadULongState.stHexadecimalStart;
						}
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimalStart:
						if( IsHexDigit(c) ) 
						{
							val += c;
							state = ReadULongState.stHexadecimal;
						}
						else
						{
							val.Remove(val.Length - 1, 1);
							_stream.Position -= 2;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimal:
						if( IsHexDigit(c) )
							val += c;
						else
						{
							_stream.Position --;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stError:
						_stream.Position = originalPosition;
						throw new IOException("No unsigned integral value exists in the stream.");
					case ReadULongState.stEnd:
						value = uint.Parse( val );
						return this;
				}
			}
		}
		public FormattedStream Read(out ushort value)
		{
			//("+")* ([0-9]+|0x[0-F]+)
			string val = "";
			char c;
			long originalPosition = _stream.Position;
			long len = _stream.Length;
			ReadULongState state = ReadULongState.stStart;
			while(true)
			{
				c = (char) _stream.ReadByte();
				switch(state)
				{
					case ReadULongState.stStart:
						if( char.IsWhiteSpace(c) )
							break;
						if( c == '+' )
							break;
						else if( c == '0' )
						{
							val += c;
							state = ReadULongState.stZeroSeen;
						}
						else if( char.IsDigit(c) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else
							state = ReadULongState.stError;
						break;
					case ReadULongState.stDecimal:
						if( char.IsDigit( c ) )
							val += c;
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stZeroSeen:
						if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else if( char.ToUpper(c) == 'X' )
						{
							val += c;
							state = ReadULongState.stHexadecimalStart;
						}
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimalStart:
						if( IsHexDigit(c) ) 
						{
							val += c;
							state = ReadULongState.stHexadecimal;
						}
						else
						{
							val.Remove(val.Length - 1, 1);
							_stream.Position -= 2;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimal:
						if( IsHexDigit(c) )
							val += c;
						else
						{
							_stream.Position --;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stError:
						_stream.Position = originalPosition;
						throw new IOException("No unsigned integral value exists in the stream.");
					case ReadULongState.stEnd:
						value = ushort.Parse( val );
						return this;
				}
			}
		}
		public FormattedStream Read(out ulong value)
		{
			//("+")* ([0-9]+|0x[0-F]+)
			string val = "";
			char c;
			long originalPosition = _stream.Position;
			long len = _stream.Length;
			ReadULongState state = ReadULongState.stStart;
			while(true)
			{
				c = (char) _stream.ReadByte();
				switch(state)
				{
					case ReadULongState.stStart:
						if( char.IsWhiteSpace(c) )
							break;
						if( c == '+' )
							break;
						else if( c == '0' )
						{
							val += c;
							state = ReadULongState.stZeroSeen;
						}
						else if( char.IsDigit(c) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else
							state = ReadULongState.stError;
						break;
					case ReadULongState.stDecimal:
						if( char.IsDigit( c ) )
							val += c;
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stZeroSeen:
						if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else if( char.ToUpper(c) == 'X' )
						{
							val += c;
							state = ReadULongState.stHexadecimalStart;
						}
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimalStart:
						if( IsHexDigit(c) ) 
						{
							val += c;
							state = ReadULongState.stHexadecimal;
						}
						else
						{
							val.Remove(val.Length - 1, 1);
							_stream.Position -= 2;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimal:
						if( IsHexDigit(c) )
							val += c;
						else
						{
							_stream.Position --;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stError:
						_stream.Position = originalPosition;
						throw new IOException("No unsigned integral value exists in the stream.");
					case ReadULongState.stEnd:
						value = ulong.Parse( val );
						return this;
				}
			}
		}
		public FormattedStream Read(out int value)
		{
			//("+"|"-")* ([0-9]+|0x[0-F]+)
			string val = "";
			char c;
			long originalPosition = _stream.Position;
			long len = _stream.Length;
			ReadULongState state = ReadULongState.stStart;
			while(true)
			{
				c = (char) _stream.ReadByte();
				switch(state)
				{
					case ReadULongState.stStart:
						if( char.IsWhiteSpace(c) )
							break;
						if( c == '+' )
							break;
						if( c == '-' )
							val += c;
						else if( c == '0' )
						{
							val += c;
							state = ReadULongState.stZeroSeen;
						}
						else if( char.IsDigit(c) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else
							state = ReadULongState.stError;
						break;
					case ReadULongState.stDecimal:
						if( char.IsDigit( c ) )
							val += c;
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stZeroSeen:
						if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else if( char.ToUpper(c) == 'X' )
						{
							val += c;
							state = ReadULongState.stHexadecimalStart;
						}
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimalStart:
						if( IsHexDigit(c) ) 
						{
							val += c;
							state = ReadULongState.stHexadecimal;
						}
						else
						{
							val.Remove(val.Length - 1, 1);
							_stream.Position -= 2;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimal:
						if( IsHexDigit(c) )
							val += c;
						else
						{
							_stream.Position --;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stError:
						_stream.Position = originalPosition;
						throw new IOException("No unsigned integral value exists in the stream.");
					case ReadULongState.stEnd:
						value = int.Parse( val );
						return this;
				}
			}
		}
		public FormattedStream Read(out short value)
		{
			//("+"|"-")* ([0-9]+|0x[0-F]+)
			string val = "";
			char c;
			long originalPosition = _stream.Position;
			long len = _stream.Length;
			ReadULongState state = ReadULongState.stStart;
			while(true)
			{
				c = (char) _stream.ReadByte();
				switch(state)
				{
					case ReadULongState.stStart:
						if( char.IsWhiteSpace(c) )
							break;
						if( c == '+' )
							break;
						if( c == '-' )
							val += c;
						else if( c == '0' )
						{
							val += c;
							state = ReadULongState.stZeroSeen;
						}
						else if( char.IsDigit(c) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else
							state = ReadULongState.stError;
						break;
					case ReadULongState.stDecimal:
						if( char.IsDigit( c ) )
							val += c;
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stZeroSeen:
						if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else if( char.ToUpper(c) == 'X' )
						{
							val += c;
							state = ReadULongState.stHexadecimalStart;
						}
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimalStart:
						if( IsHexDigit(c) ) 
						{
							val += c;
							state = ReadULongState.stHexadecimal;
						}
						else
						{
							val.Remove(val.Length - 1, 1);
							_stream.Position -= 2;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimal:
						if( IsHexDigit(c) )
							val += c;
						else
						{
							_stream.Position --;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stError:
						_stream.Position = originalPosition;
						throw new IOException("No unsigned integral value exists in the stream.");
					case ReadULongState.stEnd:
						value = short.Parse( val );
						return this;
				}
			}
		}
		public FormattedStream Read(out long value)
		{
			//("+"|"-")* ([0-9]+|0x[0-F]+)
			string val = "";
			char c;
			long originalPosition = _stream.Position;
			long len = _stream.Length;
			ReadULongState state = ReadULongState.stStart;
			while(true)
			{
				c = (char) _stream.ReadByte();
				switch(state)
				{
					case ReadULongState.stStart:
						if( char.IsWhiteSpace(c) )
							break;
						if( c == '+' )
							break;
						if( c == '-' )
							val += c;
						else if( c == '0' )
						{
							val += c;
							state = ReadULongState.stZeroSeen;
						}
						else if( char.IsDigit(c) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else
							state = ReadULongState.stError;
						break;
					case ReadULongState.stDecimal:
						if( char.IsDigit( c ) )
							val += c;
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stZeroSeen:
						if( char.IsDigit( c ) )
						{
							val += c;
							state = ReadULongState.stDecimal;
						}
						else if( char.ToUpper(c) == 'X' )
						{
							val += c;
							state = ReadULongState.stHexadecimalStart;
						}
						else
						{
							_stream.Position--;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimalStart:
						if( IsHexDigit(c) ) 
						{
							val += c;
							state = ReadULongState.stHexadecimal;
						}
						else
						{
							val.Remove(val.Length - 1, 1);
							_stream.Position -= 2;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stHexadecimal:
						if( IsHexDigit(c) )
							val += c;
						else
						{
							_stream.Position --;
							state = ReadULongState.stEnd;
						}
						break;
					case ReadULongState.stError:
						_stream.Position = originalPosition;
						throw new IOException("No unsigned integral value exists in the stream.");
					case ReadULongState.stEnd:
						value = long.Parse( val );
						return this;
				}
			}
		}
	}
}
