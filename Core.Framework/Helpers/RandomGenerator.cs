using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Helpers
{
    public class RandomGenerator
    {
        public static int RandomNumber(int min, int max)
        {
            int seed = Environment.TickCount + GetGuidNumber();
            Random random = new Random(seed);
            return random.Next(min, max);
        }

        public static int GetGuidNumber()
        {
            string guid = Guid.NewGuid().ToString();
            int number = 0;
            foreach (char item in guid)
            {
                if (Char.IsNumber(item))
                    number += int.Parse(item.ToString());
            }
            return number;
        }

        public static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static string RandomNumberAndString(int size, bool lowerCase, int from, int to)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(size, lowerCase));
            builder.Append(RandomNumber(from, to));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        public static string RandomBase64String()
        {
            Guid g = Guid.NewGuid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            guidString = guidString.Replace("=", "");
            guidString = guidString.Replace("+", "");
            return guidString;
        }
        /*
         public static int GetCompetitionQuestionId(List<int> allQuestionIdList, List<int> userQuestionIdList, int notId)
         {
             int questionId = 0;
             List<int> newQuestions;

             var userQuestionGroupList = userQuestionIdList.GroupBy(q => q).Select(g => new { Key = g.Key, Count = g.Count() }).ToList();

             if (userQuestionGroupList.Count == 0)
                 return RandomFromList(allQuestionIdList, notId);

             int maxCount = userQuestionGroupList.Max(x => x.Count);
             List<int> notAsked = allQuestionIdList.Where(u => u.Count < maxCount).Select(d => d.Key).OrderBy(x => x).ToList();

             if (notAsked.Count == 0)
                 newQuestions = allQuestionIdList.Where(s => !userQuestionGroupList.Select(c => c.Key).Contains(s)).ToList();
             else
                 newQuestions = notAsked.Where(s => s != notId).ToList(); 

             if (newQuestions.Count == 0)
                 return RandomFromList(allQuestionIdList, notId);

             questionId = RandomFromList(newQuestions, notId);

             return questionId;
         }*/

        public static int GetCompetitionQuestionId(List<int> allQuestionIdList, List<int> userQuestionIdList, int notId)
        {
            int questionId = 0;
            List<int> newQuestions;

            var userQuestionGroupList = userQuestionIdList.GroupBy(q => q).Select(g => new { Key = g.Key, Count = g.Count() }).ToList();

            if (userQuestionGroupList.Count == 0)
                return RandomFromList(allQuestionIdList, notId);

            if (allQuestionIdList.Count == userQuestionGroupList.Count)
            {
                int minCount = userQuestionGroupList.Min(x => x.Count);
                newQuestions = userQuestionGroupList.Where(u => u.Count == minCount && u.Key != notId).Select(d => d.Key).OrderBy(x => x).ToList();
                newQuestions = newQuestions.Where(s => allQuestionIdList.Select(c => c).Contains(s) && s != notId).ToList();  
            }
            else
            {
                //newQuestions = allQuestionIdList.Where(s => !userQuestionGroupList.Select(c => c.Key).Contains(s)).ToList();
                newQuestions = allQuestionIdList.Where(s => !userQuestionGroupList.Select(c => c.Key).Contains(s) && s != notId).ToList();  
            }

            if (newQuestions.Count == 0)
                return RandomFromList(allQuestionIdList, notId);

            questionId = RandomFromList(newQuestions, notId);

            return questionId;
        }
        
        public static int RandomFromList(List<int> list, int notId)
        {
            int id = 0;
            bool notExist = true;

            Random rand = new Random(GetGuidNumber());
            while (notExist)
            {
                id = rand.Next(list.Min(), list.Max() + 1);
                if (list.Exists(x => x == id) && id != notId)
                    notExist = false;
            }
            return id;
        }
    }
}
