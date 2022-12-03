using System.Text;
using Core.IO.ResourceFormats.Extensions;

namespace Core.IO.ResourceFormats.XBin
{
    public class XBinWriter : BinaryWriter
    {
        private struct StringPointer
        {
            public long FileOffset { get; set; }
            public string RequiredString { get; set; }
        }

        private string _rawStringBuffer = string.Empty;
        private readonly Dictionary<string, int> _stringBuffer = new();
        private readonly Dictionary<string, long> _objectPointersToFix = new();
        private readonly List<StringPointer> _pointersToFix = new();

        public XBinWriter(Stream output) : base(output)
        {
        }

        public XBinWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public XBinWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
        }

        public override void Close()
        {
            if (_pointersToFix.Count > 0)
            {
                throw new InvalidDataException("Still have string pointers to fix");
            }
            
            if (_objectPointersToFix.Count > 0)
            {
                throw new InvalidDataException("Still have object pointers to fix");
            }

            base.Close();
        }

        // Functions to help XBin
        public void PushStringPointer(string text)
        {
            // Create a Ptr struct for the string
            var pointer = new StringPointer
            {
                FileOffset = BaseStream.Position,
                RequiredString = text
            };

            // Push the pointer to the array.
            _pointersToFix.Add(pointer);

            // Write temporary -1
            Write(-1);
        }

        public void PushObjectPointer(string identifier)
        {
            // Push object to the array
            _objectPointersToFix.Add(identifier, BaseStream.Position);

            // Write temporary -1
            Write(-1);
        }

        public long GetObjectPointerOffset(string identifier)
        {
            if (!_objectPointersToFix.TryGetValue(identifier, out long value))
            {
                throw new KeyNotFoundException($"Cannot found identifier '{identifier}'");
            }

            return value;
        }

        public void FixUpObjectPointer(string identifier)
        {
            if (!_objectPointersToFix.TryGetValue(identifier, out long value))
            {
                return;
            }
            
            long currentPosition = BaseStream.Position;

            BaseStream.Seek(value, SeekOrigin.Begin);
            var valueToWrite = (uint)(currentPosition - value);
                
            Write(valueToWrite);

            BaseStream.Seek(currentPosition, SeekOrigin.Begin);

            _objectPointersToFix.Remove(identifier);
        }
        
        public void FixUpObjectPointerWithValue(string identifier, long value)
        {
            if (!_objectPointersToFix.TryGetValue(identifier, out long position))
            {
                return;
            }
            
            long currentPosition = BaseStream.Position;

            BaseStream.Seek(position, SeekOrigin.Begin);
            var valueToWrite = (uint)value;

            Write(valueToWrite);

            BaseStream.Seek(currentPosition, SeekOrigin.Begin);

            _objectPointersToFix.Remove(identifier);
        }
        
        public void FixUpStringPointer(string identifier)
        { 
            int index = _pointersToFix.FindIndex(stringPointer => stringPointer.RequiredString == identifier);

            if (index == -1)
            {
                throw new InvalidOperationException($"Cannot find pointer for identifier '{identifier}'");
            }
            
            StringPointer pointer = _pointersToFix[index];
            GetBufferOffsetForString(pointer.RequiredString);
        }

        public void FixUpStringPointers()
        {
            long bufferStartOffset = BaseStream.Position;

            foreach(StringPointer pointer in _pointersToFix)
            {
                int offset = GetBufferOffsetForString(pointer.RequiredString);
            
                BaseStream.Seek(pointer.FileOffset, SeekOrigin.Begin);
                var stringOffset = (int)(bufferStartOffset + offset);
                var gap = (int)(stringOffset - pointer.FileOffset);
                Write(gap);
            }

            BaseStream.Seek(bufferStartOffset, SeekOrigin.Begin);
            this.WriteString(_rawStringBuffer, trail: false);
            _pointersToFix.Clear();
        }

        private void AddStringToBuffer(string text)
        {
            _stringBuffer.Add(text, _rawStringBuffer.Length);
            _rawStringBuffer += (text + '\0');
        }

        private int GetBufferOffsetForString(string text)
        {
            if(_stringBuffer.ContainsKey(text))
            {
                return _stringBuffer[text];
            }

            AddStringToBuffer(text);
            return _stringBuffer[text];
        }
    }
}
