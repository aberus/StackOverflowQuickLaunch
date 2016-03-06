using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aberus.StackOverflowQuickLaunch
{
    [DataContract]
    public class StackOverflowSearchResult
    {
        [DataMember(Name = "items")]
        public Item[] Items { get; set; }

        [DataMember(Name = "has_more")]
        public bool HasMore { get; set; }

        [DataMember(Name = "quota_max")]
        public int QuotaMax { get; set; }

        [DataMember(Name = "quota_remaining")]
        public int QuotaRemaining { get; set; }

        [DataMember(Name = "backoff")]
        public int? Backoff { get; set; }
        
        [DataMember(Name = "error_id")] //throttle_violation – 502 
        public int? ErrorId { get; set; }

        [DataMember(Name = "error_message")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "error_name")]
        public string ErrorName { get; set; }
    }

    [DataContract]
    public class Item
    {
        [DataMember(Name = "tags")]
        public List<string> Tags { get; set; }

        [DataMember(Name = "question_score")]
        public int? QuestionScore { get; set; }

        [DataMember(Name = "is_accepted")]
        public bool? IsAccepted { get; set; }

        [DataMember(Name = "has_accepted_answer")]
        public bool? HasAcceptedAnswer { get; set; }

        [DataMember(Name = "answer_count")]
        public int? AnswerCount { get; set; }

        [DataMember(Name = "is_answered")]
        public bool? IsAnswered { get; set; }

        [DataMember(Name = "question_id")]
        public int? QuestionId { get; set; }

        [DataMember(Name = "item_type")]
        public string ItemTypeJson { get; set; }

        [IgnoreDataMember]
        public ItemType ItemType { get; set; }

        [DataMember(Name = "score")]
        public int Score { get; set; }

        [DataMember(Name = "last_activity_date")]
        public int? LastActivityDateJson { get; set; }

        [IgnoreDataMember] //(Name = "last_activity_date")]
        public DateTime? LastActivityDate { get; set; }

        [DataMember(Name = "creation_date")]
        public int? CreationDateJson { get; set; }

        [IgnoreDataMember]
        public DateTime? CreationDate { get; set; }

        [DataMember(Name = "body")]
        public string Body { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "excerpt")]
        public string Excerpt { get; set; }

        //[DataMember(Name = "last_edit_date")]
        //public int LastEditDate { get; set; }


        //[DataMember(Name = "protected_date")]
        //public int? ProtectedDate { get; set; }

        //[DataMember(Name = "accepted_answer_id")]
        //public int? AcceptedAnswerId { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            try
            {
                LastActivityDate = LastActivityDateJson.HasValue ? LastActivityDateJson.Value.ToDateTime() : (DateTime?)null;
            }
            catch (FormatException)
            {
                LastActivityDate = null;
            }

            try
            {
                CreationDate = CreationDateJson.HasValue ? CreationDateJson.Value.ToDateTime() : (DateTime?)null;
            }
            catch (FormatException)
            {
                CreationDate = null;
            }

            try
            {
                ItemType = ItemTypeJson.ToEnum<ItemType>();
            }
            catch (Exception)
            {
                ItemType = ItemType.None;    
                
            }
          
        }
    }

    [DataContract(Name = "item_type")]
    public enum ItemType
    {
        [EnumMember(Value = "question")]
        Question,
        [EnumMember(Value = "answer")]
        Answer,
        [IgnoreDataMember]
        None
    }

    public static class DataModelExtension
    {

        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum) 
            {
                  throw new ArgumentException("T must be an enumerated type");
            }

            foreach(var enumValue in Enum.GetValues(typeof(T)))
            {
                System.Reflection.FieldInfo fi = typeof(T).GetField(enumValue.ToString());
                EnumMemberAttribute[] attrs = fi.GetCustomAttributes(typeof(EnumMemberAttribute), false) as EnumMemberAttribute[];
                if (attrs.Length > 0)
                {
                    if (attrs[0].Value == value)
                    {
                        return (T)enumValue;
                    }
                }
            }
            
            return default(T);
        }



        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a DateTime to its Unix timestamp value. This is the number of seconds
        /// passed since the Unix Epoch (1/1/1970 UTC)
        /// </summary>
        /// <param name="aDate">DateTime to convert</param>
        /// <returns>Number of seconds passed since 1/1/1970 UTC </returns>
        public static int ToInt(this DateTime aDate)
        {
            if (aDate == DateTime.MinValue)
            {
                return -1;
            }

            TimeSpan span = (aDate - UnixEpoch);
            
            return (int)Math.Floor(span.TotalSeconds);
        }

        /// <summary>
        /// Converts the specified 32 bit integer to a DateTime based on the number of seconds
        /// since the Unix epoch (1/1/1970 UTC)
        /// </summary>
        /// <param name="anInt">Integer value to convert</param>
        /// <returns>DateTime for the Unix int time value</returns>
        public static DateTime ToDateTime(this int anInt)
        {
            if (anInt == -1)
            {
                return DateTime.MinValue;
            }

            return UnixEpoch.AddSeconds(anInt);
        }
    }
}
