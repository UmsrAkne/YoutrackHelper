using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using YouTrackSharp.Issues;

namespace YoutrackHelper.Models
{
    public class IssueWrapper : BindableBase, IIssue
    {
        private DateTime createdAt;
        private string status;
        private bool completed;
        private string shortName = string.Empty;
        private bool expanded;
        private TimeSpan workingDuration;
        private string temporaryComment = string.Empty;

        public IssueWrapper(Issue issue)
        {
            SetIssue(issue);
        }

        public string Title
        {
            get => Issue.Summary;
            set
            {
                RaisePropertyChanged();
                Issue.Summary = value;
            }
        }

        public DateTime CreatedAt
        {
            get
            {
                var c = Issue.Fields.FirstOrDefault(f => f.Name == "created");
                if (c != null)
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                    return TimeZoneInfo
                        .ConvertTime(DateTimeOffset.FromUnixTimeMilliseconds((long)c.Value), tz)
                        .DateTime;
                }

                return createdAt;
            }
            set => createdAt = value;
        }

        public bool Completed { get => completed; set => SetProperty(ref completed, value); }

        public string Status
        {
            get
            {
                var f = Issue.Fields.FirstOrDefault(f => f.Name == "State");
                if (f != null)
                {
                    var token = ((JArray)f.ValueId)[0];
                    var str = token.ToString();
                    Completed = str == "完了";
                    return str;
                }

                return status;
            }
            set => SetProperty(ref status, value);
        }

        public string ShortName { get => shortName; set => SetProperty(ref shortName, value); }

        public string TemporaryComment { get => temporaryComment; set => SetProperty(ref temporaryComment, value); }

        public List<Comment> Comments { get; set; } = new ();

        public List<Comment> RecentComments =>
            Comments.Count != 0
                ? Comments.OrderByDescending(c => c.Created)
                    .Take(DisplayRecentCommentCount)
                    .ToList()
                : new List<Comment>();

        public int DisplayRecentCommentCount { get; set; } = 10;

        public string FullName => $"{ShortName} {Title}";

        public bool Expanded { get => expanded; set => SetProperty(ref expanded, value); }

        public TimeSpan WorkingDuration { get => workingDuration; set => SetProperty(ref workingDuration, value); }

        public TimeSpan RegisteredWorkingDuration
        {
            get
            {
                var f = Issue?.Fields.FirstOrDefault(f => f.Name == "経過時間");
                var list = (List<string>)f?.Value;
                var val = list?.FirstOrDefault();
                if (val == null || !int.TryParse(val, out var durationMinute))
                {
                    return TimeSpan.Zero;
                }

                return TimeSpan.FromMinutes(durationMinute);
            }
        }

        public DelegateCommand ChangeVisibilityCommand => new (() =>
        {
            Expanded = !Expanded;
        });

        private DateTime StartedAt { get; set; }

        private Issue Issue { get; set; }

        public void UpdateWorkingDuration(DateTime dateTime)
        {
            if (StartedAt == DateTime.MinValue)
            {
                WorkingDuration = TimeSpan.Zero;
                return;
            }

            WorkingDuration = dateTime - StartedAt;
        }

        /// <summary>
        /// この課題の状態を変更します。
        /// 現在の実装では、状態の変更は 未完了 と 作業中 の２つの間で切り替わります。
        /// </summary>
        /// <param name="connector">Youtrack サーバーに接続するための Connector</param>
        /// <param name="counter">作業時間を記録中の TimeCounter</param>
        /// <returns>Task</returns>
        public async Task ToggleStatus(Connector connector, TimeCounter counter)
        {
            Logger.WriteMessageToFile($"課題の状態を変更 {FullName} 現在の状態 : {Status}");

            if (Status == "未完了")
            {
                counter.StartTimeTracking(shortName, DateTime.Now);
            }

            var comment = string.Empty;
            if (counter.IsTrackingNameRegistered(ShortName) && Status == "作業中")
            {
                var now = DateTime.Now;
                var duration = counter.FinishTimeTracking(shortName, now);
                var startedAt = now - duration;
                const string f = "yyyy/MM/dd HH:mm";
                comment = $"中断 作業時間 {(int)duration.TotalMinutes} min ({startedAt.ToString(f)} - {now.ToString(f)})";
                if (duration.TotalSeconds > 60)
                {
                    await connector.ApplyCommand(ShortName, $"作業 {(int)duration.TotalMinutes}m", string.Empty);
                }
            }

            switch (Status)
            {
                case "未完了":
                    SetIssue(await connector.ApplyCommand(ShortName, "state 作業中", comment));
                    StartedAt = DateTime.Now;
                    return;
                case "作業中":
                    SetIssue(await connector.ApplyCommand(ShortName, "state 未完了", comment));
                    StartedAt = DateTime.MinValue;
                    UpdateWorkingDuration(DateTime.Now);
                    break;
            }
        }

        public async Task Complete(Connector connector, TimeCounter counter)
        {
            Logger.WriteMessageToFile($"課題を完了 {FullName}");
            var comment = string.Empty;
            if (counter.IsTrackingNameRegistered(ShortName))
            {
                var now = DateTime.Now;
                var duration = counter.FinishTimeTracking(shortName, now);
                var startedAt = now - duration;
                const string f = "yyyy/MM/dd HH:mm";
                comment = $"完了 作業時間 {(int)duration.TotalMinutes} min ({startedAt.ToString(f)} - {now.ToString(f)})";
                if (duration.TotalSeconds > 60)
                {
                    await connector.ApplyCommand(ShortName, $"作業 {(int)duration.TotalMinutes}m", string.Empty);
                }
            }

            SetIssue(await connector.ApplyCommand(ShortName, "state 完了", comment));
            StartedAt = DateTime.MinValue;
            UpdateWorkingDuration(DateTime.Now);
        }

        public void SetIssue(Issue issue)
        {
            Issue = issue;
            if (Issue == null)
            {
                return;
            }

            ShortName = Issue.Id;
            var f = Issue.Fields.FirstOrDefault(f => f.Name == "State");
            if (f != null)
            {
                Completed = ((JArray)f.ValueId)[0].ToString() == "完了";
            }

            RaisePropertyChanged(nameof(Status));

            Comments = Issue.Comments
                .OrderByDescending(c => c.Created)
                .ToList();

            RaisePropertyChanged(nameof(Comments));
            RaisePropertyChanged(nameof(RecentComments));
        }
    }
}