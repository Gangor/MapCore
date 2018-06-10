using System;
using System.IO;
using System.Text;

namespace MapCore
{
	/// <summary>
	/// Create a stream reader from memory
	/// </summary>
	public class MemoryReader : MemoryStream
	{
		/// <summary>
		/// Construct a stream reader from memory
		/// </summary>
		/// <param name="buffer">Buffer to read</param>
		public MemoryReader(byte[] buffer) : base(buffer) { }
		
		/// <summary>
		/// Read boolean from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual bool ReadBoolean()
		{
			byte[] fbuff = new byte[1];
			fbuff[0] = ReadByte();
			return BitConverter.ToBoolean(fbuff, 0);
		}

		/// <summary>
		/// Read byte from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual new byte ReadByte()
		{
			return (byte)base.ReadByte();
		}

		/// <summary>
		/// Read sbyte from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual sbyte ReadSByte()
		{
			return (sbyte)ReadByte();
		}

		/// <summary>
		/// Read byte array from memory stream
		/// </summary>
		/// <param name="lenght"></param>
		/// <returns></returns>
		public virtual byte[] ReadBytes(int lenght)
		{
			byte[] buffer = new byte[lenght];
			Read(buffer, 0, lenght);
			return buffer;
		}

		/// <summary>
		/// Read char from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual char ReadChar()
		{
			byte[] buffer = ReadBytes(1);
			return BitConverter.ToChar(buffer, 0);
		}

		/// <summary>
		/// Read char array from memory stream
		/// </summary>
		/// <param name="lenght">Lenght to read</param>
		/// <returns></returns>
		public virtual char[] ReadChar(int lenght)
		{
			byte[] buffer = new byte[lenght];
			Read(buffer, 0, lenght);
			return Encoding.Default.GetChars(buffer);
		}

		/// <summary>
		/// Read date from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual DateTime ReadDate()
		{
			long value = ReadInt64();
			return DateTime.FromBinary(value);
		}

		/// <summary>
		/// Read int16 from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual short ReadInt16()
		{
			byte[] fbuff = new byte[2];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			return BitConverter.ToInt16(fbuff, 0);
		}

		/// <summary>
		/// Read uint16 from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual ushort ReadUInt16()
		{
			byte[] fbuff = new byte[2];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			return BitConverter.ToUInt16(fbuff, 0);
		}

		/// <summary>
		/// Read int32 from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual int ReadInt32()
		{
			byte[] fbuff = new byte[4];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			return BitConverter.ToInt32(fbuff, 0);
		}

		/// <summary>
		/// Read uint32 from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual uint ReadUInt32()
		{
			byte[] fbuff = new byte[4];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			return BitConverter.ToUInt32(fbuff, 0);
		}

		/// <summary>
		/// Read int64 from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual long ReadInt64()
		{
			byte[] fbuff = new byte[8];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			fbuff[4] = ReadByte();
			fbuff[5] = ReadByte();
			fbuff[6] = ReadByte();
			fbuff[7] = ReadByte();
			return BitConverter.ToInt64(fbuff, 0);
		}

		/// <summary>
		/// Read uint64 from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual ulong ReadUInt64()
		{
			byte[] fbuff = new byte[8];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			fbuff[4] = ReadByte();
			fbuff[5] = ReadByte();
			fbuff[6] = ReadByte();
			fbuff[7] = ReadByte();
			return BitConverter.ToUInt64(fbuff, 0);
		}

		/// <summary>
		/// Read single from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual float ReadSingle()
		{
			byte[] fbuff = new byte[4];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			return BitConverter.ToSingle(fbuff, 0);
		}

		/// <summary>
		/// Read string with encoded size from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual string ReadString()
		{
			int stringLenght = Read7BitEncodedInt();
			byte[] buffer = new byte[stringLenght];
			Read(buffer, 0, buffer.Length);
			return Encoding.Default.GetString(buffer, 0, buffer.Length);
		}

		/// <summary>
		/// Read string from memory stream
		/// </summary>
		/// <param name="lenght">Lenght to read</param>
		/// <returns></returns>
		public virtual string ReadString(int lenght)
		{
			byte[] buffer = new byte[lenght];
			Read(buffer, 0, lenght);
			return Encoding.Default.GetString(buffer, 0, lenght);
		}

		/// <summary>
		/// Read string line from memory stream
		/// </summary>
		/// <returns></returns>
		public virtual string ReadLine()
		{
			StringBuilder sb = new StringBuilder();
			char ch;

			while ((ch = ReadChar()) != '\0')
			{
				if (ch == '\r' || ch == '\n')
					return sb.ToString();
				
				sb.Append(ch);
			} 
			return sb.ToString();
		}
		
		/// <summary>
		/// Skip offset from memory stream
		/// </summary>
		/// <param name="num"></param>
		public void Skip(long num)
		{
			Seek(num, SeekOrigin.Current);
		}

		#region Internal 

		/*
		 * Source : https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/binaryreader.cs
		 */
		 /// <summary>
		 /// Read encoded size from memory stream
		 /// </summary>
		 /// <returns></returns>
		internal protected int Read7BitEncodedInt()
		{
			int count = 0;
			int shift = 0;
			byte b;
			do
			{
				if (shift == 5 * 7) throw new FormatException("Format_Bad7BitInt32");
				
				b = ReadByte();
				count |= (b & 0x7F) << shift;
				shift += 7;
			} while ((b & 0x80) != 0);

			return count;
		}

		#endregion
	}
}
