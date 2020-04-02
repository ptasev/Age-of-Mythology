using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoMProtoEditor
{
    public partial class PropertyBox : Form
    {
        public readonly ProtoProperty Prop;

        private ProtoPropertyEditMode editMode;

        public PropertyBox(ProtoProperty property, ProtoPropertyEditMode _editMode)
        {
            InitializeComponent();
            // Args
            Prop = property;
            editMode = _editMode;
            // FORM
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            //this.TopMost = true;
            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
            this.Text = "Add Property";
            // BUTTON
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            // COMBOBOX
            nameComboBox.Sorted = true;
            nameComboBox.DisplayMember = "Name";
            nameComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            nameComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            valueComboBox.Sorted = true;
            valueComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            valueComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            if (editMode == ProtoPropertyEditMode.ADDELEMENT)
            {
                foreach (ProtoElementDefinition def in ((ProtoElement)property).Definition)
                {
                    nameComboBox.Items.Add(def);
                }
                nameComboBox.SelectedIndex = 0;
            }
            else if (editMode == ProtoPropertyEditMode.ADDATTRIBUTE)
            {
                foreach (ProtoAttributeDefinition def in ((ProtoElement)property).Definition.Attributes)
                {
                    nameComboBox.Items.Add(def);
                }
                foreach (ProtoAttribute attr in ((ProtoElement)property).Attributes)
                {
                    nameComboBox.Items.Remove(attr.Definition);
                }
                nameComboBox.SelectedIndex = 0;
            }
            else if (editMode == ProtoPropertyEditMode.EDIT)
            {
                this.Text = "Edit Property";
                nameComboBox.Items.Add(property.Definition);
                nameComboBox.SelectedIndex = 0;
                nameComboBox.Enabled = false;
            }
        }

        private void valueComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void nameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nameComboBox.SelectedIndex < 0)
                return;

            valueComboBox.Items.Clear();
            valueComboBox.Enabled = true;

            if (editMode == ProtoPropertyEditMode.ADDELEMENT 
                || editMode == ProtoPropertyEditMode.ADDATTRIBUTE
                || editMode == ProtoPropertyEditMode.EDIT)
            {
                ProtoDefinition def = (ProtoDefinition)nameComboBox.SelectedItem;
                if (def.DefaultFormat == "VOID")
                {
                    valueComboBox.Text = string.Empty;
                    valueComboBox.Enabled = false;
                }
                else if (def.File.Schema.Format.ContainsKey(def.DefaultFormat))
                {
                    HashSet<string> vals = new HashSet<string>(def.File.Schema.Format[def.DefaultFormat].Value, StringComparer.Ordinal);
                    foreach (string link in def.File.Schema.Format[def.DefaultFormat].Link)
                    {
                        if (def.File.Schema.Format.ContainsKey(link))
                            vals.UnionWith(def.File.Schema.Format[link].Value);
                    }
                    valueComboBox.Items.AddRange(vals.ToArray());
                }

                if (valueComboBox.Enabled)
                {
                    if (editMode == ProtoPropertyEditMode.EDIT)
                    {
                        valueComboBox.Text = Prop.FormattedValue;
                    }
                    else if (valueComboBox.Items.Count > 0)
                    {
                        valueComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        valueComboBox.Text = ProtoType.ToString(ProtoType.GetDefaultValue(def.DefaultFormat), def.DefaultFormat);
                    }
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (nameComboBox.SelectedIndex < 0) 
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            ProtoDefinition def = (ProtoDefinition)nameComboBox.SelectedItem;
            object value;
            // TryParse might change the definition format, so deny it
            def.UpdateFormat = false;
            if (!ProtoType.TryParse(valueComboBox.Text, def, out value, true))
            {
                this.DialogResult = DialogResult.None;
                return;
            }

            if (editMode == ProtoPropertyEditMode.ADDELEMENT)
            {
                ProtoElement addElem = new ProtoElement(Prop.File, (ProtoElement)Prop, def.NameID, value);
                ((ProtoElement)Prop).ChildElement.Add(addElem);
                Prop.File.Editor.PushUndoAction(addElem, ProtoUndoType.REMOVE, null);
            }
            else if (editMode == ProtoPropertyEditMode.ADDATTRIBUTE)
            {
                ProtoAttribute addAttr = new ProtoAttribute(Prop.File, (ProtoElement)Prop, def.NameID, value);
                ((ProtoElement)Prop).Attribute.Add(addAttr);
                Prop.File.Editor.PushUndoAction(addAttr, ProtoUndoType.REMOVE, null);
            }
            else if (editMode == ProtoPropertyEditMode.EDIT)
            {
                Prop.File.Editor.PushUndoAction(Prop, ProtoUndoType.EDIT, Prop.Value);
                Prop.Value = value;
            }
        }
    }
}
