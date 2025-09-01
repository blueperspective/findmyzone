using System.Threading.Tasks;

public interface IAsyncInitialization
{
    Task Initialization { get; }
}