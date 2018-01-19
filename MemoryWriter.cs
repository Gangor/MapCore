using System;
using System.IO;
using System.Text;

namespace MapCore
{
	public class MemoryWriter : MemoryStream
	{
		public MemoryWriter() { }

		// Writes a boolean to this stream. A single byte is written to the stream
		// with the value 0 representing false or the value 1 representing true.
		// 
		public virtual void Write(bool value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a byte to this stream. The current position of the stream is
		// advanced by one.
		// 
		public virtual void Write(byte value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a signed byte to this stream. The current position of the stream 
		// is advanced by one.
		// 
		public virtual void Write(sbyte value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a byte array to this stream.
		// 
		// This default implementation calls the Write(Object, int, int)
		// method to write the byte array.
		// 
		public virtual void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		// Writes a character to this stream. The current position of the stream is
		// advanced by two.
		// Note this method cannot handle surrogates properly in UTF-8.
		// 
		[System.Security.SecuritySafeCritical]  // auto-generated
		public virtual void Write(char ch)
		{
			var buffer = BitConverter.GetBytes(ch);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a character array to this stream.
		// 
		// This default implementation calls the Write(Object, int, int)
		// method to write the character array.
		// 
		public virtual void Write(char[] chars)
		{
			var buffer = Encoding.UTF8.GetBytes(chars);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a section of a character array to this stream.
		//
		// This default implementation calls the Write(Object, int, int)
		// method to write the character array.
		// 
		public virtual void Write(char[] chars, int index, int count)
		{
			var buffer = Encoding.UTF8.GetBytes(chars, index, count);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a double to this stream. The current position of the stream is
		// advanced by eight.
		// 
		public virtual void Write(double value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a two-byte signed integer to this stream. The current position of
		// the stream is advanced by two.
		// 
		public virtual void Write(short value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a two-byte unsigned integer to this stream. The current position
		// of the stream is advanced by two.
		// 
		public virtual void Write(ushort value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a four-byte signed integer to this stream. The current position
		// of the stream is advanced by four.
		// 
		public virtual void Write(int value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a four-byte unsigned integer to this stream. The current position
		// of the stream is advanced by four.
		// 
		public virtual void Write(uint value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes an eight-byte signed integer to this stream. The current position
		// of the stream is advanced by eight.
		// 
		public virtual void Write(long value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes an eight-byte unsigned integer to this stream. The current 
		// position of the stream is advanced by eight.
		// 
		public virtual void Write(ulong value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a float to this stream. The current position of the stream is
		// advanced by four.
		// 
		[System.Security.SecuritySafeCritical]  // auto-generated
		public virtual void Write(float value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		// Writes a length-prefixed string to this stream in the BinaryWriter's
		// current Encoding. This method first writes the length of the string as 
		// a four-byte unsigned integer, and then writes that many characters 
		// to the stream.
		// 
		[System.Security.SecuritySafeCritical]
		public virtual void Write(string value)
		{
			var len = Encoding.UTF8.GetByteCount(value);
			Write7BitEncodedInt(len);

			var buffer = Encoding.UTF8.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		#region Internal 

		/*
		 * Source : https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/binarywriter.cs
		 */
		protected void Write7BitEncodedInt(int value)
		{
			uint v = (uint)value;
			while (v >= 0x80)
			{
				Write((byte)(v | 0x80));
				v >>= 7;
			}
			Write((byte)v);
		}

		#endregion
	}
}
