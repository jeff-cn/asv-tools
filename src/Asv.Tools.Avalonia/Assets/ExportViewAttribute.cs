using System;
using System.ComponentModel.Composition;
using Avalonia.Controls;

namespace Asv.Tools.Avalonia.Exports
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExportViewAttribute : ExportAttribute, IViewMetadata
    {
        public ExportViewAttribute(Type viewModelType)
            : base(null, typeof(IControl))
        {
            this.ViewModelType = viewModelType;
        }

        public Type ViewModelType { get; private set; }
    }

    public interface IViewMetadata
    {
        Type ViewModelType { get; }
    }
}
