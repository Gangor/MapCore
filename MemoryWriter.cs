using System;
using System.IO;
using System.Security;
using System.Text;

namespace MapCore
{
	/// <summary>
	/// Create a writer stream from memory
	/// </summary>
	public class MemoryWriter : MemoryStream
	{
		/// <summary>
		/// Construct a stream reader from memory
		/// </summary>
		public MemoryWriter() { }
		
		/// <summary>
		/// Clear the memory stream.
		/// </summary>
		public void Clear()
		{
			byte[] buffer = GetBuffer();
			Array.Clear(buffer, 0, buffer.Length);
			Position = 0;
			SetLength(0);
		}

		/// <summary>
		/// Writes a boolean to this stream. A single byte is written to the stream
		/// with the value 0 representing false or the value 1 representing true.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(bool value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Writes a byte to this stream. The current position of the stream is
		/// advanced by one.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(byte value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes a signed byte to this stream. The current position of the stream
		/// is advanced by one.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(sbyte value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes a byte array to this stream.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(byte[] value)
		{
			Write(value, 0, value.Length);
		}
		
		/// <summary>
		/// Writes a character to this stream. The current position of the stream is
		/// advanced by two.
		/// Note this method cannot handle surrogates properly in UTF-8.
		/// </summary>
		/// <param name="value">Value to write</param>
		[SecuritySafeCritical]
		public virtual void Write(char value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Writes a character array to this stream.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(char[] value)
		{
			var buffer = Encoding.UTF8.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Writes a section of a character array to this stream.
		/// </summary>
		/// <param name="chars">The buffer to write data from.</param>
		/// <param name="index">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		public virtual void Write(char[] chars, int index, int count)
		{
			var buffer = Encoding.UTF8.GetBytes(chars, index, count);
			Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Writes a double to this stream. The current position of the stream is advanced by eight.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(double value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes a two-byte signed integer to this stream. The current position of the stream is advanced by two.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(short value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes a two-byte unsigned integer to this stream. The current position of the stream is advanced by two.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(ushort value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes a four-byte signed integer to this stream. The current position of the stream is advanced by four.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(int value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes a four-byte unsigned integer to this stream. The current position of the stream is advanced by four.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(uint value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes an eight-byte signed integer to this stream. The current position of the stream is advanced by eight.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(long value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes an eight-byte unsigned integer to this stream. The current position of the stream is advanced by eight.
		/// </summary>
		/// <param name="value">Value to write</param>
		public virtual void Write(ulong value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Writes a float to this stream. The current position of the stream is advanced by four.
		/// </summary>
		/// <param name="value">Value to write</param>
		[SecuritySafeCritical]
		public virtual void Write(float value)
		{
			var buffer = BitConverter.GetBytes(value);
			Write(buffer, 0, buffer.Length);
		}
		
		/// <summary>
		/// Writes a length-prefixed string to this stream in the BinaryWriter's current Encoding.
		/// This method first writes the length of the string as 
		/// a four-byte unsigned integer, and then writes that many characters 
		/// to the stream.
		/// </summary>
		/// <param name="value">Value to write</param>
		[SecuritySafeCritical]
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
		 /// <summary>
		 /// Write a lenght-prefixed with encoded
		 /// </summary>
		 /// <param name="value"></param>
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
