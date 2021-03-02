using System;

public class TableMgrInit
{
    private static Type[] types = new Type[]
    {
        typeof(TestModelMgr),
    };

    public static void Load()
    {
        TestModelMgr.Ins = new TestModelMgr();
        TestModelMgr.Ins.Init();
    }
}