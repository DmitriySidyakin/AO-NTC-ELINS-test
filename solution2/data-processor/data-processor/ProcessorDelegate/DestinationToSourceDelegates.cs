using data_processor.Entity;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_processor.ProcessorDelegate
{
    public static class DestinationToSourceDelegates
    {
        public static void SaveEndFile(List<DestinationEntity> destinationEntities, string endFilePath)
        {
            string header = "Коды\tДолжность";

            // полная перезапись файла в UTF-8
            using (StreamWriter writer = new StreamWriter(endFilePath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine(header);
                foreach (var de in destinationEntities)
                {
                    writer.WriteLine($"{de.Codes}\t{de.JobTitle}");
                }
            }
        }

        public static List<SourceEntity> LoadSourceEntities(string sourceFilePath)
        {
            List<SourceEntity> entities = new List<SourceEntity>();
            using (TextFieldParser parser = new TextFieldParser(sourceFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters("\t"); // Устанавливаем разделитель
                // Считываем строку заголовок
                parser.ReadLine();
                while (!parser.EndOfData)
                {
                    try
                    {
                        // Обрабатываем строку
                        string[] fields = parser.ReadFields() ?? throw new Exception("Error in line");
                        entities.Add(new SourceEntity { Name = fields[0], Code = long.Parse(fields[1]), JobTitle = fields[2] });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }

            return entities;
        }

        public static List<DestinationEntity> Transform(List<SourceEntity> entities)
        {
            var result = from e in entities
                         group e by e.JobTitle into g
                         select new DestinationEntity() { JobTitle = g.Key, Codes = CodesToString(g.OrderBy(el => el.Code).ToArray()) };

            return result.ToList();
        }
        
        private static string CodesToString(SourceEntity[] sourceEntitiesGroup)
        {
            string resultCodes;
            if (sourceEntitiesGroup.Length == 1) {
                resultCodes = sourceEntitiesGroup[0].Code.ToString();
                return resultCodes;
            }

            resultCodes = String.Empty;
            bool start = true;
            for (long i = 0; i < sourceEntitiesGroup.LongLength; i++)
                ProcessNextCode(sourceEntitiesGroup, ref resultCodes, ref start, i);

            return resultCodes;
        }

        private static void ProcessNextCode(SourceEntity[] sourceEntitiesGroup, ref string resultCodes, ref bool start, long i)
        {
            if (start)
                ProcessNewCodeStart(sourceEntitiesGroup, ref resultCodes, ref start, i);
            else
                ProcessCodeContinuation(sourceEntitiesGroup, ref resultCodes, ref start, i);
        }

        private static void ProcessCodeContinuation(SourceEntity[] sourceEntitiesGroup, ref string resultCodes, ref bool start, long i)
        {
            if (i < sourceEntitiesGroup.LongLength - 1)
            {
                EndCodeContinuation(sourceEntitiesGroup, ref resultCodes, ref start, i);
            }
            else if (i == sourceEntitiesGroup.LongLength - 1)
            {
                resultCodes += $"{sourceEntitiesGroup[i].Code}";
            }
        }

        private static void EndCodeContinuation(SourceEntity[] sourceEntitiesGroup, ref string resultCodes, ref bool start, long i)
        {
            if (sourceEntitiesGroup[i].Code + 1 != sourceEntitiesGroup[i + 1].Code)
            {
                resultCodes += $"{sourceEntitiesGroup[i].Code}";
                start = true;
            }
        }

        private static void ProcessNewCodeStart(SourceEntity[] sourceEntitiesGroup, ref string resultCodes, ref bool start, long i)
        {
            if (i != 0)
                resultCodes += ", ";
            resultCodes += $"{sourceEntitiesGroup[i].Code}";
            if (i < sourceEntitiesGroup.LongLength - 1)
                if (sourceEntitiesGroup[i].Code + 1 == sourceEntitiesGroup[i + 1].Code)
                {
                    resultCodes += $"-";
                    start = false;
                }
        }
    }
}
