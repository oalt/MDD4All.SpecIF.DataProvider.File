/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Base.DataModels;
using MDD4All.SpecIF.DataProvider.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            DataModels.SpecIF specIfData = GetOrCreateProject();

            if (specIfData != null && specIfData.Hierarchies != null)
            {
                foreach (Node hieratchy in specIfData.Hierarchies)
                {
                    Node parentNode = hieratchy.GetNodeByID(parentNodeId);

                    if (parentNode != null)
                    {
                        parentNode.Nodes.Insert(0, newNode);
                        SaveDataToFile(specIfData);
                        RefreshData();
                        break;
                    }
                }
            }
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

        public override Node UpdateHierarchy(Node hierarchyToUpdate, string parentID = null, string predecessorID = null)
        {
            Node result = hierarchyToUpdate;

            if (parentID != null ^ predecessorID != null)
            {
                throw new NotImplementedException();
            }

            DataModels.SpecIF specIfData = GetOrCreateProject();

            foreach(Node hierarchy in specIfData.Hierarchies)
            {
                Node nodeToUpdate = hierarchy.GetNodeByID(hierarchyToUpdate.ID);
                if (nodeToUpdate != null)
                {
                    nodeToUpdate.ResourceReference = hierarchyToUpdate.ResourceReference;
                    nodeToUpdate.Nodes = hierarchyToUpdate.Nodes;
                    SaveDataToFile(specIfData);
                    RefreshData();
                    break;
                }
            }          

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
            DataModels.SpecIF specIfData = GetOrCreateProject();

            if (specIfData != null && specIfData.Hierarchies != null)
            {
                foreach (Node hierarchy in specIfData.Hierarchies)
                {
                    Node nodeToMove = hierarchy.GetNodeByID(nodeID);

                    if (nodeToMove != null)
                    {
                        Node oldParent = specIfData.GetParentNode(nodeID);

                        Node newParentNode = hierarchy.GetNodeByID(newParentID);

                        if(newParentNode != null && oldParent != null)
                        {
                            oldParent.Nodes.Remove(nodeToMove);

                            int insertIndex = 0;
                            int index = 0;

                            if (!string.IsNullOrEmpty(newSiblingId))
                            {
                                foreach (Node node in newParentNode.Nodes)
                                {
                                    if (node.ID == newSiblingId)
                                    {
                                        insertIndex = index + 1;
                                        break;
                                    }
                                    index++;
                                }
                            }
                            
                            newParentNode.Nodes.Insert(insertIndex, nodeToMove);

                            SaveDataToFile(specIfData);
                            RefreshData();
                            break;
                        }
                        
                    }
                }
            }
        }

        public override Resource UpdateResource(Resource resource)
        {
            throw new NotImplementedException();
        }

        public override void AddProject(ISpecIfMetadataWriter metadataWriter, DataModels.SpecIF project, string integrationID = null)
        {
            string fullName = _path + "/" + project.ID + ".specif";

            // only save the data if the file does not exist
            if(!System.IO.File.Exists(fullName))
            {
                SaveDataToFile(project);
            }
            else // update the existing project data
            {
                DataModels.SpecIF existingProject = GetOrCreateProject(project.ID);

                if (existingProject != null)
                {
                    existingProject.ID = project.ID;
                    existingProject.Title = project.Title;
                    existingProject.Description = project.Description;
                    existingProject.Schema = project.Schema;
                    existingProject.Generator = project.Generator;
                    existingProject.GeneratorVersion = project.GeneratorVersion;
                    existingProject.CreatedAt = DateTime.Now;
                    existingProject.CreatedBy = project.CreatedBy;
                    existingProject.Rights = project.Rights;

                    SaveDataToFile(existingProject);
                }
            }
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
            DataModels.SpecIF specIfData = GetOrCreateProject();

            if (specIfData != null && specIfData.Hierarchies != null)
            {
                foreach (Node hierarchy in specIfData.Hierarchies)
                {
                    Node predecessorNode = hierarchy.GetNodeByID(predecessorID);

                    if (predecessorNode != null)
                    {
                        Node parentNode = specIfData.GetParentNode(predecessorID);


                        if (parentNode != null)
                        {
                            int index = 0;


                            foreach (Node node in parentNode.Nodes)
                            {
                                if (node.ID == predecessorID)
                                {
                                    break;
                                }
                                index++;
                            }

                            parentNode.Nodes.Insert(index + 1, newNode);

                            SaveDataToFile(specIfData);
                            RefreshData();
                            break;
                        }
                    }
                }
            }
        }

        public override void DeleteNode(string nodeID)
        {
            DataModels.SpecIF specIfData = GetOrCreateProject();

            if (specIfData != null && specIfData.Hierarchies != null)
            {
                foreach (Node hierarchy in specIfData.Hierarchies)
                {
                    Node noteToDelete = hierarchy.GetNodeByID(nodeID);

                    if(noteToDelete != null)
                    {
                        Node parentNode = specIfData.GetParentNode(nodeID);
                        if(parentNode != null)
                        {
                            parentNode.Nodes.Remove(noteToDelete);
                            SaveDataToFile(specIfData);
                            RefreshData();
                            break;
                        }
                    }
                    
                }
            }
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
