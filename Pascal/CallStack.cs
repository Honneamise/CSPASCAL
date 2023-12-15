namespace Pascal;

public class CallStack
{
    Stack<ActivationRecord> records;

    public CallStack()
    {
        records = new();
    }

    public void Push(ActivationRecord record)
    {
        records.Push(record);
    }

    public ActivationRecord Pop()
    {
        return records.Pop();
    }

    public ActivationRecord Peek()
    {
        return records.Peek();
    }

    public override string ToString()
    {
        string s = "[STACK]\n";

        foreach (ActivationRecord record in records)
        {
            s += record.ToString() + "\n";
        }

        return s;
    }
}
