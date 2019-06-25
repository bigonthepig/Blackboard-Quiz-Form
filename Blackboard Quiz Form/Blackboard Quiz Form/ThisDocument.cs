﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Office.Tools.Word;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;

namespace Blackboard_Quiz_Form
{
    public class Question
    {
        public Word.ContentControl QuestionItem { get; set; }
        public int QuestionNumber { get; set; }
        public float QuestionPosition { get; set; }

    }
    public partial class ThisDocument
    {
        private List<Question> questionList = new List<Question>();
        private void ThisDocument_Startup(object sender, System.EventArgs e)
        {
            Question newQuestion = new Question();
            newQuestion.QuestionItem = SelectContentControlsByTag("question")[1];
            newQuestion.QuestionNumber = 1;
            questionList.Add(newQuestion);
        }
        private void ThisDocument_Shutdown(object sender, System.EventArgs e)
        {
        }
        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.ContentControlAfterAdd += new Microsoft.Office.Interop.Word.DocumentEvents2_ContentControlAfterAddEventHandler(this.ThisDocument_ContentControlAfterAdd);
            this.ContentControlBeforeDelete += new Microsoft.Office.Interop.Word.DocumentEvents2_ContentControlBeforeDeleteEventHandler(this.ThisDocument_ContentControlBeforeDelete);
            this.Startup += new System.EventHandler(this.ThisDocument_Startup);
            this.Shutdown += new System.EventHandler(this.ThisDocument_Shutdown);

        }

        #endregion
        private void ThisDocument_ContentControlAfterAdd(Word.ContentControl NewContentControl, bool InUndoRedo)
        {
            if (NewContentControl.Tag == "question" && InUndoRedo == false)
            {
                Question NewQuestion = new Question();
                NewQuestion.QuestionItem = NewContentControl;
                questionList.Add(NewQuestion);
                NewQuestion.QuestionPosition = NewQuestion.QuestionItem.Range.Information[Word.WdInformation.wdVerticalPositionRelativeToPage] + NewQuestion.QuestionItem.Range.Information[Word.WdInformation.wdActiveEndPageNumber] * 1000;
                var questionsAbove = questionList.FindAll(x => x.QuestionPosition >= NewQuestion.QuestionPosition);
                if(questionsAbove.Count !=0)
                { 
                    foreach(Question q in questionsAbove)
                    {
                        q.QuestionPosition = q.QuestionItem.Range.Information[Word.WdInformation.wdVerticalPositionRelativeToPage] + q.QuestionItem.Range.Information[Word.WdInformation.wdActiveEndPageNumber] * 1000;
                    }
                }
                var orderedQuestions = questionList.OrderBy(o => o.QuestionPosition).ToList();
                foreach (Question q in orderedQuestions)
                {
                        q.QuestionNumber = orderedQuestions.IndexOf(q) + 1;
                        q.QuestionItem.Title = "Question " + q.QuestionNumber;
                }
            }
            if(InUndoRedo == true)
            {
                foreach (Question q in questionList)
                {
                    if (q.QuestionItem == NewContentControl)
                    {
                        Question questionToRemove = q;
                        List<Question> questionsAbove = questionList.FindAll(x => x.QuestionPosition >= questionToRemove.QuestionPosition);
                        questionList.Remove(questionToRemove);
                        if (questionsAbove.Count != 0)
                        {
                            foreach (Question question in questionsAbove)
                            {
                                q.QuestionPosition = q.QuestionItem.Range.Information[Word.WdInformation.wdVerticalPositionRelativeToPage] + q.QuestionItem.Range.Information[Word.WdInformation.wdActiveEndPageNumber] * 1000;
                            }
                        }
                        List<Question> orderedQuestions = questionList.OrderBy(o => o.QuestionPosition).ToList();
                        foreach (Question question in orderedQuestions)
                        {
                            q.QuestionNumber = orderedQuestions.IndexOf(q) + 1;
                            q.QuestionItem.Title = "Question " + q.QuestionNumber;
                        }
                    }
                }
            }
        }
        private void ThisDocument_ContentControlBeforeDelete(Word.ContentControl OldContentControl, bool InUndoRedo)
        {
            if(OldContentControl.Tag == "question" & InUndoRedo == false)
            {
                string questionTitle = OldContentControl.Title;
                string resultString = Regex.Match(questionTitle, @"\d+").Value;
                int qNo = Int32.Parse(resultString);
                Debug.WriteLine(qNo);
                Question questionToRemove = questionList[qNo - 1];
                List<Question> questionsAbove = questionList.FindAll(x => x.QuestionPosition >= questionToRemove.QuestionPosition);
                questionList.Remove(questionToRemove);
                questionsAbove.Remove(questionToRemove);
                if (questionsAbove.Count != 0)
                {
                    foreach (Question q in questionsAbove)
                    {
                        q.QuestionPosition = q.QuestionItem.Range.Information[Word.WdInformation.wdVerticalPositionRelativeToPage] + q.QuestionItem.Range.Information[Word.WdInformation.wdActiveEndPageNumber] * 1000;
                    }
                }
                List<Question> orderedQuestions = questionList.OrderBy(o => o.QuestionPosition).ToList();
                foreach (Question q in orderedQuestions)
                {
                    q.QuestionNumber = orderedQuestions.IndexOf(q) + 1;
                    q.QuestionItem.Title = "Question " + q.QuestionNumber;
                }                                                                                   
            }
        }
    }
}
