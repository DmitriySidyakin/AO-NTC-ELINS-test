using data_processor.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace data_processor
{
    public class DataProcessor<inT, outT>
    {
        public delegate List<inT> LoadSourceEntities(string sourceFilePath);

        public delegate List<outT> Transform(List<inT> entities);

        public delegate void SaveEntities(List<outT> destinationEntities, string endFilePath);

        public string SourceFilePath { get; private set; }

        public string EndFilePath { get; private set; }

        private List<inT> SourceEntities { get; set; }

        private List<outT> DestinationEntities { get; set; }

        private LoadSourceEntities LoadSourceEntitiesDelegate { get; set; }

        private Transform TransformDelegate { get; set; }

        private SaveEntities SaveEntitiesDelegate { get; set; }

        public DataProcessor(string sourceFilePath, string endFilePath, LoadSourceEntities loadSourceEntitiesDelegate, Transform transformDelegate, SaveEntities saveEntitiesDelegate)
        {
            SourceFilePath = sourceFilePath;
            EndFilePath = endFilePath;
            LoadSourceEntitiesDelegate = loadSourceEntitiesDelegate;
            TransformDelegate = transformDelegate;
            SaveEntitiesDelegate = saveEntitiesDelegate;
        }

        public void Process()
        {
            SourceEntities = LoadSourceEntitiesDelegate(SourceFilePath);
            DestinationEntities = TransformDelegate(SourceEntities);
            SaveEntitiesDelegate(DestinationEntities, EndFilePath);
        }
    }
}
