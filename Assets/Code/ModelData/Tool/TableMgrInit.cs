using System;

public class TableMgrInit
{
    private static Type[] types = new Type[]
    {
        typeof(TestModelMgr),
    };

    public static void Init(Action callback)
    {
        var tableCount = types.Length;

        TestModelMgr.Ins = new TestModelMgr();

        TestModelMgr.Ins.Reload(callback);

        //foreach (var t in types)
        //{
        //    var m = Activator.CreateInstance(t);

        //    if (m is TableManager<ITableModel, ITableManager<ITableModel>> mgr)
        //    {
        //        mgr.Init();
        //    }
        //}
    }
}