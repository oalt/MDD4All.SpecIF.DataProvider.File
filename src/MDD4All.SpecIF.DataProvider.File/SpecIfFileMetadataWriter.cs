﻿using MDD4All.SpecIF.DataModels;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace MDD4All.SpecIF.DataProvider.File
{
    public class SpecIfFileMetadataWriter : AbstractSpecIfMetadataWriter
    {
        private DataModels.SpecIF _metadata;

        private string _metadataRootPath = "";

        private string _metadataFilePath = "";

        private string _metadataProjectID = "PRJ-METADATA";

        public SpecIfFileMetadataWriter(string metadataRootPath)
        {
            _metadataRootPath = metadataRootPath;
            InitializeMetadata();
        }

        private void InitializeMetadata()
        {
            _metadataFilePath = _metadataRootPath + "/" + _metadataProjectID + ".specif";
            if(!System.IO.File.Exists(_metadataFilePath))
            {
                ProjectDescriptor project = new ProjectDescriptor()
                {
                    CreatedAt = DateTime.Now,
                    Description = new List<MultilanguageText>
                    {
                        new MultilanguageText { Text = "SpecIF metadata project." }
                    },
                    ID = _metadataProjectID,
                    Title = new List<MultilanguageText>
                    {
                        new MultilanguageText { Text = "Metadata Project" }
                    }

                };

                DataModels.SpecIF specif = new DataModels.SpecIF();

                specif.ID = project.ID;
                specif.Title = project.Title;
                specif.Description = project.Description;
                specif.Schema = project.Schema;
                specif.Generator = project.Generator;
                specif.GeneratorVersion = project.GeneratorVersion;
                specif.CreatedAt = DateTime.Now;
                specif.CreatedBy = project.CreatedBy;
                specif.Rights = project.Rights;

                SpecIfFileReaderWriter.SaveSpecIfToFile(specif, _metadataFilePath);

                _metadata = specif;
            }
            else
            {
                string json = System.IO.File.ReadAllText(_metadataFilePath);
                _metadata = JsonConvert.DeserializeObject<DataModels.SpecIF>(json);
            }

        }

        public override void AddDataType(DataType dataType)
        {
            _metadata.DataTypes.Add(dataType);
            SpecIfFileReaderWriter.SaveSpecIfToFile(_metadata, _metadataFilePath);
        }

        public override void AddPropertyClass(PropertyClass propertyClass)
        {
            _metadata.PropertyClasses.Add(propertyClass);
            SpecIfFileReaderWriter.SaveSpecIfToFile(_metadata, _metadataFilePath);
        }

        public override void AddResourceClass(ResourceClass resourceClass)
        {
            _metadata.ResourceClasses.Add(resourceClass);
            SpecIfFileReaderWriter.SaveSpecIfToFile(_metadata, _metadataFilePath);
        }

        public override void AddStatementClass(StatementClass statementClass)
        {
            _metadata.StatementClasses.Add(statementClass);
            SpecIfFileReaderWriter.SaveSpecIfToFile(_metadata, _metadataFilePath);
        }

        public override void UpdateDataType(DataType dataType)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePropertyClass(PropertyClass propertyClass)
        {
            throw new NotImplementedException();
        }

        public override void UpdateResourceClass(ResourceClass resourceClass)
        {
            throw new NotImplementedException();
        }

        public override void UpdateStatementClass(StatementClass statementClass)
        {
            throw new NotImplementedException();
        }
    }
}
