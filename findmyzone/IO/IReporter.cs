namespace findmyzone.IO
{
    public interface IReporter
    {
        void OpEndError();
        void OpEndSuccess();
        void Info(string text, params string[] args);
        void StartOp(string text, params string[] args);
        void Error(string text, params string[] args);
    }
}