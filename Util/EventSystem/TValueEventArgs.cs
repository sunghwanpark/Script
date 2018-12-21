using System;

public class TValueEventArgs<T> : EventArgs
{
    public T arg { get; set; }

    public TValueEventArgs(T value)
    {
        arg = value;
    }
}

public class TValueEventArgs<T1, T2> : EventArgs
{
    public T1 arg1 { get; set; }
    public T2 arg2 { get; set; }

    public TValueEventArgs(T1 value1, T2 value2)
    {
        arg1 = value1;
        arg2 = value2;
    }
}

public class TValueEventArgs<T1, T2, T3> : EventArgs
{
    public T1 arg1 { get; set; }
    public T2 arg2 { get; set; }
    public T3 arg3 { get; set; }

    public TValueEventArgs(T1 value1, T2 value2, T3 value3)
    {
        arg1 = value1;
        arg2 = value2;
        arg3 = value3;
    }
}