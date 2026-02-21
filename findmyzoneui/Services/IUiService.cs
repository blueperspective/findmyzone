using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace findmyzoneui.Services;

public interface IUiService
{
    Task<ButtonResult> Ask(string title, string message);
    Task ShowMessage(string title, string message);
    Task ShowException(string title, string message, System.Exception e);
    Task ShowMessage(string title, string message, Icon icon, ButtonEnum buttonEnum);
}