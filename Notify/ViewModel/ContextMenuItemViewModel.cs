using System;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;

namespace ProductivityTool.Notify.ViewModel
{
    public class ContextMenuItemViewModel : ReactiveObject
    {
        public ICommand Command { get; }
        private string _header;

        public string Header
        {
            get => _header;
            set => this.RaiseAndSetIfChanged(ref _header, value);
        }

        private Image _icon;

        public Image Icon
        {
            get => _icon;
            set => this.RaiseAndSetIfChanged(ref _icon, value);
        }

        public ContextMenuItemViewModel(Action action)
        {
            if (action != null)
                Command = ReactiveCommand.Create(action);
        }

        public void SetIcon(string executeFile)
        {
            Icon = new Image
            {
                Source = FileToImageIconConverter.Icon(executeFile)
            };
        }
    }
}