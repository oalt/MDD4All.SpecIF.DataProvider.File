/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Base.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MDD4All.SpecIF.DataProvider.File
{
    public class SpecIfFileDataWriter : AbstractSpecIfDataWriter
    {

        //private DataModels.SpecIF _specIfData;

        private string _path;
        private string _identificatorFile = "identificators.json";

        private const string DEFAULT_PROJECT = "PRJ-DEFAULT";

        public SpecIfFileDataWriter(string path,
                                    ISpecIfMetadataReader metadataReader,
                                    ISpecIfDataReader dataReader) : base(metadataReader, dataReader)
        {
            _path = path;
            _dataReader = dataReader;
            
            InitializeIdentificators();
        }

        private void RefreshData(string projectID = null)
        {
            if(_dataReader is SpecIfFileDataReader)
            {
                SpecIfFileDataReader specIfFileDataReader = _dataReader as SpecIfFileDataReader;

                specIfFileDataReader.RefreshData(projectID);
            }
        }

        public override void InitializeIdentificators()
        {
            FileInfo fileInfo = new FileInfo(_path + "/" + _identificatorFile);
            if (fileInfo.Exists)
            {
                string json = System.IO.File.ReadAllText(fileInfo.FullName);

                JsonConvert.DeserializeObject<SpecIfIdentifiers>(json);
            }
            else
            {
                _identificators = new SpecIfIdentifiers();
                SaveIdentificators();
            }
        }

        public override void SaveIdentificators()
        {
            string json = JsonConvert.SerializeObject(_identificators, Formatting.Indented);

            System.IO.File.WriteAllText(_path + "/" + _identificatorFile, json);
        }

        public override void AddStatement(Statement statement)
        {
            DataModels.SpecIF specIfData = GetOrCreateProject();
            specIfData.Statements.Add(statement);
            SaveDataToFile(specIfData);
            RefreshData();
        }

        public override void AddResource(Resource resource)
        {
            DataModels.SpecIF specIfData = GetOrCreateProject();
            specIfData.Resources.Add(resource);
            SaveDataToFile(specIfData);
            RefreshData();
        }

        public override void AddNodeAsFirstChild(string parentNodeId, Node newNode)
        {
            throw new NotImplementedException();
        }

        public override void AddHierarchy(Node hierarchy, string projectID = null)
        {
            DataModels.SpecIF specIfData = GetOrCreateProject(projectID);
            specIfData.Hierarchies.Add(hierarchy);
            SaveDataToFile(specIfData);
            RefreshData(projectID);
        }

        public override Resource SaveResource(Resource resource, string projectID = null)
        {
            throw new NotImplementedException();
        }

        public override Node UpdateHierarchy(Node hierarchy, string parentID = null, string predecessorID = null)
        {
            Node result = null;

            //string id = hierarchy.ID;

            //int index = -1;
            //for (int counter = 0; counter < _specIfData.Hierarchies.Count; counter++)
            //{
            //	if (_specIfData?.Hierarchies[counter].ID == id)
            //	{
            //		index = counter;
            //		break;
            //	}
            //}

            //if (index != -1)
            //{
            //	_specIfData.Hierarchies[index] = hierarchy;
            //	SpecIfFileReaderWriter.SaveSpecIfToFile(_specIfData, _path);
            //}
            //else
            //{
            //	// new hierarchy
            //	_specIfData.Hierarchies.Add(hierarchy);
            //	SpecIfFileReaderWriter.SaveSpecIfToFile(_specIfData, _path);
            //}

            return result;
        }

        public override Statement SaveStatement(Statement statement, string projectID = null)
        {


            //Statement existingStatement = _specIfData?.Statements.Find(st => st.ID == statement.ID && st.Revision == statement.Revision);

            //if(existingStatement != null)
            //{
            //	existingStatement = statement;
            //}
            //else
            //{
            //	AddStatement(statement);
            //}
            //SpecIfFileReaderWriter.SaveSpecIfToFile(_specIfData, _path);

            return statement;
        }

        public override void MoveNode(string nodeID, string newParentID, string newSiblingId)
        {
            throw new NotImplementedException();
        }

        public override Resource UpdateResource(Resource resource)
        {
            throw new NotImplementedException();
        }

        public override void AddProject(ISpecIfMetadataWriter metadataWriter, DataModels.SpecIF project, string integrationID = null)
        {
            throw new NotImplementedException();
        }

        public override void UpdateProject(ISpecIfMetadataWriter metadataWriter, DataModels.SpecIF project)
        {
            throw new NotImplementedException();
        }

        public override void DeleteProject(string projectID)
        {
            throw new NotImplementedException();
        }

        public override void AddNodeAsPredecessor(string predecessorID, Node newNode)
        {
            throw new NotImplementedException();
        }

        public override void DeleteNode(string nodeID)
        {
            throw new NotImplementedException();
        }

        private DataModels.SpecIF GetOrCreateProject(string projectID = null)
        {
            DataModels.SpecIF result = null;

            string filename = "";
            if(projectID == null)
            {
                filename = DEFAULT_PROJECT + ".specif";
                projectID = DEFAULT_PROJECT;
            }
            else
            {
                filename = projectID + ".specif";
            }

            string fullName = _path + "/" + filename;

            if(System.IO.File.Exists(fullName))
            {
                result = SpecIfFileReaderWriter.ReadDataFromSpecIfFile(fullName);
            }
            else
            {
                result = new DataModels.SpecIF
                {
                    ID = projectID
                };
            }

            return result;
        }

        private void SaveDataToFile(DataModels.SpecIF specIF)
        {
            string fullName = _path + "/" + specIF.ID + ".specif";

            SpecIfFileReaderWriter.SaveSpecIfToFile(specIF, fullName);
        }
    }
}
