using System;

namespace ProductivityTool.Notify.Model
{
    public class ApplicationModel
    {
        public Guid Id { get; }
        public string FileName { get; }

        public ApplicationModel()
        {
            Id = Guid.NewGuid();
        }
        public ApplicationModel(string fileName) : this()
        {
            FileName = fileName;
        }

        public ApplicationModel(Guid id, string fileName)
        {
            Id = id;
            FileName = fileName;
        }
    }
}