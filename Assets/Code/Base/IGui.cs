namespace Omnana
{
    public interface IGui
    {
        void DoInit();
        void DoOpen();
        void DoUpdate();
        void DoClose();
        void DoDestroy();
    }
}