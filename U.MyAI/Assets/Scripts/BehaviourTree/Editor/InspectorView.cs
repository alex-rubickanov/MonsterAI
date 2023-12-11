using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, GraphView.UxmlTraits> { }

    private Editor editor;
    
    public void UpdateSelection(NodeView nodeView)
    {
        Clear();
        
        UnityEngine.Object.DestroyImmediate(editor);
        
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (editor.target)
            {
                editor.OnInspectorGUI();
            }
        });
        
        Add(container);
    }
}
