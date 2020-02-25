using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace ScriptApp
{
    public class MainViewModel : ReactiveObject
    {
        [Reactive]
        public TextDocument Document { get; set; }
        [Reactive]
        public string Text { get; set; }

        public MainViewModel()
        {
            Document = new TextDocument();

            Test = ReactiveCommand.Create(() =>
            {
                Text = Document.Text;
            });

            this.WhenAnyValue(v => v.Text).ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    Console.WriteLine(x);
                });
        }

        public ReactiveCommand<Unit,Unit> Test { get; set; }
    }
}
