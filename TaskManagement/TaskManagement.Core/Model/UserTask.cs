using ReactiveUI.Fody.Helpers;

namespace TaskManagement.Core.Model
{
    public class UserTask
    {
        [Reactive]
        public string Contents { get; set; }
        [Reactive]
        public UrgentLevel Urgent { get; set; }
        [Reactive]
        public ImportantLevel Important { get; set; }
    }
}
