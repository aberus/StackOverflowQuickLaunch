using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aberus.StackOverflowQuickLaunch
{
    [DataContract]
    public class StackOverflowSearchResult
    {
        [DataMember(Name = "items")]
        public List<Item> Items { get; set; }
    }

    [DataContract]
    public class Item
    {
        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "is_answered")]
        public bool IsAnswered { get; set; }

        [DataMember(Name = "view_count")]
        public int ViewCount { get; set; }

        [DataMember(Name = "answer_count")]
        public int AnswerCount { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }

        [DataMember(Name = "last_activity_date")]
        public int LastActivityDate { get; set; }

        [DataMember(Name = "creation_date")]
        public int CreationDate { get; set; }

        [DataMember(Name = "last_edit_date")]
        public int LastEditDate { get; set; }

        [DataMember(Name = "question_id")]
        public int QuestionId { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "protected_date")]
        public int? ProtectedDate { get; set; }

        [DataMember(Name = "accepted_answer_id")]
        public int? AcceptedAnswerId { get; set; }

        [DataMember(Name = "closed_date")]
        public int? ClosedDate { get; set; }

        [DataMember(Name = "closed_reason")]
        public string ClosedReason { get; set; }
    }

}
