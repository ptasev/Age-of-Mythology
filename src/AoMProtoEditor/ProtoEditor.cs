using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AoMProtoEditor
{
    public enum ProtoUndoType
    {
        ADD, EDIT, REMOVE
    }

    public struct ProtoUndoAction
    {
        public ProtoUndoType Type;
        public ProtoProperty Property;
        public object Value;
    }

    public class ProtoEditor
    {
        public readonly LinkedList<ProtoUndoAction> UndoAction;
        public ProtoProperty CopyProperty;

        public ProtoEditor()
        {
            UndoAction = new LinkedList<ProtoUndoAction>();
        }

        public void PushUndoAction(ProtoProperty prop, ProtoUndoType type, object Value)
        {
            ProtoUndoAction undoAction = new ProtoUndoAction();
            undoAction.Type = type;
            undoAction.Property = prop;
            undoAction.Value = Value;

            UndoAction.AddFirst(undoAction);
            if (UndoAction.Count > 10)
                UndoAction.RemoveLast();
        }
        public ProtoUndoAction PopUndoAction()
        {
            ProtoUndoAction undoAction = UndoAction.First.Value;
            UndoAction.RemoveFirst();

            if (undoAction.Type == ProtoUndoType.REMOVE)
            {
                if (undoAction.Property is ProtoElement)
                {
                    undoAction.Property.ParentProperty.ChildElement.Remove((ProtoElement)undoAction.Property);
                }
                else // is ProtoAttribute
                {
                    undoAction.Property.ParentProperty.Attribute.Remove((ProtoAttribute)undoAction.Property);
                }
            }
            else if (undoAction.Type == ProtoUndoType.EDIT)
            {
                undoAction.Property.Value = undoAction.Value;
            }
            else if (undoAction.Type == ProtoUndoType.ADD)
            {
                if (undoAction.Property is ProtoElement)
                {
                    undoAction.Property.ParentProperty.ChildElement.Add((ProtoElement)undoAction.Property);
                }
                else // is ProtoAttribute
                {
                    undoAction.Property.ParentProperty.Attribute.Add((ProtoAttribute)undoAction.Property);
                }
            }

            return undoAction;
        }

        public void Copy(ProtoProperty prop)
        {
            if (prop == null)
                return;

            if (prop is ProtoElement)
                CopyProperty = new ProtoElement((ProtoElement)prop);
            else
                CopyProperty = new ProtoAttribute((ProtoAttribute)prop);
        }
        public bool Paste(ProtoElement elem)
        {
            if (CopyProperty == null || elem == null)
                return false;

            if (CopyProperty is ProtoElement)
            {
                if (!elem.Definition.ChildElement.ContainsKey(CopyProperty.NameID)
                    || !object.ReferenceEquals(elem.Definition.ChildElement[CopyProperty.NameID], CopyProperty.Definition))
                    return false;


                CopyProperty.UpdateParentProperty(elem);
                elem.ChildElement.Add((ProtoElement)CopyProperty);
                this.PushUndoAction(CopyProperty, ProtoUndoType.REMOVE, null);
                return true;
            }
            else
            {
                if (!elem.Definition.Attribute.ContainsKey(CopyProperty.NameID)
                    || !object.ReferenceEquals(elem.Definition.Attribute[CopyProperty.NameID], CopyProperty.Definition))
                    return false;
                foreach (ProtoAttribute attr in elem.Attribute)
                {
                    if (object.ReferenceEquals(attr.Definition, CopyProperty.Definition))
                        return false;
                }

                CopyProperty.UpdateParentProperty(elem);
                elem.Attribute.Add((ProtoAttribute)CopyProperty);
                this.PushUndoAction(CopyProperty, ProtoUndoType.REMOVE, null);
                return true;
            }
        }
    }
}
