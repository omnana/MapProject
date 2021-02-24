using System;

public class TableMgrInit
{
    private static Type[] types = new Type[]
    {
        typeof(FiguresModelMgr),
    };

    public static void Init(Action callback)
    {
        var tableCount = types.Length;

        FiguresModelMgr.Ins = new FiguresModelMgr();

        FiguresModelMgr.Ins.Reload(callback);

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