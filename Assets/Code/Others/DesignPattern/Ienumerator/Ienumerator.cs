
namespace OmnanaTest
{
    /// <summary>
    /// 迭代器模式
    /// </summary>
    public interface Ienumerator
    {
        bool MoveNext();

        object Current { get; }

        void Reset();
    }
}
