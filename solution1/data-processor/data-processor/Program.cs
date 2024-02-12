using Microsoft.VisualBasic.FileIO;

namespace data_processor
{
    internal class Program
    {
        /**
         * Первый аргумент - полный путь и имя входного файла.
         * Второй аргумент - полный путь и имя выходного файла.
         * */
        static void Main(string[] args)
        {

            (var sourceFilePath, var endFilePath) = GetConfiguration(args);
            List<SourceEntity> entities = LoadSourceEntities(sourceFilePath);
            List<DestinationEntity> destinationEntities = Transform(entities);
            SaveEndFile(ref endFilePath, ref destinationEntities);
        }

        private static void SaveEndFile(ref string endFilePath, ref List<DestinationEntity> destinationEntities)
        {
            string header = "Коды\tДолжность";

            // полная перезапись файла в UTF-8
            using (StreamWriter writer = new StreamWriter(endFilePath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine(header);
                foreach(var de in destinationEntities)
                {
                    writer.WriteLine($"{de.Codes}\t{de.JobTitle}");
                }
            }
        }

        private static List<SourceEntity> LoadSourceEntities(string sourceFilePath)
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

        private static List<DestinationEntity> Transform(List<SourceEntity> entities)
        {
            var result = from e in entities
                         group e by e.JobTitle into g
                         select new DestinationEntity() { JobTitle = g.Key, Codes = CodesToString(g.OrderBy(el => el.Code).ToArray()) };

            return result.ToList();
        }

        private static string CodesToString(SourceEntity[] sourceEntitiesGroup)
        {
            string resultCodes;
            if (sourceEntitiesGroup.Length == 1)
                resultCodes = sourceEntitiesGroup[0].Code.ToString();
            else
            {
                resultCodes = String.Empty;

                bool start = true;
                for (long i = 0; i < sourceEntitiesGroup.LongLength; i++)
                {
                    if (start)
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
                    else
                    {
                        if (i < sourceEntitiesGroup.LongLength - 1)
                        { 
                            if (sourceEntitiesGroup[i].Code + 1 != sourceEntitiesGroup[i + 1].Code)
                            {
                                resultCodes += $"{sourceEntitiesGroup[i].Code}";
                                start = true;
                            }
                        }
                        else if(i == sourceEntitiesGroup.LongLength - 1)
                        {
                            resultCodes += $"{sourceEntitiesGroup[i].Code}";
                        }
                    }
                }
            }
                
            return resultCodes;
        }

        private static (string sourceFilePath, string endFilePath) GetConfiguration(string[] args)
        {
            return (args[0], args[1]);
        }
    }
}