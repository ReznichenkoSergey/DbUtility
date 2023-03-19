namespace SqlAnalyzer.Core.Models.ExecPlanModels
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class MissingIndexes
    {

        private MissingIndexesMissingIndexGroup[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MissingIndexGroup", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MissingIndexesMissingIndexGroup[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MissingIndexesMissingIndexGroup
    {

        private MissingIndexesMissingIndexGroupMissingIndex[] missingIndexField;

        private string impactField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("MissingIndex", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MissingIndexesMissingIndexGroupMissingIndex[] MissingIndex
        {
            get
            {
                return this.missingIndexField;
            }
            set
            {
                this.missingIndexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Impact
        {
            get
            {
                return this.impactField;
            }
            set
            {
                this.impactField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MissingIndexesMissingIndexGroupMissingIndex
    {

        private MissingIndexesMissingIndexGroupMissingIndexColumnGroup[] columnGroupField;

        private string databaseField;

        private string schemaField;

        private string tableField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ColumnGroup", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MissingIndexesMissingIndexGroupMissingIndexColumnGroup[] ColumnGroup
        {
            get
            {
                return this.columnGroupField;
            }
            set
            {
                this.columnGroupField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Database
        {
            get
            {
                return this.databaseField;
            }
            set
            {
                this.databaseField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Schema
        {
            get
            {
                return this.schemaField;
            }
            set
            {
                this.schemaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Table
        {
            get
            {
                return this.tableField;
            }
            set
            {
                this.tableField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MissingIndexesMissingIndexGroupMissingIndexColumnGroup
    {

        private MissingIndexesMissingIndexGroupMissingIndexColumnGroupColumn[] columnField;

        private string usageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Column", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MissingIndexesMissingIndexGroupMissingIndexColumnGroupColumn[] Column
        {
            get
            {
                return this.columnField;
            }
            set
            {
                this.columnField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Usage
        {
            get
            {
                return this.usageField;
            }
            set
            {
                this.usageField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class MissingIndexesMissingIndexGroupMissingIndexColumnGroupColumn
    {

        private string nameField;

        private string columnIdField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ColumnId
        {
            get
            {
                return this.columnIdField;
            }
            set
            {
                this.columnIdField = value;
            }
        }
    }

}
