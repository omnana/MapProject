using System.Collections.Generic;

public interface ITableManager<T> where T : ITableModel
{
    string TableName();

    void InitModel(T model, Dictionary<string, object> cellMap);

    object TableData { get; }
}