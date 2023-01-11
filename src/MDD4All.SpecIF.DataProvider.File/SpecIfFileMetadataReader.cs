/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Base;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MDD4All.SpecIF.DataProvider.File
{
    public class SpecIfFileMetadataReader : AbstractSpecIfMetadataReader
	{

        private string _metadataRootPath = "";

		private DataModels.SpecIF _metaData;

		public SpecIfFileMetadataReader()
		{
			_metaData = SpecIfFileReaderWriter.ReadDataFromSpecIfFile(@"c:\specif\metadata.specif");
		}

        public SpecIfFileMetadataReader(DataModels.SpecIF metaData)
        {
            _metaData = metaData;
        }

        public SpecIfFileMetadataReader(string metadataRootPath)
        {
            _metadataRootPath = metadataRootPath;
            InitializeMetadata();
        }

        private void InitializeMetadata()
        {
            _metaData = new DataModels.SpecIF();

            InitializeMetadataRecusrsively(_metadataRootPath);
        }

        private void InitializeMetadataRecusrsively(string currentPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);
            FileInfo[] specifFiles = directoryInfo.GetFiles("*.specif");

            foreach(FileInfo fileInfo in specifFiles)
            {
                DataModels.SpecIF currentSepcIF = SpecIfFileReaderWriter.ReadDataFromSpecIfFile(fileInfo.FullName);

                _metaData.DataTypes.AddRange(currentSepcIF.DataTypes);

                _metaData.PropertyClasses.AddRange(currentSepcIF.PropertyClasses);

                _metaData.ResourceClasses.AddRange(currentSepcIF.ResourceClasses);

                _metaData.StatementClasses.AddRange(currentSepcIF.StatementClasses);
            }

            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
            {
                InitializeMetadataRecusrsively(subDirectoryInfo.FullName);
            }



        }

        public override List<DataType> GetAllDataTypes()
		{
			return _metaData?.DataTypes;
		}

		public override DataType GetDataTypeByKey(Key key)
		{
			DataType result = null;

			if (!string.IsNullOrEmpty(key.ID))
			{
				if (!string.IsNullOrEmpty(key.Revision))
				{
					result = _metaData.DataTypes.Find(dataType => dataType.ID == key.ID && dataType.Revision == key.Revision);
				}
				else
				{
					List<DataType> dataTypesWithSameID = _metaData?.DataTypes.FindAll(res => res.ID == key.ID);

					if (dataTypesWithSameID.Any())
					{
						List<DataType> orderedByDateList = dataTypesWithSameID.OrderBy(x => x.ChangedAt).ToList();

						result = orderedByDateList[0];
					}

				}
			}

			return result;
		}

		
		public override ResourceClass GetResourceClassByKey(Key key)
		{
			ResourceClass result = null;

			if (!string.IsNullOrEmpty(key.ID))
			{
				if (!string.IsNullOrEmpty(key.Revision))
				{
					result = _metaData.ResourceClasses.Find(resourceClass => resourceClass.ID == key.ID && resourceClass.Revision == key.Revision);
				}
				else
				{
					List<ResourceClass> resourceClassesWithSameID = _metaData?.ResourceClasses.FindAll(res => res.ID == key.ID);

					if (resourceClassesWithSameID.Any())
					{
						List<ResourceClass> orderedByDateList = resourceClassesWithSameID.OrderBy(x => x.ChangedAt).ToList();

						result = orderedByDateList[0];
					}
					
				}
			}
			
			return result;
		}

		public override PropertyClass GetPropertyClassByKey(Key key)
		{
			PropertyClass result = null;

			if (!string.IsNullOrEmpty(key.ID))
			{
				if (!string.IsNullOrEmpty(key.Revision))
				{
					result = _metaData.PropertyClasses.Find(propertyClass => propertyClass.ID == key.ID && propertyClass.Revision == key.Revision);
				}
				else
				{
					List<PropertyClass> propertyClassesWithSameID = _metaData?.PropertyClasses.FindAll(res => res.ID == key.ID);

					if (propertyClassesWithSameID.Any())
					{
						List<PropertyClass> orderedByDateList = propertyClassesWithSameID.OrderBy(x => x.ChangedAt).ToList();

						result = orderedByDateList[0];
					}

				}
			}

			return result;
		}

		public override List<PropertyClass> GetAllPropertyClasses()
		{
			return _metaData?.PropertyClasses;
		}

		public override StatementClass GetStatementClassByKey(Key key)
		{
			StatementClass result = null;

			if (!string.IsNullOrEmpty(key.ID))
			{
				if (!string.IsNullOrEmpty(key.Revision))
				{
					result = _metaData.StatementClasses.Find(statementClass => statementClass.ID == key.ID && statementClass.Revision == key.Revision);
				}
				else
				{
					List<StatementClass> statementClassesWithSameID = _metaData?.StatementClasses.FindAll(res => res.ID == key.ID);

					if (statementClassesWithSameID.Any())
					{
						List<StatementClass> orderedByDateList = statementClassesWithSameID.OrderBy(x => x.ChangedAt).ToList();

						result = orderedByDateList[0];
					}

				}
			}

			return result;
		}

		public override List<ResourceClass> GetAllResourceClasses()
		{
			return _metaData?.ResourceClasses;
		}

		public override string GetLatestPropertyClassRevision(string propertyClassID)
		{
			throw new System.NotImplementedException();
		}

		public override string GetLatestResourceClassRevision(string resourceClassID)
		{
			throw new System.NotImplementedException();
		}

		public override string GetLatestStatementClassRevision(string statementClassID)
		{
			throw new System.NotImplementedException();
		}

        public override List<StatementClass> GetAllStatementClasses()
        {
            return _metaData?.StatementClasses;
        }

        public override List<DataType> GetAllDataTypeRevisions(string dataTypeID)
        {
            throw new System.NotImplementedException();
        }

        public override List<PropertyClass> GetAllPropertyClassRevisions(string propertyClassID)
        {
            throw new System.NotImplementedException();
        }

        public override List<ResourceClass> GetAllResourceClassRevisions(string resourceClassID)
        {
            throw new System.NotImplementedException();
        }

        public override List<StatementClass> GetAllStatementClassRevisions(string statementClassID)
        {
            throw new System.NotImplementedException();
        }
    }
}
