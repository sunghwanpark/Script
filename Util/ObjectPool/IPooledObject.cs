public interface IPooledObject<T> where T : UnityEngine.Object
{
    T GetPoolItem(string objName);
}