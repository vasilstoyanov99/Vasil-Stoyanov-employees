namespace Vasil_Stoyanov_employees.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;

    using Models;

    public class CalculatorService : ICalculatorService
    {
        public BestPairServiceModel GetTheBestPair(string fileName)
        {
            var records = new List<Record>();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", fileName);

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.Trim();

                    if (!string.IsNullOrEmpty(line))
                    {
                        var inputData = line.Split(';', StringSplitOptions.TrimEntries);
                        var newRecord = new Record
                        {
                            EmpId = int.Parse(inputData[0]),
                            ProjectId = int.Parse(inputData[1]),
                            DateFrom = DateTime.Parse(inputData[2],
                                CultureInfo.InvariantCulture),
                            DateTo = inputData[3].ToUpper() == "NULL"
                                ? DateTime.Now
                                : DateTime.Parse(inputData[3], CultureInfo.InvariantCulture)
                        };
                        records.Add(newRecord);
                    }
                }
            }

            var sortedRecords = records.OrderByDescending(x => x.ProjectId).ToList();
            var commonProjects = new List<Record>();

            for (int i = 0; i < sortedRecords.Count; i++)
            {
                var currProjectId = sortedRecords[i].ProjectId;
                var allRecordsOfTheSameProject = sortedRecords.FindAll(a => a.ProjectId == currProjectId);

                if (allRecordsOfTheSameProject.Count > 1)
                {
                    commonProjects.AddRange(allRecordsOfTheSameProject);
                    i += commonProjects.Count - 1;
                }
            }

            var allPairs = new HashSet<Pair>();

            for (int i = 0; i < commonProjects.Count; i++)
            {
                var projectId = commonProjects[i].ProjectId;
                var allRecordsOfTheSameProject = commonProjects.FindAll
                    (x => x.ProjectId == projectId);

                if (allRecordsOfTheSameProject.Count == 2)
                {
                    var startDateOne = allRecordsOfTheSameProject[0].DateFrom;
                    var startDateTwo = allRecordsOfTheSameProject[1].DateFrom;
                    var endDateOne = allRecordsOfTheSameProject[0].DateTo;
                    var endDateTwo = allRecordsOfTheSameProject[1].DateTo;
                    var empOneId = commonProjects[i].EmpId;
                    var empTwoId = commonProjects[i + 1].EmpId;
                    CreatePair(empOneId, empTwoId, projectId, startDateOne, startDateTwo,
                        endDateOne, endDateTwo, allPairs);

                    i += allRecordsOfTheSameProject.Count - 1;
                    continue;
                }

                for (int j = 0; j < allRecordsOfTheSameProject.Count - 1; j++)
                {
                    var empOne = allRecordsOfTheSameProject[j];

                    for (int k = j + 1; k < allRecordsOfTheSameProject.Count; k++)
                    {
                        var empTwo = allRecordsOfTheSameProject[k];
                        CreatePair(empOne.EmpId, empTwo.EmpId, allRecordsOfTheSameProject[j].ProjectId, 
                            empOne.DateFrom, empTwo.DateFrom, empOne.DateTo, empTwo.DateTo, allPairs);
                    }
                }

                i += allRecordsOfTheSameProject.Count - 1;
            }

            var bestPair = allPairs.OrderByDescending(x => x.TotalDaysWorked).FirstOrDefault();

            if (bestPair != null)
            {
                var bestPairModel = new BestPairServiceModel()
                {
                    EmpOneId = bestPair.EmpOneId,
                    EmpTwoId = bestPair.EmpTwoId,
                    TotalDaysWorked = bestPair.TotalDaysWorked,
                    ProjectsAndDaysWorked = bestPair.ProjectsAndDaysWorked
                };

                return bestPairModel;
            }

            return null;
        }

        /// <summary>
        /// Calculates the total days during which a pair of employees worked together on the same project
        /// and includes the last day as a whole day, e.g., the spam between the 1st and the 2nd of April will be calculated as 2 days.
        /// </summary>
        /// <param name="startDateOne"></param>
        /// <param name="startDateTwo"></param>
        /// <param name="endDateOne"></param>
        /// <param name="endDateTwo"></param>
        /// <returns></returns>
        private static int GetDateDiff(DateTime startDateOne, DateTime startDateTwo,
            DateTime endDateOne, DateTime endDateTwo)
        {
            var startDate = startDateOne < startDateTwo ? startDateTwo : startDateOne;
            var endDate = endDateOne < endDateTwo ? endDateOne : endDateTwo;

            if (endDate >= startDate)
            {
                var totalDaysDiff = (endDate - startDate).Days;
                return totalDaysDiff + 1;
            }

            return -1;
        }

        /// <summary>
        /// Creates or updates an existing pair if certain conditions are met.
        /// </summary>
        private static void CreatePair(int empOneId, int empTwoId, int projectId,
            DateTime startDateOne, DateTime startDateTwo, DateTime endDateOne,
            DateTime endDateTwo, HashSet<Pair> allPairs)
        {
            if (empOneId != empTwoId)
            {
                var dateDiff = GetDateDiff(startDateOne, startDateTwo, endDateOne, endDateTwo);

                if (dateDiff != -1)
                {
                    var pair = allPairs.FirstOrDefault
                        (x => x.EmpOneId == empOneId && x.EmpTwoId == empTwoId);

                    if (pair != null)
                    {
                        pair.TotalDaysWorked += dateDiff;

                        if (pair.ProjectsAndDaysWorked.ContainsKey(projectId))
                            pair.ProjectsAndDaysWorked[projectId] += dateDiff;
                        else
                            pair.ProjectsAndDaysWorked.Add(projectId, dateDiff);
                    }
                    else
                    {
                        pair = new Pair()
                        {
                            EmpOneId = empOneId,
                            EmpTwoId = empTwoId,
                            TotalDaysWorked = dateDiff
                        };
                        pair.ProjectsAndDaysWorked.Add(projectId, dateDiff);
                        allPairs.Add(pair);
                    }
                }
            }
        }
    }
}
