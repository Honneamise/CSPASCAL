namespace Pascal;

public enum ArType
{
    PROGRAM,
    PROCEDURE
}

public class ActivationRecord
{
    public string Name { get; }
    public ArType Type { get; }
    public uint Level { get; }
    public Dictionary<string, float> Members { get; }

    public ActivationRecord(string name, ArType type, uint level)
    {
        Name = name;
        Type = type;
        Level = level;
        Members = new();
    }

    public void Set(string name, float value)
    {
        Members[name] = value;
    }

    public float Get(string name)
    {
        Members.TryGetValue(name, out float value);

        return value;
    }

    public override string ToString()
    {
        string s = $"<{Level.ToString()}><{Type.ToString()}><{Name}>";

        foreach (var item in Members)
        {
            s += "\n" + item.Key + ":" + item.Value.ToString();
        }

        return s;
    }
}
