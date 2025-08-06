/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataModels.Manipulation;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Contracts;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace MDD4All.SpecIF.DataProvider.File
{
    public class SpecIfFileDataReader<T> : AbstractSpecIfDataReader where T : ExtendedSpecIF
	{
        private string _dataRootPath;

		public Dictionary<string, T> SpecIfData;

        public SpecIfFileDataReader(string dataRootPath)
        {
            _dataRootPath = dataRootPath;
            InitializeData();
        }

        

        private void InitializeData()
        {
            SpecIfData = new Dictionary<string, T>();

            InitializeDataRecusrsively(_dataRootPath);
        }

        private void InitializeDataRecusrsively(string currentPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);
            FileInfo[] specifFiles = directoryInfo.GetFiles("*.specif");

            foreach (FileInfo fileInfo in specifFiles)
            {
                T currentSepcIF = SpecIfFileReaderWriter.ReadDataFromSpecIfFile<T>(fileInfo.FullName);

                if(!SpecIfData.ContainsKey(fileInfo.FullName))
                {
                    SpecIfData.Add(fileInfo.FullName, currentSepcIF);
                }
                
            }

            foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
            {
                InitializeDataRecusrsively(subDirectoryInfo.FullName);
            }



        }

        public void RefreshData(string projectID = null)
        {
            if(projectID == null)
            {
                projectID = "PRJ-DEFAULT";
            }


            RefreshDataRecursively(_dataRootPath, projectID + ".specif");
        }

        private void RefreshDataRecursively(string currentPath, string filename)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(currentPath);

            
            string fullName = directoryInfo.FullName + "/" + filename;

            if (System.IO.File.Exists(fullName))
            {
                FileInfo specifFileInfo = new FileInfo(fullName);

                T specIF = SpecIfFileReaderWriter.ReadDataFromSpecIfFile<T>(fullName);
                if(SpecIfData.ContainsKey(specifFileInfo.FullName))
                {
                    SpecIfData[specifFileInfo.FullName] = specIF;
                }
                else
                {
                    SpecIfData.Add(specifFileInfo.FullName, specIF);
                }
            }
            else
            {
                foreach (DirectoryInfo subDirectoryInfo in directoryInfo.GetDirectories())
                {
                    RefreshDataRecursively(subDirectoryInfo.FullName, filename);
                }
            }
        }

        public override List<Node> GetAllHierarchies()
		{
			List<Node> result = new List<Node>();

            foreach(KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                List<Node> allHierarchiyNodes = keyValuePair.Value?.Hierarchies;

                foreach (Node node in allHierarchiyNodes)
                {
                    bool predecessorFound = false;
                    foreach (Node n in allHierarchiyNodes)
                    {
                        if (n.ResourceReference.ID == node.ID && n.ResourceReference.Revision == node.Revision)
                        {
                            predecessorFound = true;
                            break;
                        }
                    }
                    if (predecessorFound == false)
                    {
                        result.Add(node);
                    }
                }
            }
			

			return result;
		}

		public override Node GetHierarchyByKey(Key key)
		{
			Node result = null;

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                List<Node> hierarchiesWithSameID = keyValuePair.Value?.Hierarchies.FindAll(res => res.ID == key.ID);

                if (hierarchiesWithSameID.Count != 0 )
                {
                    if (key.Revision != null)
                    {
                        result = hierarchiesWithSameID.Find(r => r.Revision == key.Revision);
                        break;
                    }
                    else
                    {
                        result = hierarchiesWithSameID[0];
                        break;
                    }
                    
                }
            }

			return result;
		}

		public override Resource GetResourceByKey(Key key)
		{
			Resource result = null;

            if (!string.IsNullOrEmpty(key.ID))
            {
                foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
                {
                    Resource resource = null;
                    if (!string.IsNullOrEmpty(key.Revision))
                    {
                         resource = keyValuePair.Value?.Resources.Find(res => res.ID == key.ID && res.Revision == key.Revision);

                        
                    }
                    else
                    {
                        List<Resource> allRevisions = keyValuePair.Value?.Resources.FindAll(element => element.ID == key.ID);

                        if(allRevisions.Any())
                        {
                            List<Resource> orderedList = allRevisions.OrderBy(element => element.ChangedAt).ToList();
                            resource = orderedList[0];
                        }
                    }

                    if (resource != null)
                    {
                        result = resource;
                        break;
                    }
                }
            }

			return result;
		}

		public override string GetLatestResourceRevisionForBranch(string resourceID, string branchName)
		{
			string result = null;

			// TODO
			//try
			//{
			//	string? latestRevision = _specIfData?.Resources.FindAll(res => res.ID == resourceID).OrderByDescending(r => r.Revision).First().Revision;

			//	if (latestRevision.HasValue)
			//	{
			//		result = latestRevision.Value;
			//	}
			//}
			//catch (Exception)
			//{
			//	;
			//}

			return result;
		}

		public override Statement GetStatementByKey(Key key)
		{
			Statement result = null;

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                Statement statement = keyValuePair.Value?.Statements.Find(res => res.ID == key.ID && res.Revision == key.Revision);

                if (statement!=null)
                {
                    result = statement;
                    break;
                }
                
            }

			return result;
		}

		public override byte[] GetFile(string filename)
		{
			string path = @"C:\specif\files_and_images\" + filename;

			byte[] result = System.IO.File.ReadAllBytes(path);

			return result;
		}

		public override string GetLatestHierarchyRevision(string hierarchyID)
		{
			string result = null;

			//try
			//{
			//	int? latestRevision = _specIfData?.Hierarchies.FindAll(res => res.ID == hierarchyID).OrderByDescending(r => r.Revision).First().Revision;

			//	if (latestRevision.HasValue)
			//	{
			//		result = latestRevision.Value;
			//	}
			//}
			//catch (Exception)
			//{
			//	;
			//}

			return result;
		}

		public override string GetLatestStatementRevision(string statementID)
		{
			string result = null;

			//try
			//{
			//	int? latestRevision = _specIfData?.Statements.FindAll(res => res.ID == statementID).OrderByDescending(r => r.Revision).First().Revision;

			//	if (latestRevision.HasValue)
			//	{
			//		result = latestRevision.Value;
			//	}
			//}
			//catch (Exception)
			//{
			//	;
			//}

			return result;
		}

		public override List<Statement> GetAllStatementsForResource(Key resourceKey)
		{
            List<Statement> result = new List<Statement>();

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                List<Statement> statements = keyValuePair.Value?.Statements.FindAll(stm => stm.StatementSubject.ID == resourceKey.ID || 
                                                                                 stm.StatementObject.ID == resourceKey.ID);

                result.AddRange(statements);
            }

            return result;
        }

		public override List<Node> GetContainingHierarchyRoots(Key resourceKey)
		{
			throw new NotImplementedException();
		}

        public override List<Node> GetAllHierarchyRootNodes(string projectID = null)
        {
            List<Node> result = new List<Node>();

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                DataModels.SpecIF specIF = keyValuePair.Value;

                if(projectID != null)
                {
                    if(specIF.ID == projectID)
                    {
                        result.AddRange(specIF.Hierarchies);
                        break;
                    }
                }
                else
                {
                    result.AddRange(specIF.Hierarchies);
                }
            }

            // delete child nodes in result
            //foreach(Node rootNode in result)
            //{
            //    rootNode.Nodes = new List<Node>();
            //}

            return result;
        }

        public override List<Resource> GetAllResourceRevisions(string resourceID)
        {
            List<Resource> result = new List<Resource>();

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                result.AddRange(keyValuePair.Value.Resources.FindAll(element => element.ID == resourceID));
            }

            return result;
        }

        public override List<Statement> GetAllStatements()
        {
            List<Statement> result = new List<Statement>();

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                result.AddRange(keyValuePair.Value.Statements);
            }

            return result;
        }

        public override List<Statement> GetAllStatementRevisions(string statementID)
        {
            List<Statement> result = new List<Statement>();

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                result.AddRange(keyValuePair.Value.Statements.FindAll(stm => stm.ID == statementID));
            }

            return result;
        }

        public override List<Node> GetChildNodes(Key parentNodeKey)
        {
            List<Node> result = new List<Node>();

            foreach(KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                DataModels.SpecIF specif = keyValuePair.Value;

                foreach(Node hierarchy in specif.Hierarchies)
                {
                    Node parentNode = hierarchy.GetNodeByID(parentNodeKey.ID);

                    if(parentNode != null)
                    {
                        result.AddRange(parentNode.Nodes);
                        break;
                    }
                }
            }

            return result;
        }

        public override Node GetNodeByKey(Key key)
        {
            Node result = null;

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                DataModels.SpecIF specif = keyValuePair.Value;

                foreach (Node hierarchy in specif.Hierarchies)
                {
                    Node parentNode = hierarchy.GetNodeByID(key.ID);

                    if (parentNode != null)
                    {
                        result = parentNode;
                        break;
                    }
                }
            }

            return result;
        }

        public override Node GetParentNode(Key childNode)
        {
            Node result = null;

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                DataModels.SpecIF specif = keyValuePair.Value;

                Node node = specif.GetParentNode(childNode.ID);
                if(node != null)
                {
                    result = node;
                    break;
                }
            }

            return result;
        }

        //private void FindNodeRecusrsively(Node currentNode, Key key, ref Node result)
        //{
        //    if(currentNode.ID == key.ID && currentNode.Revision == key.Revision)
        //    {
        //        result = currentNode;
        //    }
        //    else
        //    {
        //        foreach(Node childNode in currentNode.Nodes)
        //        {
        //            FindNodeRecusrsively(childNode, key, ref result);
        //        }
        //    }
        //}

        //private void FindParentNodeRecusrsively(Node currentNode, Key key, ref Node result)
        //{
        //    foreach (Node childNode in currentNode.Nodes)
        //    {
        //        if (childNode.ID == key.ID && childNode.Revision == key.Revision)
        //        {
        //            result = currentNode;
        //            break;
        //        }
        //    }
            
        //    if(result == null)
        //    {
        //        foreach (Node childNode in currentNode.Nodes)
        //        {
        //            FindParentNodeRecusrsively(childNode, key, ref result);
        //        }
        //    }
            
        //}

        public override DataModels.SpecIF GetProject(ISpecIfMetadataReader metadataReader, string projectID,
            List<Key> hierarchyFilter = null, bool includeMetadata = true)
        {
            DataModels.SpecIF result = null;

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                if (keyValuePair.Value.ID == projectID)
                {
                    result = keyValuePair.Value;
                    break;
                }
            }

            return result;
        }

        public override List<ProjectDescriptor> GetProjectDescriptions()
        {
            List<ProjectDescriptor> result = new List<ProjectDescriptor>();

            foreach (KeyValuePair<string, T> keyValuePair in SpecIfData)
            {
                ProjectDescriptor projectDescriptor = new ProjectDescriptor(keyValuePair.Value);
                result.Add(projectDescriptor);
            }

            return result;
        }
        public override string GetProjectIDFromNodeID(string nodeID)
        {
            string projectID = "PRJ-DEFAULT";
            List<string> ProjectIDList = new List<string>();
            foreach(ProjectDescriptor descriptor in GetProjectDescriptions())
            {
                ProjectIDList.Add(descriptor.ID);
            }
            foreach(string ProjectID in  ProjectIDList)
            {
                bool nodeHasBeenFound = false;
                List<Node> NodeList = GetAllHierarchyRootNodes(ProjectID);
                foreach (Node node in NodeList)
                {
                    if(node.ID == nodeID)
                    {
                        return ProjectID;
                        nodeHasBeenFound = true;
                        break;
                    }
                }
                if (nodeHasBeenFound) break;
            }
            return projectID;
        }
    }
}
