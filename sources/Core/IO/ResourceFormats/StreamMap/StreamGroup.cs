using System.ComponentModel;

namespace Core.IO.ResourceFormats.StreamMap;

public class StreamGroup
{
    string name;
    public int nameIDX;
    GroupTypes type;
    int unk01;
    public int startOffset; //start
    public int endOffset; //end
    public int unk5;

    [ReadOnly(true)]
    public string Name {
        get { return name; }
        set { name = value; }
    }
    [ReadOnly(true)]
    public GroupTypes Type {
        get { return type; }
        set { type = value; }
    }
    public int Unk01 {
        get { return unk01; }
        set { unk01 = value; }
    }
    public int Unk05 {
        get { return unk5; }
        set { unk5 = value; }
    }

    public override string ToString()
    {
        return string.Format("{0} {1} {2} {3} {4} {5}", nameIDX, type, unk01, startOffset, endOffset, unk5);
    }
}