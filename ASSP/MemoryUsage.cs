/*============================================================================
  File:    MemoryUsage.cs

  Summary: Provides a useful way of analyzing the results of the
           DISCOVER_MEMORYUSAGE command by putting those results into a cube

  Date:    April 03, 2007

  ----------------------------------------------------------------------------
  This file is part of the Analysis Services Stored Procedure Project.
  http://www.codeplex.com/ASStoredProcedures
  
  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
  PARTICULAR PURPOSE.
============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using AdomdServer = Microsoft.AnalysisServices.AdomdServer;
using AdomdClient = Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices;
using System.Data;

namespace ASStoredProcs
{
    public class MemoryUsage
    {
        private const string DATABASE_NAME = "Memory Usage";
        private const string TABLE_NAME = "MemoryUsage";
        private const string PARTITION_NAME = "MemoryUsagePartition";
        private const string AGG_DESIGN_NAME = "AggDesign";
        private const string NUM_SNAPSHOTS_ANNOTATION_NAME = "NumSnapshots";
        private const int ROWS_PER_BATCH = 10000;
        private const int SNAPSHOTS_PER_PARTITION = 100; //a moderately used 32-bit server will end up partitions around 50MB

        private Server svr;
        private Database db;
        private DateTime SnapshotDate = DateTime.Now;
        private StringBuilder sRowsBuffer;
        private int iRowsBuffered = 0;
        private long iMaxMemoryRowID = 0; //the surrogate key for the Memory dimension which will be unique across runs
        private string CurrentPartitionID = PARTITION_NAME + " 0001";

        //holds the mapping of SPID to username for sessions on the server
        private Dictionary<int, string> sessions = new Dictionary<int, string>();

        private Dictionary<string, long> dictExistingMemoryRowID = new Dictionary<string, long>(); //there are many rows which have the exact same dimension values, so these can share one MemoryRowID
        private Dictionary<string, ASXmlDescriptorFileInfo> dictXmlDefinitionFiles;

        public MemoryUsage()
        {
            svr = new Microsoft.AnalysisServices.Server();
            svr.Connect("Data Source=" + AdomdServer.Context.CurrentServerID); //connect to a new session... otherwise the changes that are saved under this connection won't be committed until the sproc ends... and we need yet other connections to be able to see the changes prior to the sproc completing
            CreateDatabase();
        }

        public void SnapshotMemoryUsageTotals()
        {
            LoadExistingMemoryDimensionSignatures();
            LoadSessions();
            DiscoverMemoryUsage();
        }

        #region Create Database
        private void CreateDatabase()
        {
            db = svr.Databases.FindByName(DATABASE_NAME);
            bool bExists = (db != null);
            if (bExists)
            {
                try
                {
                    //find max MemoryRowID
                    AdomdClient.AdomdCommand cmd = new AdomdClient.AdomdCommand("with member MaxMemoryRowID as Tail([Memory].[Memory Name].[Memory Name].Members,1).Item(0).Properties('Key0') select MaxMemoryRowID on 0 from [Memory Usage]");
                    cmd.Connection = new AdomdClient.AdomdConnection("Data Source=" + AdomdServer.Context.CurrentServerID + ";Initial Catalog=" + DATABASE_NAME);
                    cmd.Connection.Open();
                    iMaxMemoryRowID = long.Parse(cmd.ExecuteCellSet().Cells[0].Value.ToString());
                    cmd.Connection.Close();
                }
                catch { }

                //find the current partition
                MeasureGroup measureGroup = db.Cubes[DATABASE_NAME].MeasureGroups[DATABASE_NAME];
                try
                {
                    foreach (Partition p in measureGroup.Partitions)
                    {
                        if (int.Parse(p.ID.Split(new char[] { ' ' })[1]) > int.Parse(CurrentPartitionID.Split(new char[] { ' ' })[1]))
                        {
                            this.CurrentPartitionID = p.ID;
                        }
                    }
                }
                catch { }

                Partition currentPartition = measureGroup.Partitions[CurrentPartitionID];
                int iNumSnapshots = int.Parse(currentPartition.Annotations[NUM_SNAPSHOTS_ANNOTATION_NAME].Value.InnerText);
                if (SNAPSHOTS_PER_PARTITION <= iNumSnapshots)
                {
                    //create a new partition
                    int iHighestPartitionNumber = int.Parse(CurrentPartitionID.Split(new char[] { ' ' })[1]);
                    this.CurrentPartitionID = PARTITION_NAME + " " + (iHighestPartitionNumber + 1).ToString("0000");
                    CreateNewPartition(measureGroup);
                }
                else
                {
                    //keep same partition... need to update the annotation
                    currentPartition.Annotations[NUM_SNAPSHOTS_ANNOTATION_NAME].Value.InnerText = (iNumSnapshots + 1).ToString();
                }
                measureGroup.Update(UpdateOptions.ExpandFull);

                return; //don't continue because it already exists
            }

            //create the database because it doesn't exist
            db = svr.Databases.Add(DATABASE_NAME);

            // Create the data source
            DataSource ds = db.DataSources.Add(DATABASE_NAME);
            ds.ConnectionString = "NotNeeded"; //all that's needed is a non-empty string

            // Create the data source view. Even thought it only uses out-of-line bindings, this is still necessary
            DataSourceView dsv = db.DataSourceViews.Add(DATABASE_NAME);
            dsv.DataSourceID = DATABASE_NAME;
            dsv.Schema = new DataSet(DATABASE_NAME);
            dsv.Schema.Locale = System.Globalization.CultureInfo.CurrentCulture;
            DataTable t = dsv.Schema.Tables.Add(TABLE_NAME);
            t.Columns.Add("MemoryID", typeof(long));
            t.Columns.Add("MemoryName", typeof(string));
            t.Columns.Add("SPID", typeof(int));
            t.Columns.Add("CreationTime", typeof(DateTime));
            t.Columns.Add("MemoryUsed", typeof(long));
            t.Columns.Add("MemoryAllocated", typeof(long));
            t.Columns.Add("MemoryAllocBase", typeof(long));
            t.Columns.Add("MemoryAllocFromAlloc", typeof(long));
            t.Columns.Add("ElementCount", typeof(long));
            t.Columns.Add("Shrinkable", typeof(string));
            t.Columns.Add("ShrinkableMemory", typeof(long));
            t.Columns.Add("Folder1", typeof(string));
            t.Columns.Add("Folder2", typeof(string));
            t.Columns.Add("Folder3", typeof(string));
            t.Columns.Add("Folder4", typeof(string));
            t.Columns.Add("Level1", typeof(string));
            t.Columns.Add("Level2", typeof(string));
            t.Columns.Add("Level3", typeof(string));
            t.Columns.Add("Level4", typeof(string));
            t.Columns.Add("SnapshotDate", typeof(DateTime));
            t.Columns.Add("MemoryCount", typeof(long));
            t.Columns.Add("MemoryRowID", typeof(long));
            t.Columns.Add("FileExtension", typeof(string));
            t.Columns.Add("User", typeof(string));
            t.Columns.Add("DiskUsed", typeof(long));

            // Create the Date dimension
            Dimension dim = db.Dimensions.Add("Snapshot Date");
            dim.Type = DimensionType.Time;
            dim.UnknownMember = UnknownMemberBehavior.Hidden;
            dim.AttributeAllMemberName = "All";
            dim.Source = new DataSourceViewBinding(DATABASE_NAME);
            dim.StorageMode = DimensionStorageMode.Molap;

            DimensionAttribute attr;
            attr = dim.Attributes.Add("Snapshot Date");
            attr.Usage = AttributeUsage.Key;
            attr.Type = AttributeType.Date;
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "SnapshotDate"));

            // Create the Memory dimension
            dim = db.Dimensions.Add("Memory");
            dim.Type = DimensionType.Regular;
            dim.UnknownMember = UnknownMemberBehavior.Hidden;
            dim.AttributeAllMemberName = "All";
            dim.Source = new DataSourceViewBinding(DATABASE_NAME);
            dim.StorageMode = DimensionStorageMode.Molap;

            attr = dim.Attributes.Add("Memory Name");
            attr.Usage = AttributeUsage.Key;
            attr.OrderBy = OrderBy.Name;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "MemoryRowID")); //MemoryID won't be unique across snapshots... MemoryRowID is an incrementing ID field we create
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "MemoryName");
            attr.AttributeHierarchyOptimizedState = OptimizationType.NotOptimized; //is expensive and not used in reports

            attr = dim.Attributes.Add("User");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "User"));

            attr = dim.Attributes.Add("Creation Time");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "CreationTime"));

            //BaseObjectType isn't helpful, and it causes the dimension size to increase because there are more distinct combinations which requires more MemoryRowID values
            //attr = dim.Attributes.Add("Base Object Type");
            //attr.OrderBy = OrderBy.Key;
            //attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "BaseObjectType"));

            attr = dim.Attributes.Add("Shrinkable");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Shrinkable"));

            attr = dim.Attributes.Add("Folder 1");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));

            attr = dim.Attributes.Add("Folder 2");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder2"));
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "Folder2");

            attr = dim.Attributes.Add("Folder 3");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder3"));
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "Folder3");

            attr = dim.Attributes.Add("Folder 4");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder3"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level3"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder4"));
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "Folder4");

            attr = dim.Attributes.Add("Level 1");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level1"));
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "Level1");

            attr = dim.Attributes.Add("Level 2");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level2"));
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "Level2");

            attr = dim.Attributes.Add("Level 3");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder3"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level3"));
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "Level3");

            attr = dim.Attributes.Add("Level 4");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level1"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level2"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder3"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level3"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Folder4"));
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "Level4"));
            attr.NameColumn = CreateDataItem(dsv, TABLE_NAME, "Level4");

            attr = dim.Attributes.Add("File Extension");
            attr.OrderBy = OrderBy.Key;
            attr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "FileExtension"));

            dim.Attributes["Level 1"].AttributeRelationships.Add("Folder 1");
            dim.Attributes["Level 2"].AttributeRelationships.Add("Folder 2");
            dim.Attributes["Level 3"].AttributeRelationships.Add("Folder 3");
            dim.Attributes["Level 4"].AttributeRelationships.Add("Folder 4");
            dim.Attributes["Folder 2"].AttributeRelationships.Add("Level 1");
            dim.Attributes["Folder 3"].AttributeRelationships.Add("Level 2");
            dim.Attributes["Folder 4"].AttributeRelationships.Add("Level 3");
            //no need to specify the attribute relationships that are from the key attribute... AMO takes care of those

            Hierarchy hier = dim.Hierarchies.Add("Tree");
            hier.Levels.Add("Folder 1").SourceAttributeID = "Folder 1";
            hier.Levels.Add("Level 1").SourceAttributeID = "Level 1";
            hier.Levels.Add("Folder 2").SourceAttributeID = "Folder 2";
            hier.Levels.Add("Level 2").SourceAttributeID = "Level 2";
            hier.Levels.Add("Folder 3").SourceAttributeID = "Folder 3";
            hier.Levels.Add("Level 3").SourceAttributeID = "Level 3";
            hier.Levels.Add("Folder 4").SourceAttributeID = "Folder 4";
            hier.Levels.Add("Level 4").SourceAttributeID = "Level 4";
            foreach (Level l in hier.Levels)
            {
                l.HideMemberIf = HideIfValue.NoName;
            }

            // Create the Adventure Works cube
            Cube cube = db.Cubes.Add(DATABASE_NAME);
            cube.DefaultMeasure = "[Memory Used]";
            cube.Source = new DataSourceViewBinding(DATABASE_NAME);
            cube.StorageMode = StorageMode.Molap;

            dim = db.Dimensions.GetByName("Snapshot Date");
            cube.Dimensions.Add(dim.ID);

            dim = db.Dimensions.GetByName("Memory");
            cube.Dimensions.Add(dim.ID);

            MeasureGroup mg = cube.MeasureGroups.Add(DATABASE_NAME);
            mg.StorageMode = StorageMode.Molap;
            mg.ProcessingMode = ProcessingMode.Regular;
            mg.Type = MeasureGroupType.Regular;


            Measure meas;
            meas = mg.Measures.Add("Memory Used");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "MemoryUsed");

            meas = mg.Measures.Add("Memory Allocated");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "MemoryAllocated");

            meas = mg.Measures.Add("Memory Alloc Base");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "MemoryAllocBase");

            meas = mg.Measures.Add("Memory Alloc From Alloc");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "MemoryAllocFromAlloc");

            meas = mg.Measures.Add("Element Count");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "ElementCount");

            meas = mg.Measures.Add("Shrinkable Memory");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "ShrinkableMemory");

            meas = mg.Measures.Add("Memory Count");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "MemoryCount");

            meas = mg.Measures.Add("Disk Used");
            meas.AggregateFunction = AggregationFunction.LastChild;
            meas.FormatString = "#,#";
            meas.Source = CreateDataItem(dsv, TABLE_NAME, "DiskUsed");

            CubeDimension cubeDim;
            RegularMeasureGroupDimension regMgDim;
            MeasureGroupAttribute mgAttr;

            cubeDim = cube.Dimensions.GetByName("Snapshot Date");
            regMgDim = new RegularMeasureGroupDimension(cubeDim.ID);
            mg.Dimensions.Add(regMgDim);
            mgAttr = regMgDim.Attributes.Add(cubeDim.Dimension.KeyAttribute.ID);
            mgAttr.Type = MeasureGroupAttributeType.Granularity;
            mgAttr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "SnapshotDate"));

            cubeDim = cube.Dimensions.GetByName("Memory");
            regMgDim = new RegularMeasureGroupDimension(cubeDim.ID);
            mg.Dimensions.Add(regMgDim);
            mgAttr = regMgDim.Attributes.Add(cubeDim.Dimension.KeyAttribute.ID);
            mgAttr.Type = MeasureGroupAttributeType.Granularity;
            mgAttr.KeyColumns.Add(CreateDataItem(dsv, TABLE_NAME, "MemoryRowID")); //MemoryID can have different meanings over time, so we need a composite key

            CreateNewPartition(mg);

            db.Update(UpdateOptions.ExpandFull);
        }

        private void CreateNewPartition(MeasureGroup mg)
        {
            Partition part;
            part = mg.Partitions.Add(this.CurrentPartitionID);
            part.StorageMode = StorageMode.Molap;
            part.Source = new QueryBinding(mg.Parent.Source.DataSourceViewID, "NotNeeded");
            part.Annotations.Add(new Annotation(NUM_SNAPSHOTS_ANNOTATION_NAME, "1"));
            //setting the slice won't work because you can't modify it for future data without unprocessing the partition... have to rely on the auto-slice which should do fine in this situation for determining which partition a snapshot date is in

            if (!mg.AggregationDesigns.Contains(AGG_DESIGN_NAME))
            {
                AggregationDesign design = mg.AggregationDesigns.Add(AGG_DESIGN_NAME);
                Aggregation agg = design.Aggregations.Add("Agg1");
                AggregationDimension aggdim;

                aggdim = agg.Dimensions.Add("Snapshot Date");
                aggdim.Attributes.Add(aggdim.Dimension.KeyAttribute.ID);

                aggdim = agg.Dimensions.Add("Memory");
                aggdim.Attributes.Add(aggdim.Dimension.Attributes["Level 4"].ID);
            }

            part.AggregationDesignID = AGG_DESIGN_NAME;
        }

        private static DataItem CreateDataItem(DataSourceView dsv, string tableName, string columnName)
        {
            DataTable dataTable = ((DataSourceView)dsv).Schema.Tables[tableName];
            DataColumn dataColumn = dataTable.Columns[columnName];
            return new DataItem(tableName, columnName, OleDbTypeConverter.GetRestrictedOleDbType(dataColumn.DataType));
        }
        #endregion

        #region XMLA process commands
        private void ProcessAddSnapshotDate()
        {
            string sXMLA = "<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\r\n"
             + "<Body>" + "\r\n"
             + "<Execute xmlns=\"urn:schemas-microsoft-com:xml-analysis\">" + "\r\n"
             + "  <Command>" + "\r\n"
             + "    <Process xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "      <Type>" + (db.Dimensions["Snapshot Date"].State == AnalysisState.Unprocessed ? "ProcessFull" : "ProcessAdd") + "</Type>" + "\r\n"
             + "      <Object xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" + "\r\n"
             + "        <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "        <DimensionID>Snapshot Date</DimensionID>" + "\r\n"
             + "      </Object>" + "\r\n"
             + "      <Bindings xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\">" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Snapshot Date</DimensionID>" + "\r\n"
             + "          <AttributeID>Snapshot Date</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>SnapshotDate</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>SnapshotDate</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "      </Bindings>" + "\r\n"
             + "      <DataSource xsi:type=\"PushedDataSource\">" + "\r\n"
             + "        <root Parameter=\"InputRowset\"/>" + "\r\n"
             + "        <EndOfData Parameter=\"EndOfInputRowset\"/>" + "\r\n"
             + "      </DataSource>" + "\r\n"
             + "    </Process>" + "\r\n"
             + "  </Command>" + "\r\n"
             + "  <Properties>" + "\r\n"
             + "  </Properties>" + "\r\n"
             + "  <Parameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"urn:schemas-microsoft-com:xml-analysis\">" + "\r\n"
             + "    <Parameter>" + "\r\n"
             + "      <Name>EndOfInputRowset</Name>" + "\r\n"
             + "      <Value xsi:type=\"xsd:boolean\">true</Value>" + "\r\n"
             + "    </Parameter>" + "\r\n"
             + "    <Parameter>" + "\r\n"
             + "      <Name>InputRowset</Name>" + "\r\n"
             + "      <Value xmlns=\"urn:schemas-microsoft-com:xml-analysis:rowset\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + "\r\n"
             + "        <xsd:schema targetNamespace=\"urn:schemas-microsoft-com:xml-analysis:rowset\" xmlns:sql=\"urn:schemas-microsoft-com:xml-sql\" elementFormDefault=\"qualified\">" + "\r\n"
             + "          <xsd:element name=\"root\">" + "\r\n"
             + "            <xsd:complexType>" + "\r\n"
             + "              <xsd:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">" + "\r\n"
             + "                <xsd:element name=\"row\" type=\"row\" />" + "\r\n"
             + "              </xsd:sequence>" + "\r\n"
             + "            </xsd:complexType>" + "\r\n"
             + "          </xsd:element>" + "\r\n"
             + "          <xsd:complexType name=\"row\">" + "\r\n"
             + "            <xsd:sequence>" + "\r\n"
             + "              <xsd:element sql:field=\"SnapshotDate\" name=\"SnapshotDate\" type=\"xsd:dateTime\" minOccurs=\"0\" />" + "\r\n"
             + "            </xsd:sequence>" + "\r\n"
             + "          </xsd:complexType>" + "\r\n"
             + "        </xsd:schema>" + "\r\n"
             + "        <row>" + "\r\n"
             + "          <SnapshotDate>" + this.SnapshotDate.ToString("s") + "</SnapshotDate>" + "\r\n"
             + "        </row>" + "\r\n"
             + "      </Value>" + "\r\n"
             + "    </Parameter>" + "\r\n"
             + "  </Parameters>" + "\r\n"
             + "</Execute>" + "\r\n"
             + "</Body>" + "\r\n"
             + "</Envelope>" + "\r\n";

            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(sXMLA));
            XmlReader reader = null;
            try
            {
                reader = svr.SendXmlaRequest(XmlaRequestType.Execute, ms);
                if (reader.Read())
                {
                    string sResponse = reader.ReadOuterXml();
                    if (sResponse.Contains("</soap:Fault>") || sResponse.Contains("</Exception>"))
                    {
                        throw new Exception(sResponse);
                    }
                }
            }
            finally
            {
                try { reader.Close(); }
                catch { }
            }
        }

        private void ProcessAddMemoryDimension()
        {
            db.Dimensions["Memory"].Refresh(); //to get the latest processed state
            string sXMLA = "<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\r\n"
             + "<Body>" + "\r\n"
             + "<Execute xmlns=\"urn:schemas-microsoft-com:xml-analysis\">" + "\r\n"
             + "  <Command>" + "\r\n"
             + "    <Process xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\r\n"
             + "      <Type>" + (db.Dimensions["Memory"].State == AnalysisState.Unprocessed ? "ProcessFull" : "ProcessAdd") + "</Type>" + "\r\n"
             + "      <Object xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" + "\r\n"
             + "        <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "        <DimensionID>Memory</DimensionID>" + "\r\n"
             + "      </Object>" + "\r\n"
             + "      <Bindings xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Memory Name</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>MemoryRowID</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>MemoryName</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>User</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>User</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>User</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Creation Time</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>CreationTime</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>CreationTime</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Shrinkable</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Shrinkable</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Shrinkable</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Folder 1</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Folder 2</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Folder2</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Folder 3</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder3</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Folder3</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Folder 4</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder3</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level3</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder4</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Folder4</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Level 1</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Level1</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Level 2</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Level2</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Level 3</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder3</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level3</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Level3</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>Level 4</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level1</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level2</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder3</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level3</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Folder4</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>Level4</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>Level4</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <DimensionID>Memory</DimensionID>" + "\r\n"
             + "          <AttributeID>File Extension</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>FileExtension</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "          <NameColumn>" + "\r\n"
             + "            <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "              <TableID/>" + "\r\n"
             + "              <ColumnID>FileExtension</ColumnID>" + "\r\n"
             + "            </Source>" + "\r\n"
             + "          </NameColumn>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "      </Bindings>" + "\r\n"
             + "      <DataSource xsi:type=\"PushedDataSource\">" + "\r\n"
             + "        <root Parameter=\"InputRowset\"/>" + "\r\n"
             + "        <EndOfData Parameter=\"EndOfInputRowset\"/>" + "\r\n"
             + "      </DataSource>" + "\r\n"
             + "    </Process>" + "\r\n"
             + "  </Command>" + "\r\n"
             + "<Properties>" + "\r\n"
             + "</Properties>" + "\r\n"
             + "<Parameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"urn:schemas-microsoft-com:xml-analysis\">" + "\r\n"
             + "  <Parameter>" + "\r\n"
             + "    <Name>EndOfInputRowset</Name>" + "\r\n"
             + "    <Value xsi:type=\"xsd:boolean\">true</Value>" + "\r\n"
             + "  </Parameter>" + "\r\n"
             + "  <Parameter>" + "\r\n"
             + "    <Name>InputRowset</Name>" + "\r\n"
             + "    <Value xmlns=\"urn:schemas-microsoft-com:xml-analysis:rowset\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + "\r\n"
             + "      <xsd:schema targetNamespace=\"urn:schemas-microsoft-com:xml-analysis:rowset\" xmlns:sql=\"urn:schemas-microsoft-com:xml-sql\" elementFormDefault=\"qualified\">" + "\r\n"
             + "        <xsd:element name=\"root\">" + "\r\n"
             + "          <xsd:complexType>" + "\r\n"
             + "            <xsd:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">" + "\r\n"
             + "              <xsd:element name=\"row\" type=\"row\" />" + "\r\n"
             + "            </xsd:sequence>" + "\r\n"
             + "          </xsd:complexType>" + "\r\n"
             + "        </xsd:element>" + "\r\n"
             + "        <xsd:complexType name=\"row\">" + "\r\n"
             + "          <xsd:sequence>" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryID\" name=\"MemoryID\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryName\" name=\"MemoryName\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"SPID\" name=\"SPID\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"CreationTime\" name=\"CreationTime\" type=\"xsd:dateTime\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"BaseObjectType\" name=\"BaseObjectType\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryUsed\" name=\"MemoryUsed\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryAllocated\" name=\"MemoryAllocated\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryAllocBase\" name=\"MemoryAllocBase\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryAllocFromAlloc\" name=\"MemoryAllocFromAlloc\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"ElementCount\" name=\"ElementCount\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Shrinkable\" name=\"Shrinkable\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"ShrinkableMemory\" name=\"ShrinkableMemory\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder1\" name=\"Folder1\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder2\" name=\"Folder2\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder3\" name=\"Folder3\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder4\" name=\"Folder4\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level1\" name=\"Level1\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level2\" name=\"Level2\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level3\" name=\"Level3\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level4\" name=\"Level4\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"SnapshotDate\" name=\"SnapshotDate\" type=\"xsd:dateTime\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryCount\" name=\"MemoryCount\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryRowID\" name=\"MemoryRowID\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"FileExtension\" name=\"FileExtension\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"User\" name=\"User\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"DiskUsed\" name=\"DiskUsed\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "          </xsd:sequence>" + "\r\n"
             + "        </xsd:complexType>" + "\r\n"
             + "      </xsd:schema>" + "\r\n"
             + this.sRowsBuffer.ToString() + "\r\n"
             + "    </Value>" + "\r\n"
             + "  </Parameter>" + "\r\n"
             + "</Parameters>" + "\r\n"
             + "</Execute>" + "\r\n"
             + "</Body>" + "\r\n"
             + "</Envelope>" + "\r\n";

            int iRetries = 0;
            while (iRetries < 3)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(sXMLA));
                XmlReader reader = null;
                try
                {
                    reader = svr.SendXmlaRequest(XmlaRequestType.Execute, ms);
                    if (reader.Read())
                    {
                        string sResponse = reader.ReadOuterXml();
                        if (sResponse.Contains("</soap:Fault>") || sResponse.Contains("</Exception>"))
                        {
                            throw new Exception(sResponse);
                        }
                    }
                    iRetries = 10;
                }
                catch (Exception ex)
                {
                    iRetries++;
                    try { reader.Close(); }
                    catch { }
                    if (iRetries >= 3) throw new Exception("Too many retries");
                }
                finally
                {
                    try { reader.Close(); }
                    catch { }
                }
            }
        }


        private void ProcessAddPartition()
        {
            Partition p = db.Cubes[DATABASE_NAME].MeasureGroups[DATABASE_NAME].Partitions[this.CurrentPartitionID];
            p.Refresh(); //so that we'll have the latest state
            string sXMLA = "<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\r\n"
             + "<Body>" + "\r\n"
             + "<Execute xmlns=\"urn:schemas-microsoft-com:xml-analysis\">" + "\r\n"
             + "  <Command>" + "\r\n"
             + "    <Process xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\r\n"
             + "      <Type>" + (p.State == AnalysisState.Unprocessed ? "ProcessData" : "ProcessAdd") + "</Type>" + "\r\n"
             + "      <Object xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" + "\r\n"
             + "        <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "        <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "        <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "        <PartitionID>" + this.CurrentPartitionID + "</PartitionID>" + "\r\n"
             + "      </Object>" + "\r\n"
             + "      <Bindings xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <CubeDimensionID>Snapshot Date</CubeDimensionID>" + "\r\n"
             + "          <AttributeID>Snapshot Date</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>SnapshotDate</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Memory Used</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>MemoryUsed</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Memory Allocated</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>MemoryAllocated</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Memory Alloc Base</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>MemoryAllocBase</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Memory Alloc From Alloc</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>MemoryAllocFromAlloc</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Element Count</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>ElementCount</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Shrinkable Memory</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>ShrinkableMemory</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <CubeDimensionID>Memory</CubeDimensionID>" + "\r\n"
             + "          <AttributeID>Memory Name</AttributeID>" + "\r\n"
             + "          <KeyColumns>" + "\r\n"
             + "            <KeyColumn>" + "\r\n"
             + "              <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "                <TableID/>" + "\r\n"
             + "                <ColumnID>MemoryRowID</ColumnID>" + "\r\n"
             + "              </Source>" + "\r\n"
             + "            </KeyColumn>" + "\r\n"
             + "          </KeyColumns>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Memory Count</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>MemoryCount</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "        <Binding xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <MeasureID>Disk Used</MeasureID>" + "\r\n"
             + "          <Source xsi:type=\"ColumnBinding\">" + "\r\n"
             + "            <TableID/>" + "\r\n"
             + "            <ColumnID>DiskUsed</ColumnID>" + "\r\n"
             + "          </Source>" + "\r\n"
             + "        </Binding>" + "\r\n"
             + "      </Bindings>" + "\r\n"
             + "      <DataSource xsi:type=\"PushedDataSource\">" + "\r\n"
             + "        <root Parameter=\"InputRowset\"/>" + "\r\n"
             + "        <EndOfData Parameter=\"EndOfInputRowset\"/>" + "\r\n"
             + "      </DataSource>" + "\r\n"
             + "    </Process>" + "\r\n"
             + "  </Command>" + "\r\n"
             + "  <Properties>" + "\r\n"
             + "  </Properties>" + "\r\n"
             + "<Parameters xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns=\"urn:schemas-microsoft-com:xml-analysis\">" + "\r\n"
             + "  <Parameter>" + "\r\n"
             + "    <Name>EndOfInputRowset</Name>" + "\r\n"
             + "    <Value xsi:type=\"xsd:boolean\">true</Value>" + "\r\n"
             + "  </Parameter>" + "\r\n"
             + "  <Parameter>" + "\r\n"
             + "    <Name>InputRowset</Name>" + "\r\n"
             + "    <Value xmlns=\"urn:schemas-microsoft-com:xml-analysis:rowset\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + "\r\n"
             + "      <xsd:schema targetNamespace=\"urn:schemas-microsoft-com:xml-analysis:rowset\" xmlns:sql=\"urn:schemas-microsoft-com:xml-sql\" elementFormDefault=\"qualified\">" + "\r\n"
             + "        <xsd:element name=\"root\">" + "\r\n"
             + "          <xsd:complexType>" + "\r\n"
             + "            <xsd:sequence minOccurs=\"0\" maxOccurs=\"unbounded\">" + "\r\n"
             + "              <xsd:element name=\"row\" type=\"row\" />" + "\r\n"
             + "            </xsd:sequence>" + "\r\n"
             + "          </xsd:complexType>" + "\r\n"
             + "        </xsd:element>" + "\r\n"
             + "        <xsd:complexType name=\"row\">" + "\r\n"
             + "          <xsd:sequence>" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryID\" name=\"MemoryID\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryName\" name=\"MemoryName\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"SPID\" name=\"SPID\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"CreationTime\" name=\"CreationTime\" type=\"xsd:dateTime\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"BaseObjectType\" name=\"BaseObjectType\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryUsed\" name=\"MemoryUsed\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryAllocated\" name=\"MemoryAllocated\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryAllocBase\" name=\"MemoryAllocBase\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryAllocFromAlloc\" name=\"MemoryAllocFromAlloc\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"ElementCount\" name=\"ElementCount\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Shrinkable\" name=\"Shrinkable\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"ShrinkableMemory\" name=\"ShrinkableMemory\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder1\" name=\"Folder1\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder2\" name=\"Folder2\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder3\" name=\"Folder3\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Folder4\" name=\"Folder4\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level1\" name=\"Level1\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level2\" name=\"Level2\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level3\" name=\"Level3\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"Level4\" name=\"Level4\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"SnapshotDate\" name=\"SnapshotDate\" type=\"xsd:dateTime\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryCount\" name=\"MemoryCount\" type=\"xsd:unsignedInt\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"MemoryRowID\" name=\"MemoryRowID\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"FileExtension\" name=\"FileExtension\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"User\" name=\"User\" type=\"xsd:string\" minOccurs=\"0\" />" + "\r\n"
             + "            <xsd:element sql:field=\"DiskUsed\" name=\"DiskUsed\" type=\"xsd:unsignedLong\" minOccurs=\"0\" />" + "\r\n"
             + "          </xsd:sequence>" + "\r\n"
             + "        </xsd:complexType>" + "\r\n"
             + "      </xsd:schema>" + "\r\n"
             + this.sRowsBuffer.ToString() + "\r\n"
             + "    </Value>" + "\r\n"
             + "  </Parameter>" + "\r\n"
             + "</Parameters>" + "\r\n"
             + "</Execute>" + "\r\n"
             + "</Body>" + "\r\n"
             + "</Envelope>" + "\r\n";

            int iRetries = 0;
            while (iRetries < 3)
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(sXMLA));
                XmlReader reader = null;
                try
                {
                    reader = svr.SendXmlaRequest(XmlaRequestType.Execute, ms);
                    if (reader.Read())
                    {
                        string sResponse = reader.ReadOuterXml();
                        if (sResponse.Contains("</soap:Fault>") || sResponse.Contains("</Exception>"))
                        {
                            throw new Exception(sResponse);
                        }
                    }
                    iRetries = 10;
                }
                catch (Exception ex)
                {
                    iRetries++;
                    try { reader.Close(); }
                    catch { }
                    if (iRetries >= 3) throw new Exception("Too many retries");
                }
                finally
                {
                    try { reader.Close(); }
                    catch { }
                }
            }
        }

        //processes aggregations
        private void ProcessIndexes()
        {
            string sXMLA = "<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\">" + "\r\n"
             + "<Body>" + "\r\n"
             + "<Execute xmlns=\"urn:schemas-microsoft-com:xml-analysis\">" + "\r\n"
             + "  <Command>" + "\r\n"
             + "    <Batch xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" + "\r\n"
             + "      <Process xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:ddl2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2\" xmlns:ddl2_2=\"http://schemas.microsoft.com/analysisservices/2003/engine/2/2\">" + "\r\n"
             + "        <Object>" + "\r\n"
             + "          <DatabaseID>Memory Usage</DatabaseID>" + "\r\n"
             + "          <CubeID>Memory Usage</CubeID>" + "\r\n"
             + "          <MeasureGroupID>Memory Usage</MeasureGroupID>" + "\r\n"
             + "          <PartitionID>" + this.CurrentPartitionID + "</PartitionID>" + "\r\n"
             + "        </Object>" + "\r\n"
             + "        <Type>ProcessIndexes</Type>" + "\r\n"
             + "        <WriteBackTableCreation>UseExisting</WriteBackTableCreation>" + "\r\n"
             + "      </Process>" + "\r\n"
             + "    </Batch>" + "\r\n"
             + "  </Command>" + "\r\n"
             + "  <Properties>" + "\r\n"
             + "  </Properties>" + "\r\n"
             + "</Execute>" + "\r\n"
             + "</Body>" + "\r\n"
             + "</Envelope>" + "\r\n";

            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(sXMLA));
            XmlReader reader = null;
            try
            {
                reader = svr.SendXmlaRequest(XmlaRequestType.Execute, ms);
                if (reader.Read())
                {
                    string sResponse = reader.ReadOuterXml();
                    if (sResponse.Contains("</soap:Fault>") || sResponse.Contains("</Exception>"))
                    {
                        throw new Exception(sResponse);
                    }
                }
            }
            finally
            {
                try { reader.Close(); }
                catch { }
            }
        }
        #endregion

        private void LoadSessions()
        {
            AdomdClient.AdomdConnection conn = new AdomdClient.AdomdConnection("Data Source=" + AdomdServer.Context.CurrentServerID);
            conn.Open();
            DataSet ds = conn.GetSchemaDataSet("DISCOVER_SESSIONS", null);
            conn.Close();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                sessions.Add(Convert.ToInt32(dr["SESSION_SPID"]), Convert.ToString(dr["SESSION_USER_NAME"]));
            }
        }

        private string GetUserForSPID(int SPID)
        {
            if (SPID == 0) return "System";
            if (sessions.ContainsKey(SPID))
            {
                return sessions[SPID];
            }
            else
            {
                return "Unknown";
            }
        }

        //Scans all .xml files in the DataDir looking for the metadata files for each AS object.
        //The metadata files help us determine which files are tied to which AS object
        //All metadata files will be in the DataDir, even when the StorageLocation is changed, I think
        private Dictionary<string, ASXmlDescriptorFileInfo> ScanXmlFilesInDataDir()
        {
            Dictionary<string, ASXmlDescriptorFileInfo> dict = new Dictionary<string, ASXmlDescriptorFileInfo>();

            foreach (string file in System.IO.Directory.GetFiles(svr.ServerProperties["DataDir"].Value, "*.xml", System.IO.SearchOption.AllDirectories))
            {
                ASXmlDescriptorFileInfo info = ProcessXmlFile(new System.IO.FileInfo(file));
                if (info != null)
                {
                    dict.Add(info.DirectoryName, info);
                }
            }
            return dict;
        }

        private void DiscoverMemoryUsage()
        {
            dictXmlDefinitionFiles = ScanXmlFilesInDataDir();

            ProcessAddSnapshotDate();

            string sCmd = "<Envelope xmlns=\"http://schemas.xmlsoap.org/soap/envelope/\"><Body>"
             + "<Discover xmlns=\"urn:schemas-microsoft-com:xml-analysis\">"
             + " <RequestType>DISCOVER_MEMORYUSAGE</RequestType>"
             + " <Restrictions></Restrictions>"
             + " <Properties></Properties>"
             + "</Discover>"
             + "</Body></Envelope>";

            XmlReader reader = null;
            try
            {
                Server server = new Server();
                server.Connect("Data Source=" + AdomdServer.Context.CurrentServerID); //need to open a separate connection for the discover memory usage because it will be open while the main session is running process commands

                System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.ASCII.GetBytes(sCmd));
                reader = server.SendXmlaRequest(Microsoft.AnalysisServices.XmlaRequestType.Discover, ms);
                reader.ReadToFollowing("row");

                int iTotalCombinedMemoryRowIDs = 0;
                XmlDocument xml = new XmlDocument(reader.NameTable);
                xml.AppendChild(xml.CreateElement("row", "urn:schemas-microsoft-com:xml-analysis:rowset"));
                while (reader.Name == "row")
                {
                    iMaxMemoryRowID++;
                    XmlReader rowReader = reader.ReadSubtree();
                    xml.Load(rowReader);
                    rowReader.Close();
                    if (xml.DocumentElement == null) break;
                    reader.Read(); //advance to the next row node in preparation for the next ReadSubtree
                    bool bProcessed = false;
                    bool bIsFileStore = false;
                    string sMemoryName = GetInnerTextFromChildElement(xml.DocumentElement, "MemoryName");
                    string sFixedMemoryName = StripUnprintableChars(sMemoryName).Trim(); //unprintable characters cause AS errors: https://connect.microsoft.com/SQLServer/feedback/ViewFeedback.aspx?FeedbackID=264939
                    if (sFixedMemoryName != sMemoryName)
                    {
                        SetInnerTextForChildElement(xml.DocumentElement, "MemoryName", sFixedMemoryName);
                        sMemoryName = sFixedMemoryName;
                    }
                    if (sMemoryName.StartsWith("FileStore: "))
                    {
                        string sWithoutPrefix = sMemoryName.Substring("FileStore: ".Length).Trim();
                        if (sWithoutPrefix.StartsWith(@"\\?\")) sWithoutPrefix = sWithoutPrefix.Substring(@"\\?\".Length);
                        bIsFileStore = true;
                        string[] ar = sWithoutPrefix.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (ar.Length > 1) throw new Exception("multiple files!");
                        string sPath = ar[0].Replace("\\\\", "\\"); //apparently there's a bug in that the path sometimes contains double back slashes
                        if (System.IO.File.Exists(sPath))
                        {
                            System.IO.FileInfo fi = new System.IO.FileInfo(sPath);
                            if (dictXmlDefinitionFiles.ContainsKey(fi.Directory.FullName))
                            {
                                ASXmlDescriptorFileInfo info = dictXmlDefinitionFiles[fi.Directory.FullName];
                                if (info.DataFileList.Contains(fi.Name))
                                {
                                    info.AppendRowFields(xml, this.SnapshotDate, iMaxMemoryRowID, sPath);
                                    bProcessed = true;
                                }
                            }
                        }
                    }

                    XmlElement el;
                    if (!bProcessed)
                    {
                        el = xml.CreateElement("ShrinkableMemory", "urn:schemas-microsoft-com:xml-analysis:rowset");
                        el.InnerText = (GetInnerTextFromChildElement(xml.DocumentElement, "Shrinkable") == "true" ? GetInnerTextFromChildElement(xml.DocumentElement, "MemoryUsed") : "0");
                        if (el.InnerText != String.Empty) xml.DocumentElement.AppendChild(el);
                        el = xml.CreateElement("Folder1", "urn:schemas-microsoft-com:xml-analysis:rowset");
                        el.InnerText = "Others";
                        if (el.InnerText != String.Empty) xml.DocumentElement.AppendChild(el);
                        el = xml.CreateElement("Level1", "urn:schemas-microsoft-com:xml-analysis:rowset");
                        el.InnerText = (bIsFileStore ? "Unknown" : sMemoryName);
                        if (el.InnerText != String.Empty) xml.DocumentElement.AppendChild(el);
                        el = xml.CreateElement("SnapshotDate", "urn:schemas-microsoft-com:xml-analysis:rowset");
                        el.InnerText = SnapshotDate.ToString("s");
                        if (el.InnerText != String.Empty) xml.DocumentElement.AppendChild(el);
                        el = xml.CreateElement("MemoryCount", "urn:schemas-microsoft-com:xml-analysis:rowset");
                        el.InnerText = "1";
                        if (el.InnerText != String.Empty) xml.DocumentElement.AppendChild(el);
                        el = xml.CreateElement("MemoryRowID", "urn:schemas-microsoft-com:xml-analysis:rowset");
                        el.InnerText = iMaxMemoryRowID.ToString();
                        if (el.InnerText != String.Empty) xml.DocumentElement.AppendChild(el);
                    }
                    el = xml.CreateElement("User", "urn:schemas-microsoft-com:xml-analysis:rowset");
                    el.InnerText = GetUserForSPID(int.Parse(GetInnerTextFromChildElement(xml.DocumentElement, "SPID")));
                    if (el.InnerText != String.Empty) xml.DocumentElement.AppendChild(el);

                    string signature = GetRowDimensionSignature(xml.DocumentElement);
                    if (dictExistingMemoryRowID.ContainsKey(signature))
                    {
                        //there is already a row that shares that exact same dimension signature, so use that MemoryRowID
                        //this reduces the size of the Memory dimension key attribute around 90%
                        SetInnerTextForChildElement(xml.DocumentElement, "MemoryRowID", dictExistingMemoryRowID[signature].ToString());
                        iMaxMemoryRowID--;
                        iTotalCombinedMemoryRowIDs++;
                    }
                    else
                    {
                        //there isn't yet a row with a matching dimension signature, so keep using the next MemoryRowID
                        dictExistingMemoryRowID.Add(signature, iMaxMemoryRowID);
                    }

                    ProcessRow(xml.DocumentElement);
                }

                foreach (ASXmlDescriptorFileInfo info in dictXmlDefinitionFiles.Values)
                {
                    if (info.SubDirectorySuffix == "prt" || info.SubDirectorySuffix == "dim" || info.SubDirectorySuffix == "dms")
                    {
                        //sum up sizes of all files for this object
                        long directorySize = 0;
                        try
                        {
                            directorySize = (new System.IO.FileInfo(info.XmlFilePath)).Length; //start with the size of the descriptor XML file size
                        }
                        catch { }
                        try
                        {
                            foreach (System.IO.FileInfo file in new System.IO.DirectoryInfo(info.DirectoryName).GetFiles())
                            {
                                directorySize += file.Length;
                            }
                        }
                        catch { }

                        XmlElement xmlDiskUsage = info.GetDiskUsageRowXml(this.SnapshotDate, ++iMaxMemoryRowID, directorySize);
                        string signature = GetRowDimensionSignature(xmlDiskUsage);
                        if (dictExistingMemoryRowID.ContainsKey(signature))
                        {
                            //see notes on this about 30 lines above
                            SetInnerTextForChildElement(xmlDiskUsage, "MemoryRowID", dictExistingMemoryRowID[signature].ToString());
                            iMaxMemoryRowID--;
                        } //no need to write to the dictionary since it won't be used again on this run
                        ProcessRow(xmlDiskUsage);
                    }
                }

                ProcessBatch();
                ProcessIndexes();
            }
            finally
            {
                try { reader.Close(); }
                catch { }
            }
        }

        private string GetRowDimensionSignature(XmlNode row)
        {
            return GetInnerTextFromChildElement(row, "MemoryName") + "\r\n"
             + GetInnerTextFromChildElement(row, "User") + "\r\n"
             + GetInnerTextFromChildElement(row, "CreationTime") + "\r\n"
             + GetInnerTextFromChildElement(row, "Shrinkable") + "\r\n"
             + GetInnerTextFromChildElement(row, "Folder1") + "\r\n"
             + GetInnerTextFromChildElement(row, "Level1") + "\r\n"
             + GetInnerTextFromChildElement(row, "Folder2") + "\r\n"
             + GetInnerTextFromChildElement(row, "Level2") + "\r\n"
             + GetInnerTextFromChildElement(row, "Folder3") + "\r\n"
             + GetInnerTextFromChildElement(row, "Level3") + "\r\n"
             + GetInnerTextFromChildElement(row, "Folder4") + "\r\n"
             + GetInnerTextFromChildElement(row, "Level4") + "\r\n"
             + GetInnerTextFromChildElement(row, "FileExtension");
        }

        private void LoadExistingMemoryDimensionSignatures()
        {
            //retrieve all the existing dimension members
            AdomdClient.AdomdCommand cmd = new AdomdClient.AdomdCommand("with member [Measures].[Must Have A Measure] as null select [Measures].[Must Have A Measure] on 0, Leaves([Memory]) properties MEMBER_CAPTION, [Memory].[Memory Name].[Key0] on 1 from [$Memory]");
            try
            {
                cmd.Connection = new AdomdClient.AdomdConnection("Data Source=" + AdomdServer.Context.CurrentServerID + ";Initial Catalog=" + DATABASE_NAME);
                cmd.Connection.Open();
                cmd.Properties.Add("ReturnCellProperties", true);
                AdomdClient.AdomdDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (!Convert.IsDBNull(dr["[Memory].[Memory Name].[Memory Name].[Key]"])) //SSAS Unknown member will have null key
                    {
                        DateTime dt;
                        string signature = ObjToString(dr["[Memory].[Memory Name].[Memory Name].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[User].[User].[MEMBER_CAPTION]"]) + "\r\n"
                         + (string.IsNullOrEmpty(ObjToString(dr["[Memory].[Creation Time].[Creation Time].[MEMBER_CAPTION]"])) || !DateTime.TryParse(Convert.ToString(dr["[Memory].[Creation Time].[Creation Time].[MEMBER_CAPTION]"]), out dt) ? "" : Convert.ToDateTime(dr["[Memory].[Creation Time].[Creation Time].[MEMBER_CAPTION]"]).ToString("s")) + "\r\n"
                         + ObjToString(dr["[Memory].[Shrinkable].[Shrinkable].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Folder 1].[Folder 1].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Level 1].[Level 1].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Folder 2].[Folder 2].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Level 2].[Level 2].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Folder 3].[Folder 3].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Level 3].[Level 3].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Folder 4].[Folder 4].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[Level 4].[Level 4].[MEMBER_CAPTION]"]) + "\r\n"
                         + ObjToString(dr["[Memory].[File Extension].[File Extension].[MEMBER_CAPTION]"]);
                        long key = Convert.ToInt64(dr["[Memory].[Memory Name].[Memory Name].[Key]"]);
                        if (!dictExistingMemoryRowID.ContainsKey(signature))
                        {
                            dictExistingMemoryRowID.Add(signature, key);
                        }
                    }
                }
                dr.Close();
            }
            catch { }
            finally
            {
                try
                {
                    cmd.Connection.Close();
                }
                catch { }
            }
        }

        private string ObjToString(object obj)
        {
            if (Convert.IsDBNull(obj))
            {
                return String.Empty;
            }
            else
            {
                return Convert.ToString(obj);
            }
        }

        private void ProcessRow(XmlNode row)
        {
            if (iRowsBuffered++ == 0)
            {
                sRowsBuffer = new StringBuilder();
            }
            sRowsBuffer.Append(row.OuterXml);
            if (iRowsBuffered >= ROWS_PER_BATCH)
            {
                ProcessBatch();
                iRowsBuffered = 0;
            }
        }

        private void ProcessBatch()
        {
            if (iRowsBuffered > 0)
            {
                ProcessAddMemoryDimension();
                ProcessAddPartition();
            }
        }

        private string StripUnprintableChars(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            foreach (char c in str.ToCharArray())
            {
                if (!(c < 0x20 || c > 0x7e))
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private ASXmlDescriptorFileInfo ProcessXmlFile(System.IO.FileInfo f)
        {
            ASXmlDescriptorFileInfo info = new ASXmlDescriptorFileInfo();
            info.XmlFilePath = f.FullName;
            XmlDocument fxml = new XmlDocument();
            fxml.Load(f.FullName);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(fxml.NameTable);
            nsmgr.AddNamespace("x", "http://schemas.microsoft.com/analysisservices/2003/engine");

            XmlNode xmlObjDefinition = fxml.SelectSingleNode("x:Load/x:ObjectDefinition", nsmgr);
            XmlNode xmlParentObj = fxml.SelectSingleNode("x:Load/x:ParentObject", nsmgr);
            if (xmlObjDefinition == null || xmlParentObj == null || xmlObjDefinition.FirstChild == null)
            {
                return null;
            }
            info.ID = GetInnerTextFromChildElement(xmlObjDefinition.FirstChild, "ID");
            if (xmlObjDefinition.FirstChild.Name == "Dimension")
            {
                info.SubDirectorySuffix = "dim";
                info.Folder1 = "Databases";
                info.Level1 = GetInnerTextFromChildElement(xmlParentObj, "DatabaseID");
                info.Folder2 = "Dimensions";
                info.Level2 = info.ID;
                try
                {
                    Database d = svr.Databases[info.Level1];
                    info.Level1 = d.Name;
                    Dimension dim = d.Dimensions[info.Level2];
                    info.Level2 = dim.Name;
                }
                catch { }
            }
            else if (xmlObjDefinition.FirstChild.Name == "Partition")
            {
                info.SubDirectorySuffix = "prt";
                info.Folder1 = "Databases";
                info.Level1 = GetInnerTextFromChildElement(xmlParentObj, "DatabaseID");
                info.Folder2 = "Cubes";
                info.Level2 = GetInnerTextFromChildElement(xmlParentObj, "CubeID");
                info.Folder3 = "Measure Groups";
                info.Level3 = GetInnerTextFromChildElement(xmlParentObj, "MeasureGroupID");
                info.Folder4 = "Partitions";
                info.Level4 = info.ID;
                try
                {
                    Database d = svr.Databases[info.Level1];
                    info.Level1 = d.Name;
                    Cube c = d.Cubes[info.Level2];
                    info.Level2 = c.Name;
                    MeasureGroup mg = c.MeasureGroups[info.Level3];
                    info.Level3 = mg.Name;
                    Partition p = mg.Partitions[info.Level4];
                    info.Level4 = p.Name;
                }
                catch { }
            }
            else if (xmlObjDefinition.FirstChild.Name == "Cube")
            {
                info.SubDirectorySuffix = "cub";
                info.Folder1 = "Databases";
                info.Level1 = GetInnerTextFromChildElement(xmlParentObj, "DatabaseID");
                info.Folder2 = "Cubes";
                info.Level2 = info.ID;
                try
                {
                    Database d = svr.Databases[info.Level1];
                    info.Level1 = d.Name;
                    Cube c = d.Cubes[info.Level2];
                    info.Level2 = c.Name;
                }
                catch { }
            }
            else if (xmlObjDefinition.FirstChild.Name == "MiningStructure")
            {
                info.SubDirectorySuffix = "dms";
                info.Folder1 = "Databases";
                info.Level1 = GetInnerTextFromChildElement(xmlParentObj, "DatabaseID");
                info.Folder2 = "Mining Structures";
                info.Level2 = info.ID;
                try
                {
                    Database d = svr.Databases[info.Level1];
                    info.Level1 = d.Name;
                    MiningStructure min = d.MiningStructures[info.Level2];
                    info.Level2 = min.Name;
                }
                catch { }
            }
            else
            {
                return null;
            }
            string sDataFileList = GetInnerTextFromChildElement(xmlObjDefinition.FirstChild, "DataFileList");
            if (sDataFileList != string.Empty)
            {
                info.DataFileList = new List<string>(sDataFileList.Split(new char[] { ';' }));
                info.PersistLocation = GetInnerTextFromChildElement(xmlObjDefinition.FirstChild, "PersistLocation");
                info.ParentDirectoryName = f.Directory.FullName;
                info.ObjectID = GetInnerTextFromChildElement(xmlObjDefinition.FirstChild, "ObjectID");
                info.StorageLocation = GetInnerTextFromChildElement(xmlObjDefinition.FirstChild, "StorageLocation");
                if (System.IO.Directory.Exists(info.DirectoryName))
                {
                    return info;
                }
            }
            return null;
        }

        protected static string GetInnerTextFromChildElement(XmlNode node, string name)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == name)
                {
                    return child.InnerText;
                }
            }
            return String.Empty;
        }

        protected static void SetInnerTextForChildElement(XmlNode node, string name, string value)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == name)
                {
                    child.InnerText = value;
                }
            }
        }

        #region ASXmlDescriptorFileInfo class
        private class ASXmlDescriptorFileInfo
        {
            //path to object
            public string Folder1; //Databases, Others
            public string Level1; //holds database name
            public string Folder2; //Cubes, Dimensions, MiningStructures
            public string Level2; //holds cube, dimension, mining structure name
            public string Folder3; //Measure Groups
            public string Level3; //holds measure group name
            public string Folder4; //Partitions
            public string Level4; //holds partition name

            //components of the DirectoryName
            public string ID;
            public string PersistLocation;
            public string SubDirectorySuffix;
            public string ParentDirectoryName;
            public string StorageLocation;
            public string ObjectID;
            public string DirectoryName
            {
                get
                {
                    if (string.IsNullOrEmpty(StorageLocation))
                    {
                        return ParentDirectoryName + "\\" + ID + "." + PersistLocation + "." + SubDirectorySuffix;
                    }
                    else
                    {
                        //if the storage location isn't the default
                        string sPath = StorageLocation;
                        string[] dirs = System.IO.Directory.GetDirectories(sPath, ObjectID + "*", System.IO.SearchOption.TopDirectoryOnly); //if the StorageLocation isn't the default, then the directory looks like <ObjectID>.0.0.0.0. Not sure whether those will always be zeros, so this is the safest way to figure out the right directory name
                        if (dirs.Length >= 1)
                        {
                            return dirs[0];
                        }
                        else
                        {
                            if (!sPath.EndsWith("\\")) sPath += "\\";
                            sPath += ObjectID;
                            return sPath;
                        }
                    }
                }
            }

            public string XmlFilePath;

            //the files under that subdirectory
            public List<string> DataFileList;

            private static string[] arrFileValidMultiDotExtensions = new string[] { "data.hdr", "string.data", "fact.map.hdr", "fact.map", "agg.rigid.data.hdr", "agg.rigid.data", "agg.rigid.map.hdr", "agg.rigid.map", "agg.flex.data.hdr", "agg.flex.data", "agg.flex.map.hdr", "agg.flex.map", "cnt.bin", "mrg.ccmap", "mrg.ccstat", "nb.ccmap", "nb.ccstat" };
            private string GetASFileExtension(string filename)
            {
                foreach (string e in arrFileValidMultiDotExtensions)
                {
                    if (filename.EndsWith(e)) return e;
                }

                string[] segments = filename.Split(new char[] { '.' });
                return segments[segments.Length - 1];
            }

            public void AppendRowFields(XmlDocument doc, DateTime SnapshotDt, long iMemoryRowID, string sPath)
            {
                XmlElement el;
                el = doc.CreateElement("ShrinkableMemory", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = (GetInnerTextFromChildElement(doc.DocumentElement, "Shrinkable") == "true" ? GetInnerTextFromChildElement(doc.DocumentElement, "MemoryUsed") : "0");
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Folder1", "urn:schemas-microsoft-com:xml-analysis:rowset"); //TODO: Combine this code with other places so it is not duplicated code
                el.InnerText = Folder1;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Folder2", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Folder2;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Folder3", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Folder3;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Folder4", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Folder4;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level1", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level1;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level2", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level2;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level3", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level3;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level4", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level4;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("SnapshotDate", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = SnapshotDt.ToString("s");
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("MemoryCount", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = "1";
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("MemoryRowID", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = iMemoryRowID.ToString();
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("FileExtension", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = GetASFileExtension(sPath);
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
            }

            public XmlElement GetDiskUsageRowXml(DateTime SnapshotDt, long iMemoryRowID, long diskUsed)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<row></row>");
                XmlElement el;
                el = doc.CreateElement("Folder1", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Folder1;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Folder2", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Folder2;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Folder3", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Folder3;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Folder4", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Folder4;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level1", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level1;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level2", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level2;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level3", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level3;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("Level4", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = Level4;
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("SnapshotDate", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = SnapshotDt.ToString("s");
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("MemoryRowID", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = iMemoryRowID.ToString();
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                el = doc.CreateElement("DiskUsed", "urn:schemas-microsoft-com:xml-analysis:rowset");
                el.InnerText = diskUsed.ToString();
                if (el.InnerText != String.Empty) doc.DocumentElement.AppendChild(el);
                return doc.DocumentElement;
            }
        } //end of ASXmlDescriptorFileInfo class
        #endregion
    }
}
