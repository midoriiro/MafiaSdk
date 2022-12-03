using System.ComponentModel;

namespace Core.IO.ResourceFormats.StreamMap;

public class StreamLine
{
    string name;
    string group;
    string flags;
    public int nameIDX;
    public int lineID;
    public int groupID;
    int loadType;
    public int flagIDX;
    int unk5;
    ulong unk10;
    ulong unk11;
    int unk12;
    int unk13;
    int unk14;
    int unk15;
    public StreamLoader[] loadList;

    [Description("Name of line, this will be used in LUA scripts.")]
    public string Name {
        get { return name; }
        set { name = value; }
    }
    [Browsable(false)]
    public string Group {
        get { return group; }
        set { group = value; }
    }
    [Description("0 - 6, unknown what they do.")]
    public int LoadType {
        get { return loadType; }
        set { loadType = value; }
    }
    [Description("Flags, referencing SDSConfig.bin")]
    public string Flags {
        get { return flags; }
        set { flags = value; }
    }
    [Description("Hash #1")]
    public ulong Unk10 {
        get { return unk10; }
        set { unk10 = value; }
    }
    [Description("Hash #2")]
    public ulong Unk11 {
        get { return unk11; }
        set { unk11 = value; }
    }
    [Description("The assets which will loaded when this line is activated.")]
    public StreamLoader[] LoadList {
        get { return loadList; }
        set { loadList = value; }
    }
    public int Unk5 {
        get { return unk5; }
        set { unk5 = value; }
    }
    [Browsable(false)]
    public int Unk12 {
        get { return unk12; }
        set { unk12 = value; }
    }
    [Browsable(false)]
    public int Unk13 {
        get { return unk13; }
        set { unk13 = value; }
    }
    [Browsable(false)]
    public int Unk14 {
        get { return unk14; }
        set { unk14 = value; }
    }
    [Browsable(false)]
    public int Unk15 {
        get { return unk15; }
        set { unk15 = value; }
    }

    public StreamLine()
    {
        name = "New Line";
        loadList = new StreamLoader[0];
    }

    public StreamLine(StreamLine other)
    {
        name = other.name + "_duplicated";
        group = other.group;
        loadType = other.loadType;
        flags = other.flags;
        unk10 = other.unk10;
        unk11 = other.unk11;
        unk5 = other.unk5;
        unk12 = other.unk12;
        unk13 = other.unk13;
        unk14 = other.unk14;
        unk15 = other.unk15;

        loadList = new StreamLoader[other.loadList.Length];
        for (int i = 0; i < other.loadList.Length; i++)
        {
            loadList[i] = new StreamLoader(other.loadList[i]);
        }
    }


    public override string ToString()
    {
        return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11}", nameIDX, lineID, groupID, loadType, flagIDX, unk5, unk10, unk11, unk12, unk13, unk14, unk15);
    }
}