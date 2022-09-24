
/// <summary>
/// C#类型的单例基类
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonBase<T> where T: new()
{
    private static T _instance;

    public static T GetInstance()
    {
        if(_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }
}