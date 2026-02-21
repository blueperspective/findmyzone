namespace findmyzone
{
    interface IReporter
    {
        void OpEndError();
        void OpEndSuccess();
        void Info(string text, params string[] args);
        void StartOp(string text, params string[] args);
    }
}