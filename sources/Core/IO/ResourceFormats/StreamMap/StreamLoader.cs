using System.ComponentModel;

namespace Core.IO.ResourceFormats.StreamMap;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class StreamLoader
{
    string path;
    string entity;
    public int start;
    public int end;
    private GroupTypes type;
    private int loaderSubID;
    private int loaderID;
    private int loadType;
    public int pathIDX;
    public int entityIDX;
    private string preferredGroup;
    private string assignedGroup;

    [Description("Loading type, 0 - 6.")]
    public int LoadType {
        get { return loadType; }
        set { loadType = value; }
    }
    [Description("Path to the asset.")]
    public string Path {
        get { return path; }
        set { path = value; }
    }
    [Description("Entity name to use in scripts or 'City-1'")]
    public string Entity {
        get { return entity; }
        set { entity = value; }
    }
    public int LoaderSubID {
        get { return loaderSubID; }
        set { loaderSubID = value; }
    }
    [Browsable(false)]
    public int LoaderID {
        get { return loaderID; }
        set { loaderID = value; }
    }
    [Browsable(false)]
    public int GroupID { get; set; }
    [ReadOnly(true), Description("The Assigned group for this line. This is determined when saving the Stream Map file.")]
    public string AssignedGroup {
        get { return assignedGroup; }
        set { assignedGroup = value; preferredGroup = ""; }
    }
    [Description("When this is set, the 'Type' is ignored and the editor will automatically assign this line to the preferred group. The list is available under the 'Stream Groups' Tab.")]
    public string PreferredGroup {
        set { preferredGroup = value; assignedGroup = ""; }
        get { return preferredGroup; }
    }
    [Description("The group this asset is under, every group can be seen under 'Stream Groups'")]
    public GroupTypes Type {
        get { return type; }
        set { type = value; preferredGroup = ""; }
    }


    public StreamLoader()
    {
        GroupID = -1;
    }
    public StreamLoader(StreamLoader other)
    {
        path = other.path;
        entity = other.entity;
        GroupID = other.GroupID;
        loadType = other.loadType;
        loaderSubID = other.loaderSubID;
        AssignedGroup = other.AssignedGroup;
        type = other.type;
    }

    public override string ToString()
    {
        return string.Format("{0} {1} {2} {3}", LoadType, path, entity, Type);
    }

    public override int GetHashCode()
    {
        var hashCode = 1843194125;
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(path);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(entity);
        hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AssignedGroup);
        hashCode = hashCode * -1521134295 + loadType.GetHashCode();
        return hashCode;
    }
}