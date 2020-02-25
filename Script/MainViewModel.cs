using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

using IronPython.Hosting;

using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Script
{
    public class MainViewModel : ReactiveObject, IDisposable
    {
        private ScriptEngine _engine;
        private ScriptScope _scope;
        private MemoryStream _outputStream;
        private MemoryStream _errorStream;
        [Reactive]
        public TextDocument Document { get; set; }
        [Reactive]
        public string Output { get; set; }

        public MainViewModel()
        {
            _engine = Python.CreateEngine();
            _scope = _engine.CreateScope();

            Document = new TextDocument();
            Test = ReactiveCommand.CreateFromTask<Unit, string>((_) =>
            {
                var text = Document.Text;
                Mouse.OverrideCursor = Cursors.Wait;
                return Task.Run(() =>
                {
                    return ExecuteScript(text);
                });
            });

            this.WhenAnyObservable(x => x.Test)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(output =>
                {
                    Output = output;
                    Mouse.OverrideCursor = Cursors.Arrow;
                });
        }

        public ReactiveCommand<Unit, string> Test { get; set; }

        /// <summary>
        /// 작성된 스크립트를 수행 후 출력을 리턴
        /// </summary>
        /// <param name="txtScript"></param>
        /// <returns></returns>
        private string ExecuteScript(string txtScript)
        {
            StringBuilder builder = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            builder.AppendLine(GetFormattedString("start script"));


            _scope.ImportModule("clr");
            sw.Start();
            _engine.Execute("import clr", _scope);

#if ADD_REFERENCE
            engine.Execute($"clr.AddReference(\" {refernce}\")");
            engine.Execute($"from {reference} import *", scope);
#endif

            var script = _engine.CreateScriptSourceFromString(txtScript, SourceCodeKind.AutoDetect);

            using (var streamOut = new MemoryStream())
            using (var streamErr = new MemoryStream())
            {
                _engine.Runtime.IO.SetOutput(streamOut, Encoding.Default);
                _engine.Runtime.IO.SetErrorOutput(streamErr, Encoding.Default);
                try
                {
                    script.Execute(_scope);
                    sw.Stop();
                    builder.AppendLine(GetFormattedString($"elapse time is {sw.ElapsedMilliseconds} msec"));
                }
                catch (Exception e)
                {
                    builder.AppendLine(GetFormattedString($"error : {e.Message}"));
                }

                var output = Encoding.Default.GetString(streamOut.ToArray());
                var error = Encoding.Default.GetString(streamErr.ToArray());

                builder.AppendLine(output);
                builder.AppendLine(error);
            }

            return builder.ToString();
        }

        private string GetFormattedString(string msg)
        {
            return $"[{DateTime.Now.ToString("hh:mm:ss.fff")}]: {msg}";
        }

        public void Dispose()
        {
            
        }
    }
}
