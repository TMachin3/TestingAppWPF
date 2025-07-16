using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingAppWPF
{
    public enum questionType { textBox, checkBox, radioButton }
    public struct QuizResults
    {
        public Grade FinalGrade { get; set; }
        public int TotalScore { get; set; }
        public int CorrectAnswersCount { get; set; }
        public int IncorrectAnswersCount { get; set; }
        public int TotalPenalty { get; set; }
    }
    public class Answer
    {
        public string Content { get; set; } = string.Empty;
        public int Score { get; set; }
        public bool Correct { get; set; }
        public bool IsUserSelected { get; set; } = false;
        public virtual int Award
        {
            get
            {
                return Score > 0 ? Score : 0;
            }
        }
        public virtual int Penalty
        {
            get
            {
                return Score < 0 ? Math.Abs(Score) : 0;
            }
        }
    }
    public class Question
    {
        public string Content { get; set; } = string.Empty;
        public Answer[]? Answers { get; set; }
        public questionType QuestionType
        {
            get; set;
            /* Old virtual method, replaced with static declaration for flexibility, kept for reference only
            get
            {
                if (this.Answers.Length == 0) { return questionType.textBox; }
                else
                {
                    int trueAnswerCount = 0;
                    foreach (Answer answer in Answers) { if (answer.Correct) { trueAnswerCount++; } }
            If there are only true answers, define as textBox
                    if (this.Answers.Length == trueAnswerCount) { return questionType.textBox; }
            If there is only one true answer, define as radioButton
                    else if (trueAnswerCount == 1) { return questionType.radioButton; }
            If there are multiple correct answers with any incorrect ones, define as checkBox
                    else { return questionType.checkBox; }
                }
            */
        }

        //Fetching Question Score, Award or Penalty will calculate total score applied based on IsUserSelected flag
        public virtual int Score
        {
            get
            {
                if (Answers == null) return 0;
                else
                {
                    int totalScore = 0;
                    switch (this.QuestionType)
                    {
                        case questionType.textBox:
                            {
                                var selectedTbAnswer = Answers.FirstOrDefault(a => a.IsUserSelected);
                                //Return score value, including negative points, if such answer exists (specifically penalized options)
                                if (selectedTbAnswer != null)
                                {
                                    totalScore += selectedTbAnswer.Score; break;
                                }
                            }
                            break;
                        case questionType.radioButton:
                            {
                                var selectedRbAnswer = Answers.FirstOrDefault(a => a.IsUserSelected);
                                if (selectedRbAnswer != null && selectedRbAnswer.Correct) { totalScore += selectedRbAnswer.Score; break; }
                            }
                            break;
                        case questionType.checkBox:
                            {
                                bool voidAward = false; //Flag for voiding award score if incorrect answers are selected
                                foreach (var answer in Answers)
                                {
                                    if (answer.IsUserSelected && !answer.Correct) voidAward = true; break;
                                }
                                if (voidAward)
                                {
                                    foreach (var answer in Answers)
                                    {
                                        //Count only penalty if award is voided
                                        if (!answer.Correct && answer.IsUserSelected) totalScore += answer.Score;
                                    }
                                }
                                else
                                {
                                    foreach (var answer in Answers)
                                    {
                                        if (answer.IsUserSelected) totalScore += answer.Score;
                                    }
                                }
                            }
                            break;
                    }
                    return totalScore;
                }
            }
        }
        public virtual int Award
        {
            get
            {
                int totalAward = this.Score;
                if (totalAward > 0) return totalAward;
                else return 0;
            }
        }
        public virtual int Penalty
        {
            get
            {
                int totalPenalty = this.Score;
                if (totalPenalty < 0) return Math.Abs(totalPenalty);
                else return 0;
            }
        }
        public virtual bool IsCorrect
        {
            get
            {
                if (Answers == null) return false;
                else
                {
                    switch (this.QuestionType)
                    {
                        case questionType.textBox:
                            {
                                var selectedTbAnswer = Answers.FirstOrDefault(a => a.IsUserSelected);
                                if (selectedTbAnswer != null && selectedTbAnswer.Correct) return true;
                                else return false;
                            }
                        case questionType.radioButton:
                            {
                                var selectedRbAnswer = Answers.FirstOrDefault(a => a.IsUserSelected);
                                if (selectedRbAnswer != null && selectedRbAnswer.Correct) return true;
                                else return false;
                            }
                        case questionType.checkBox:
                            {
                                bool voidAward = false; //Flag for voiding award score if incorrect answers are selected
                                foreach (var answer in Answers)
                                {
                                    if (answer.IsUserSelected && !answer.Correct)
                                    {
                                        voidAward = true; break;
                                    }
                                }
                                return Answers.Any(answer => answer.IsUserSelected && answer.Correct && !voidAward);
                            }
                        default: return false;
                    }
                }
            }
        }
    }
    public class Grade
    {
        public string? Title { get; set; }
        public int MinScore { get; set; }
        public int MinCorrectAnswers { get; set; }
        public int FailPenalty { get; set; } //Penalty points to fail the grade
        public int FailIncorrectAnswers { get; set; } //Number of wrong answers to fail the grade
        public bool IsPassingGrade { get; set; }
    }
    public class Questionnaire
    {
        public Question[]? Questions { get; set; }
        public Grade[]? Grades { get; set; }
        public int DefaultAward { get; set; } = 1;
        public int DefaultPenalty { get; set; } = 0;
        public void SetDefaultPercentageGrades()
        {
            int maxPossibleScore = 0;
            int maxPossibleCorrectAnswers = 0;

            if (Questions != null)
            {
                foreach (var question in Questions)
                {
                    if (question.Answers != null)
                    {
                        // For each question, sum up the scores of its true answers for max possible score.
                        // For text box questions, assume the 'Correct' answer is the only one counted towards score.
                        if (question.QuestionType == questionType.textBox)
                        {
                            var trueAnswer = question.Answers.FirstOrDefault(a => a.Correct);
                            if (trueAnswer != null)
                            {
                                maxPossibleScore += trueAnswer.Score;
                                maxPossibleCorrectAnswers++;
                            }
                        }
                        else // For radioButton and checkBox
                        {
                            maxPossibleScore += question.Answers.Where(a => a.Correct).Sum(a => a.Score);
                            maxPossibleCorrectAnswers += question.Answers.Count(a => a.Correct);
                        }
                    }
                }
            }

            int score90Percent = (int)Math.Ceiling(maxPossibleScore * 0.90);
            int score75Percent = (int)Math.Ceiling(maxPossibleScore * 0.75);
            int score60Percent = (int)Math.Ceiling(maxPossibleScore * 0.60);

            int correct90Percent = (int)Math.Ceiling(maxPossibleCorrectAnswers * 0.90);
            int correct75Percent = (int)Math.Ceiling(maxPossibleCorrectAnswers * 0.75);
            int correct60Percent = (int)Math.Ceiling(maxPossibleCorrectAnswers * 0.60);

            List<Grade> defaultGrades = new List<Grade>
            {
                new Grade { Title = "5", MinScore = score90Percent, MinCorrectAnswers = correct90Percent, FailPenalty = maxPossibleScore, FailIncorrectAnswers = maxPossibleCorrectAnswers, IsPassingGrade = true },
                new Grade { Title = "4", MinScore = score75Percent, MinCorrectAnswers = correct75Percent, FailPenalty = maxPossibleScore, FailIncorrectAnswers = maxPossibleCorrectAnswers, IsPassingGrade = true },
                new Grade { Title = "3", MinScore = score60Percent, MinCorrectAnswers = correct60Percent, FailPenalty = maxPossibleScore, FailIncorrectAnswers = maxPossibleCorrectAnswers, IsPassingGrade = true },
                new Grade { Title = "2", MinScore = 0, MinCorrectAnswers = 0, FailPenalty = maxPossibleScore, FailIncorrectAnswers = maxPossibleCorrectAnswers, IsPassingGrade = false }
            };

            // Assign the newly created grades, ordered by MinScore descending
            Grades = defaultGrades.OrderByDescending(g => g.MinScore).ToArray();

            Debug.WriteLine($"\nDefault grades initialized based on Max Possible Score: {maxPossibleScore} and Max Possible Correct Answers: {maxPossibleCorrectAnswers}");
            foreach (var grade in Grades)
            {
                Debug.WriteLine($"- {grade.Title}: MinScore={grade.MinScore}, MinCorrectAnswers={grade.MinCorrectAnswers}, FailPenalty={grade.FailPenalty}, FailIncorrectAnswers={grade.FailIncorrectAnswers}, Passing={grade.IsPassingGrade}");
            }
        }
        public QuizResults GradeUserAnswers()
        {
            int userTotalScore = 0;
            int userCorrectAnswersCount = 0;
            int userIncorrectAnswersCount = 0;
            int userTotalPenalty = 0;

            if (Questions == null)
            {
                return new QuizResults();
            }

            foreach (var question in Questions)
            {
                int questionScore = 0;
                int questionCorrectCount = 0;
                int questionIncorrectCount = 0;
                int questionPenalty = 0;

                if (question.Answers == null) continue;

                switch (question.QuestionType)
                {
                    case questionType.radioButton:
                        // For radio buttons, only one answer can be selected.
                        var selectedRbAnswer = question.Answers.FirstOrDefault(a => a.IsUserSelected);
                        if (selectedRbAnswer != null)
                        {
                            if (selectedRbAnswer.Correct)
                            {
                                questionScore += selectedRbAnswer.Score;
                                questionCorrectCount++;
                            }
                            else
                            {
                                questionScore += selectedRbAnswer.Score; // Apply penalty if score is negative
                                questionIncorrectCount++;
                                questionPenalty += selectedRbAnswer.Penalty;
                            }
                        }
                        break;

                    case questionType.checkBox:
                        // For checkboxes, iterate through all answers to check selections.
                        foreach (var answer in question.Answers)
                        {
                            if (answer.IsUserSelected)
                            {
                                if (answer.Correct)
                                {
                                    questionScore += answer.Score;
                                    questionCorrectCount++;
                                }
                                else
                                {
                                    questionScore += answer.Score;
                                    questionIncorrectCount++;
                                    questionPenalty += answer.Penalty;
                                }
                            }
                        }

                        // VOIDING LOGIC FOR CHECKBOXES: If any incorrect answers were chosen, void the award score.
                        if (questionIncorrectCount > 0)
                        {
                            Debug.WriteLine($"  (Question: '{question.Content}') Incorrect answers were selected. Award score for this question is voided.");
                            // If the questionScore is positive (meaning awards outweighed penalties), set it to 0.
                            // If it's already negative (penalties outweighed awards), keep it negative.
                            if (questionScore > 0)
                            {
                                questionScore = 0;
                            }
                            questionCorrectCount = 0; // No correct answers are counted towards the total if incorrect ones were selected
                        }
                        break;

                    case questionType.textBox:
                        var correctTextBoxAnswer = question.Answers.FirstOrDefault(a => a.Correct);
                        var selectedTextBoxAnswer = question.Answers.FirstOrDefault(a => a.IsUserSelected);

                        if (correctTextBoxAnswer != null && selectedTextBoxAnswer != null &&
                            selectedTextBoxAnswer.Content.Equals(correctTextBoxAnswer.Content, StringComparison.OrdinalIgnoreCase))
                        {
                            questionScore += correctTextBoxAnswer.Score;
                            questionCorrectCount++;
                        }
                        else
                        {
                            if (question.Answers.Any(a => a.Score < 0))
                            {
                                questionScore += question.Answers.Where(a => a.Score < 0).Sum(a => a.Score);
                                questionPenalty += question.Answers.Where(a => a.Score < 0).Sum(a => a.Penalty);
                            }
                            questionIncorrectCount++;
                        }
                        break;
                }
                userTotalScore += questionScore;
                userCorrectAnswersCount += questionCorrectCount;
                userIncorrectAnswersCount += questionIncorrectCount;
                userTotalPenalty += questionPenalty;
            }

            if (Grades != null)
            {
                Grade finalGrade = Grades.FirstOrDefault(g =>
                userTotalScore >= g.MinScore &&
                userCorrectAnswersCount >= g.MinCorrectAnswers &&
                userTotalPenalty <= g.FailPenalty &&
                userIncorrectAnswersCount <= g.FailIncorrectAnswers
                );

                return new QuizResults
                {
                    FinalGrade = finalGrade,
                    TotalScore = userTotalScore,
                    CorrectAnswersCount = userCorrectAnswersCount,
                    IncorrectAnswersCount = userIncorrectAnswersCount,
                    TotalPenalty = userTotalPenalty
                };
            }
            else throw new ArgumentException("Grade cannot be null", nameof(Grades));
        }
    }
}
