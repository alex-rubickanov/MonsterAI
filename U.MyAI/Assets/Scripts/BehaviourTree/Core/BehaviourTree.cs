using System.Collections.Generic;
using Pathfinding;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Behaviour Tree")]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public List<Node> nodes = new List<Node>();
    public Blackboard blackboard = new Blackboard();

    private Node.State treeState = Node.State.Running;
    
    public Node.State Update()
    {
        if (rootNode.state == Node.State.Running)
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        node.started = false;
        
        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        nodes.Add(node);

        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node,this);
        }
        
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        nodes.Remove(node);
        
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
            rootNode.child = child;
            EditorUtility.SetDirty(rootNode);
        }
        
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            Undo.RecordObject(decoratorNode, "Behaviour Tree (AddChild)");
            decoratorNode.child = child;
            EditorUtility.SetDirty(decoratorNode);
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            Undo.RecordObject(compositeNode, "Behaviour Tree (AddChild)");
            compositeNode.children.Add(child);
            EditorUtility.SetDirty(compositeNode);
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }
        
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            Undo.RecordObject(decoratorNode, "Behaviour Tree (RemoveChild)");
            decoratorNode.child = null;
            EditorUtility.SetDirty(decoratorNode);
        }
        
        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
            composite.children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        
        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }
        
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode && decoratorNode.child != null)
        {
            children.Add(decoratorNode.child);
        }
        
        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.children;
        }

        return children;
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        tree.nodes = new List<Node>();
        Traverse(tree.rootNode, (n) =>
        {
            tree.nodes.Add(n);
            n.started = false;
        });
        return tree;
    }

    public void Traverse(Node node, System.Action<Node> visitor)
    {
        if (node)
        {
            visitor.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visitor));
        }
    }

    public void Bind(Unit unit)
    {
        Traverse(rootNode, node =>
        {
            node.unit = unit;
            node.blackboard = blackboard;
        });
    }
}
