using System;

namespace AndroidLogViewer.Tests.Annotations
{
    /// <summary>
    /// Tests annotated with this attribute ensure that the functionality of a GitHub issue does not break.
    /// </summary>
    public class IssueAttribute : Attribute
    {
        public IssueAttribute(int issueId)
        {
            IssueId = issueId;
        }

        public int IssueId { get; private set; }
    }
}