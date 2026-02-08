using CommunityToolkit.Mvvm.Input;
using FirstAndriodApp.Models;

namespace FirstAndriodApp.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}