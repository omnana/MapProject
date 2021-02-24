
public class RestResult<T>
{
    //public T Result;
    //data
    public T data;
    //状态
    public bool status;
    /// Http错误代码
    public int code;
    /// 消息
    public string message;
}