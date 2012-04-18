using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackApi
{
    public class AnswerOwner
    {
        public int user_id { get; set; }
        public string user_type { get; set; }
        public string display_name { get; set; }
        public int reputation { get; set; }
        public string email_hash { get; set; }
    }

    public class Answer
    {
        public int answer_id { get; set; }
        public bool accepted { get; set; }
        public string answer_comments_url { get; set; }
        public int question_id { get; set; }
        public AnswerOwner owner { get; set; }
        public int creation_date { get; set; }
        public int last_activity_date { get; set; }
        public int up_vote_count { get; set; }
        public int down_vote_count { get; set; }
        public int view_count { get; set; }
        public int score { get; set; }
        public bool community_owned { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public int? last_edit_date { get; set; }
    }

    public class AnswerObject
    {
        public int total { get; set; }
        public int page { get; set; }
        public int pagesize { get; set; }
        public Answer[] answers { get; set; }
    }

    public class QuestionObject
    {
        public int total { get; set; }
        public int page { get; set; }
        public int pagesize { get; set; }
        public Question[] questions { get; set; }
    }

    public class QuestionOwner
    {
        public int user_id { get; set; }
        public string user_type { get; set; }
        public string display_name { get; set; }
        public int reputation { get; set; }
        public string email_hash { get; set; }
    }

    public class Question
    {
        public string[] tags { get; set; }
        public int answer_count { get; set; }
        public Answer[] answers { get; set; }
        public int accepted_answer_id { get; set; }
        public int favorite_count { get; set; }
        public string question_timeline_url { get; set; }
        public string question_comments_url { get; set; }
        public string question_answers_url { get; set; }
        public int question_id { get; set; }
        public int creation_date { get; set; }
        public int last_edit_date { get; set; }
        public int last_activity_date { get; set; }
        public int up_vote_count { get; set; }
        public int down_vote_count { get; set; }
        public int view_count { get; set; }
        public int score { get; set; }
        public bool community_owned { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public QuestionOwner owner { get; set; }
        public int? closed_date { get; set; }
        public string closed_reason { get; set; }
    }

    public class RootObject
    {
        public Question[] questions { get; set; }
        public string bugId { get; set; }
    }
}
