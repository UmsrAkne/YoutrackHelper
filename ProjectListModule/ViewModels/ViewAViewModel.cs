using System.Collections.Generic;
using Prism.Mvvm;
using YouTrackSharp.Projects;

namespace ProjectListModule.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ViewAViewModel : BindableBase
    {
        public List<Project> Projects { get; set; }
    }
}