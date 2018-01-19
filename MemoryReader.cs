using System;
using System.IO;
using System.Text;

namespace MapCore
{
	public class MemoryReader : MemoryStream
	{
		public MemoryReader(byte[] buffer) : base(buffer) { }
		
		public virtual bool ReadBoolean()
		{
			byte[] fbuff = new byte[1];
			fbuff[0] = ReadByte();
			return BitConverter.ToBoolean(fbuff, 0);
		}
		
		public virtual new byte ReadByte()
		{
			return (byte)base.ReadByte();
		}
		
		public virtual sbyte ReadSByte()
		{
			return (sbyte)ReadByte();
		}
		
		public virtual byte[] ReadBytes(int lenght)
		{
			byte[] buffer = new byte[lenght];
			Read(buffer, 0, lenght);
			return buffer;
		}
		
		public virtual char ReadChar(int lenght)
		{
			byte[] buffer = new byte[lenght];
			Read(buffer, 0, lenght);
			return BitConverter.ToChar(buffer, 0);
		}
		
		public virtual DateTime ReadDate()
		{
			long value = ReadInt64();
			return DateTime.FromBinary(value);
		}
		
		public virtual short ReadInt16()
		{
			byte[] fbuff = new byte[2];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			return BitConverter.ToInt16(fbuff, 0);
		}
		
		public virtual ushort ReadUInt16()
		{
			byte[] fbuff = new byte[2];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			return BitConverter.ToUInt16(fbuff, 0);
		}
		
		public virtual int ReadInt32()
		{
			byte[] fbuff = new byte[4];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			return BitConverter.ToInt32(fbuff, 0);
		}
		
		public virtual uint ReadUInt32()
		{
			byte[] fbuff = new byte[4];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			return BitConverter.ToUInt32(fbuff, 0);
		}
		
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
		
		public virtual float ReadSingle()
		{
			byte[] fbuff = new byte[4];
			fbuff[0] = ReadByte();
			fbuff[1] = ReadByte();
			fbuff[2] = ReadByte();
			fbuff[3] = ReadByte();
			return BitConverter.ToSingle(fbuff, 0);
		}
		
		public virtual string ReadString()
		{
			int stringLenght = Read7BitEncodedInt();
			byte[] buffer = new byte[stringLenght];
			Read(buffer, 0, buffer.Length);
			return Encoding.Default.GetString(buffer, 0, buffer.Length);
		}
		
		public virtual string ReadString(int lenght)
		{
			byte[] buffer = new byte[lenght];
			Read(buffer, 0, lenght);
			return Encoding.Default.GetString(buffer, 0, lenght);
		}
		
		public void Skip(long num)
		{
			Seek(num, SeekOrigin.Current);
		}

		#region Internal 

		/*
		 * Source : https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/binaryreader.cs
		 */
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
