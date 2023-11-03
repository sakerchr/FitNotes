using FitNotes.Core.Models;
using Microsoft.Data.Sqlite;

namespace FitNotes.Core.FitNotesBackup
{
    public class BackupParser
    {
        public static async Task<TrainingLogBackup> ParseFitNotesBackupFile(string connectionString)
        {
            var trainingLogsBackUp = new TrainingLogBackup();
            var trainingLogs = new List<TrainingLog>();
            var exercises = new HashSet<Exercise>();
            var categories = new List<Category>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                        SELECT TL._id, TL.date, E.name, CAT.name, TL.metric_weight, TL.reps, TL.is_personal_record, C.comment
                        FROM training_log as TL
                        LEFT JOIN Comment as C ON TL._id = C.owner_id
                        JOIN Exercise as E ON TL.exercise_id = E._id
                        JOIN Category as CAT ON E.category_id = CAT._id
                    ";

                using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var exerciseCategory = reader.GetString(3);
                    var exerciseName = reader.GetString(2);

                    exercises.Add(new Exercise { ExerciseName = exerciseName, ExerciseCategory = exerciseCategory });
                    categories.Add(new Category { ExerciseCategory = exerciseCategory });
                    trainingLogs.Add(
                        new TrainingLog
                        {
                            Id = reader.GetInt32(0),
                            Date = reader.GetDateTime(1),
                            Exercise = exerciseName,
                            Category = exerciseCategory,
                            MetricWeight = reader.GetFloat(4),
                            Reps = reader.GetInt32(5),
                            IsPersonalRecord = reader.GetBoolean(6),
                            Comment = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                        }
                    );
                }
            }

            trainingLogsBackUp.TrainingLogs = trainingLogs;
            trainingLogsBackUp.Exercises = exercises.ToList();
            trainingLogsBackUp.Categories = categories.ToList();
            trainingLogsBackUp.TrainingSessions = GroupTrainingLogSessionsFromTrainingLogs(trainingLogs);

            return trainingLogsBackUp;
        }

        private static List<TrainingLogSession> GroupTrainingLogSessionsFromTrainingLogs(List<TrainingLog> trainingLogs) => trainingLogs
            .GroupBy(tl => DateOnly.FromDateTime(tl.Date))
            .Select(trainingLogsGroupedByDate =>
                new TrainingLogSession
                {
                    Date = trainingLogsGroupedByDate.Key,
                    TrainingLogs = trainingLogsGroupedByDate.ToList()
                }
            )
            .ToList();
    }
}
