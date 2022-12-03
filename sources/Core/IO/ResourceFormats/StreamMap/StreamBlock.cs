namespace Core.IO.ResourceFormats.StreamMap;

public class StreamBlock
{
    public int startOffset;
    public int endOffset;
    ulong[] hashes;

    public ulong[] Hashes {
        get { return hashes; }
        set { hashes = value; }
    }

    public StreamBlock()
    {
        startOffset = 0;
        endOffset = 0;
        hashes = new ulong[endOffset - startOffset];
    }

    public override string ToString()
    {
        return string.Format("{0} {1}", startOffset, endOffset);
    }
}