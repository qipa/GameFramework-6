using System;
public class Singleton<T>  where T : new() 
{
    private static T _instace;
    private static readonly object sync = new object();

    public static T Instance
    {
        get
        {
            if (_instace == null)
            {
                lock (sync)
                {
                    if (_instace == null)
                        _instace = new T();

                }
            }
            return _instace;
        }
    }

}
