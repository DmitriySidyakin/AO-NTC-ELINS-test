using data_processor.Entity;
using data_processor.ProcessorDelegate;
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
            DataProcessor<SourceEntity, DestinationEntity> dataProcessor = 
                new DataProcessor<SourceEntity, DestinationEntity>(
                    sourceFilePath, 
                    endFilePath,
                    DestinationToSourceDelegates.LoadSourceEntities,
                    DestinationToSourceDelegates.Transform, 
                    DestinationToSourceDelegates.SaveEndFile);
            dataProcessor.Process();
        }

        private static (string sourceFilePath, string endFilePath) GetConfiguration(string[] args)
        {
            return (args[0], args[1]);
        }
    }
}