using ObjectEditor.Json;

namespace ObjectEditor.Editor
{
    public class EditorIndex : DataObject
    {
        public int listIndex = -1, itemIndex = -1;
        public EditorIndex nextIndex;

        public static void Load()
        {
            ReflectionConstructor<EditorIndex>.Register("EditorIndex");
        }

        public EditorIndex RemoveNext()
        {
            nextIndex = null;
            return this;
        }

        public void AddFields(FieldData fields)
        {
            listIndex = fields.add(listIndex, "listIndex");
            itemIndex = fields.add(itemIndex, "itemIndex");
            nextIndex = fields.add(nextIndex, "nextIndex");
        }

        public void AddParent(EditorIndex index)
        {
            EditorIndex child = new EditorIndex();
            child.listIndex = listIndex;
            child.itemIndex = itemIndex;
            child.nextIndex = nextIndex;

            listIndex = index.listIndex;
            itemIndex = index.itemIndex;
            nextIndex = child;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}] -> {2}", 
                listIndex, itemIndex, 
                nextIndex == null ? "{-}" : nextIndex.ToString());
        }

        public bool Equals(EditorIndex index)
        {
            if (this.listIndex != index.listIndex ||
                this.itemIndex != index.itemIndex) return false;
            if (index.nextIndex != null)
            {
                if (this.nextIndex == null) return false;
                return nextIndex.Equals(index.nextIndex);
            }
            if (this.nextIndex != null) return false;
            return true;
        }
    }
}
